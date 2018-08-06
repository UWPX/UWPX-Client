using Logging;
using System;
using System.Threading.Tasks;
using Windows.System.Threading;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.XEP_0384;

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
        /// <summary>
        /// The default timeout is 5000 ms = 5 sec.
        /// </summary>
        public TimeSpan timeout;
        private const int TIMEOUT_5_SEC = 5;
        private ThreadPoolTimer timer;

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
            this.CONNECTION = connection;
            reset();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private void setState(OmemoHelperState newState)
        {
            if (STATE != newState)
            {
                if (newState == OmemoHelperState.ERROR)
                {
                    CONNECTION.ConnectionStateChanged -= CONNECTION_ConnectionStateChanged;
                    CONNECTION.ConnectionNewValidMessage -= CONNECTION_ConnectionNewValidMessage;
                }
                else if (STATE == OmemoHelperState.ERROR && newState != OmemoHelperState.ERROR)
                {
                    reset();
                }
                Logger.Debug("[OMEMO HELPER] " + STATE + "->" + newState);
                STATE = newState;
            }
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

            this.msgId = null;
        }

        #endregion

        #region --Misc Methods (Private)--
        private async Task requestDeviceListAsync()
        {
            OmemoRequestDeviceListMessage msg = new OmemoRequestDeviceListMessage(CONNECTION.account.getIdDomainAndResource(), CONNECTION.account.user.domain);
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
                CONNECTION.account.omemoDeviceId = OmemoUtils.generateDeviceId(devicesRemote.DEVICES);
                // ToDo: save omemoDeviceId to DB via an event or so
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
                OmemoSetDeviceListMessage msg = new OmemoSetDeviceListMessage(CONNECTION.account.getIdDomainAndResource(), devicesRemote);
                msgId = msg.ID;
                resetTimer();
                await CONNECTION.sendAsync(msg, false, false);
            }
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
                    if (args.MESSAGE is OmemoDeviceListResultMessage msg)
                    {
                        await updateDevicesIfNeeded(msg.DEVICES);
                    }
                    else if (args.MESSAGE is IQErrorMessage errMsg)
                    {
                        if (errMsg.ERROR_OBJ.ERROR_NAME == ErrorName.ITEM_NOT_FOUND)
                        {
                            Logger.Warn("Failed to request OMEMO device list - node does not exist. Creating node");
                            await updateDevicesIfNeeded(null);
                        }
                        else
                        {
                            Logger.Error("Failed to request OMEMO device list form: " + CONNECTION.account.user.domain + "\n" + errMsg.ERROR_OBJ.ToString());
                            setState(OmemoHelperState.ERROR);
                        }
                    }
                    break;

                case OmemoHelperState.UPDATING_DEVICE_LIST:
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
                        Logger.Error("OMEMO helper failed - no keys!");
                    }
                    else if(STATE == OmemoHelperState.DISABLED)
                    {
                        setState(OmemoHelperState.REQUESTING_DEVICE_LIST);
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
