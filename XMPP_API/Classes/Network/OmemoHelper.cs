using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using libsignal;
using libsignal.state;
using Logging;
using XMPP_API.Classes.Crypto;
using XMPP_API.Classes.Events;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.Helper;
using XMPP_API.Classes.Network.XML.Messages.XEP_0384;
using XMPP_API.Classes.Network.XML.Messages.XEP_0384.Signal.Session;

namespace XMPP_API.Classes.Network
{
    public class OmemoHelper: IDisposable
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public OmemoHelperState STATE { get; private set; }
        public OmemoDevices DEVICES { get; private set; }

        private readonly XMPPConnection2 CONNECTION;

        private uint tmpDeviceId;

        // Keep sessions during App runtime:
        private readonly Dictionary<string, OmemoSession> OMEMO_SESSIONS;
        private readonly Dictionary<string, Tuple<List<OmemoMessageMessage>, OmemoSessionBuildHelper>> MESSAGE_CACHE;
        public readonly IOmemoStore OMEMO_STORE;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 06/08/2018 Created [Fabian Sauter]
        /// </history>
        public OmemoHelper(XMPPConnection2 connection, IOmemoStore omemoStore)
        {
            CONNECTION = connection;
            OMEMO_STORE = omemoStore;

            OMEMO_SESSIONS = new Dictionary<string, OmemoSession>();
            MESSAGE_CACHE = new Dictionary<string, Tuple<List<OmemoMessageMessage>, OmemoSessionBuildHelper>>();

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
                Logger.Debug("[OMEMO HELPER](" + CONNECTION.account.getBareJid() + ") " + oldState + " -> " + STATE);
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
                    Logger.Info("[OMEMO HELPER](" + CONNECTION.account.getBareJid() + ") Enabled.");
                }
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public SessionCipher loadCipher(SignalProtocolAddress address)
        {
            return new SessionCipher(OMEMO_STORE, address);
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
        }

        public SignalProtocolAddress newSession(string chatJid, OmemoBundleInformationResultMessage bundleInfoMsg)
        {
            return newSession(chatJid, bundleInfoMsg.DEVICE_ID, bundleInfoMsg.BUNDLE_INFO.getRandomPreKey(bundleInfoMsg.DEVICE_ID));
        }

        public SignalProtocolAddress newSession(string chatJid, uint recipientDeviceId, PreKeyBundle recipientPreKey)
        {
            SignalProtocolAddress address = new SignalProtocolAddress(chatJid, recipientDeviceId);
            SessionBuilder builder = new SessionBuilder(OMEMO_STORE, address);
            builder.process(recipientPreKey);
            return address;
        }

        public async Task sendOmemoMessageAsync(OmemoMessageMessage msg, string chatJid, string accountJid)
        {
            // Check if already trying to build a new session:
            if (MESSAGE_CACHE.ContainsKey(chatJid))
            {
                MESSAGE_CACHE[chatJid].Item1.Add(msg);
            }
            else
            {
                // If not start a new session build helper:
                OmemoSessionBuildHelper sessionHelper = new OmemoSessionBuildHelper(chatJid, accountJid, CONNECTION.account.getFullJid(), CONNECTION, this);
                MESSAGE_CACHE[chatJid] = new Tuple<List<OmemoMessageMessage>, OmemoSessionBuildHelper>(new List<OmemoMessageMessage>(), sessionHelper);
                MESSAGE_CACHE[chatJid].Item1.Add(msg);
                Tuple<OmemoDeviceListSubscriptionState, DateTime> subscription = OMEMO_STORE.LoadDeviceListSubscription(chatJid);

                OmemoSessionBuildResult result = await sessionHelper.buildSessionAsync(subscription.Item1);

                if (result.SUCCESS)
                {
                    OMEMO_SESSIONS[result.SESSION.CHAT_JID] = result.SESSION;
                    await sendAllOutstandingMessagesAsync(result.SESSION);
                }
                else
                {
                    OmemoSessionBuildErrorEventArgs args = new OmemoSessionBuildErrorEventArgs(chatJid, result.ERROR, MESSAGE_CACHE[chatJid]?.Item1 ?? new List<OmemoMessageMessage>());
                    MESSAGE_CACHE.Remove(chatJid);
                    CONNECTION.onOmemoSessionBuildError(args);
                    Logger.Error("Failed to build OMEMO session for: " + chatJid + " with: " + result.ERROR);
                }
            }
        }

        public async Task removePreKeyAndRepublishAsync(uint preKeyId)
        {
            CONNECTION.account.replaceOmemoPreKey(preKeyId, OMEMO_STORE);
            await announceBundleInfoAsync();
        }

        #endregion

        #region --Misc Methods (Private)--
        private async Task sendAllOutstandingMessagesAsync(OmemoSession omemoSession)
        {
            Tuple<List<OmemoMessageMessage>, OmemoSessionBuildHelper> cache = MESSAGE_CACHE[omemoSession.CHAT_JID];
            foreach (OmemoMessageMessage msg in cache.Item1)
            {
                msg.encrypt(omemoSession, CONNECTION.account.omemoDeviceId);
                await CONNECTION.sendAsync(msg, false);
            }
            MESSAGE_CACHE.Remove(omemoSession.CHAT_JID);
            Logger.Info("[OMEMO HELPER] Send all outstanding OMEMO messages for: " + omemoSession.CHAT_JID + " to " + omemoSession.DEVICE_SESSIONS_OWN.Count + " own and " + omemoSession.DEVICE_SESSIONS_REMOTE.Count + " remote recipient(s).");
        }

        private async Task requestDeviceListAsync()
        {
            setState(OmemoHelperState.REQUESTING_DEVICE_LIST);

            Logger.Info("[OMEMO HELPER](" + CONNECTION.account.getBareJid() + ") Requesting device list.");
            MessageResponseHelperResult<IQMessage> result = await CONNECTION.OMEMO_COMMAND_HELPER.requestDeviceListAsync(CONNECTION.account.getBareJid());

            if (result.STATE == MessageResponseHelperResultState.SUCCESS)
            {
                if (result.RESULT is OmemoDeviceListResultMessage devMsg)
                {
                    await updateDevicesIfNeededAsync(devMsg.DEVICES);
                }
                else if (result.RESULT is IQErrorMessage errMsg)
                {
                    if (errMsg.ERROR_OBJ.ERROR_NAME == ErrorName.ITEM_NOT_FOUND)
                    {
                        Logger.Warn("[OMEMO HELPER](" + CONNECTION.account.getBareJid() + ") Failed to request OMEMO device list - node does not exist. Creating node.");
                        await updateDevicesIfNeededAsync(null);
                    }
                    else
                    {
                        Logger.Error("[OMEMO HELPER](" + CONNECTION.account.getBareJid() + ") Failed to request OMEMO device list form: " + CONNECTION.account.user.domainPart + "\n" + errMsg.ERROR_OBJ.ToString());
                        setState(OmemoHelperState.ERROR);
                    }
                }
            }
            else
            {
                onRequestError(result);
            }
        }

        private async Task announceBundleInfoAsync()
        {
            setState(OmemoHelperState.ANNOUNCING_BUNDLE_INFO);

            Logger.Info("[OMEMO HELPER](" + CONNECTION.account.getBareJid() + ") Announcing bundle information for: " + CONNECTION.account.omemoDeviceId);
            MessageResponseHelperResult<IQMessage> result = await CONNECTION.OMEMO_COMMAND_HELPER.setBundleInfoAsync(CONNECTION.account.getOmemoBundleInformation(), CONNECTION.account.omemoDeviceId);

            if (result.STATE == MessageResponseHelperResultState.SUCCESS)
            {
                if (result.RESULT is IQMessage)
                {
                    Logger.Info("[OMEMO HELPER](" + CONNECTION.account.getBareJid() + ") Bundle info announced.");
                    setState(OmemoHelperState.ENABLED);
                    CONNECTION.account.omemoBundleInfoAnnounced = true;
                }
                else if (result.RESULT is IQErrorMessage errMsg)
                {
                    Logger.Error("[OMEMO HELPER](" + CONNECTION.account.getBareJid() + ") Failed to announce OMEMO bundle info to: " + CONNECTION.account.user.domainPart + "\n" + errMsg.ERROR_OBJ.ToString());
                    setState(OmemoHelperState.ERROR);
                }
            }
            else
            {
                onRequestError(result);
            }
        }

        private async Task updateDevicesIfNeededAsync(OmemoDevices devicesRemote)
        {
            if (devicesRemote is null)
            {
                devicesRemote = new OmemoDevices();
            }

            bool updateDeviceList = false;
            // Device id hasn't been set. Pick a random, unique one:
            if (CONNECTION.account.omemoDeviceId == 0)
            {
                tmpDeviceId = CryptoUtils.generateOmemoDeviceIds(devicesRemote.IDS.ToList());
                devicesRemote.IDS.Add(tmpDeviceId);
                updateDeviceList = true;
            }
            else
            {
                if (!devicesRemote.IDS.Contains(CONNECTION.account.omemoDeviceId))
                {
                    devicesRemote.IDS.Add(CONNECTION.account.omemoDeviceId);
                    updateDeviceList = true;
                }
            }

            if (updateDeviceList)
            {
                await updateDeviceListAsync(devicesRemote);
            }
            else if (!CONNECTION.account.omemoBundleInfoAnnounced)
            {
                await announceBundleInfoAsync();
            }
            else
            {
                setState(OmemoHelperState.ENABLED);
            }
            DEVICES = devicesRemote;
        }

        private async Task updateDeviceListAsync(OmemoDevices devices)
        {
            setState(OmemoHelperState.UPDATING_DEVICE_LIST);

            // Make sure we set the item ID to "current":
            devices.setId();

            Logger.Info("[OMEMO HELPER](" + CONNECTION.account.getBareJid() + ") Updating device list.");
            MessageResponseHelperResult<IQMessage> result = await CONNECTION.OMEMO_COMMAND_HELPER.setDeviceListAsync(devices);

            if (result.STATE == MessageResponseHelperResultState.SUCCESS)
            {
                if (result.RESULT is IQMessage)
                {
                    Logger.Info("[OMEMO HELPER](" + CONNECTION.account.getBareJid() + ") Device list updated.");
                    if (CONNECTION.account.omemoDeviceId == 0)
                    {
                        CONNECTION.account.omemoDeviceId = tmpDeviceId;
                    }
                    if (!CONNECTION.account.omemoBundleInfoAnnounced)
                    {
                        await announceBundleInfoAsync();
                    }
                    else
                    {
                        setState(OmemoHelperState.ENABLED);
                    }
                }
                else if (result.RESULT is IQErrorMessage errMsg)
                {
                    Logger.Error("[OMEMO HELPER](" + CONNECTION.account.getBareJid() + ") Failed to set OMEMO device list to: " + CONNECTION.account.user.domainPart + "\n" + errMsg.ERROR_OBJ.ToString());
                    setState(OmemoHelperState.ERROR);
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
                case OmemoHelperState.REQUESTING_DEVICE_LIST:
                case OmemoHelperState.UPDATING_DEVICE_LIST:
                case OmemoHelperState.ANNOUNCING_BUNDLE_INFO:
                    Logger.Error("[OMEMO HELPER](" + CONNECTION.account.getBareJid() + ") Failed in state " + STATE + " - timeout!");
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
            string senderBareJid = Utils.getBareJidFromFullJid(msg.getFrom());
            if (string.Equals(senderBareJid, CONNECTION.account.getBareJid()))
            {
                if (!msg.DEVICES.IDS.Contains(CONNECTION.account.omemoDeviceId))
                {
                    msg.DEVICES.IDS.Add(CONNECTION.account.omemoDeviceId);
                    OmemoSetDeviceListMessage setMsg = new OmemoSetDeviceListMessage(CONNECTION.account.getFullJid(), msg.DEVICES);
                    await CONNECTION.sendAsync(setMsg, false);
                }
                DEVICES = msg.DEVICES;
            }

            OMEMO_STORE.StoreDevices(msg.DEVICES.toSignalProtocolAddressList(senderBareJid));
            OMEMO_STORE.StoreDeviceListSubscription(senderBareJid, new Tuple<OmemoDeviceListSubscriptionState, DateTime>(OmemoDeviceListSubscriptionState.SUBSCRIBED, DateTime.Now));
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

        private async void CONNECTION_ConnectionStateChanged(AbstractConnection2 connection, Events.ConnectionStateChangedEventArgs arg)
        {
            switch (arg.newState)
            {
                case ConnectionState.CONNECTED:
                    if (!CONNECTION.account.checkOmemoKeys())
                    {
                        setState(OmemoHelperState.ERROR);
                        Logger.Error("[OMEMO HELPER](" + CONNECTION.account.getBareJid() + ") Failed - no keys!");
                    }
                    else if (STATE == OmemoHelperState.DISABLED)
                    {
                        await requestDeviceListAsync();
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
