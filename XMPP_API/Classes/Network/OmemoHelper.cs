using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Logging;
using Omemo.Classes;
using Omemo.Classes.Keys;
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
            if (STATE == OmemoHelperState.DISABLED)
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

        public async Task refreshDevicesAsync()
        {
            MessageResponseHelperResult<IQMessage> result = await CONNECTION.OMEMO_COMMAND_HELPER.requestDeviceListAsync(CONNECTION.account.getBareJid());

            if (result.STATE == MessageResponseHelperResultState.SUCCESS)
            {
                if (result.RESULT is OmemoDeviceListResultMessage devMsg)
                {
                    DEVICES = devMsg.DEVICES;
                    // Store the new list in the DB:
                    OMEMO_STORAGE.StoreDevices(DEVICES.toOmemoProtocolAddress(CONNECTION.account.getBareJid()), CONNECTION.account.getBareJid());
                }
                else if (result.RESULT is IQErrorMessage errMsg)
                {
                    Logger.Error("[OMEMO HELPER](" + CONNECTION.account.getBareJid() + ") Failed to request OMEMO device list form: " + CONNECTION.account.user.domainPart + "\n" + errMsg.ERROR_OBJ.ToString());
                }
            }
            else
            {
                Logger.Error("[OMEMO HELPER](" + CONNECTION.account.getBareJid() + ") Failed to request OMEMO device list form: " + CONNECTION.account.user.domainPart + " with: " + result.STATE);
            }
        }

        public async Task sendOmemoMessageAsync(OmemoEncryptedMessage msg, string srcBareJid, string dstBareJid, bool trustedSrcKeysOnly, bool trustedDstKeysOnly)
        {
            // Check if already trying to build a new session:
            if (MESSAGE_CACHE.ContainsKey(dstBareJid))
            {
                MESSAGE_CACHE[dstBareJid].Item1.Add(msg);
            }
            else
            {
                // If not start a new session build helper:
                OmemoSessionBuildHelper sessionHelper = new OmemoSessionBuildHelper(srcBareJid, dstBareJid, CONNECTION, this, trustedSrcKeysOnly, trustedDstKeysOnly);
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

        public async Task decryptOmemoEncryptedMessageAsync(OmemoEncryptedMessage msg, bool trustedKeysOnly)
        {
            XMPPAccount account = CONNECTION.account;
            OmemoProtocolAddress receiverAddress = new OmemoProtocolAddress(account.getBareJid(), account.omemoDeviceId);
            // Try to decrypt the message, in case no exception occurred, everything went fine:
            OmemoDecryptionContext decryptCtx = new OmemoDecryptionContext(receiverAddress, account.omemoIdentityKey, account.omemoSignedPreKey, account.OMEMO_PRE_KEYS, trustedKeysOnly, OMEMO_STORAGE);
            msg.decrypt(decryptCtx);
            Debug.Assert(!msg.ENCRYPTED);
            Logger.Debug("Successfully decrypted an " + nameof(OmemoEncryptedMessage) + " for '" + receiverAddress.BARE_JID + "'.");

            // Republish bundle information in case the message is a key exchange message and used a PreKey:
            if (decryptCtx.keyExchange)
            {
                Logger.Info("Received a OMEMO key exchange message. Republishing bundle for '" + receiverAddress.BARE_JID + "'...");
                PreKeyModel newPreKey = decryptCtx.STORAGE.ReplaceOmemoPreKey(decryptCtx.usedPreKey);
                account.OMEMO_PRE_KEYS.Remove(decryptCtx.usedPreKey);
                account.OMEMO_PRE_KEYS.Add(newPreKey);
                await announceBundleInfoAsync();
                Logger.Info("Bundle for '" + receiverAddress.BARE_JID + "' republished.");

                // Reply with an empty message to confirm the successful key exchange:
                // TODO: This is no good way, since there it would be possible to stalk people without them knowing.
                OmemoEncryptedMessage reply = new OmemoEncryptedMessage(msg.getTo(), msg.getFrom(), null, msg.TYPE, false);
                OmemoDeviceGroup deviceGroup = new OmemoDeviceGroup(decryptCtx.senderAddress.BARE_JID);
                deviceGroup.SESSIONS.Add(decryptCtx.senderAddress.DEVICE_ID, decryptCtx.session);
                try
                {
                    reply.encrypt(CONNECTION.account.omemoDeviceId, CONNECTION.account.omemoIdentityKey, OMEMO_STORAGE, new List<OmemoDeviceGroup> { deviceGroup });
                    await CONNECTION.SendAsync(reply);
                }
                catch (Exception e)
                {
                    Logger.Error("[OMEMO HELPER] Failed to encrypt and the empty OMEMO message reply with: ", e);
                }
                Logger.Info($"Send an empty OMEMO message to confirm the successful key exchange with '{msg.getFrom()}'.");
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
                try
                {
                    msg.encrypt(CONNECTION.account.omemoDeviceId, CONNECTION.account.omemoIdentityKey, OMEMO_STORAGE, omemoSessions.toList());
                    await CONNECTION.SendAsync(msg, false);
                }
                catch (Exception e)
                {
                    Logger.Error("[OMEMO HELPER] Failed to encrypt and send OMEMO message with: ", e);
                }
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
                if (result.RESULT is IQErrorMessage errMsg)
                {
                    CONNECTION.account.omemoBundleInfoAnnounced = false;
                    Logger.Error("[OMEMO HELPER](" + CONNECTION.account.getBareJid() + ") Failed to announce OMEMO bundle info to: " + CONNECTION.account.user.domainPart + "\n" + errMsg.ERROR_OBJ.ToString());
                    setState(OmemoHelperState.ERROR);
                }
                else
                {
                    CONNECTION.account.omemoBundleInfoAnnounced = true;
                    Logger.Info("[OMEMO HELPER](" + CONNECTION.account.getBareJid() + ") Bundle info announced.");
                    setState(OmemoHelperState.ENABLED);
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
            /**
             * Device id hasn't been set or we have an invalid one. Pick a random, unique one.
             * Versions of UWPX < v.0.41.0.0 had an invalid one.
             * OMEMO allows only device IDs between 1 and (2^31 - 1)
             * https://xmpp.org/extensions/xep-0384.html#usecases-setup
             **/
            if (CONNECTION.account.omemoDeviceId == 0 || CONNECTION.account.omemoDeviceId > 0x7FFFFFFF)
            {
                tmpDeviceId = CryptoUtils.GenerateUniqueDeviceId(devicesRemote.DEVICES.Select(d => d.ID).ToList());
                devicesRemote.DEVICES.Add(new OmemoXmlDevice(tmpDeviceId, CONNECTION.account.omemoDeviceLabel));
                updateDeviceList = true;
            }
            else
            {
                tmpDeviceId = CONNECTION.account.omemoDeviceId;
                OmemoXmlDevice remoteDevice = devicesRemote.DEVICES.Where(d => d.ID == tmpDeviceId).FirstOrDefault();
                // Device does not exist:
                if (remoteDevice is null)
                {
                    devicesRemote.DEVICES.Add(new OmemoXmlDevice(tmpDeviceId, CONNECTION.account.omemoDeviceLabel));
                    updateDeviceList = true;
                }
                // Device label changed:
                else if (!string.Equals(remoteDevice.label, CONNECTION.account.omemoDeviceLabel))
                {
                    remoteDevice.label = CONNECTION.account.omemoDeviceLabel;
                    updateDeviceList = true;
                }
            }

            if (updateDeviceList)
            {
                await updateDeviceListAsync(devicesRemote);
            }
            else if (!CONNECTION.account.omemoBundleInfoAnnounced || updateDeviceList)
            {
                await announceBundleInfoAsync();
            }
            else
            {
                setState(OmemoHelperState.ENABLED);
            }
            DEVICES = devicesRemote;
            // Store the new list in the DB:
            OMEMO_STORAGE.StoreDevices(DEVICES.toOmemoProtocolAddress(CONNECTION.account.getBareJid()), CONNECTION.account.getBareJid());
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
                if (result.RESULT is IQErrorMessage errMsg)
                {
                    Logger.Error("[OMEMO HELPER](" + CONNECTION.account.getBareJid() + ") Failed to set OMEMO device list to: " + CONNECTION.account.user.domainPart + "\n" + errMsg.ERROR_OBJ.ToString());
                    setState(OmemoHelperState.ERROR);
                }
                else
                {
                    Logger.Info("[OMEMO HELPER](" + CONNECTION.account.getBareJid() + ") Device list updated.");
                    if (tmpDeviceId != 0)
                    {
                        if (CONNECTION.account.omemoDeviceId != tmpDeviceId)
                        {
                            CONNECTION.account.omemoDeviceId = tmpDeviceId;
                            CONNECTION.account.omemoBundleInfoAnnounced = false;
                        }
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
                if (CONNECTION.account.omemoDeviceId != 0)
                {
                    if (!msg.DEVICES.DEVICES.Any(d => d.ID == CONNECTION.account.omemoDeviceId))
                    {
                        msg.DEVICES.DEVICES.Add(new OmemoXmlDevice(CONNECTION.account.omemoDeviceId, CONNECTION.account.omemoDeviceLabel));
                        OmemoSetDeviceListMessage setMsg = new OmemoSetDeviceListMessage(CONNECTION.account.getFullJid(), msg.DEVICES);
                        await CONNECTION.SendAsync(setMsg, false);
                    }
                    DEVICES = msg.DEVICES;
                }
                else
                {
                    return;
                }
            }

            OMEMO_STORAGE.StoreDevices(msg.DEVICES.toOmemoProtocolAddress(senderBareJid), senderBareJid);
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
