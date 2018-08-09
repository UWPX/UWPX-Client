using libsignal;
using libsignal.state;
using Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.System.Threading;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.XEP_0060;
using XMPP_API.Classes.Network.XML.Messages.XEP_0384;
using XMPP_API.Classes.Network.XML.Messages.XEP_0384.Signal;

namespace XMPP_API.Classes.Network
{
    public class OmemoHelper : IDisposable
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public OmemoHelperState STATE { get; private set; }
        public OmemoDevices DEVICES { get; private set; }

        private readonly XMPPConnection2 CONNECTION;

        private string msgId;
        private uint tmpDeviceId;
        /// <summary>
        /// The default timeout is 5000 ms = 5 sec.
        /// </summary>
        public TimeSpan timeout;
        private const int TIMEOUT_5_SEC = 5;
        private ThreadPoolTimer timer;

        // Keep sessions during App runtime:
        private readonly Dictionary<string, Tuple<SignalProtocolAddress, SessionBuilder>> SESSIONS_BUILDER;
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
        public OmemoHelper(XMPPConnection2 connection)
        {
            this.timeout = TimeSpan.FromSeconds(TIMEOUT_5_SEC);
            this.CONNECTION = connection;

            this.SESSIONS_BUILDER = new Dictionary<string, Tuple<SignalProtocolAddress, SessionBuilder>>();
            this.SESSION_STORE = new OmemoSessionStore(connection.account);
            this.PRE_KEY_STORE = new OmemoPreKeyStore(connection.account);
            this.SIGNED_PRE_KEY_STORE = new OmemoSignedPreKeyStore(connection.account);
            this.IDENTITY_STORE = new OmemoIdentityKeyStore(connection.account);

            reset();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private void setState(OmemoHelperState newState)
        {
            if (STATE != newState)
            {
                Logger.Debug("[OMEMO HELPER](" + CONNECTION.account.getIdAndDomain() + ") " + STATE + "->" + newState);
                if (newState == OmemoHelperState.ERROR)
                {
                    stopTimer();
                    CONNECTION.ConnectionNewValidMessage -= CONNECTION_ConnectionNewValidMessage;
                    STATE = newState;
                }
                else if (STATE == OmemoHelperState.ERROR && newState != OmemoHelperState.ERROR)
                {
                    STATE = newState;
                    reset();
                }

                if (STATE == OmemoHelperState.ENABLED)
                {
                    Logger.Info("[OMEMO HELPER](" + CONNECTION.account.getIdAndDomain() + ") Enabled.");
                }
            }
        }

        public Tuple<SignalProtocolAddress, SessionBuilder> getSession(string name)
        {
            if (SESSIONS_BUILDER.ContainsKey(name))
            {
                return SESSIONS_BUILDER[name];
            }
            return null;
        }

        public SessionCipher getSessionCipher(SignalProtocolAddress remoteAddress)
        {
            if (SESSIONS_BUILDER.ContainsKey(remoteAddress.getName()))
            {
                return new SessionCipher(SESSION_STORE, PRE_KEY_STORE, SIGNED_PRE_KEY_STORE, IDENTITY_STORE, remoteAddress);
            }
            return null;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void Dispose()
        {
            CONNECTION.ConnectionStateChanged -= CONNECTION_ConnectionStateChanged;
            CONNECTION.ConnectionNewValidMessage -= CONNECTION_ConnectionNewValidMessage;
        }

        public void reset()
        {
            stopTimer();

            setState(OmemoHelperState.DISABLED);

            CONNECTION.ConnectionStateChanged -= CONNECTION_ConnectionStateChanged;
            CONNECTION.ConnectionStateChanged += CONNECTION_ConnectionStateChanged;

            CONNECTION.ConnectionNewValidMessage -= CONNECTION_ConnectionNewValidMessage;
            CONNECTION.ConnectionNewValidMessage += CONNECTION_ConnectionNewValidMessage;

            msgId = null;
            tmpDeviceId = 0;
        }

        public SessionBuilder newSession(string chatJid, uint recipientDeviceId, PreKeyBundle recipientPreKey)
        {
            SignalProtocolAddress address = new SignalProtocolAddress(chatJid, recipientDeviceId);
            SessionBuilder builder = new SessionBuilder(SESSION_STORE, PRE_KEY_STORE, SIGNED_PRE_KEY_STORE, IDENTITY_STORE, address);
            builder.process(recipientPreKey);
            SESSIONS_BUILDER[chatJid] = new Tuple<SignalProtocolAddress, SessionBuilder>(address, builder);
            return builder;
        }

        #endregion

        #region --Misc Methods (Private)--
        private async Task requestDeviceListAsync()
        {
            setState(OmemoHelperState.REQUESTING_DEVICE_LIST);
            Logger.Info("[OMEMO HELPER](" + CONNECTION.account.getIdAndDomain() + ") Requesting device list.");
            OmemoRequestDeviceListMessage msg = new OmemoRequestDeviceListMessage(CONNECTION.account.getIdDomainAndResource(), null);
            msgId = msg.ID;
            resetTimer();
            await CONNECTION.sendAsync(msg, false, false);
        }

        private async Task announceBundleInfoAsync()
        {
            setState(OmemoHelperState.ANNOUNCING_BUNDLE_INFO);
            Logger.Info("[OMEMO HELPER](" + CONNECTION.account.getIdAndDomain() + ") Announcing bundle information for: " + CONNECTION.account.omemoDeviceId);
            OmemoSetBundleInformationMessage msg = new OmemoSetBundleInformationMessage(CONNECTION.account.getIdDomainAndResource(), CONNECTION.account.getOmemoBundleInformation(), CONNECTION.account.omemoDeviceId);
            msgId = msg.ID;
            resetTimer();
            await CONNECTION.sendAsync(msg, false, false);
        }

        private async Task updateDevicesIfNeeded(OmemoDevices devicesRemote)
        {
            if (devicesRemote == null)
            {
                devicesRemote = new OmemoDevices();
            }

            bool updateDeviceList = false;
            // Device id hasn't been set. Pick a random, unique one:
            if (CONNECTION.account.omemoDeviceId == 0)
            {
                tmpDeviceId = OmemoUtils.generateDeviceId(devicesRemote.DEVICES);
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
                OmemoSetDeviceListMessage msg = new OmemoSetDeviceListMessage(CONNECTION.account.getIdDomainAndResource(), devicesRemote);
                msgId = msg.ID;
                resetTimer();
                await CONNECTION.sendAsync(msg, false, false);
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

        private void startTimer()
        {
            timer = ThreadPoolTimer.CreateTimer(onTimerTimeout, timeout);
        }

        private void resetTimer()
        {
            stopTimer();
            startTimer();
        }

        private void stopTimer()
        {
            timer?.Cancel();
            timer = null;
        }

        private void onTimerTimeout(ThreadPoolTimer source)
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
        private async void CONNECTION_ConnectionNewValidMessage(XMPPConnection2 connection, Events.NewValidMessageEventArgs args)
        {
            if (args.MESSAGE is OmemoDeviceListEventMessage eventMsg)
            {
                await onOmemoDeviceListEventMessageAsync(eventMsg);
            }

            switch (STATE)
            {
                case OmemoHelperState.REQUESTING_DEVICE_LIST:
                    if (args.MESSAGE is OmemoDeviceListResultMessage msg && string.Equals(msgId, msg.ID))
                    {
                        stopTimer();
                        await updateDevicesIfNeeded(msg.DEVICES);
                    }
                    else if (args.MESSAGE is IQErrorMessage errMsg && string.Equals(msgId, errMsg.ID))
                    {
                        stopTimer();
                        if (errMsg.ERROR_OBJ.ERROR_NAME == ErrorName.ITEM_NOT_FOUND)
                        {
                            Logger.Warn("[OMEMO HELPER](" + CONNECTION.account.getIdAndDomain() + ") Failed to request OMEMO device list - node does not exist. Creating node.");
                            await updateDevicesIfNeeded(null);
                        }
                        else
                        {
                            Logger.Error("[OMEMO HELPER](" + CONNECTION.account.getIdAndDomain() + ") Failed to request OMEMO device list form: " + CONNECTION.account.user.domain + "\n" + errMsg.ERROR_OBJ.ToString());
                            setState(OmemoHelperState.ERROR);
                        }
                    }
                    break;

                case OmemoHelperState.UPDATING_DEVICE_LIST:
                    if (args.MESSAGE is PubSubPublishResultMessage pubSubResultMsg && string.Equals(msgId, pubSubResultMsg.ID))
                    {
                        stopTimer();
                        Logger.Info("[OMEMO HELPER](" + CONNECTION.account.getIdAndDomain() + ") Device list updated.");
                        if (CONNECTION.account.omemoDeviceId == 0)
                        {
                            CONNECTION.account.omemoDeviceId = tmpDeviceId;
                            CONNECTION.account.onPropertyChanged(nameof(CONNECTION.account.omemoDeviceId));
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
                    else if (args.MESSAGE is IQErrorMessage errMsg && string.Equals(msgId, errMsg.ID))
                    {
                        stopTimer();
                        Logger.Error("[OMEMO HELPER](" + CONNECTION.account.getIdAndDomain() + ") Failed to set OMEMO device list to: " + CONNECTION.account.user.domain + "\n" + errMsg.ERROR_OBJ.ToString());
                        setState(OmemoHelperState.ERROR);
                    }
                    break;

                case OmemoHelperState.ANNOUNCING_BUNDLE_INFO:
                    if (args.MESSAGE is PubSubPublishResultMessage pubSubBundleResultMsg && string.Equals(msgId, pubSubBundleResultMsg.ID))
                    {
                        stopTimer();
                        Logger.Info("[OMEMO HELPER](" + CONNECTION.account.getIdAndDomain() + ") Bundle info announced.");
                        CONNECTION.account.omemoBundleInfoAnnounced = true;
                        CONNECTION.account.onPropertyChanged(nameof(CONNECTION.account.omemoBundleInfoAnnounced));
                        setState(OmemoHelperState.ENABLED);
                    }
                    else if (args.MESSAGE is IQErrorMessage errMsg && string.Equals(msgId, errMsg.ID))
                    {
                        stopTimer();
                        Logger.Error("[OMEMO HELPER](" + CONNECTION.account.getIdAndDomain() + ") Failed to announce OMEMO bundle info to: " + CONNECTION.account.user.domain + "\n" + errMsg.ERROR_OBJ.ToString());
                        setState(OmemoHelperState.ERROR);
                    }
                    break;

                case OmemoHelperState.ENABLED:
                    break;
            }
        }

        private async void CONNECTION_ConnectionStateChanged(AbstractConnection2 connection, Events.ConnectionStateChangedEventArgs arg)
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
