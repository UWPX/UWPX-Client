using Data_Manager2.Classes;
using Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Networking.PushNotifications;
using XMPP_API.Classes;
using XMPP_API.Classes.Network.TCP;

namespace Push_App_Server.Classes
{
    public class DataWriter
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly PushConnection PUSH_CONNECTION;
        private XMPPClient client;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 18/11/2017 Created [Fabian Sauter]
        /// </history>
        public DataWriter(XMPPClient client)
        {
            this.PUSH_CONNECTION = new PushConnection();
            this.client = client;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private bool getChannelSuccess()
        {
            return Settings.getSettingBoolean(SettingsConsts.PUSH_CHANNEL_SEND_SUCCESS + client.getXMPPAccount().getBareJid());
        }

        private void setChannelSuccess(bool success)
        {
            Settings.setSetting(SettingsConsts.PUSH_CHANNEL_SEND_SUCCESS + client.getXMPPAccount().getBareJid(), success);
        }

        private string getOldChannelTokenUrl()
        {
            return Settings.getSettingString(SettingsConsts.PUSH_CHANNEL_TOKEN_URL + client.getXMPPAccount().getBareJid());
        }

        private void setChannelTokenUrl(string url)
        {
            Settings.setSetting(SettingsConsts.PUSH_CHANNEL_TOKEN_URL + client.getXMPPAccount().getBareJid(), url);
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        /// <summary>
        /// Requests a PushNotificationChannel and sends if needed the new token to the push server.
        /// </summary>
        /// <returns>Returns true, if the token is still up to date or the token was send successfully to the push server.
        /// Returns false, if the process failed.</returns>
        public async Task<bool> connectAndSendAsync()
        {
            PushNotificationChannel channel = await requestChannelAsync();
            if (channel is null)
            {
                setChannelSuccess(false);
                return false;
            }
            string oldChannelUrl = getOldChannelTokenUrl();
            if (oldChannelUrl != null && oldChannelUrl.Equals(channel.Uri) && getChannelSuccess())
            {
                Logger.Info("Chanel URL token still up to date. No need to send the token again.");
                return true;
            }

            string result = null;
            for (int i = 1; i < 4; i++)
            {
                if (client.getConnetionState() != XMPP_API.Classes.Network.ConnectionState.CONNECTED)
                {
                    return false;
                }
                result = null;
                try
                {
                    Task<Task<string>> t = Task<Task<string>>.Factory.StartNew(async () =>
                    {
                        Logger.Info("Connecting to the push server (" + Consts.PUSH_SERVER_ADDRESS + ")...");
                        await PUSH_CONNECTION.connectAsync();
                        string certInfo = client.getXMPPAccount().CONNECTION_INFO.getCertificateInformation();
                        Logger.Info("Connected to the push server (" + Consts.PUSH_SERVER_ADDRESS + ").");
                        await PUSH_CONNECTION.sendAsync(getMessage(channel.Uri));
                        TCPReadResult s = await PUSH_CONNECTION.readNextString();
                        return s.DATA;
                    });
                    CancellationTokenSource cTS = new CancellationTokenSource();
                    if (t.Wait((int)TimeSpan.FromSeconds(3).TotalMilliseconds, cTS.Token) && t.Result.Wait((int)TimeSpan.FromSeconds(3).TotalMilliseconds, cTS.Token))
                    {
                        result = t.Result.Result;
                        cTS.Cancel();
                    }
                    else
                    {
                        Logger.Error("Unable to connect to the push server - timeout.");
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(i + " try to connect to: " + Consts.PUSH_SERVER_ADDRESS + " - FAILED! " + e.Message);
                    continue;
                }
                if (result != null)
                {
                    if (result.Contains("success"))
                    {
                        setChannelSuccess(true);
                        setChannelTokenUrl(channel.Uri);
                        Logger.Info("Successfully send a new push token URL to the push server.");
                        return true;
                    }
                    else if (result.Contains("error"))
                    {
                        Logger.Error("Unable to send token to the push server. Server answered: " + result);
                    }
                }
            }
            setChannelSuccess(false);
            return false;
        }

        public async Task<PushNotificationChannel> requestChannelAsync()
        {
            PushNotificationChannel channel = null;
            for (int i = 1; i < 4; i++)
            {
                try
                {
                    channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();
                    if (channel != null)
                    {
                        Logger.Info("Successfully requested a new push channel token.");
                        return channel;
                    }
                }
                catch (Exception e)
                {
                    Logger.Error("Try " + i + "to request a push notification channel failed!", e);
                }
            }
            Logger.Error("Unable to request push notification channel!");
            return null;
        }

        #endregion

        #region --Misc Methods (Private)--
        private string getMessage(string url)
        {
            XElement n = new XElement("push");
            n.Add(new XAttribute("clientId", client.getXMPPAccount().getBareJid()));
            n.Add(new XAttribute("pushChannel", url));
            return n.ToString();
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
