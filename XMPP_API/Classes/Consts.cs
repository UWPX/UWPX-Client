using System.Text.RegularExpressions;

namespace XMPP_API.Classes
{
    class Consts
    {
        public const string XML_VERSION = "1.0";
        public const string XML_LANG = "en";
        public const string XML_CLIENT = " xmlns=\"jabber:client\"";

        public const string XML_HEADER = "<?xml version=\"1.0\"?>";
        public const string XML_STREAM_START = "<stream:stream";
        public const string XML_STREAM_NAMESPACE = " xmlns:stream=\"http://etherx.jabber.org/streams\"";
        public const string XML_STREAM_CLOSE = "</stream:stream>";
        public const string XML_STREAM_ERROR_START = "<stream:error>";
        public const string XML_STREAM_FEATURE_START = "<stream:features>";

        // Error:
        public const string XML_FAILURE = "<failure xmlns=\"urn:ietf:params:xml:ns:xmpp-tls\"/>";

        public const string XML_XMLNS = "xmlns";

        // SASL failure:
        public const string XML_SASL_FAILURE_NAMESPACE = "urn:ietf:params:xml:ns:xmpp-sasl";

        // XEP-0085 (chat state):
        public const string XML_XEP_0085_NAMESPACE = "http://jabber.org/protocol/chatstates";
        // XEP-0357 (push notifications):
        public const string XML_XEP_0357_NAMESPACE = "urn:xmpp:push:0";
        // XEP-0045 (MUC):
        public const string XML_XEP_0045_NAMESPACE = "http://jabber.org/protocol/muc";
        public const string XML_XEP_0045_NAMESPACE_USER = "http://jabber.org/protocol/muc#user";
        public const string XML_XEP_0045_NAMESPACE_ADMIN = "http://jabber.org/protocol/muc#admin";
        public const string XML_XEP_0045_NAMESPACE_ROOM_CONFIG = "http://jabber.org/protocol/muc#roomconfig";
        public const string XML_XEP_0045_ROOM_INFO_DATA_NAMESPACE = "jabber:x:data";
        public static readonly Regex MUC_ROOM_INFO_NAMESPACE_REGEX = new Regex(@"^http:\/\/jabber\.org\/protocol\/muc#((owner)|(admin)|(member)|(none))$");
        // XEP-0249 (Direct MUC Invitations):
        public const string XML_XEP_0249_NAMESPACE = "jabber:x:conference";
    }
}
