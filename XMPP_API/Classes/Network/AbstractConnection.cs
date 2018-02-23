using System.Threading.Tasks;
using XMPP_API.Classes.Network.Events;

namespace XMPP_API.Classes.Network
{
    public abstract class AbstractConnection
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        /// <summary>
        /// The current state of the connection (connecting, error, ...).
        /// </summary>
        public ConnectionState state { get; private set; }
        /// <summary>
        /// Where to connect to?
        /// </summary>
        public XMPPAccount account { get; private set; }

        public event ConnectionStateChangedEventHandler ConnectionStateChanged;

        public delegate void ConnectionStateChangedEventHandler(AbstractConnection connection, ConnectionStateChangedEventArgs arg);

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 07/12/2017 Created [Fabian Sauter]
        /// </history>
        public AbstractConnection(XMPPAccount account)
        {
            this.account = account;
            this.state = ConnectionState.DISCONNECTED;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        /// <summary>
        /// Sets the given state and invokes the ConnectionStateChangedEventHandler.
        /// </summary>
        /// <param name="state">The state to set.</param>
        /// <param name="param">An additional parameter for invoking the ConnectionStateChangedEventHandler (e.g. a list of connection exceptions, ...).</param>
        public virtual void setState(ConnectionState newState, object param)
        {
            ConnectionState oldState = this.state;
            this.state = newState;
            ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedEventArgs(newState, oldState, param));
        }

        /// <summary>
        /// Sets the given state and invokes the ConnectionStateChangedEventHandler.
        /// </summary>
        /// <param name="state">The state to set.</param>
        public void setState(ConnectionState newState)
        {
            setState(newState, null);
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        /// <summary>
        /// Starts a connection.
        /// </summary>
        public abstract Task connectAsync();

        /// <summary>
        /// Disconnects from the current server.
        /// </summary>
        public abstract Task disconnectAsync();

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--
        /// <summary>
        /// Cleans up all artifacts of the connection.
        /// Shouldn't get called during a connection.
        /// </summary>
        /// <returns></returns>
        protected abstract Task cleanupAsync();

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
