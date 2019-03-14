using libsignal;
using Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XMPP_API.Classes.Network.XML.Messages.Helper;
using XMPP_API.Classes.Network.XML.Messages.XEP_0060;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0384.Signal.Session
{
    public class OmemoSessionBuildHelper
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public OmemoSessionBuildHelperState STATE { get; private set; }

        private readonly Action<OmemoSessionBuildHelper, OmemoSessionBuildResult> ON_SESSION_RESULT;
        private readonly XMPPConnection2 CONNECTION;
        public readonly string CHAT_JID;
        private readonly string BARE_ACCOUNT_JID;
        private readonly string FULL_ACCOUNT_JID;
        private readonly OmemoHelper OMEMO_HELPER;
        private IList<SignalProtocolAddress> toDoDevicesRemote;
        private IList<SignalProtocolAddress> toDoDevicesOwn;
        private SignalProtocolAddress curAddress;
        private readonly OmemoSession SESSION;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 10/08/2018 Created [Fabian Sauter]
        /// </history>
        internal OmemoSessionBuildHelper(string chatJid, string bareAccountJid, string fullAccountJid, Action<OmemoSessionBuildHelper, OmemoSessionBuildResult> onSessionResult, XMPPConnection2 connection, OmemoHelper omemoHelper)
        {
            this.CONNECTION = connection;
            this.ON_SESSION_RESULT = onSessionResult;
            this.CHAT_JID = chatJid;
            this.BARE_ACCOUNT_JID = bareAccountJid;
            this.FULL_ACCOUNT_JID = fullAccountJid;
            this.OMEMO_HELPER = omemoHelper;
            this.STATE = OmemoSessionBuildHelperState.NOT_STARTED;
            this.SESSION = new OmemoSession(chatJid);
            this.curAddress = null;
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

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public async Task startAsync(OmemoDeviceListSubscriptionState subscriptionState)
        {
            switch (subscriptionState)
            {
                case OmemoDeviceListSubscriptionState.SUBSCRIBED:
                    // Because we are subscribed, the device list should be up to date:
                    IList<SignalProtocolAddress> devices = OMEMO_HELPER.OMEMO_STORE.LoadDevices(CHAT_JID);
                    await createSessionsForDevicesAsync(devices);
                    break;

                default:
                    await requestDeviceListAsync();
                    break;
            }
        }

        #endregion

        #region --Misc Methods (Private)--
        private async Task requestDeviceListAsync()
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
                        await subscribeToDeviceListAsync();
                        await createSessionsForDevicesAsync(devMsg.DEVICES.toSignalProtocolAddressList(CHAT_JID));
                    }
                    else
                    {
                        Logger.Error("[OmemoSessionBuildHelper] Failed to establish session - " + CHAT_JID + " doesn't support OMEMO: No devices");
                        setState(OmemoSessionBuildHelperState.ERROR);
                        ON_SESSION_RESULT(this, new OmemoSessionBuildResult(OmemoSessionBuildError.TARGET_DOES_NOT_SUPPORT_OMEMO));
                    }
                }
                else if (result.RESULT is IQErrorMessage errMsg)
                {
                    if (errMsg.ERROR_OBJ.ERROR_NAME == ErrorName.ITEM_NOT_FOUND)
                    {
                        Logger.Error("[OmemoSessionBuildHelper] Failed to establish session - " + CHAT_JID + " doesn't support OMEMO: " + errMsg.ERROR_OBJ.ToString());
                        setState(OmemoSessionBuildHelperState.ERROR);
                        ON_SESSION_RESULT(this, new OmemoSessionBuildResult(OmemoSessionBuildError.TARGET_DOES_NOT_SUPPORT_OMEMO));
                    }
                    else
                    {
                        Logger.Error("[OmemoSessionBuildHelper] Failed to establish session - request device list failed: " + errMsg.ERROR_OBJ.ToString());
                        setState(OmemoSessionBuildHelperState.ERROR);
                        ON_SESSION_RESULT(this, new OmemoSessionBuildResult(OmemoSessionBuildError.REQUEST_DEVICE_LIST_IQ_ERROR));
                    }
                }
            }
            else
            {
                onRequestError(result);
            }
        }

        private async Task subscribeToDeviceListAsync()
        {
            setState(OmemoSessionBuildHelperState.SUBSCRIBING_TO_DEVICE_LIST);

            MessageResponseHelperResult<IQMessage> result = await CONNECTION.OMEMO_COMMAND_HELPER.subscribeToDeviceListAsync(CHAT_JID);

            if (result.STATE == MessageResponseHelperResultState.SUCCESS)
            {
                if (result.RESULT is PubSubSubscriptionMessage subMsg)
                {
                    if (subMsg.SUBSCRIPTION != PubSubSubscriptionState.SUBSCRIBED)
                    {
                        Logger.Warn("[OmemoSessionBuildHelper] Failed to subscribe to device list node - " + CHAT_JID + " returned: " + subMsg.SUBSCRIPTION);
                        OMEMO_HELPER.OMEMO_STORE.StoreDeviceListSubscription(CHAT_JID, new Tuple<OmemoDeviceListSubscriptionState, DateTime>(OmemoDeviceListSubscriptionState.NONE, DateTime.Now));
                    }
                    else
                    {
                        OMEMO_HELPER.OMEMO_STORE.StoreDeviceListSubscription(CHAT_JID, new Tuple<OmemoDeviceListSubscriptionState, DateTime>(OmemoDeviceListSubscriptionState.SUBSCRIBED, DateTime.Now));
                    }
                }
                else if (result.RESULT is IQErrorMessage errMsg)
                {
                    if (errMsg.ERROR_OBJ.ERROR_NAME == ErrorName.ITEM_NOT_FOUND)
                    {
                        Logger.Error("[OmemoSessionBuildHelper] Failed to subscribe to device list node - " + CHAT_JID + " returned node does not exist: " + errMsg.ERROR_OBJ.ToString());
                    }
                    else
                    {
                        Logger.Warn("[OmemoSessionBuildHelper] Failed to subscribe to device list node: " + errMsg.ERROR_OBJ.ToString());
                    }
                    OMEMO_HELPER.OMEMO_STORE.StoreDeviceListSubscription(CHAT_JID, new Tuple<OmemoDeviceListSubscriptionState, DateTime>(OmemoDeviceListSubscriptionState.ERROR, DateTime.Now));
                }
            }
            else
            {
                onRequestError(result);
            }
        }

        private async Task requestBundleInformationAsync()
        {
            setState(OmemoSessionBuildHelperState.REQUESTING_BUNDLE_INFORMATION);

            MessageResponseHelperResult<IQMessage> result = await CONNECTION.OMEMO_COMMAND_HELPER.requestBundleInformationAsync(curAddress.getName(), curAddress.getDeviceId());

            if (result.STATE == MessageResponseHelperResultState.SUCCESS)
            {
                if (result.RESULT is OmemoBundleInformationResultMessage bundleMsg)
                {
                    if (bundleMsg.BUNDLE_INFO.PUBLIC_PRE_KEYS.Count < 20)
                    {
                        Logger.Error("[OmemoSessionBuildHelper] Failed to establish session - " + curAddress.getName() + ':' + curAddress.getDeviceId() + " device offered less than 20 pre keys: " + bundleMsg.BUNDLE_INFO.PUBLIC_PRE_KEYS.Count);
                    }
                    else
                    {
                        Logger.Info("[OmemoSessionBuildHelper] Session with " + curAddress.getName() + ':' + curAddress.getDeviceId() + " established.");
                        SignalProtocolAddress address = OMEMO_HELPER.newSession(CHAT_JID, bundleMsg);
                        SessionCipher cipher = OMEMO_HELPER.loadCipher(address);
                        SESSION.DEVICE_SESSIONS.Add(curAddress.getDeviceId(), cipher);
                    }
                    await createSessionForNextDeviceAsync();
                }
                else if (result.RESULT is IQErrorMessage errMsg)
                {
                    if (errMsg.ERROR_OBJ.ERROR_NAME == ErrorName.ITEM_NOT_FOUND)
                    {
                        Logger.Error("[OmemoSessionBuildHelper] Failed to establish session - " + curAddress.getName() + ':' + curAddress.getDeviceId() + " doesn't support OMEMO: " + errMsg.ERROR_OBJ.ToString());
                    }
                    else
                    {
                        Logger.Error("[OmemoSessionBuildHelper] Failed to establish session - request bundle info failed (" + curAddress.getName() + ':' + curAddress.getDeviceId() + "): " + errMsg.ERROR_OBJ.ToString());
                    }
                    await createSessionForNextDeviceAsync();
                }
            }
            else
            {
                onRequestError(result);
            }
        }

        private void onRequestError(MessageResponseHelperResult<IQMessage> result)
        {
            switch (STATE)
            {
                case OmemoSessionBuildHelperState.REQUESTING_DEVICE_LIST:
                    Logger.Error("[OmemoSessionBuildHelper] Failed to establish session - " + CHAT_JID + " didn't respond in time!");
                    setState(OmemoSessionBuildHelperState.ERROR);
                    ON_SESSION_RESULT(this, new OmemoSessionBuildResult(OmemoSessionBuildError.REQUEST_DEVICE_LIST_TIMEOUT));
                    break;

                case OmemoSessionBuildHelperState.SUBSCRIBING_TO_DEVICE_LIST:
                    Logger.Error("[OmemoSessionBuildHelper] Failed to establish session - " + CHAT_JID + " didn't respond in time!");
                    setState(OmemoSessionBuildHelperState.ERROR);
                    ON_SESSION_RESULT(this, new OmemoSessionBuildResult(OmemoSessionBuildError.SUBSCRIBE_TO_DEVICE_LIST_TIMEOUT));
                    break;

                case OmemoSessionBuildHelperState.REQUESTING_BUNDLE_INFORMATION:
                    Logger.Error("[OmemoSessionBuildHelper] Failed to establish session - " + curAddress.getName() + ':' + curAddress.getDeviceId() + " didn't respond in time!");
                    setState(OmemoSessionBuildHelperState.ERROR);
                    ON_SESSION_RESULT(this, new OmemoSessionBuildResult(OmemoSessionBuildError.REQUEST_BUNDLE_INFORMATION_TIMEOUT));
                    break;
            }
        }

        private async Task createSessionsForDevicesAsync(IList<SignalProtocolAddress> remoteDevices)
        {
            // Add remote devices:
            toDoDevicesRemote = remoteDevices;

            // Add own devices:
            toDoDevicesOwn = new List<SignalProtocolAddress>();
            if (!(OMEMO_HELPER.DEVICES is null))
            {
                foreach (uint i in OMEMO_HELPER.DEVICES.IDS)
                {
                    if (i != CONNECTION.account.omemoDeviceId)
                    {
                        toDoDevicesOwn.Add(new SignalProtocolAddress(BARE_ACCOUNT_JID, i));
                    }
                }
            }

            await createSessionForNextDeviceAsync();
        }

        private async Task createSessionForNextDeviceAsync()
        {
            if ((toDoDevicesRemote is null || toDoDevicesRemote.Count <= 0) && (toDoDevicesOwn is null || toDoDevicesOwn.Count <= 0))
            {
                // All sessions created:
                if (SESSION.DEVICE_SESSIONS.Count <= 0)
                {
                    setState(OmemoSessionBuildHelperState.ERROR);
                    ON_SESSION_RESULT(this, new OmemoSessionBuildResult(OmemoSessionBuildError.TARGET_DOES_NOT_SUPPORT_OMEMO));
                }
                else
                {
                    setState(OmemoSessionBuildHelperState.ESTABLISHED);
                    ON_SESSION_RESULT(this, new OmemoSessionBuildResult(SESSION));
                }
            }
            else
            {
                if (toDoDevicesRemote is null || toDoDevicesRemote.Count <= 0)
                {
                    curAddress = toDoDevicesOwn[0];
                    toDoDevicesOwn.RemoveAt(0);
                }
                else
                {
                    curAddress = toDoDevicesRemote[0];
                    toDoDevicesRemote.RemoveAt(0);
                }

                if (OMEMO_HELPER.OMEMO_STORE.ContainsSession(curAddress))
                {
                    SessionCipher cipher = OMEMO_HELPER.loadCipher(curAddress);
                    SESSION.DEVICE_SESSIONS.Add(curAddress.getDeviceId(), cipher);
                    await createSessionForNextDeviceAsync();
                }
                else
                {
                    await requestBundleInformationAsync();
                }
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
