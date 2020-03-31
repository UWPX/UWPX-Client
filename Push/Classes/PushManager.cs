using System;
using System.Threading;
using System.Threading.Tasks;
using Data_Manager2.Classes;
using Logging;
using Push.Classes.Events;
using Push.Classes.Messages;
using Windows.Networking.PushNotifications;
using XMPP_API.Classes.Network.TCP;

namespace Push.Classes
{
    public class PushManager
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public static PushManager INSTANCE = new PushManager();

        private readonly SemaphoreSlim INIT_SEMA = new SemaphoreSlim(1);
        private readonly SemaphoreSlim STATE_SEMA = new SemaphoreSlim(1);
        private PushManagerState state;

        public delegate void StateChangedEventHandler(PushManager sender, PushManagerStateChangedEventArgs args);
        public event StateChangedEventHandler StateChanged;

        private PushNotificationChannel channel;
        private DateTime channelCreated;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        private PushManager() { }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private async Task SetStateAsync(PushManagerState state)
        {
            await STATE_SEMA.WaitAsync();
            if (state != this.state)
            {
                PushManagerState oldState = this.state;
                this.state = state;
                STATE_SEMA.Release();
                StateChanged?.Invoke(this, new PushManagerStateChangedEventArgs(state, oldState));
            }
            else
            {
                STATE_SEMA.Release();
            }
        }

        public PushManagerState GetState()
        {
            return state;
        }

        public PushNotificationChannel GetChannel()
        {
            return channel;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void Init()
        {
            Task.Run(async () =>
            {
                await INIT_SEMA.WaitAsync();
                if (state != PushManagerState.NOT_INITIALIZED && state != PushManagerState.DONE && state != PushManagerState.ERROR)
                {
                    INIT_SEMA.Release();
                    Logger.Debug(Consts.LOGGER_TAG + "Init() called, but push service is already initialized.");
                    return;
                }

                await SetStateAsync(PushManagerState.INITIALIZING);
                INIT_SEMA.Release();

                Logger.Info(Consts.LOGGER_TAG + "Initializing push...");
                await SetStateAsync(PushManagerState.REQUESTING_CHANNEL);
                if (!await RequestChannelAsync())
                {
                    return;
                }

                await SetStateAsync(PushManagerState.STORING_CHANNEL);
                if (StoreAndCompareChannel() && !ShouldSendChannelUriToPushServer())
                {
                    await SetStateAsync(PushManagerState.DONE);
                }

                await SetStateAsync(PushManagerState.SENDING_UPDATED_CHANNEL_URI_TO_PUSH_SERVER);
                if (await SendUpdatedChannelUriToPushServerAsync())
                {
                    await SetStateAsync(PushManagerState.DONE);
                    return;
                }
                await SetStateAsync(PushManagerState.ERROR);
            });
        }

        #endregion

        #region --Misc Methods (Private)--
        private async Task<bool> RequestChannelAsync()
        {
            Logger.Info(Consts.LOGGER_TAG + "Requesting a new push channel...");
            for (int i = 1; i < 4; i++)
            {
                try
                {
                    channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();
                    if (!(channel is null))
                    {
                        Logger.Info(Consts.LOGGER_TAG + "Successfully requested a new push channel.");
                        Logger.Debug(Consts.LOGGER_TAG + "Channel URI: " + channel.Uri);
                        channelCreated = DateTime.Now;
                        return true;
                    }
                    else
                    {
                        Logger.Error(Consts.LOGGER_TAG + "Try " + i + " to request a push channel failed! Response is null.");
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(Consts.LOGGER_TAG + "Try " + i + " to request a push channel failed!", e);
                }

                if (i < 3)
                {
                    // Start a 10 second delay between requests:
                    await Task.Delay(10 * 1000);
                }
            }
            Logger.Error(Consts.LOGGER_TAG + "Unable to request a push channel!");
            return false;
        }

        private bool StoreAndCompareChannel()
        {
            string oldChannelUri = Settings.getSettingString(SettingsConsts.PUSH_CHANNEL_URI);
            if (string.Equals(channel.Uri, oldChannelUri))
            {
                channelCreated = Settings.getSettingDateTime(SettingsConsts.PUSH_CHANNEL_CREATED_DATE_TIME);
                return true;
            }

            Settings.setSetting(SettingsConsts.PUSH_CHANNEL_URI, channel.Uri);
            Settings.setSetting(SettingsConsts.PUSH_CHANNEL_CREATED_DATE_TIME, channelCreated);
            return false;
        }

        /// <summary>
        /// Sends the updated channel URI to the push server.
        /// </summary>
        /// <returns>True on success.</returns>
        private async Task<bool> SendUpdatedChannelUriToPushServerAsync()
        {
            using (TcpConnection connection = new TcpConnection("push.uwpx.org", 1997))
            {
                try
                {
                    await connection.ConnectAsync();
                    SetChannelUriMessage msg = new SetChannelUriMessage(channel.Uri);
                    await connection.SendAsync(msg.ToString());
                }
                catch (Exception e)
                {
                    Logger.Error("Failed to connect and send the channel URL to the push server.", e);
                    return false;
                }
                Logger.Debug("Send channel URI update message. Waiting for a response...");

                try
                {
                    TcpReadResult result = await connection.ReadAsync();
                    connection.Disconnect();
                    if (result.STATE != TcpReadState.SUCCESS)
                    {
                        Logger.Error("Failed to read a response from the push server: " + result.STATE);
                        return false;
                    }
                    Messages.AbstractMessage message = MessageParser.Parse(result.DATA);
                    if (message is SuccessResultMessage)
                    {
                        Logger.Info("Send the push channel to the push server.");
                        // Mark the channel URI as send to the push server:
                        Settings.setSetting(SettingsConsts.PUSH_CHANNEL_URI_SEND_TO_PUSH_SERVER, channel.Uri);
                        return true;
                    }

                    if (message is ErrorResultMessage errMsg)
                    {
                        Logger.Error("Failed to send the push channel to the server. The server responded: " + errMsg.error);
                    }
                    else
                    {
                        Logger.Error("Failed to send the push channel to the server. The server responded with an unexpected answer: " + result.DATA);
                    }
                }
                catch (Exception e)
                {
                    Logger.Error("Failed to read a response from the push server.", e);
                }
                return false;
            }
        }

        /// <summary>
        /// Returns whether the current channel URI has been send successfully to the push server.
        /// </summary>
        private bool ShouldSendChannelUriToPushServer()
        {
            string remoteUri = Settings.getSettingString(SettingsConsts.PUSH_CHANNEL_URI_SEND_TO_PUSH_SERVER);
            return !string.Equals(channel.Uri, remoteUri);
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
