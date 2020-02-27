using System.Threading.Tasks;
using XMPP_API.Classes.Network;
using XMPP_API.Classes.Network.Events;
using XMPP_API.Classes.Network.TCP;

namespace Push_App_Server.Classes
{
    /// <summary>
    /// This class is a wrapper for the TCPConnection class.
    /// </summary>
    public class PushConnection: AbstractConnection2
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        /// <summary>
        /// The TCP connection to the push server.
        /// </summary>
        private TcpConnection tcpConnection;

        /// <summary>
        /// A dummy XMPPAccount its only purpose it is to allow using the TCPConnection.
        /// </summary>
        private static readonly XMPPAccount DUMMY_XMPP_ACCOUNT = new XMPPAccount(null)
        {
            serverAddress = Consts.PUSH_SERVER_ADDRESS,
            port = Consts.PORT
        };

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
            tcpConnection = new TcpConnection(DUMMY_XMPP_ACCOUNT);
            tcpConnection.ConnectionStateChanged += TCPConnection_ConnectionStateChanged;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public async Task connectAsync()
        {
            await tcpConnection.ConnectAsync();
        }

        public async Task disconnectAsync()
        {
            await tcpConnection.DisconnectAsync();
        }

        public async Task sendAsync(string msg)
        {
            await tcpConnection.SendAsync(msg);
        }

        public async Task<TCPReadResult> readNextString()
        {
            return await tcpConnection.ReadAsync();
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void TCPConnection_ConnectionStateChanged(AbstractConnection sender, ConnectionStateChangedEventArgs arg)
        {
            setState(arg.newState);
        }

        #endregion
    }
}
