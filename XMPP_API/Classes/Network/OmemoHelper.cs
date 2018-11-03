using libsignal;
using libsignal.state;
using Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XMPP_API.Classes.Crypto;
using XMPP_API.Classes.Network.XML.DBEntries;
using XMPP_API.Classes.Network.XML.DBManager;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.XEP_0060;
using XMPP_API.Classes.Network.XML.Messages.XEP_0384;
using XMPP_API.Classes.Network.XML.Messages.XEP_0384.Signal.Session;

namespace XMPP_API.Classes.Network
{
    public class OmemoHelper : IDisposable
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public OmemoHelperState STATE { get; private set; }
        public OmemoDevices DEVICES { get; private set; }

        private readonly XMPPConnection2 CONNECTION;

        private uint tmpDeviceId;
        private MessageResponseHelper<IQMessage> requestDeviceListHelper;
        private MessageResponseHelper<IQMessage> updateDeviceListHelper;
        private MessageResponseHelper<IQMessage> announceBundleInfoHelper;

        private MessageResponseHelper<IQMessage> requestDeviceListStatelessHelper;
        private MessageResponseHelper<IQMessage> resetDeviceListStatelessHelper;
        private Action<bool, OmemoDevices> requestDeviceListStatelessOnResult;
        private Action<bool> resetDeviceListStatelessOnResult;

        // Keep sessions during App runtime:
        private readonly Dictionary<string, OmemoSession> OMEMO_SESSIONS;
        private readonly Dictionary<string, Tuple<List<OmemoMessageMessage>, OmemoSessionBuildHelper>> MESSAGE_CACHE;
        private readonly SessionStore SESSION_STORE;
        private readonly PreKeyStore PRE_KEY_STORE;
        private readonly SignedPreKeyStore SIGNED_PRE_KEY_STORE;
        private readonly IdentityKeyStore IDENTITY_STORE;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 06/08/2018 Created [Fabian Sauter]
        /// </history>
        public OmemoHelper(XMPPConnection2 connection, SessionStore sessionStore, PreKeyStore preKeyStore, SignedPreKeyStore signedPreKeyStore, IdentityKeyStore identityKeyStore)
        {
            this.CONNECTION = connection;

            this.OMEMO_SESSIONS = new Dictionary<string, OmemoSession>();
            this.MESSAGE_CACHE = new Dictionary<string, Tuple<List<OmemoMessageMessage>, OmemoSessionBuildHelper>>();
            this.SESSION_STORE = sessionStore;
            this.PRE_KEY_STORE = preKeyStore;
            this.SIGNED_PRE_KEY_STORE = signedPreKeyStore;
            this.IDENTITY_STORE = identityKeyStore;

            reset();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private void setState(OmemoHelperState newState)
        {
            if (STATE != newState)
            {
                OmemoHelperState oldState = STATE;
                STATE = newState;
                Logger.Debug("[OMEMO HELPER](" + CONNECTION.account.getIdAndDomain() + ") " + oldState + " -> " + STATE);
                if (STATE == OmemoHelperState.ERROR)
                {
                    CONNECTION.NewValidMessage -= CONNECTION_ConnectionNewValidMessage;

                }
                else if (oldState == OmemoHelperState.ERROR && STATE != OmemoHelperState.ERROR)
                {
                    reset();
                }

                if (STATE == OmemoHelperState.ENABLED)
                {
                    Logger.Info("[OMEMO HELPER](" + CONNECTION.account.getIdAndDomain() + ") Enabled.");
                }
            }
        }

        public IList<uint> getDevicesIdsForChat(string chatJid)
        {
            return OmemoDeviceDBManager.INSTANCE.getDeviceIds(chatJid, CONNECTION.account.getIdAndDomain());
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public SessionCipher loadCipher(SignalProtocolAddress address)
        {
            return new SessionCipher(SESSION_STORE, PRE_KEY_STORE, SIGNED_PRE_KEY_STORE, IDENTITY_STORE, address);
        }

        public bool containsSession(SignalProtocolAddress address)
        {
            return SESSION_STORE.ContainsSession(address);
        }

        public void Dispose()
        {
            CONNECTION.ConnectionStateChanged -= CONNECTION_ConnectionStateChanged;
            CONNECTION.NewValidMessage -= CONNECTION_ConnectionNewValidMessage;
        }

        public void reset()
        {
            setState(OmemoHelperState.DISABLED);

            CONNECTION.ConnectionStateChanged -= CONNECTION_ConnectionStateChanged;
            CONNECTION.ConnectionStateChanged += CONNECTION_ConnectionStateChanged;

            CONNECTION.NewValidMessage -= CONNECTION_ConnectionNewValidMessage;
            CONNECTION.NewValidMessage += CONNECTION_ConnectionNewValidMessage;

            tmpDeviceId = 0;

            if (requestDeviceListHelper != null)
            {
                requestDeviceListHelper.Dispose();
                requestDeviceListHelper = null;
            }
            if (updateDeviceListHelper != null)
            {
                updateDeviceListHelper.Dispose();
                updateDeviceListHelper = null;
            }
            if (announceBundleInfoHelper != null)
            {
                announceBundleInfoHelper.Dispose();
                announceBundleInfoHelper = null;
            }
            if (requestDeviceListStatelessHelper != null)
            {
                requestDeviceListStatelessHelper.Dispose();
                requestDeviceListStatelessHelper = null;
            }
            if (resetDeviceListStatelessHelper != null)
            {
                resetDeviceListStatelessHelper.Dispose();
                resetDeviceListStatelessHelper = null;
            }

            requestDeviceListStatelessOnResult = null;
            resetDeviceListStatelessOnResult = null;
        }

        public SignalProtocolAddress newSession(string chatJid, OmemoBundleInformationResultMessage bundleInfoMsg)
        {
            return newSession(chatJid, bundleInfoMsg.DEVICE_ID, bundleInfoMsg.BUNDLE_INFO.getRandomPreKey(bundleInfoMsg.DEVICE_ID));
        }

        public SignalProtocolAddress newSession(string chatJid, uint recipientDeviceId, PreKeyBundle recipientPreKey)
        {
            SignalProtocolAddress address = new SignalProtocolAddress(chatJid, recipientDeviceId);
            SessionBuilder builder = new SessionBuilder(SESSION_STORE, PRE_KEY_STORE, SIGNED_PRE_KEY_STORE, IDENTITY_STORE, address);
            builder.process(recipientPreKey);
            return address;
        }

        public void sendOmemoMessage(OmemoMessageMessage msg, string chatJid, string accountJid)
        {
            // Check if already trying to build a new session:
            if (MESSAGE_CACHE.ContainsKey(chatJid))
            {
                MESSAGE_CACHE[chatJid].Item1.Add(msg);
            }
            else
            {
                // If not start a new session build helper:
                OmemoDeviceListSubscriptionTable subscriptionTable = OmemoDeviceDBManager.INSTANCE.getDeviceListSubscription(chatJid, accountJid);
                OmemoSessionBuildHelper sessionHelper = new OmemoSessionBuildHelper(chatJid, accountJid, CONNECTION.account.getIdDomainAndResource(), onSessionBuilderResult, CONNECTION, this);
                MESSAGE_CACHE[chatJid] = new Tuple<List<OmemoMessageMessage>, OmemoSessionBuildHelper>(new List<OmemoMessageMessage>(), sessionHelper);
                MESSAGE_CACHE[chatJid].Item1.Add(msg);
                sessionHelper.start(subscriptionTable.state);
            }
        }

        public void onOmemoDeviceListEventMessage(OmemoDeviceListEventMessage msg)
        {
            string chatJid = Utils.getBareJidFromFullJid(msg.getFrom());
            OmemoDeviceDBManager.INSTANCE.setDevices(msg.DEVICES, chatJid, CONNECTION.account.getIdAndDomain());
            OmemoDeviceDBManager.INSTANCE.setDeviceListSubscription(new OmemoDeviceListSubscriptionTable(chatJid, CONNECTION.account.getIdAndDomain(), OmemoDeviceListSubscriptionState.SUBSCRIBED, DateTime.Now));
        }

        public void requestDeviceListStateless(Action<bool, OmemoDevices> onResult)
        {
            requestDeviceListStatelessOnResult = onResult;

            if (requestDeviceListStatelessHelper != null)
            {
                requestDeviceListStatelessHelper.Dispose();
                requestDeviceListStatelessHelper = null;
            }
            requestDeviceListStatelessHelper = new MessageResponseHelper<IQMessage>(CONNECTION, onRequestDeviceListStatelessMessage, onRequestDeviceListStatelessTimeout);
            OmemoRequestDeviceListMessage msg = new OmemoRequestDeviceListMessage(CONNECTION.account.getIdDomainAndResource(), null);
            requestDeviceListStatelessHelper.start(msg);
        }

        public void resetDeviceListStateless(Action<bool> onResult)
        {
            resetDeviceListStatelessOnResult = onResult;
            deleteDeviceListNode();
        }

        #endregion

        #region --Misc Methods (Private)--
        private void deleteDeviceListNode()
        {
            if (resetDeviceListStatelessHelper != null)
            {
                resetDeviceListStatelessHelper.Dispose();
                resetDeviceListStatelessHelper = null;
            }

            resetDeviceListStatelessHelper = new MessageResponseHelper<IQMessage>(CONNECTION, onDeleteDeviceListNodeMessage, onDeleteDeviceListNodeTimeout);
            PubSubDeleteNodeMessage msg = new PubSubDeleteNodeMessage(CONNECTION.account.getIdDomainAndResource(), null, Consts.XML_XEP_0384_DEVICE_LIST_NODE);
            resetDeviceListStatelessHelper.start(msg);
        }

        private void setDeviceListToOwnDevice()
        {
            if (resetDeviceListStatelessHelper != null)
            {
                resetDeviceListStatelessHelper.Dispose();
                resetDeviceListStatelessHelper = null;
            }
            OmemoDevices devices = new OmemoDevices();
            devices.DEVICES.Add(CONNECTION.account.omemoDeviceId);
            resetDeviceListStatelessHelper = new MessageResponseHelper<IQMessage>(CONNECTION, onResetDeviceListStatelessMessage, onResetDeviceListStatelessTimeout);
            OmemoSetDeviceListMessage msg = new OmemoSetDeviceListMessage(CONNECTION.account.getIdDomainAndResource(), devices);
            resetDeviceListStatelessHelper.start(msg);
        }

        private bool onDeleteDeviceListNodeMessage(MessageResponseHelper<IQMessage> helper, AbstractMessage msg)
        {
            if (msg is IQErrorMessage || (msg is IQMessage iMsg && string.Equals(iMsg.TYPE, IQMessage.RESULT)))
            {
                setDeviceListToOwnDevice();
                return true;
            }
            return false;
        }

        private void onDeleteDeviceListNodeTimeout(MessageResponseHelper<IQMessage> helper)
        {
            setDeviceListToOwnDevice();
        }

        private bool onResetDeviceListStatelessMessage(MessageResponseHelper<IQMessage> helper, IQMessage msg)
        {
            if (msg is IQErrorMessage errMsg)
            {
                resetDeviceListStatelessOnResult?.Invoke(false);
                resetDeviceListStatelessOnResult = null;
                return true;
            }
            else if (msg is IQMessage)
            {
                DEVICES.DEVICES.Clear();
                DEVICES.DEVICES.Add(CONNECTION.account.omemoDeviceId);
                resetDeviceListStatelessOnResult?.Invoke(true);
                resetDeviceListStatelessOnResult = null;
                return true;
            }
            return false;
        }

        private void onResetDeviceListStatelessTimeout(MessageResponseHelper<IQMessage> helper)
        {
            resetDeviceListStatelessOnResult?.Invoke(false);
            resetDeviceListStatelessOnResult = null;
        }

        private bool onRequestDeviceListStatelessMessage(MessageResponseHelper<IQMessage> helper, IQMessage msg)
        {
            if (msg is OmemoDeviceListResultMessage devMsg)
            {
                requestDeviceListStatelessOnResult?.Invoke(true, devMsg.DEVICES);
                requestDeviceListStatelessOnResult = null;
                return true;
            }
            else if (msg is IQErrorMessage errMsg)
            {
                if (errMsg.ERROR_OBJ.ERROR_NAME == ErrorName.ITEM_NOT_FOUND)
                {
                    requestDeviceListStatelessOnResult?.Invoke(true, new OmemoDevices());
                }
                else
                {
                    requestDeviceListStatelessOnResult?.Invoke(false, null);
                }
                requestDeviceListStatelessOnResult = null;
                return true;
            }
            return false;
        }

        private void onRequestDeviceListStatelessTimeout(MessageResponseHelper<IQMessage> helper)
        {
            requestDeviceListStatelessOnResult?.Invoke(false, null);
            requestDeviceListStatelessOnResult = null;
        }

        private void onSessionBuilderResult(OmemoSessionBuildHelper sender, OmemoSessionBuildResult result)
        {
            if (result.SUCCESS)
            {
                OMEMO_SESSIONS[result.SESSION.CHAT_JID] = result.SESSION;
                Task.Run(() => sendAllOutstandingMessagesAsync(result.SESSION));
            }
            else
            {
                Logger.Error("Failed to build OMEMO session for: " + sender.CHAT_JID);
                // ToDo: Error handling - show message
            }
        }

        private async Task sendAllOutstandingMessagesAsync(OmemoSession omemoSession)
        {
            Tuple<List<OmemoMessageMessage>, OmemoSessionBuildHelper> cache = MESSAGE_CACHE[omemoSession.CHAT_JID];
            foreach (OmemoMessageMessage msg in cache.Item1)
            {
                msg.encrypt(omemoSession, CONNECTION.account.omemoDeviceId);
                await CONNECTION.sendAsync(msg, true, false);
            }
            cache.Item2.Dispose();
            MESSAGE_CACHE.Remove(omemoSession.CHAT_JID);
            Logger.Info("[OMEMO HELPER] Send all outstanding OMEMO messages for: " + omemoSession.CHAT_JID);
        }

        private void requestDeviceList()
        {
            setState(OmemoHelperState.REQUESTING_DEVICE_LIST);
            Logger.Info("[OMEMO HELPER](" + CONNECTION.account.getIdAndDomain() + ") Requesting device list.");
            if (requestDeviceListHelper != null)
            {
                requestDeviceListHelper.Dispose();
            }
            requestDeviceListHelper = new MessageResponseHelper<IQMessage>(CONNECTION, onRequestDeviceListMsg, onTimeout);
            OmemoRequestDeviceListMessage msg = new OmemoRequestDeviceListMessage(CONNECTION.account.getIdDomainAndResource(), null);
            requestDeviceListHelper.start(msg);
        }

        private bool onRequestDeviceListMsg(MessageResponseHelper<IQMessage> helper, AbstractMessage msg)
        {
            if (msg is OmemoDeviceListResultMessage devMsg)
            {
                updateDevicesIfNeeded(devMsg.DEVICES);
                return true;
            }
            else if (msg is IQErrorMessage errMsg)
            {
                if (errMsg.ERROR_OBJ.ERROR_NAME == ErrorName.ITEM_NOT_FOUND)
                {
                    Logger.Warn("[OMEMO HELPER](" + CONNECTION.account.getIdAndDomain() + ") Failed to request OMEMO device list - node does not exist. Creating node.");
                    updateDevicesIfNeeded(null);
                }
                else
                {
                    Logger.Error("[OMEMO HELPER](" + CONNECTION.account.getIdAndDomain() + ") Failed to request OMEMO device list form: " + CONNECTION.account.user.domain + "\n" + errMsg.ERROR_OBJ.ToString());
                    setState(OmemoHelperState.ERROR);
                }
                return true;
            }
            return false;
        }

        private void announceBundleInfo()
        {
            setState(OmemoHelperState.ANNOUNCING_BUNDLE_INFO);
            Logger.Info("[OMEMO HELPER](" + CONNECTION.account.getIdAndDomain() + ") Announcing bundle information for: " + CONNECTION.account.omemoDeviceId);
            if (announceBundleInfoHelper != null)
            {
                announceBundleInfoHelper.Dispose();
            }
            announceBundleInfoHelper = new MessageResponseHelper<IQMessage>(CONNECTION, announceBundleInfoMsg, onTimeout);
            OmemoSetBundleInformationMessage msg = new OmemoSetBundleInformationMessage(CONNECTION.account.getIdDomainAndResource(), CONNECTION.account.getOmemoBundleInformation(), CONNECTION.account.omemoDeviceId);
            announceBundleInfoHelper.start(msg);
        }

        private bool announceBundleInfoMsg(MessageResponseHelper<IQMessage> helper, AbstractMessage msg)
        {
            if (msg is IQErrorMessage errMsg)
            {
                Logger.Error("[OMEMO HELPER](" + CONNECTION.account.getIdAndDomain() + ") Failed to announce OMEMO bundle info to: " + CONNECTION.account.user.domain + "\n" + errMsg.ERROR_OBJ.ToString());
                setState(OmemoHelperState.ERROR);
                return true;
            }
            else if (msg is IQMessage)
            {
                Logger.Info("[OMEMO HELPER](" + CONNECTION.account.getIdAndDomain() + ") Bundle info announced.");
                setState(OmemoHelperState.ENABLED);
                CONNECTION.account.omemoBundleInfoAnnounced = true;
                CONNECTION.account.onPropertyChanged(nameof(CONNECTION.account.omemoBundleInfoAnnounced));
                return true;
            }
            return false;
        }

        private void updateDevicesIfNeeded(OmemoDevices devicesRemote)
        {
            if (devicesRemote == null)
            {
                devicesRemote = new OmemoDevices();
            }

            bool updateDeviceList = false;
            // Device id hasn't been set. Pick a random, unique one:
            if (CONNECTION.account.omemoDeviceId == 0)
            {
                tmpDeviceId = CryptoUtils.generateOmemoDeviceIds(devicesRemote.DEVICES);
                devicesRemote.DEVICES.Add(tmpDeviceId);
                updateDeviceList = true;
            }
            else
            {
                if (!devicesRemote.DEVICES.Contains(CONNECTION.account.omemoDeviceId))
                {
                    devicesRemote.DEVICES.Add(CONNECTION.account.omemoDeviceId);
                    updateDeviceList = true;
                }
            }

            if (updateDeviceList)
            {
                setState(OmemoHelperState.UPDATING_DEVICE_LIST);
                Logger.Info("[OMEMO HELPER](" + CONNECTION.account.getIdAndDomain() + ") Updating device list.");
                if (updateDeviceListHelper != null)
                {
                    updateDeviceListHelper.Dispose();
                }
                updateDeviceListHelper = new MessageResponseHelper<IQMessage>(CONNECTION, updateDevicesIfNeededMsg, onTimeout);
                OmemoSetDeviceListMessage msg = new OmemoSetDeviceListMessage(CONNECTION.account.getIdDomainAndResource(), devicesRemote);
                updateDeviceListHelper.start(msg);
            }
            else if (!CONNECTION.account.omemoBundleInfoAnnounced)
            {
                announceBundleInfo();
            }
            else
            {
                setState(OmemoHelperState.ENABLED);
            }
            DEVICES = devicesRemote;
        }

        private bool updateDevicesIfNeededMsg(MessageResponseHelper<IQMessage> helper, AbstractMessage msg)
        {
            if (msg is IQErrorMessage errMsg)
            {
                Logger.Error("[OMEMO HELPER](" + CONNECTION.account.getIdAndDomain() + ") Failed to set OMEMO device list to: " + CONNECTION.account.user.domain + "\n" + errMsg.ERROR_OBJ.ToString());
                setState(OmemoHelperState.ERROR);
                return true;
            }
            else if (msg is IQMessage)
            {
                Logger.Info("[OMEMO HELPER](" + CONNECTION.account.getIdAndDomain() + ") Device list updated.");
                if (CONNECTION.account.omemoDeviceId == 0)
                {
                    CONNECTION.account.omemoDeviceId = tmpDeviceId;
                    CONNECTION.account.onPropertyChanged(nameof(CONNECTION.account.omemoDeviceId));
                }
                if (!CONNECTION.account.omemoBundleInfoAnnounced)
                {
                    announceBundleInfo();
                }
                else
                {
                    setState(OmemoHelperState.ENABLED);
                }
                return true;
            }
            return false;
        }

        private void onTimeout(MessageResponseHelper<IQMessage> helper)
        {
            switch (STATE)
            {
                case OmemoHelperState.REQUESTING_DEVICE_LIST:
                case OmemoHelperState.UPDATING_DEVICE_LIST:
                case OmemoHelperState.ANNOUNCING_BUNDLE_INFO:
                    Logger.Error("[OMEMO HELPER](" + CONNECTION.account.getIdAndDomain() + ") Failed in state " + STATE + " - timeout!");
                    setState(OmemoHelperState.ERROR);
                    break;
            }
        }

        /// <summary>
        /// If the OMEMO device list got changed and does not contain the local device id update it and add it again.
        /// </summary>
        /// <param name="msg">The received OmemoDeviceListEventMessage.</param>
        private async Task onOmemoDeviceListEventMessageAsync(OmemoDeviceListEventMessage msg)
        {
            if (!msg.DEVICES.DEVICES.Contains(CONNECTION.account.omemoDeviceId))
            {
                msg.DEVICES.DEVICES.Add(CONNECTION.account.omemoDeviceId);
                OmemoSetDeviceListMessage setMsg = new OmemoSetDeviceListMessage(CONNECTION.account.getIdDomainAndResource(), msg.DEVICES);
                await CONNECTION.sendAsync(setMsg, false, false);
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private async void CONNECTION_ConnectionNewValidMessage(IMessageSender sender, Events.NewValidMessageEventArgs args)
        {
            if (args.MESSAGE is OmemoDeviceListEventMessage eventMsg)
            {
                await onOmemoDeviceListEventMessageAsync(eventMsg);
            }
        }

        private void CONNECTION_ConnectionStateChanged(AbstractConnection2 connection, Events.ConnectionStateChangedEventArgs arg)
        {
            switch (arg.newState)
            {
                case ConnectionState.CONNECTED:
                    if (!CONNECTION.account.hasOmemoKeys())
                    {
                        setState(OmemoHelperState.ERROR);
                        Logger.Error("[OMEMO HELPER](" + CONNECTION.account.getIdAndDomain() + ") Failed - no keys!");
                    }
                    else if (STATE == OmemoHelperState.DISABLED)
                    {
                        requestDeviceList();
                    }
                    break;

                case ConnectionState.DISCONNECTED:
                case ConnectionState.ERROR:
                    reset();
                    break;
            }
        }

        #endregion
    }
}
