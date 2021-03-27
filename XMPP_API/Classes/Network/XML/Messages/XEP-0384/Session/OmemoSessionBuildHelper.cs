using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Logging;
using Omemo.Classes;
using XMPP_API.Classes.Network.XML.Messages.Helper;
using XMPP_API.Classes.Network.XML.Messages.XEP_0060;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0384.Session
{
    public class OmemoSessionBuildHelper
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public OmemoSessionBuildHelperState STATE { get; private set; }

        private readonly XmppConnection CONNECTION;
        public readonly string SRC_BARE_JID;
        private readonly string DST_BARE_JID;
        private readonly OmemoHelper OMEMO_HELPER;
        private readonly bool TRUSTED_SRC_KEYS_ONLY;
        private readonly bool TRUSTED_DST_KEYS_ONLY;
        private OmemoSessionBuildResult sessionBuildResult;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        internal OmemoSessionBuildHelper(string srcBareJid, string dstBareJid, XmppConnection connection, OmemoHelper omemoHelper, bool trustedSrcKeysOnly, bool trustedDstKeysOnly)
        {
            CONNECTION = connection;
            SRC_BARE_JID = srcBareJid;
            DST_BARE_JID = dstBareJid;
            OMEMO_HELPER = omemoHelper;
            TRUSTED_SRC_KEYS_ONLY = trustedSrcKeysOnly;
            TRUSTED_DST_KEYS_ONLY = trustedDstKeysOnly;
            STATE = OmemoSessionBuildHelperState.NOT_STARTED;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private void setState(OmemoSessionBuildHelperState newState)
        {
            if (STATE != newState)
            {
                STATE = newState;
            }
        }

        /// <summary>
        /// Returns all own OMEMO devices except the current device:
        /// </summary>
        private List<OmemoProtocolAddress> getOwnOmemoDevices()
        {
            List<OmemoProtocolAddress> ownDevices = new List<OmemoProtocolAddress>();
            if (!(OMEMO_HELPER.DEVICES is null))
            {
                foreach (OmemoXmlDevice d in OMEMO_HELPER.DEVICES.DEVICES)
                {
                    if (d.ID != CONNECTION.account.omemoDeviceId)
                    {
                        ownDevices.Add(new OmemoProtocolAddress(SRC_BARE_JID, d.ID));
                    }
                }
            }

            return ownDevices;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public async Task<OmemoSessionBuildResult> buildSessionAsync(OmemoDeviceListSubscriptionState subscriptionState)
        {
            sessionBuildResult = null;
            List<OmemoProtocolAddress> devicesOwn = getOwnOmemoDevices();
            List<OmemoProtocolAddress> devicesRemote = null;

            if (subscriptionState == OmemoDeviceListSubscriptionState.SUBSCRIBED)
            {
                // Because we are subscribed, the device list should be up to date:
                devicesRemote = OMEMO_HELPER.OMEMO_STORAGE.LoadDevices(DST_BARE_JID);
            }

            if (devicesRemote is null || devicesRemote.Count <= 0)
            {
                // Request devices and try to subscribe to list:
                devicesRemote = await requestDeviceListAsync(DST_BARE_JID);

                if (devicesRemote is null)
                {
                    return sessionBuildResult;
                }

                // Does not have to be successful:
                await subscribeToDeviceListAsync(DST_BARE_JID);
            }

            // Build sessions for all devices:
            OmemoSessions sessions = new OmemoSessions(new OmemoDeviceGroup(SRC_BARE_JID), new OmemoDeviceGroup(DST_BARE_JID));
            await buildSessionForDevicesAsync(sessions.DST_DEVICE_GROUP, devicesRemote);

            if (sessions.DST_DEVICE_GROUP.SESSIONS.Count > 0)
            {
                await buildSessionForDevicesAsync(sessions.SRC_DEVICE_GROUP, devicesOwn);
                sessionBuildResult = new OmemoSessionBuildResult(sessions);
            }
            else
            {
                Logger.Error("Failed to establish OMEMO session with: " + SRC_BARE_JID + ". Target does not support OMEMO - not enough OMEMO sessions!");
                sessionBuildResult = new OmemoSessionBuildResult(OmemoSessionBuildError.TARGET_DOES_NOT_SUPPORT_OMEMO);
            }

            return sessionBuildResult;
        }

        #endregion

        #region --Misc Methods (Private)--
        private async Task buildSessionForDevicesAsync(OmemoDeviceGroup deviceGroup, IList<OmemoProtocolAddress> devices)
        {
            if (devices.Count <= 0)
            {
                return;
            }
            OmemoProtocolAddress device = devices[0];
            devices.RemoveAt(0);

            OmemoFingerprint fingerprint = OMEMO_HELPER.OMEMO_STORAGE.LoadFingerprint(device);
            // Check if there exists already a session for this device:
            OmemoSessionModel session = OMEMO_HELPER.OMEMO_STORAGE.LoadSession(device);
            if (session is null)
            {
                // Try to build a new session by requesting the devices bundle information:
                OmemoBundleInformationResultMessage bundleMsg = await requestBundleInformationAsync(device);
                if (!(bundleMsg is null) && !(bundleMsg.BUNDLE_INFO.bundle is null))
                {
                    int preKeyIndex = bundleMsg.BUNDLE_INFO.bundle.GetRandomPreKeyIndex();
                    session = new OmemoSessionModel(bundleMsg.BUNDLE_INFO.bundle, preKeyIndex, CONNECTION.account.omemoIdentityKey);

                    // Validate fingerprints:
                    if (fingerprint is null)
                    {
                        fingerprint = new OmemoFingerprint(bundleMsg.BUNDLE_INFO.bundle.identityKey, device);
                        OMEMO_HELPER.OMEMO_STORAGE.StoreFingerprint(fingerprint);
                    }
                    else
                    {
                        OmemoFingerprint receivedFingerprint = new OmemoFingerprint(bundleMsg.BUNDLE_INFO.bundle.identityKey, device);
                        // Make sure the fingerprint did not change or somebody is performing an attack:
                        if (!fingerprint.checkIdentityKey(receivedFingerprint.IDENTITY_KEY))
                        {
                            Logger.Warn("[OmemoSessionBuildHelper] Unable to establish session with " + device.ToString() + " - other fingerprint received than stored locally.");
                            await buildSessionForDevicesAsync(deviceGroup, devices);
                            return;
                        }
                    }
                }
                else
                {
                    Logger.Warn("[OmemoSessionBuildHelper] Unable to establish session with: " + device.ToString());
                }
            }
            else
            {
                Logger.Debug("[OmemoSessionBuildHelper] Session for " + device.ToString() + " loaded from cache.");
            }

            if (!(session is null))
            {
                // Check if the fingerprint is trusted:
                if (IsTrustedFingerprint(fingerprint))
                {
                    deviceGroup.SESSIONS[device.DEVICE_ID] = session;

                    Logger.Debug("[OmemoSessionBuildHelper] Session with " + device.ToString() + " established.");
                }
                else
                {
                    Logger.Warn("[OmemoSessionBuildHelper] Unable to establish session with " + device.ToString() + " - key not trusted.");
                }
            }

            await buildSessionForDevicesAsync(deviceGroup, devices);
        }

        private bool IsTrustedFingerprint(OmemoFingerprint fingerprint)
        {
            if (fingerprint is null)
            {
                return false;
            }

            if (string.Equals(fingerprint.ADDRESS.BARE_JID, SRC_BARE_JID))
            {
                return !TRUSTED_SRC_KEYS_ONLY || fingerprint.trusted;
            }

            if (string.Equals(fingerprint.ADDRESS.BARE_JID, DST_BARE_JID))
            {
                return !TRUSTED_DST_KEYS_ONLY || fingerprint.trusted;
            }

            // Should not happen:
            throw new InvalidOperationException($"Invalid fingerprint for '{fingerprint.ADDRESS.BARE_JID}' when establishing a session between '{SRC_BARE_JID}' and '{DST_BARE_JID}' found.");
        }

        private async Task<List<OmemoProtocolAddress>> requestDeviceListAsync(string bareJid)
        {
            setState(OmemoSessionBuildHelperState.REQUESTING_DEVICE_LIST);

            MessageResponseHelperResult<IQMessage> result = await CONNECTION.OMEMO_COMMAND_HELPER.requestDeviceListAsync(bareJid);

            if (result.STATE == MessageResponseHelperResultState.SUCCESS)
            {
                if (result.RESULT is OmemoDeviceListResultMessage devMsg)
                {
                    // Store the result in the DB:
                    List<OmemoProtocolAddress> devices = devMsg.DEVICES.toOmemoProtocolAddress(bareJid);
                    OMEMO_HELPER.OMEMO_STORAGE.StoreDevices(devices, bareJid);

                    if (devices.Count > 0)
                    {
                        // On success return the device list:
                        return devices;
                    }
                    else
                    {
                        Logger.Error("[OmemoSessionBuildHelper] Failed to establish session - " + bareJid + " doesn't support OMEMO: No devices");
                        sessionBuildResult = new OmemoSessionBuildResult(OmemoSessionBuildError.TARGET_DOES_NOT_SUPPORT_OMEMO);
                    }
                }
                else if (result.RESULT is IQErrorMessage errMsg)
                {
                    if (errMsg.ERROR_OBJ.ERROR_NAME == ErrorName.ITEM_NOT_FOUND)
                    {
                        Logger.Error("[OmemoSessionBuildHelper] Failed to establish session - " + bareJid + " doesn't support OMEMO: " + errMsg.ERROR_OBJ.ToString());
                        sessionBuildResult = new OmemoSessionBuildResult(OmemoSessionBuildError.TARGET_DOES_NOT_SUPPORT_OMEMO);
                    }
                    else
                    {
                        Logger.Error("[OmemoSessionBuildHelper] Failed to establish session - request device list failed: " + errMsg.ERROR_OBJ.ToString());
                        sessionBuildResult = new OmemoSessionBuildResult(OmemoSessionBuildError.REQUEST_DEVICE_LIST_IQ_ERROR);
                    }
                }
            }
            else
            {
                Logger.Error("[OmemoSessionBuildHelper] Failed to establish session - " + bareJid + " didn't respond in time!");
                sessionBuildResult = new OmemoSessionBuildResult(OmemoSessionBuildError.REQUEST_BUNDLE_INFORMATION_TIMEOUT);
            }
            setState(OmemoSessionBuildHelperState.ERROR);
            return null;
        }

        private async Task<bool> subscribeToDeviceListAsync(string bareJid)
        {
            setState(OmemoSessionBuildHelperState.SUBSCRIBING_TO_DEVICE_LIST);

            MessageResponseHelperResult<IQMessage> result = await CONNECTION.OMEMO_COMMAND_HELPER.subscribeToDeviceListAsync(bareJid);

            if (result.STATE == MessageResponseHelperResultState.SUCCESS)
            {
                if (result.RESULT is PubSubSubscriptionMessage subMsg)
                {
                    if (subMsg.SUBSCRIPTION != PubSubSubscriptionState.SUBSCRIBED)
                    {
                        Logger.Warn("[OmemoSessionBuildHelper] Failed to subscribe to device list node - " + subMsg.getFrom() + " returned: " + subMsg.SUBSCRIPTION);
                        OMEMO_HELPER.OMEMO_STORAGE.StoreDeviceListSubscription(bareJid, new Tuple<OmemoDeviceListSubscriptionState, DateTime>(OmemoDeviceListSubscriptionState.NONE, DateTime.Now));
                    }
                    else
                    {
                        Logger.Info("[OmemoSessionBuildHelper] Subscribed to device list node for: " + bareJid);
                        OMEMO_HELPER.OMEMO_STORAGE.StoreDeviceListSubscription(bareJid, new Tuple<OmemoDeviceListSubscriptionState, DateTime>(OmemoDeviceListSubscriptionState.SUBSCRIBED, DateTime.Now));
                        return true;
                    }
                }
                else if (result.RESULT is IQErrorMessage errMsg)
                {
                    if (errMsg.ERROR_OBJ.ERROR_NAME == ErrorName.ITEM_NOT_FOUND)
                    {
                        Logger.Error("[OmemoSessionBuildHelper] Failed to subscribe to device list node - " + errMsg.getFrom() + " returned node does not exist: " + errMsg.ERROR_OBJ.ToString());
                    }
                    else
                    {
                        Logger.Warn("[OmemoSessionBuildHelper] Failed to subscribe to device list node - " + errMsg.getFrom() + " returned: " + errMsg.ERROR_OBJ.ToString());
                    }
                    OMEMO_HELPER.OMEMO_STORAGE.StoreDeviceListSubscription(bareJid, new Tuple<OmemoDeviceListSubscriptionState, DateTime>(OmemoDeviceListSubscriptionState.ERROR, DateTime.Now));
                }
            }
            else
            {
                Logger.Error("[OmemoSessionBuildHelper] Failed to establish session - " + bareJid + " didn't respond in time!");
            }
            return false;
        }

        private async Task<OmemoBundleInformationResultMessage> requestBundleInformationAsync(OmemoProtocolAddress device)
        {
            setState(OmemoSessionBuildHelperState.REQUESTING_BUNDLE_INFORMATION);

            MessageResponseHelperResult<IQMessage> result = await CONNECTION.OMEMO_COMMAND_HELPER.requestBundleInformationAsync(device.BARE_JID, device.DEVICE_ID);

            if (result.STATE == MessageResponseHelperResultState.SUCCESS)
            {
                if (result.RESULT is OmemoBundleInformationResultMessage bundleMsg)
                {
                    if (!(bundleMsg.BUNDLE_INFO.bundle is null) && bundleMsg.BUNDLE_INFO.bundle.preKeys.Count < 20)
                    {
                        Logger.Error("[OmemoSessionBuildHelper] Failed to request bundle information - " + device.ToString() + " device offered less than 20 PreKeys: " + bundleMsg.BUNDLE_INFO.bundle.preKeys.Count);
                    }
                    else
                    {
                        return bundleMsg;
                    }
                }
                else if (result.RESULT is IQErrorMessage errMsg)
                {
                    if (errMsg.ERROR_OBJ.ERROR_NAME == ErrorName.ITEM_NOT_FOUND)
                    {
                        Logger.Error("[OmemoSessionBuildHelper] Failed to request bundle information - " + device.ToString() + " doesn't support OMEMO: " + errMsg.ERROR_OBJ.ToString());
                    }
                    else
                    {
                        Logger.Error("[OmemoSessionBuildHelper] Failed to request bundle information - " + device.ToString() + ": " + errMsg.ERROR_OBJ.ToString());
                    }
                }
            }
            else
            {
                Logger.Error("[OmemoSessionBuildHelper] Failed to request bundle information - " + device.ToString() + " TIMEOUT!");
            }
            return null;
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
