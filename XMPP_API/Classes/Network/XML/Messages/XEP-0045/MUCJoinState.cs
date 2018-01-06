namespace XMPP_API.Classes.Network.XML.Messages.XEP_0045
{
    public enum MUCJoinState
    {
        NOT_STARTED,
        SEND_REQUESTING_RESERVED_NICKS,
        RECEIVED_RESERVED_NICKS,
        SEND_ENTER_ROOM,
        RECEIVED_ENTER_ROOM_ANSWER,
        ERROR
    }
}
