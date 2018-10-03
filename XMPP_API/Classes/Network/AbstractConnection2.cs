using Logging;
using XMPP_API.Classes.Network.Events;

namespace XMPP_API.Classes.Network
{
    public abstract class AbstractConnection2
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        /// <summary>
        /// The current state of the connection (DISCONNECTED, ERROR, ...).
        /// </summary>
        public ConnectionState state { get; private set; }
        /// <summary>
        /// Where to connect to?
        /// </summary>
        public XMPPAccount account { get; private set; }

        public event ConnectionStateChangedEventHandler ConnectionStateChanged;

        public delegate void ConnectionStateChangedEventHandler(AbstractConnection2 connection, ConnectionStateChangedEventArgs arg);

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 05/05/2018 Created [Fabian Sauter]
        /// </history>
        protected AbstractConnection2(XMPPAccount account)
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
        protected virtual void setState(ConnectionState newState, object param)
        {
            // Only trigger if the state actually changed:
            if (newState == state)
            {
                return;
            }

            ConnectionState oldState = state;
            state = newState;
            ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedEventArgs(newState, oldState, param));

            Logger.Debug("[" + this.GetType().Name + "] " + oldState + " -> " + newState);
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


        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
