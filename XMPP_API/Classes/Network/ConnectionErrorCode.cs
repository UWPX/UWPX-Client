namespace XMPP_API.Classes.Network
{
    public enum ConnectionErrorCode
    {
        UNKNOWN = 0,
        SOCKET_ERROR = 1,
        CONNECT_CANCELED = 2,
        CONNECT_TIMEOUT = 3,
        READING_CANCELED = 4,
        READING_LOOP = 5,
        NO_INTERNET = 6,
        TLS_CONNECTION_FAILED = 7,
        SASL_FAILED = 8,
        XMPP_CONNECTION_TIMEOUT = 9,
        SENDING_FAILED = 10,
        SERVER_ERROR = 11,
    }
}
