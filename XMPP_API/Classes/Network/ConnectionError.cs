using System.Text;
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
            ERROR_CODE = errorCode;
            ERROR_MESSAGE = null;
            SOCKET_ERROR = SocketErrorStatus.Unknown;
        }

        public ConnectionError(ConnectionErrorCode errorCode, string errorMessage) : this(errorCode)
        {
            ERROR_MESSAGE = errorMessage;
            SOCKET_ERROR = SocketErrorStatus.Unknown;
        }

        public ConnectionError(SocketErrorStatus socketError) : this(ConnectionErrorCode.SOCKET_ERROR)
        {
            SOCKET_ERROR = socketError;
        }

        public ConnectionError(SocketErrorStatus socketError, string errorMessage) : this(ConnectionErrorCode.SOCKET_ERROR)
        {
            SOCKET_ERROR = socketError;
            ERROR_MESSAGE = errorMessage;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("[ConnectionError] ");
            sb.Append("SOCKET_ERROR: ");
            sb.Append(SOCKET_ERROR.ToString());
            sb.Append(", ERROR_CODE: ");
            sb.Append(ERROR_CODE.ToString());
            sb.Append(", ERROR_MESSAGE: ");
            sb.Append(ERROR_MESSAGE ?? "NULL");
            return sb.ToString();
        }

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
