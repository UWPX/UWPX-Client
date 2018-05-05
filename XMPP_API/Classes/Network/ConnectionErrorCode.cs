namespace XMPP_API.Classes.Network
{
    public enum ConnectionErrorCode
    {
        UNKNOWN = 0,
        SOCKET_ERROR = 1,
        CONNECT_CANCELED = 2,
        CONNECT_TIMEOUT = 3,
        READING_CANCELED = 4,
        READING_LOOP = 5
    }
}
