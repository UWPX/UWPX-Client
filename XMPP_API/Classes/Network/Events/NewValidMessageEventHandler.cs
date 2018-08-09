using XMPP_API.Classes.Network.XML.Messages;

namespace XMPP_API.Classes.Network.Events
{
    public delegate void NewValidMessageEventHandler(IMessageSender sender, NewValidMessageEventArgs args);
}
