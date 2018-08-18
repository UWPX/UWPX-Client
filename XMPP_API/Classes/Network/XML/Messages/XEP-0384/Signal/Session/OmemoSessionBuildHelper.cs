using libsignal;
using Logging;
using System;
using System.Collections.Generic;
using XMPP_API.Classes.Network.XML.DBEntries;
using XMPP_API.Classes.Network.XML.DBManager;
using XMPP_API.Classes.Network.XML.Messages.XEP_0060;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0384.Signal.Session
{
    public class OmemoSessionBuildHelper : IDisposable
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public OmemoSessionBuildHelperState STATE { get; private set; }

        private readonly Action<OmemoSessionBuildHelper, OmemoSessionBuildResult> ON_SESSION_RESULT;
        private readonly IMessageSender MESSAGE_SENDER;
        public readonly string CHAT_JID;
        private readonly string BARE_ACCOUNT_JID;
        private readonly string FULL_ACCOUNT_JID;
        private readonly OmemoHelper OMEMO_HELPER;
        private List<uint> toDoDevices;
        private uint curDevice;
        private readonly OmemoSession SESSION;
        private MessageResponseHelper<IQMessage> requestDeviceListHelper;
        private MessageResponseHelper<IQMessage> requestBundleInfoHelper;
        private MessageResponseHelper<IQMessage> subscribeToDeviceListHelper;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 10/08/2018 Created [Fabian Sauter]
        /// </history>
        internal OmemoSessionBuildHelper(string chatJid, string bareAccountJid, string fullAccountJid, Action<OmemoSessionBuildHelper, OmemoSessionBuildResult> onSessionResult, IMessageSender messageSender, OmemoHelper omemoHelper)
        {
            this.MESSAGE_SENDER = messageSender;
            this.ON_SESSION_RESULT = onSessionResult;
            this.CHAT_JID = chatJid;
            this.BARE_ACCOUNT_JID = bareAccountJid;
            this.FULL_ACCOUNT_JID = fullAccountJid;
            this.OMEMO_HELPER = omemoHelper;
            this.STATE = OmemoSessionBuildHelperState.NOT_STARTED;
            this.requestDeviceListHelper = null;
            this.requestBundleInfoHelper = null;
            this.SESSION = new OmemoSession(chatJid);
            this.curDevice = 0;
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
        public void start(OmemoDeviceListSubscriptionState subscriptionState)
        {
            switch (subscriptionState)
            {
                case OmemoDeviceListSubscriptionState.SUBSCRIBED:
                    // Because we are subscribed, the device list should be up to date:
                    List<uint> devices = OmemoDeviceDBManager.INSTANCE.getDeviceIds(CHAT_JID, BARE_ACCOUNT_JID);
                    createSessionsForDevices(devices);
                    break;

                default:
                    requestDeviceList();
                    break;
            }
        }

        public void Dispose()
        {
            requestDeviceListHelper?.Dispose();
            requestBundleInfoHelper?.Dispose();
            subscribeToDeviceListHelper?.Dispose();
        }

        #endregion

        #region --Misc Methods (Private)--
        private void requestDeviceList()
        {
            setState(OmemoSessionBuildHelperState.REQUESTING_DEVICE_LIST);
            if (requestDeviceListHelper != null)
            {
                requestDeviceListHelper?.Dispose();
                requestDeviceListHelper = null;
            }

            requestDeviceListHelper = new MessageResponseHelper<IQMessage>(MESSAGE_SENDER, onRequestDeviceListMessage, onTimeout);
            OmemoRequestDeviceListMessage msg = new OmemoRequestDeviceListMessage(BARE_ACCOUNT_JID, CHAT_JID);
            requestDeviceListHelper.start(msg);
        }

        private void subscribeToDeviceList()
        {
            setState(OmemoSessionBuildHelperState.SUBSCRIBING_TO_DEVICE_LIST);
            if (subscribeToDeviceListHelper != null)
            {
                subscribeToDeviceListHelper?.Dispose();
                subscribeToDeviceListHelper = null;
            }

            subscribeToDeviceListHelper = new MessageResponseHelper<IQMessage>(MESSAGE_SENDER, onSubscribeToDeviceListMessage, onTimeout);
            OmemoSubscribeToDeviceListMessage msg = new OmemoSubscribeToDeviceListMessage(FULL_ACCOUNT_JID, BARE_ACCOUNT_JID, CHAT_JID);
            subscribeToDeviceListHelper.start(msg);
        }

        private void requestBundleInformation(uint remoteDeviceId)
        {
            setState(OmemoSessionBuildHelperState.REQUESTING_BUNDLE_INFORMATION);
            if (requestBundleInfoHelper != null)
            {
                requestBundleInfoHelper?.Dispose();
                requestBundleInfoHelper = null;
            }

            requestBundleInfoHelper = new MessageResponseHelper<IQMessage>(MESSAGE_SENDER, onRequestBundleInformationMessage, onTimeout);
            OmemoRequestBundleInformationMessage msg = new OmemoRequestBundleInformationMessage(BARE_ACCOUNT_JID, CHAT_JID, remoteDeviceId);
            requestBundleInfoHelper.start(msg);
        }

        private void onTimeout()
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
                    Logger.Error("[OmemoSessionBuildHelper] Failed to establish session - " + CHAT_JID + " didn't respond in time!");
                    setState(OmemoSessionBuildHelperState.ERROR);
                    ON_SESSION_RESULT(this, new OmemoSessionBuildResult(OmemoSessionBuildError.REQUEST_BUNDLE_INFORMATION_TIMEOUT));
                    break;
            }
        }

        private void createSessionsForDevices(List<uint> devices)
        {
            toDoDevices = devices;
            createSessionForNextDevice();
        }

        private void createSessionForNextDevice()
        {
            if (toDoDevices == null || toDoDevices.Count <= 0)
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
                curDevice = toDoDevices[0];
                toDoDevices.RemoveAt(0);
                SignalProtocolAddress address = new SignalProtocolAddress(CHAT_JID, curDevice);
                if (OMEMO_HELPER.containsSession(address))
                {
                    SessionCipher cipher = OMEMO_HELPER.loadCipher(address);
                    SESSION.DEVICE_SESSIONS.Add(curDevice, cipher);
                    createSessionForNextDevice();
                }
                else
                {
                    requestBundleInformation(curDevice);
                }
            }
        }

        private bool onRequestDeviceListMessage(IQMessage msg)
        {
            if (STATE != OmemoSessionBuildHelperState.REQUESTING_DEVICE_LIST)
            {
                return true;
            }

            if (msg is OmemoDeviceListResultMessage devMsg)
            {
                // Update devices in DB:
                string chatJid = Utils.getBareJidFromFullJid(devMsg.getFrom());
                OmemoDeviceDBManager.INSTANCE.setDevices(devMsg.DEVICES, chatJid, BARE_ACCOUNT_JID);

                if (devMsg.DEVICES.DEVICES.Count > 0)
                {
                    subscribeToDeviceList();
                    createSessionsForDevices(devMsg.DEVICES.DEVICES);
                }
                else
                {
                    Logger.Error("[OmemoSessionBuildHelper] Failed to establish session - " + CHAT_JID + " doesn't support OMEMO: No devices");
                    setState(OmemoSessionBuildHelperState.ERROR);
                    ON_SESSION_RESULT(this, new OmemoSessionBuildResult(OmemoSessionBuildError.TARGET_DOES_NOT_SUPPORT_OMEMO));
                }
                return true;
            }
            else if (msg is IQErrorMessage errMsg)
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
                return true;
            }
            return false;
        }

        private bool onSubscribeToDeviceListMessage(IQMessage msg)
        {
            if (STATE != OmemoSessionBuildHelperState.SUBSCRIBING_TO_DEVICE_LIST)
            {
                return true;
            }

            if (msg is PubSubSubscriptionMessage subMsg)
            {
                if (subMsg.SUBSCRIPTION != PubSubSubscription.SUBSCRIBED)
                {
                    Logger.Warn("[OmemoSessionBuildHelper] Failed to subscribe to device list node - " + CHAT_JID + " returned: " + subMsg.SUBSCRIPTION);
                    OmemoDeviceDBManager.INSTANCE.setDeviceListSubscription(new OmemoDeviceListSubscriptionTable(CHAT_JID, BARE_ACCOUNT_JID, OmemoDeviceListSubscriptionState.NONE, DateTime.Now));
                }
                else
                {
                    OmemoDeviceDBManager.INSTANCE.setDeviceListSubscription(new OmemoDeviceListSubscriptionTable(CHAT_JID, BARE_ACCOUNT_JID, OmemoDeviceListSubscriptionState.SUBSCRIBED, DateTime.Now));
                }
                return true;
            }
            else if (msg is IQErrorMessage errMsg)
            {
                if (errMsg.ERROR_OBJ.ERROR_NAME == ErrorName.ITEM_NOT_FOUND)
                {
                    Logger.Error("[OmemoSessionBuildHelper] Failed to subscribe to device list node - " + CHAT_JID + " returned node does not exist: " + errMsg.ERROR_OBJ.ToString());
                }
                else
                {
                    Logger.Warn("[OmemoSessionBuildHelper] Failed to subscribe to device list node: " + errMsg.ERROR_OBJ.ToString());
                }
                OmemoDeviceDBManager.INSTANCE.setDeviceListSubscription(new OmemoDeviceListSubscriptionTable(CHAT_JID, BARE_ACCOUNT_JID, OmemoDeviceListSubscriptionState.ERROR, DateTime.Now));
                return true;
            }
            return false;
        }

        private bool onRequestBundleInformationMessage(IQMessage msg)
        {
            if (STATE != OmemoSessionBuildHelperState.REQUESTING_BUNDLE_INFORMATION)
            {
                return true;
            }

            if (msg is OmemoBundleInformationResultMessage bundleMsg)
            {
                Logger.Info("[OmemoSessionBuildHelper] Session with " + CHAT_JID + ':' + curDevice + " established.");
                SignalProtocolAddress address = OMEMO_HELPER.newSession(CHAT_JID, bundleMsg);
                SessionCipher cipher = OMEMO_HELPER.loadCipher(address);
                SESSION.DEVICE_SESSIONS.Add(curDevice, cipher);
                createSessionForNextDevice();
                return true;
            }
            else if (msg is IQErrorMessage errMsg)
            {
                if (errMsg.ERROR_OBJ.ERROR_NAME == ErrorName.ITEM_NOT_FOUND)
                {
                    Logger.Error("[OmemoSessionBuildHelper] Failed to establish session - " + CHAT_JID + ':' + curDevice + " doesn't support OMEMO: " + errMsg.ERROR_OBJ.ToString());
                    setState(OmemoSessionBuildHelperState.ERROR);
                    ON_SESSION_RESULT(this, new OmemoSessionBuildResult(OmemoSessionBuildError.TARGET_DOES_NOT_SUPPORT_OMEMO));
                }
                else
                {
                    Logger.Error("[OmemoSessionBuildHelper] Failed to establish session - request bundle info failed(" + CHAT_JID + ':' + curDevice + "): " + errMsg.ERROR_OBJ.ToString());
                    setState(OmemoSessionBuildHelperState.ERROR);
                    ON_SESSION_RESULT(this, new OmemoSessionBuildResult(OmemoSessionBuildError.REQUEST_BUNDLE_INFORMATION_IQ_ERROR));
                }
                return true;
            }
            return false;
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
