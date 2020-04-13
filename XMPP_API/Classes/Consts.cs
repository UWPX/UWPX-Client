using System.Text.RegularExpressions;

namespace XMPP_API.Classes
{
    public static class Consts
    {
        public const string XML_VERSION = "1.0";
        public const string XML_LANG = "en";
        public const string XML_CLIENT = "jabber:client";

        public const string XML_HEADER = "<?xml version=\"1.0\"?>";
        public const string XML_STREAM_START = "<stream:stream";
        public const string XML_STREAM_NAMESPACE = "xmlns:stream=\"http://etherx.jabber.org/streams\"";
        public const string XML_STREAM_CLOSE = "</stream:stream>";
        public const string XML_STREAM_ERROR_START = "<stream:error>";
        public const string XML_STREAM_FEATURE_START = "<stream:features>";

        // Error:
        public const string XML_FAILURE = "<failure xmlns=\"urn:ietf:params:xml:ns:xmpp-tls\"/>";
        public const string XML_ERROR_NAMESPACE = "urn:ietf:params:xml:ns:xmpp-stanzas";
        public const string XML_STREAM_ERROR_NAMESPACE = "urn:ietf:params:xml:ns:xmpp-streams";

        public const string XML_XMLNS = "xmlns";

        // SASL:
        public const string XML_SASL_NAMESPACE = "urn:ietf:params:xml:ns:xmpp-sasl";

        // Binding:
        public const string XML_BIND_NAMESPACE = "urn:ietf:params:xml:ns:xmpp-bind";

        // Session:
        public const string XML_SESSION_NAMESPACE = "urn:ietf:params:xml:ns:xmpp-session";

        // Roster:
        public const string XML_ROSTER_NAMESPACE = "jabber:iq:roster";

        // XEP-0085 (chat state):
        public const string XML_XEP_0085_NAMESPACE = "http://jabber.org/protocol/chatstates";
        // XEP-0357 (push notifications):
        public const string XML_XEP_0357_NAMESPACE = "urn:xmpp:push:0";
        // XEP-0045 (MUC):
        public const string XML_XEP_0045_NAMESPACE = "http://jabber.org/protocol/muc";
        public const string XML_XEP_0045_NAMESPACE_USER = "http://jabber.org/protocol/muc#user";
        public const string XML_XEP_0045_NAMESPACE_ADMIN = "http://jabber.org/protocol/muc#admin";
        public const string XML_XEP_0045_NAMESPACE_ROOM_CONFIG = "http://jabber.org/protocol/muc#roomconfig";
        public static readonly Regex MUC_ROOM_INFO_NAMESPACE_REGEX = new Regex(@"^http:\/\/jabber\.org\/protocol\/muc#((owner)|(admin)|(member)|(none))$");
        // XEP-0249 (Direct MUC Invitations):
        public const string XML_XEP_0249_NAMESPACE = "jabber:x:conference";
        // XEP-0198 (Stream Management):
        public const string XML_XEP_0198_NAMESPACE = "urn:xmpp:sm:3";
        // XEP-0363 (HTTP File Upload):
        public const string XML_XEP_0363_NAMESPACE = "urn:xmpp:http:upload:0";
        // XEP-0060 (Publish-Subscribe):
        public const string XML_XEP_0060_NAMESPACE = "http://jabber.org/protocol/pubsub";
        public const string XML_XEP_0060_NAMESPACE_OWNER = "http://jabber.org/protocol/pubsub#owner";
        public const string XML_XEP_0060_NAMESPACE_ERRORS = "http://jabber.org/protocol/pubsub#errors";
        public const string XML_XEP_0060_NAMESPACE_EVENT = "http://jabber.org/protocol/pubsub#event";
        public const string XML_XEP_0060_NAMESPACE_NODE_CONFIG = "http://jabber.org/protocol/pubsub#node_config";
        public const string XML_XEP_0060_NAMESPACE_PUBLISH_OPTIONS = "http://jabber.org/protocol/pubsub#publish-options";

        // XEP-0004 (Data Forms):
        public const string XML_XEP_0004_NAMESPACE = "jabber:x:data";
        // XEP-0030 (Disco):
        public const string XML_XEP_0030_ITEMS_NAMESPACE = "http://jabber.org/protocol/disco#items";
        public const string XML_XEP_0030_INFO_NAMESPACE = "http://jabber.org/protocol/disco#info";
        // XEP-0054 (vcard-temp):
        public const string XML_XEP_0054_NAMESPACE = "vcard-temp";
        // XEP-0402 (Bookmarks 2):
        public const string XML_XEP_0402_NAMESPACE = "urn:xmpp:bookmarks:0";
        // XEP-0048 (Bookmarks):
        public const string XML_XEP_0048_NAMESPACE = "storage:bookmarks";
        // XEP-0203 (Delayed Delivery):
        public const string XML_XEP_0203_NAMESPACE = "urn:xmpp:delay";
        // XEP-0184 (Message Delivery Receipts):
        public const string XML_XEP_0184_NAMESPACE = "urn:xmpp:receipts";
        // XEP-0384 (OMEMO Encryption):
        public const string XML_XEP_0384_NAMESPACE = "eu.siacs.conversations.axolotl";
        public const string XML_XEP_0384_DEVICE_LIST_NODE = "eu.siacs.conversations.axolotl.devicelist";
        public const string XML_XEP_0384_BUNDLE_INFO_NODE = "eu.siacs.conversations.axolotl.bundles:";
        // XEP-0280 (Message Carbons):
        public const string XML_XEP_0280_NAMESPACE = "urn:xmpp:carbons:2";
        public const string XML_XEP_0280_NAMESPACE_FORWARDED = "urn:xmpp:forward:0";
        // XEP-0334 (Message Processing Hints):
        public const string XML_XEP_0334_NAMESPACE = "urn:xmpp:hints";
        // XEP-0336: (Data Forms - Dynamic Forms):
        public const string XML_XEP_0336_NAMESPACE = "urn:xmpp:xdata:dynamic";
        // XEP-IoT:
        public const string XML_XEP_IOT_NAMESPACE = "urn:xmpp:uwpx:iot";
        // XEP-0313 (Message Archive Management)
        public const string XML_XEP_0313_NAMESPACE = "urn:xmpp:mam:tmp";
        // XEP-0359 (Unique and Stable Stanza IDs)
        public const string XML_XEP_0359_NAMESPACE = "urn:xmpp:sid:0";
    }
}
