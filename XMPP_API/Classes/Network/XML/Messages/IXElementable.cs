using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages
{
    public interface IXElementable
    {
        XElement toXElement(XNamespace ns);
    }
}
