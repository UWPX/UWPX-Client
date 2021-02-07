using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Logging;
using Manager.Classes;
using Push.Classes.Events;
using Push.Classes.Messages;
using Storage.Classes;
using Windows.Networking.PushNotifications;
using XMPP_API.Classes.Network;
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

        private string GetServerAddress()
        {
            return "push.uwpx.org";
        }

        private ushort GetServerPort()
        {
            return 1997;
        }

        /// <summary>
        /// Returns the hash value for all accounts based on a sorted list.
        /// </summary>
        private static int GetAccountsHash()
        {
            IEnumerable<string> accounts = ConnectionHandler.INSTANCE.GetClients().Select(x => x.client.dbAccount.bareJid);
            // The last bit always indicates push enabled:
            return unchecked(GetOrderIndependentHashCode(accounts) << 1) ^ Settings.GetSettingBoolean(SettingsConsts.PUSH_ENABLED).GetHashCode();
        }

        /// <summary>
        /// Returns the hash code of a list.
        /// It does not depend on the order of the list.
        /// Based on: https://stackoverflow.com/questions/670063/getting-hash-of-a-list-of-strings-regardless-of-order
        /// </summary>
        private static int GetOrderIndependentHashCode(IEnumerable<string> source)
        {
            int hash = 0;
            int curHash;
            // Stores number of occurrences so far of each value.
            Dictionary<string, int> valueCounts = new Dictionary<string, int>();

            foreach (string element in source)
            {
                if (valueCounts.TryGetValue(element, out int bitOffset))
                {
                    valueCounts[element] = ++bitOffset;
                }
                else
                {
                    valueCounts.Add(element, bitOffset);
                }

                // The current hash code is shifted (with wrapping) one bit
                // further left on each successive recurrence of a certain
                // value to widen the distribution.
                // 37 is an arbitrary low prime number that helps the
                // algorithm to smooth out the distribution.
                curHash = GetDeterministicHashCode(element);
                hash = unchecked(hash + (((curHash << bitOffset) | (curHash >> (32 - bitOffset))) * 37));
            }
            return hash;
        }

        /// <summary>
        /// Returns a stable hash across executions.
        /// The default <seealso cref="string.GetHashCode()"/> method returns only deterministic values for the current run.
        /// Based on: https://andrewlock.net/why-is-string-gethashcode-different-each-time-i-run-my-program-in-net-core/
        /// </summary>
        private static int GetDeterministicHashCode(string str)
        {
            unchecked
            {
                int hash1 = (5381 << 16) + 5381;
                int hash2 = hash1;

                for (int i = 0; i < str.Length; i += 2)
                {
                    hash1 = ((hash1 << 5) + hash1) ^ str[i];
                    if (i == str.Length - 1)
                    {
                        break;
                    }

                    hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
                }

                return hash1 + (hash2 * 1566083941);
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void Init()
        {
            Task.Run(async () =>
            {
                // Push is disabled:
                if (!Settings.GetSettingBoolean(SettingsConsts.PUSH_ENABLED))
                {
                    await SetStateAsync(PushManagerState.DEAKTIVATED);
                    Logger.Info("Push is disabled. Discarding initialization.");
                    return;
                }

                // Push is enabled:
                await INIT_SEMA.WaitAsync();
                if (state != PushManagerState.NOT_INITIALIZED && state != PushManagerState.INITIALIZED && state != PushManagerState.ERROR && state != PushManagerState.DEAKTIVATED)
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
                    await SetStateAsync(PushManagerState.INITIALIZED);
                }

                await SetStateAsync(PushManagerState.SENDING_UPDATED_CHANNEL_URI_TO_PUSH_SERVER);
                if (await SendUpdatedChannelUriToPushServerAsync())
                {
                    await SetStateAsync(PushManagerState.INITIALIZED);
                    return;
                }
                await SetStateAsync(PushManagerState.ERROR);
            });
        }

        private void UpdatePushForAccounts(List<PushAccount> accounts, string pushServerBareJid)
        {
            Logger.Debug("Updating push settings for " + accounts.Count + " accounts.");
            foreach (PushAccount pushAccount in accounts)
            {
                Client client = ConnectionHandler.INSTANCE.GetClient(pushAccount.bareJid);
                if (!(client is null))
                {
                    XMPPAccount account = client.xmppClient.getXMPPAccount();
                    account.pushNodePublished = account.pushNodePublished && string.Equals(account.pushNode, pushAccount.node) && string.Equals(account.pushNodeSecret, pushAccount.secret);
                    account.pushNode = pushAccount.node;
                    account.pushNodeSecret = pushAccount.secret;
                    account.pushServerBareJid = pushServerBareJid;
                    account.pushEnabled = true;
                    Logger.Info("Push node and secret updated for: " + pushAccount.bareJid);
                    Logger.Debug("Node: '" + pushAccount.node + "', secret: '" + pushAccount.secret + "'");
                }
                else
                {
                    Logger.Error("Failed to update push for account '" + pushAccount.bareJid + "' - account not found");
                }
            }
        }

        public async Task InitPushForAccountsAsync()
        {
            using (TcpConnection connection = new TcpConnection(GetServerAddress(), GetServerPort()))
            {
                try
                {
                    await connection.ConnectAsync();
                    // If push is disabled, send an empty list:
                    SetPushAccountsMessage msg = Settings.GetSettingBoolean(SettingsConsts.PUSH_ENABLED) ? new SetPushAccountsMessage(ConnectionHandler.INSTANCE.GetClients().Select(x => x.client.dbAccount.bareJid).ToList()) : new SetPushAccountsMessage(new List<string>());
                    await connection.SendAsync(msg.ToString());
                }
                catch (Exception e)
                {
                    Logger.Error("Failed to connect and set push accounts.", e);
                    return;
                }
                Logger.Debug("Set push accounts send. Waiting for a response...");

                try
                {
                    TcpReadResult result = await connection.ReadAsync();
                    connection.Disconnect();
                    if (result.STATE != TcpReadState.SUCCESS)
                    {
                        Logger.Error("Failed to read a response from the push server: " + result.STATE);
                        return;
                    }
                    AbstractMessage message = MessageParser.Parse(result.DATA);
                    if (message is SuccessSetPushAccountsMessage msg)
                    {
                        UpdatePushForAccounts(msg.accounts, msg.pushServerBareJid);
                        Settings.SetSetting(SettingsConsts.PUSH_LAST_ACCOUNT_HASH, GetAccountsHash());
                        Logger.Info("Setting push accounts successful.");
                        return;
                    }

                    if (message is ErrorResultMessage errMsg)
                    {
                        Logger.Error("Failed to set push accounts. The server responded: " + errMsg.error);
                    }
                    else
                    {
                        Logger.Error("Failed to set push accounts. The server responded with an unexpected answer: " + result.DATA);
                    }
                }
                catch (Exception e)
                {
                    Logger.Error("Failed to read a response from the push server.", e);
                }
            }
        }
        /// <summary>
        /// Sends a request to the push server to send a test push notification to the device.
        /// </summary>
        /// <returns>True on success.</returns>
        public async Task<bool> RequestTestPushMessageAsync()
        {
            Logger.Info("Requesting a test push notification...");
            using (TcpConnection connection = new TcpConnection(GetServerAddress(), GetServerPort()))
            {
                try
                {
                    await connection.ConnectAsync();
                    RequestTestPushMessage msg = new RequestTestPushMessage();
                    await connection.SendAsync(msg.ToString());
                }
                catch (Exception e)
                {
                    Logger.Error("Failed to connect and request a test push message.", e);
                    return false;
                }
                Logger.Debug("Request for test push send. Waiting for a response...");

                try
                {
                    TcpReadResult result = await connection.ReadAsync();
                    connection.Disconnect();
                    if (result.STATE != TcpReadState.SUCCESS)
                    {
                        Logger.Error("Failed to read a response from the push server: " + result.STATE);
                        return false;
                    }
                    AbstractMessage message = MessageParser.Parse(result.DATA);
                    if (message is SuccessResultMessage)
                    {
                        Logger.Info("Requested test push successfully.");
                        return true;
                    }

                    if (message is ErrorResultMessage errMsg)
                    {
                        Logger.Error("Failed request a test push. The server responded: " + errMsg.error);
                    }
                    else
                    {
                        Logger.Error("Failed request a test push. The server responded with an unexpected answer: " + result.DATA);
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
        /// Determines based on the hash value of all accounts if the push accounts should be updated.
        /// </summary>
        /// <returns></returns>
        public static bool ShouldUpdatePushForAccounts()
        {
            return Settings.GetSettingInt(SettingsConsts.PUSH_LAST_ACCOUNT_HASH) != GetAccountsHash();
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
            string oldChannelUri = Settings.GetSettingString(SettingsConsts.PUSH_CHANNEL_URI);
            if (string.Equals(channel.Uri, oldChannelUri))
            {
                channelCreated = Settings.GetSettingDateTime(SettingsConsts.PUSH_CHANNEL_CREATED_DATE_TIME);
                return true;
            }

            Settings.SetSetting(SettingsConsts.PUSH_CHANNEL_URI, channel.Uri);
            Settings.SetSetting(SettingsConsts.PUSH_CHANNEL_CREATED_DATE_TIME, channelCreated);
            return false;
        }

        /// <summary>
        /// Sends the updated channel URI to the push server.
        /// </summary>
        /// <returns>True on success.</returns>
        private async Task<bool> SendUpdatedChannelUriToPushServerAsync()
        {
            using (TcpConnection connection = new TcpConnection(GetServerAddress(), GetServerPort()))
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
                    AbstractMessage message = MessageParser.Parse(result.DATA);
                    if (message is SuccessResultMessage)
                    {
                        Logger.Info("Send the push channel to the push server.");
                        // Mark the channel URI as send to the push server:
                        Settings.SetSetting(SettingsConsts.PUSH_CHANNEL_URI_SEND_TO_PUSH_SERVER, channel.Uri);
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
            string remoteUri = Settings.GetSettingString(SettingsConsts.PUSH_CHANNEL_URI_SEND_TO_PUSH_SERVER);
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
