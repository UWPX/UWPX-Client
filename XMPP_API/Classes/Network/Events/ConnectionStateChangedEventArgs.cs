using System;

namespace XMPP_API.Classes.Network.Events
{
    public class ConnectionStateChangedEventArgs : EventArgs
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public ConnectionState newState { get; private set; }
        public ConnectionState oldState { get; private set; }
        public object param { get; private set; }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 07/12/2017 Created [Fabian Sauter]
        /// </history>
        public ConnectionStateChangedEventArgs(ConnectionState newState, ConnectionState oldState, object param)
        {
            this.newState = newState;
            this.oldState = oldState;
            this.param = param;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


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
