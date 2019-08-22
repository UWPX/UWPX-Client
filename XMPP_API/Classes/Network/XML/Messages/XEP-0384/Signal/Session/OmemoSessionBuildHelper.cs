using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using libsignal;
using Logging;
using XMPP_API.Classes.Network.XML.Messages.Helper;
using XMPP_API.Classes.Network.XML.Messages.XEP_0060;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0384.Signal.Session
{
    public class OmemoSessionBuildHelper
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public OmemoSessionBuildHelperState STATE { get; private set; }

        private readonly XMPPConnection2 CONNECTION;
        public readonly string CHAT_JID;
        private readonly string BARE_ACCOUNT_JID;
        private readonly string FULL_ACCOUNT_JID;
        private readonly OmemoHelper OMEMO_HELPER;
        private OmemoSessionBuildResult sessionBuildResult;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 10/08/2018 Created [Fabian Sauter]
        /// </history>
        internal OmemoSessionBuildHelper(string chatJid, string bareAccountJid, string fullAccountJid, XMPPConnection2 connection, OmemoHelper omemoHelper)
        {
            CONNECTION = connection;
            CHAT_JID = chatJid;
            BARE_ACCOUNT_JID = bareAccountJid;
            FULL_ACCOUNT_JID = fullAccountJid;
            OMEMO_HELPER = omemoHelper;
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
        private IList<SignalProtocolAddress> getOwnOmemoDevices()
        {
            List<SignalProtocolAddress> ownDevices = new List<SignalProtocolAddress>();
            if (!(OMEMO_HELPER.DEVICES is null))
            {
                foreach (uint i in OMEMO_HELPER.DEVICES.IDS)
                {
                    if (i != CONNECTION.account.omemoDeviceId)
                    {
                        ownDevices.Add(new SignalProtocolAddress(BARE_ACCOUNT_JID, i));
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
            IList<SignalProtocolAddress> devicesOwn = getOwnOmemoDevices();
            IList<SignalProtocolAddress> devicesRemote = null;

            if (subscriptionState == OmemoDeviceListSubscriptionState.SUBSCRIBED)
            {
                // Because we are subscribed, the device list should be up to date:
                devicesRemote = OMEMO_HELPER.OMEMO_STORE.LoadDevices(CHAT_JID);
            }

            if (devicesRemote is null || devicesRemote.Count <= 0)
            {
                // Request devices and try to subscribe to list:
                devicesRemote = await requestDeviceListAsync();

                if (devicesRemote is null)
                {
                    return sessionBuildResult;
                }

                // Does not have to be successful:
                await subscribeToDeviceListAsync();
            }

            // Build sessions for all devices:
            OmemoSession session = new OmemoSession(CHAT_JID);
            await buildSessionForDevicesAsync(session.DEVICE_SESSIONS_REMOTE, devicesRemote);

            if (session.DEVICE_SESSIONS_REMOTE.Count > 0)
            {
                await buildSessionForDevicesAsync(session.DEVICE_SESSIONS_OWN, devicesOwn);
                sessionBuildResult = new OmemoSessionBuildResult(session);
            }
            else
            {
                Logger.Error("Failed to establish OMEMO session with: " + CHAT_JID + ". Target does not support OMEMO - not enough OMEMO sessions!");
                sessionBuildResult = new OmemoSessionBuildResult(OmemoSessionBuildError.TARGET_DOES_NOT_SUPPORT_OMEMO);
            }

            return sessionBuildResult;
        }

        #endregion

        #region --Misc Methods (Private)--
        private async Task buildSessionForDevicesAsync(Dictionary<uint, SessionCipher> sessions, IList<SignalProtocolAddress> devices)
        {
            if (devices.Count <= 0)
            {
                return;
            }
            SignalProtocolAddress device = devices[0];
            devices.RemoveAt(0);

            // Check if there exists already a session for this device:
            if (OMEMO_HELPER.OMEMO_STORE.ContainsSession(device))
            {
                // If yes, the load it:
                SessionCipher cipher = OMEMO_HELPER.loadCipher(device);
                sessions.Add(device.getDeviceId(), cipher);

                Logger.Info("[OmemoSessionBuildHelper] Session for " + device.ToString() + " loaded from cache.");
            }
            else
            {
                OmemoFingerprint fingerprint = OMEMO_HELPER.OMEMO_STORE.LoadFingerprint(device);
                if (!(fingerprint is null) && !OMEMO_HELPER.OMEMO_STORE.IsFingerprintTrusted(fingerprint))
                {
                    Logger.Warn("[OmemoSessionBuildHelper] Not building a session with " + device.ToString() + " - key not trusted.");
                }

                // Else try to build a new one by requesting the devices bundle information:
                OmemoBundleInformationResultMessage bundleMsg = await requestBundleInformationAsync(device);

                if (!(bundleMsg is null))
                {
                    OMEMO_HELPER.newSession(CHAT_JID, bundleMsg);

                    // Validate fingerprints:
                    if (fingerprint is null)
                    {
                        fingerprint = new OmemoFingerprint(bundleMsg.BUNDLE_INFO.PUBLIC_IDENTITY_KEY.getPublicKey(), device);
                        OMEMO_HELPER.OMEMO_STORE.StoreFingerprint(fingerprint);
                    }
                    else
                    {
                        OmemoFingerprint receivedFingerprint = new OmemoFingerprint(bundleMsg.BUNDLE_INFO.PUBLIC_IDENTITY_KEY.getPublicKey(), device);
                        // Make sure the fingerprint did not change or somebody is doing an attack:
                        if (!fingerprint.checkIdentityKey(receivedFingerprint.IDENTITY_PUB_KEY))
                        {
                            Logger.Warn("[OmemoSessionBuildHelper] Unable to establish session with " + device.ToString() + " - other fingerprint received than stored locally.");
                            return;
                        }
                    }

                    // Check if the fingerprint is trusted:
                    if (OMEMO_HELPER.OMEMO_STORE.IsFingerprintTrusted(fingerprint))
                    {
                        SessionCipher cipher = OMEMO_HELPER.loadCipher(device);
                        sessions.Add(device.getDeviceId(), cipher);

                        Logger.Info("[OmemoSessionBuildHelper] Session with " + device.ToString() + " established.");
                    }
                    else
                    {
                        Logger.Warn("[OmemoSessionBuildHelper] Unable to establish session with " + device.ToString() + " - key not trusted.");
                    }
                }
                else
                {
                    Logger.Warn("[OmemoSessionBuildHelper] Unable to establish session with: " + device.ToString());
                }
            }

            await buildSessionForDevicesAsync(sessions, devices);
        }

        private async Task<IList<SignalProtocolAddress>> requestDeviceListAsync()
        {
            setState(OmemoSessionBuildHelperState.REQUESTING_DEVICE_LIST);

            MessageResponseHelperResult<IQMessage> result = await CONNECTION.OMEMO_COMMAND_HELPER.requestDeviceListAsync(CHAT_JID);

            if (result.STATE == MessageResponseHelperResultState.SUCCESS)
            {
                if (result.RESULT is OmemoDeviceListResultMessage devMsg)
                {
                    // Update devices in DB:
                    string chatJid = Utils.getBareJidFromFullJid(devMsg.getFrom());
                    OMEMO_HELPER.OMEMO_STORE.StoreDevices(devMsg.DEVICES.toSignalProtocolAddressList(CHAT_JID));

                    if (devMsg.DEVICES.IDS.Count > 0)
                    {
                        // On success return the device list:
                        return devMsg.DEVICES.toSignalProtocolAddressList(CHAT_JID);
                    }
                    else
                    {
                        Logger.Error("[OmemoSessionBuildHelper] Failed to establish session - " + CHAT_JID + " doesn't support OMEMO: No devices");
                        sessionBuildResult = new OmemoSessionBuildResult(OmemoSessionBuildError.TARGET_DOES_NOT_SUPPORT_OMEMO);
                    }
                }
                else if (result.RESULT is IQErrorMessage errMsg)
                {
                    if (errMsg.ERROR_OBJ.ERROR_NAME == ErrorName.ITEM_NOT_FOUND)
                    {
                        Logger.Error("[OmemoSessionBuildHelper] Failed to establish session - " + CHAT_JID + " doesn't support OMEMO: " + errMsg.ERROR_OBJ.ToString());
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
                Logger.Error("[OmemoSessionBuildHelper] Failed to establish session - " + CHAT_JID + " didn't respond in time!");
                sessionBuildResult = new OmemoSessionBuildResult(OmemoSessionBuildError.REQUEST_BUNDLE_INFORMATION_TIMEOUT);
            }
            setState(OmemoSessionBuildHelperState.ERROR);
            return null;
        }

        private async Task<bool> subscribeToDeviceListAsync()
        {
            setState(OmemoSessionBuildHelperState.SUBSCRIBING_TO_DEVICE_LIST);

            MessageResponseHelperResult<IQMessage> result = await CONNECTION.OMEMO_COMMAND_HELPER.subscribeToDeviceListAsync(CHAT_JID);

            if (result.STATE == MessageResponseHelperResultState.SUCCESS)
            {
                if (result.RESULT is PubSubSubscriptionMessage subMsg)
                {
                    if (subMsg.SUBSCRIPTION != PubSubSubscriptionState.SUBSCRIBED)
                    {
                        Logger.Warn("[OmemoSessionBuildHelper] Failed to subscribe to device list node - " + subMsg.getFrom() + " returned: " + subMsg.SUBSCRIPTION);
                        OMEMO_HELPER.OMEMO_STORE.StoreDeviceListSubscription(CHAT_JID, new Tuple<OmemoDeviceListSubscriptionState, DateTime>(OmemoDeviceListSubscriptionState.NONE, DateTime.Now));
                    }
                    else
                    {
                        Logger.Info("[OmemoSessionBuildHelper] Subscribed to device list node for: " + CHAT_JID);
                        OMEMO_HELPER.OMEMO_STORE.StoreDeviceListSubscription(CHAT_JID, new Tuple<OmemoDeviceListSubscriptionState, DateTime>(OmemoDeviceListSubscriptionState.SUBSCRIBED, DateTime.Now));
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
                    OMEMO_HELPER.OMEMO_STORE.StoreDeviceListSubscription(CHAT_JID, new Tuple<OmemoDeviceListSubscriptionState, DateTime>(OmemoDeviceListSubscriptionState.ERROR, DateTime.Now));
                }
            }
            else
            {
                Logger.Error("[OmemoSessionBuildHelper] Failed to establish session - " + CHAT_JID + " didn't respond in time!");
            }
            return false;
        }

        private async Task<OmemoBundleInformationResultMessage> requestBundleInformationAsync(SignalProtocolAddress device)
        {
            setState(OmemoSessionBuildHelperState.REQUESTING_BUNDLE_INFORMATION);

            MessageResponseHelperResult<IQMessage> result = await CONNECTION.OMEMO_COMMAND_HELPER.requestBundleInformationAsync(device.getName(), device.getDeviceId());

            if (result.STATE == MessageResponseHelperResultState.SUCCESS)
            {
                if (result.RESULT is OmemoBundleInformationResultMessage bundleMsg)
                {
                    if (bundleMsg.BUNDLE_INFO.PUBLIC_PRE_KEYS.Count < 20)
                    {
                        Logger.Error("[OmemoSessionBuildHelper] Failed to request bundle information - " + device.ToString() + " device offered less than 20 pre keys: " + bundleMsg.BUNDLE_INFO.PUBLIC_PRE_KEYS.Count);
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
