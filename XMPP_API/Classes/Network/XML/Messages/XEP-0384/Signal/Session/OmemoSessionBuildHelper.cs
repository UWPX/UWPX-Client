using Logging;
using System;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0384.Signal.Session
{
    public class OmemoSessionBuildHelper : IDisposable
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public OmemoSessionBuildHelperState STATE { get; private set; }

        private readonly Action<OmemoSessionBuildResult> ON_SESSION_RESULT;
        private readonly IMessageSender MESSAGE_SENDER;
        private readonly string CHAT_JID;
        private readonly string ACCOUNT_JID;
        private readonly OmemoHelper OMEMO_HELPER;
        MessageResponseHelper<IQMessage> requestDeviceListHelper;
        MessageResponseHelper<IQMessage> requestBundleInfoHelper;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 10/08/2018 Created [Fabian Sauter]
        /// </history>
        internal OmemoSessionBuildHelper(string chatJid, string accountJid, Action<OmemoSessionBuildResult> onSessionResult, IMessageSender messageSender, OmemoHelper omemoHelper)
        {
            this.MESSAGE_SENDER = messageSender;
            this.ON_SESSION_RESULT = onSessionResult;
            this.CHAT_JID = chatJid;
            this.ACCOUNT_JID = accountJid;
            this.OMEMO_HELPER = omemoHelper;
            this.STATE = OmemoSessionBuildHelperState.NOT_STARTED;
            this.requestDeviceListHelper = null;
            this.requestBundleInfoHelper = null;
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
        public void start()
        {
            requestDeviceList();
        }

        public void Dispose()
        {
            requestDeviceListHelper?.Dispose();
            requestBundleInfoHelper?.Dispose();
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
            OmemoRequestDeviceListMessage msg = new OmemoRequestDeviceListMessage(ACCOUNT_JID, CHAT_JID);
            requestDeviceListHelper.start(msg);
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
            OmemoRequestBundleInformationMessage msg = new OmemoRequestBundleInformationMessage(ACCOUNT_JID, CHAT_JID, remoteDeviceId);
            requestBundleInfoHelper.start(msg);
        }

        private void onTimeout()
        {
            switch (STATE)
            {
                case OmemoSessionBuildHelperState.REQUESTING_DEVICE_LIST:
                    Logger.Error("[OmemoSessionBuildHelper] Failed to establish session - " + CHAT_JID + " didn't respond in time!");
                    setState(OmemoSessionBuildHelperState.ERROR);
                    ON_SESSION_RESULT(new OmemoSessionBuildResult(OmemoSessionBuildError.REQUEST_DEVICE_LIST_TIMEOUT));
                    break;

                case OmemoSessionBuildHelperState.REQUESTING_BUNDLE_INFORMATION:
                    Logger.Error("[OmemoSessionBuildHelper] Failed to establish session - " + CHAT_JID + " didn't respond in time!");
                    setState(OmemoSessionBuildHelperState.ERROR);
                    ON_SESSION_RESULT(new OmemoSessionBuildResult(OmemoSessionBuildError.REQUEST_BUNDLE_INFORMATION_TIMEOUT));
                    break;
            }
        }

        private bool onRequestDeviceListMessage(IQMessage msg)
        {
            if (msg is OmemoDeviceListResultMessage devMsg)
            {
                if (devMsg.DEVICES.DEVICES.Count > 0)
                {
                    uint deviceId = devMsg.DEVICES.getRandomDeviceId();
                    requestBundleInformation(deviceId);
                }
                else
                {
                    Logger.Error("[OmemoSessionBuildHelper] Failed to establish session - " + CHAT_JID + " doesn't support OMEMO: No devices");
                    setState(OmemoSessionBuildHelperState.ERROR);
                    ON_SESSION_RESULT(new OmemoSessionBuildResult(OmemoSessionBuildError.TARGET_DOES_NOT_SUPPORT_OMEMO));
                }
                return true;
            }
            else if (msg is IQErrorMessage errMsg)
            {
                if (errMsg.ERROR_OBJ.ERROR_NAME == ErrorName.ITEM_NOT_FOUND)
                {
                    Logger.Error("[OmemoSessionBuildHelper] Failed to establish session - " + CHAT_JID + " doesn't support OMEMO: " + errMsg.ERROR_OBJ.ToString());
                    setState(OmemoSessionBuildHelperState.ERROR);
                    ON_SESSION_RESULT(new OmemoSessionBuildResult(OmemoSessionBuildError.TARGET_DOES_NOT_SUPPORT_OMEMO));
                }
                else
                {
                    Logger.Error("[OmemoSessionBuildHelper] Failed to establish session - request device list failed: " + errMsg.ERROR_OBJ.ToString());
                    setState(OmemoSessionBuildHelperState.ERROR);
                    ON_SESSION_RESULT(new OmemoSessionBuildResult(OmemoSessionBuildError.REQUEST_DEVICE_LIST_IQ_ERROR));
                }
                return true;
            }
            return false;
        }

        private bool onRequestBundleInformationMessage(IQMessage msg)
        {
            if (msg is OmemoBundleInformationResultMessage bundleMsg)
            {
                Logger.Info("[OmemoSessionBuildHelper] Session with " + CHAT_JID + " established.");
                setState(OmemoSessionBuildHelperState.ESTABLISHED);
                ON_SESSION_RESULT(new OmemoSessionBuildResult(OMEMO_HELPER.newSession(CHAT_JID, bundleMsg)));
                return true;
            }
            else if (msg is IQErrorMessage errMsg)
            {
                if (errMsg.ERROR_OBJ.ERROR_NAME == ErrorName.ITEM_NOT_FOUND)
                {
                    Logger.Error("[OmemoSessionBuildHelper] Failed to establish session - " + CHAT_JID + " doesn't support OMEMO: " + errMsg.ERROR_OBJ.ToString());
                    setState(OmemoSessionBuildHelperState.ERROR);
                    ON_SESSION_RESULT(new OmemoSessionBuildResult(OmemoSessionBuildError.TARGET_DOES_NOT_SUPPORT_OMEMO));
                }
                else
                {
                    Logger.Error("[OmemoSessionBuildHelper] Failed to establish session - request device list failed: " + errMsg.ERROR_OBJ.ToString());
                    setState(OmemoSessionBuildHelperState.ERROR);
                    ON_SESSION_RESULT(new OmemoSessionBuildResult(OmemoSessionBuildError.REQUEST_BUNDLE_INFORMATION_IQ_ERROR));
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
