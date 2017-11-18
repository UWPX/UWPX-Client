using Data_Manager2.Classes.DBTables;
using Logging;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Networking.PushNotifications;
using XMPP_API.Classes;

namespace Push_App_Server.Classes
{
    public class DataWriter
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly TCPConnectionHandler TCP_CONNECTION_HANDLER;
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
            this.TCP_CONNECTION_HANDLER = new TCPConnectionHandler();
            this.client = client;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public async Task connectAndSendAsync()
        {
            await TCP_CONNECTION_HANDLER.connectAsync();
            Logger.Info("Connected");
            await TCP_CONNECTION_HANDLER.sendMessageToServerAsync(getMessage());
            Logger.Info("Send");
            Logger.Info(TCP_CONNECTION_HANDLER.readMessageFromServer());
            await TCP_CONNECTION_HANDLER.disconnectAsync();
        }

        public async Task requestNotificationChannelAsync()
        {
            PushNotificationChannel channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync("");
        }

        #endregion

        #region --Misc Methods (Private)--
        private string getMessage()
        {
            XElement n = new XElement("push");
            n.Add(new XAttribute("clientId", client.getXMPPAccount().getIdAndDomain()));
            n.Add(new XAttribute("wns", 42));
            n.Add(new XAttribute("key", "someKey"));
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
