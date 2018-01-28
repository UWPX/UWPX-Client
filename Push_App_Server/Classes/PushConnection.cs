using System.Threading.Tasks;
using XMPP_API.Classes.Network;
using XMPP_API.Classes.Network.Events;
using XMPP_API.Classes.Network.TCP;

namespace Push_App_Server.Classes
{
    /// <summary>
    /// This class is a wrapper for the TCPConnection class.
    /// </summary>
    public class PushConnection : AbstractConnection
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        /// <summary>
        /// The TCP connection to the push server.
        /// </summary>
        private TCPConnection tCPConnection;

        /// <summary>
        /// A dummy XMPPAccount its only purpose it is to allow using the TCPConnection.
        /// </summary>
        private static readonly XMPPAccount DUMMY_XMPP_ACCOUNT = new XMPPAccount(null, Consts.PUSH_SERVER_ADDRESS, Consts.PORT);

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 28/01/2018 Created [Fabian Sauter]
        /// </history>
        public PushConnection() : base(DUMMY_XMPP_ACCOUNT)
        {
            this.tCPConnection = new TCPConnection(DUMMY_XMPP_ACCOUNT);
            this.tCPConnection.ConnectionStateChanged += TCPConnection_ConnectionStateChanged;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public string getCertificateInformation()
        {
            return tCPConnection.getCertificateInformation();
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public async override Task connectAsync()
        {
            await tCPConnection.connectAsync();
        }

        public async override Task disconnectAsync()
        {
            await tCPConnection.disconnectAsync();
        }

        public async Task sendAsync(string msg)
        {
            await tCPConnection.sendAsync(msg);
        }

        public string readNextString()
        {
            return tCPConnection.readNextString();
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--
        /// <summary>
        /// Unused here.
        /// </summary>
        /// <returns></returns>
        protected override Task cleanupAsync()
        {
            return null;
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void TCPConnection_ConnectionStateChanged(AbstractConnection connection, ConnectionStateChangedEventArgs arg)
        {
            setState(arg.newState);
        }

        #endregion
    }
}
