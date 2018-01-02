
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
        public static readonly string XML_STREAM_ERROR_CLOSE = "</stream:error>";
        public static readonly string XML_STREAM_FEATURE_START = "<stream:features>";
        public static readonly string XML_STREAM_FEATURE_CLOSE = "</stream:features>";

        public static readonly string XML_QUERY_ROOSTER = "<query xmlns=\"jabber:iq:roster\"/>";
        public static readonly string XML_STARTTLS = "<starttls xmlns=\"urn:ietf:params:xml:ns:xmpp-tls\"/>";

        // Error:
        public static readonly string XML_FAILURE = "<failure xmlns=\"urn:ietf:params:xml:ns:xmpp-tls\"/>";

        // Name spaces:
        public static readonly string XML_XMLNS = "xmlns";
        // XEP-0085 (chat state):
        public static readonly string XML_XEP_0085_NAMESPACE = "http://jabber.org/protocol/chatstates";
        // XEP-0357 (push notifications):
        public static readonly string XML_XEP_0357_NAMESPACE = "urn:xmpp:push:0";


        // XEP-0048 (bookmarks):
        public static readonly string XML_XEP_0048_1_0_BOOKMARKS_REQUEST = "<query xmlns='jabber:iq:private'><storage xmlns='storage:bookmarks'/></query>";

        public static readonly bool ENABLE_DEBUG_OUTPUT = true;
    }
}
