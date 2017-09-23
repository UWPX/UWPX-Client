using System;
using System.Threading.Tasks;
using XMPP_API.Classes.Network.Events;

namespace XMPP_API.Classes.Network
{
    public abstract class AbstractConnectionHandler
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public delegate void NewDataEventHandler(AbstractConnectionHandler handler, NewDataEventArgs args);
        public delegate void ConnectionEventHandler(AbstractConnectionHandler handler, EventArgs args);
        public delegate void ConnectionStateChangedEventHandler(AbstractConnectionHandler handler, ConnectionState state);

        public event ConnectionEventHandler ConnectionConnecting;
        public event ConnectionEventHandler ConnectionConnected;
        public event ConnectionEventHandler ConnectionDisconnecting;
        public event ConnectionEventHandler ConnectionDisconnected;
        public event ConnectionEventHandler ConnectionError;
        public event ConnectionStateChangedEventHandler ConnectionStateChanged;
        public event NewDataEventHandler ConnectionNewData;

        protected readonly XMPPAccount ACCOUNT;
        private ConnectionState state;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 17/08/2017 Created [Fabian Sauter]
        /// </history>
        public AbstractConnectionHandler(XMPPAccount account)
        {
            this.ACCOUNT = account;
            this.state = ConnectionState.DISCONNECTED;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        protected void setState(ConnectionState state)
        {
            this.state = state;
            ConnectionStateChanged?.Invoke(this, this.state);
            switch (this.state)
            {
                case ConnectionState.DISCONNECTED:
                    ConnectionDisconnected?.Invoke(this, new EventArgs());
                    break;
                case ConnectionState.CONNECTING:
                    ConnectionConnecting?.Invoke(this, new EventArgs());
                    break;
                case ConnectionState.CONNECTED:
                    ConnectionConnected?.Invoke(this, new EventArgs());
                    break;
                case ConnectionState.DISCONNECTING:
                    ConnectionDisconnecting?.Invoke(this, new EventArgs());
                    break;
                case ConnectionState.ERROR:
                    ConnectionError?.Invoke(this, new EventArgs());
                    break;
                default:
                    throw new InvalidOperationException("Invalid given state: " + state);
            }
        }

        public ConnectionState getState()
        {
            return state;
        }

        public XMPPAccount getXMPPAccount()
        {
            return ACCOUNT;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public abstract Task connectToServerAsync();

        public abstract Task disconnectFromServerAsync();

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--
        protected abstract void cleanupConnection();

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        protected void onConnectionNewData(string data)
        {
            ConnectionNewData?.Invoke(this, new NewDataEventArgs(data));
        }

        #endregion
    }
}
