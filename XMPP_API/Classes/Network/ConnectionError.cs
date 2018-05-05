using Windows.Networking.Sockets;

namespace XMPP_API.Classes.Network
{
    public class ConnectionError
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly string ERROR_MESSAGE;
        public readonly ConnectionErrorCode ERROR_CODE;
        public readonly SocketErrorStatus SOCKET_ERROR;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 05/05/2018 Created [Fabian Sauter]
        /// </history>
        public ConnectionError(ConnectionErrorCode errorCode)
        {
            this.ERROR_CODE = errorCode;
            this.ERROR_MESSAGE = null;
            this.SOCKET_ERROR = SocketErrorStatus.Unknown;
        }

        public ConnectionError(ConnectionErrorCode errorCode, string errorMessage) : this(errorCode)
        {
            this.ERROR_MESSAGE = errorMessage;
            this.SOCKET_ERROR = SocketErrorStatus.Unknown;
        }

        public ConnectionError(SocketErrorStatus socketError) : this(ConnectionErrorCode.SOCKET_ERROR)
        {
            this.SOCKET_ERROR = socketError;
        }

        public ConnectionError(SocketErrorStatus socketError, string errorMessage) : this(ConnectionErrorCode.SOCKET_ERROR)
        {
            this.SOCKET_ERROR = socketError;
            this.ERROR_MESSAGE = errorMessage;
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
