using System.Text.RegularExpressions;

namespace XMPP_API.Classes
{
    class Consts
    {
        public static readonly string XML_VERSION = "1.0";
        public static readonly string XML_LANG = "en";
        public static readonly string XML_CLIENT = " xmlns=\"jabber:client\"";

        public static readonly string XML_HEADER = "<?xml version=\"1.0\"?>";
        public static readonly string XML_STREAM_START = "<stream:stream";
        public static readonly string XML_STREAM_NAMESPACE = " xmlns:stream=\"http://etherx.jabber.org/streams\"";
        public static readonly string XML_STREAM_CLOSE = "</stream:stream>";
        public static readonly string XML_STREAM_ERROR_START = "<stream:error>";
        public static readonly string XML_STREAM_FEATURE_START = "<stream:features>";

        // Error:
        public static readonly string XML_FAILURE = "<failure xmlns=\"urn:ietf:params:xml:ns:xmpp-tls\"/>";

        // Name spaces:
        public static readonly string XML_XMLNS = "xmlns";
        // XEP-0085 (chat state):
        public static readonly string XML_XEP_0085_NAMESPACE = "http://jabber.org/protocol/chatstates";
        // XEP-0357 (push notifications):
        public static readonly string XML_XEP_0357_NAMESPACE = "urn:xmpp:push:0";
        // XEP-0045 (MUC):
        public static readonly string XML_XEP_0045_NAMESPACE = "http://jabber.org/protocol/muc";

        public static readonly bool ENABLE_DEBUG_OUTPUT = false;

        public static readonly Regex MUC_ROOM_INFO_NAMESPACE_REGEX = new Regex(@"^http:\/\/jabber\.org\/protocol\/muc#((owner)|(admin)|(member)|(none))$");
    }
}
