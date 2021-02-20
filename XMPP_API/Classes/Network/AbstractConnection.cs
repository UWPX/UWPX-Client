using System.Threading;
using Logging;
using XMPP_API.Classes.Network.Events;

namespace XMPP_API.Classes.Network
{
    public abstract class AbstractConnection
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        /// <summary>
        /// The current state of the connection (DISCONNECTED, ERROR, ...).
        /// </summary>
        public ConnectionState state { get; private set; } = ConnectionState.DISCONNECTED;
        protected readonly Mutex STATE_MUTEX = new Mutex();
        protected readonly SemaphoreSlim STATE_SEMA = new SemaphoreSlim(1, 1);
        /// <summary>
        /// Where to connect to?
        /// </summary>
        public XMPPAccount account { get; private set; }

        public event ConnectionStateChangedEventHandler ConnectionStateChanged;

        public delegate void ConnectionStateChangedEventHandler(AbstractConnection sender, ConnectionStateChangedEventArgs args);

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        protected AbstractConnection(XMPPAccount account)
        {
            this.account = account;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        /// <summary>
        /// Sets the given state and invokes the ConnectionStateChangedEventHandler.
        /// </summary>
        /// <param name="state">The state to set.</param>
        /// <param name="param">An additional parameter for invoking the ConnectionStateChangedEventHandler (e.g. a list of connection exceptions, ...).</param>
        protected virtual void SetState(ConnectionState newState, object param = null)
        {
            STATE_MUTEX.WaitOne();
            // Only trigger if the state actually changed:
            if (newState == state)
            {
                STATE_MUTEX.ReleaseMutex();
                return;
            }

            ConnectionState oldState = state;
            state = newState;
            STATE_MUTEX.ReleaseMutex();

            ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedEventArgs(newState, oldState, param));

            if (param is not string s)
            {
                s = param?.ToString();
            }
            Logger.Debug("[" + GetType().Name + "] " + oldState + " -> " + newState + ": " + s);
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
