using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Logging;
using XMPP_API.Classes.Events;
using XMPP_API.Classes.Network.Events;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.Helper;
using XMPP_API.Classes.Network.XML.Messages.XEP_0384;
using XMPP_API.Classes.Network.XML.Messages.XEP_0384.Session;

namespace XMPP_API.Classes.Network
{
    public class OmemoHelper: IDisposable
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public OmemoHelperState STATE { get; private set; }
        public OmemoXmlDevices DEVICES { get; private set; }

        private readonly XmppConnection CONNECTION;

        private uint tmpDeviceId;

        // Keep sessions during app runtime:
        private readonly Dictionary<string, OmemoSessions> OMEMO_SESSIONS = new Dictionary<string, OmemoSessions>();
        private readonly Dictionary<string, Tuple<List<OmemoEncryptedMessage>, OmemoSessionBuildHelper>> MESSAGE_CACHE = new Dictionary<string, Tuple<List<OmemoEncryptedMessage>, OmemoSessionBuildHelper>>();
        public readonly IExtendedOmemoStorage OMEMO_STORAGE;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public OmemoHelper(XmppConnection connection, IExtendedOmemoStorage omemoStorage)
        {
            CONNECTION = connection;
            OMEMO_STORAGE = omemoStorage;

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
                    CONNECTION.NewValidMessage -= OnNewValidMessage;

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
        public async Task initAsync()
        {
            if (!CONNECTION.account.checkOmemoKeys())
            {
                setState(OmemoHelperState.ERROR);
                Logger.Error("[OMEMO HELPER](" + CONNECTION.account.getBareJid() + ") Failed - no keys!");
            }
            else if (STATE == OmemoHelperState.DISABLED)
            {
                CONNECTION.ConnectionStateChanged -= OnConnectionStateChanged;
                CONNECTION.ConnectionStateChanged += OnConnectionStateChanged;

                CONNECTION.NewValidMessage -= OnNewValidMessage;
                CONNECTION.NewValidMessage += OnNewValidMessage;

                await requestDeviceListAsync();
            }
        }

        public void Dispose()
        {
            reset();
        }

        public async Task sendOmemoMessageAsync(OmemoEncryptedMessage msg, string srcBareJid, string dstBareJid)
        {
            // Check if already trying to build a new session:
            if (MESSAGE_CACHE.ContainsKey(dstBareJid))
            {
                MESSAGE_CACHE[dstBareJid].Item1.Add(msg);
            }
            else
            {
                // If not start a new session build helper:
                OmemoSessionBuildHelper sessionHelper = new OmemoSessionBuildHelper(srcBareJid, dstBareJid, CONNECTION, this);
                MESSAGE_CACHE[dstBareJid] = new Tuple<List<OmemoEncryptedMessage>, OmemoSessionBuildHelper>(new List<OmemoEncryptedMessage>(), sessionHelper);
                MESSAGE_CACHE[dstBareJid].Item1.Add(msg);
                Tuple<OmemoDeviceListSubscriptionState, DateTime> subscription = OMEMO_STORAGE.LoadDeviceListSubscription(dstBareJid);

                OmemoSessionBuildResult result = await sessionHelper.buildSessionAsync(subscription.Item1);

                if (result.SUCCESS)
                {
                    OMEMO_SESSIONS[dstBareJid] = result.SESSIONS;
                    await sendAllOutstandingMessagesAsync(result.SESSIONS);
                }
                else
                {
                    OmemoSessionBuildErrorEventArgs args = new OmemoSessionBuildErrorEventArgs(dstBareJid, result.ERROR, MESSAGE_CACHE[dstBareJid]?.Item1 ?? new List<OmemoEncryptedMessage>());
                    MESSAGE_CACHE.Remove(dstBareJid);
                    CONNECTION.OnOmemoSessionBuildError(args);
                    Logger.Error("Failed to build OMEMO session for: " + dstBareJid + " with: " + result.ERROR);
                }
            }
        }

        #endregion

        #region --Misc Methods (Private)--
        private void reset()
        {
            CONNECTION.ConnectionStateChanged -= OnConnectionStateChanged;
            CONNECTION.NewValidMessage -= OnNewValidMessage;

            setState(OmemoHelperState.DISABLED);
            tmpDeviceId = 0;
        }

        private async Task sendAllOutstandingMessagesAsync(OmemoSessions omemoSessions)
        {
            Tuple<List<OmemoEncryptedMessage>, OmemoSessionBuildHelper> cache = MESSAGE_CACHE[omemoSessions.DST_DEVICE_GROUP.BARE_JID];
            foreach (OmemoEncryptedMessage msg in cache.Item1)
            {
                msg.encrypt(CONNECTION.account.omemoDeviceId, CONNECTION.account.omemoIdentityKey, OMEMO_STORAGE, omemoSessions.toList());
                await CONNECTION.SendAsync(msg, false);
            }
            MESSAGE_CACHE.Remove(omemoSessions.DST_DEVICE_GROUP.BARE_JID);
            Logger.Info("[OMEMO HELPER] Send all outstanding OMEMO messages for: " + omemoSessions.DST_DEVICE_GROUP.BARE_JID + " to " + omemoSessions.SRC_DEVICE_GROUP.SESSIONS.Count + " own and " + omemoSessions.DST_DEVICE_GROUP.SESSIONS.Count + " remote recipient(s).");
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
            MessageResponseHelperResult<IQMessage> result = await CONNECTION.OMEMO_COMMAND_HELPER.setBundleInfoAsync(CONNECTION.account.getOmemoBundleInformation());

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

        private async Task updateDevicesIfNeededAsync(OmemoXmlDevices devicesRemote)
        {
            if (devicesRemote is null)
            {
                devicesRemote = new OmemoXmlDevices();
            }

            bool updateDeviceList = false;
            // Device id hasn't been set. Pick a random, unique one:
            if (CONNECTION.account.omemoDeviceId == 0)
            {
                tmpDeviceId = Omemo.Classes.CryptoUtils.GenerateUniqueDeviceId(devicesRemote.DEVICES.Select(d => d.ID).ToList());
                devicesRemote.DEVICES.Add(new OmemoXmlDevice(tmpDeviceId, CONNECTION.account.omemoDeviceLabel));
                updateDeviceList = true;
            }
            else
            {
                if (!devicesRemote.DEVICES.Any(d => d.ID == CONNECTION.account.omemoDeviceId))
                {
                    devicesRemote.DEVICES.Add(new OmemoXmlDevice(tmpDeviceId, CONNECTION.account.omemoDeviceLabel));
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

        private async Task updateDeviceListAsync(OmemoXmlDevices devices)
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
                if (!msg.DEVICES.DEVICES.Any(d => d.ID == CONNECTION.account.omemoDeviceId))
                {
                    msg.DEVICES.DEVICES.Add(new OmemoXmlDevice(CONNECTION.account.omemoDeviceId, CONNECTION.account.omemoDeviceLabel));
                    OmemoSetDeviceListMessage setMsg = new OmemoSetDeviceListMessage(CONNECTION.account.getFullJid(), msg.DEVICES);
                    await CONNECTION.SendAsync(setMsg, false);
                }
                DEVICES = msg.DEVICES;
            }

            OMEMO_STORAGE.StoreDevices(msg.DEVICES.toOmemoProtocolAddress(senderBareJid));
            OMEMO_STORAGE.StoreDeviceListSubscription(senderBareJid, new Tuple<OmemoDeviceListSubscriptionState, DateTime>(OmemoDeviceListSubscriptionState.SUBSCRIBED, DateTime.Now));
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private async void OnNewValidMessage(IMessageSender sender, NewValidMessageEventArgs args)
        {
            if (args.MESSAGE is OmemoDeviceListEventMessage eventMsg)
            {
                await onOmemoDeviceListEventMessageAsync(eventMsg);
            }
        }

        private void OnConnectionStateChanged(AbstractConnection sender, ConnectionStateChangedEventArgs arg)
        {
            if (arg.newState != ConnectionState.CONNECTED)
            {
                reset();
            }
        }

        #endregion
    }
}
