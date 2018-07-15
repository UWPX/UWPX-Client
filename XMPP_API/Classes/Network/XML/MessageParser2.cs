using Logging;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.Features;
using XMPP_API.Classes.Network.XML.Messages.Features.SASL;
using XMPP_API.Classes.Network.XML.Messages.Features.TLS;
using XMPP_API.Classes.Network.XML.Messages.XEP_0030;
using XMPP_API.Classes.Network.XML.Messages.XEP_0045;
using XMPP_API.Classes.Network.XML.Messages.XEP_0045.Configuration;
using XMPP_API.Classes.Network.XML.Messages.XEP_0402;
using XMPP_API.Classes.Network.XML.Messages.XEP_0085;
using XMPP_API.Classes.Network.XML.Messages.XEP_0198;
using XMPP_API.Classes.Network.XML.Messages.XEP_0249;
using XMPP_API.Classes.Network.XML.Messages.XEP_0363;

namespace XMPP_API.Classes.Network.XML
{
    public class MessageParser2
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly XmlReaderSettings READER_SETTINGS;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 01/09/2017 Created [Fabian Sauter]
        /// </history>
        public MessageParser2()
        {
            this.READER_SETTINGS = new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Fragment };
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public List<AbstractMessage> parseMessages(string msg)
        {
            List<AbstractMessage> messages = new List<AbstractMessage>();

            if (msg.Equals("<stream:features/>"))
            {
                return new List<AbstractMessage>() { new StreamFeaturesMessage(null) };
            }
            // Stream close:
            else if ((msg.Contains(Consts.XML_STREAM_CLOSE)))
            {
                string s = msg.Replace(Consts.XML_STREAM_CLOSE, "");
                if (string.IsNullOrEmpty(s))
                {
                    return new List<AbstractMessage>() { new CloseStreamMessage((XmlNode)null) };
                }
                else
                {
                    if (!s.Contains(Consts.XML_STREAM_START))
                    {
                        msg = s;
                    }
                    messages.Add(new CloseStreamMessage((XmlNode)null));
                }
            }

            // Fix non valid xml strings:
            bool hasCloseStream;
            if (!(hasCloseStream = msg.Contains(Consts.XML_STREAM_CLOSE)))
            {
                if (msg.Contains(Consts.XML_STREAM_START))
                {
                    msg += Consts.XML_STREAM_CLOSE;
                }
            }

            // Fix missing namespace for features:
            // https://github.com/ubiety/xmpp/blob/6abea007d09bdd45313773bdab1c4f920fbf8fbb/ubiety/Common/Namespaces.cs
            msg = msg.Replace(Consts.XML_STREAM_FEATURE_START, "<stream:features " + Consts.XML_STREAM_NAMESPACE + ">");
            msg = msg.Replace(Consts.XML_STREAM_ERROR_START, "<stream:error " + Consts.XML_STREAM_NAMESPACE + ">");

            // Fix for compatibility with the 'smartsupp.com' XMPP server:
            msg = msg.Replace("xmlns=\"http://etherx.jabber.org/streams\"", Consts.XML_STREAM_NAMESPACE);

            // Pars each node:
            foreach (XmlNode n in parseToXmlNodes(msg))
            {
                switch (n.Name)
                {
                    // XEP-0198 (Stream Management):
                    case "a":
                        messages.Add(new SMAnswerMessage(n));
                        break;

                    // XEP-0198 (Stream Management):
                    case "r":
                        messages.Add(new SMRequestMessage(n));
                        break;

                    // Messages:
                    case "message":
                        // XEP-0249 (Direct MUC Invitations):
                        if (XMLUtils.getChildNode(n, "x", Consts.XML_XMLNS, Consts.XML_XEP_0249_NAMESPACE) != null)
                        {
                            messages.Add(new DirectMUCInvitationMessage(n));
                        }
                        // XEP-0085 (chat state):
                        else if (XMLUtils.getChildNode(n, Consts.XML_XMLNS, Consts.XML_XEP_0085_NAMESPACE) != null)
                        {
                            messages.Add(new ChatStateMessage(n));

                            // Chat state messages can contain a body:
                            if (XMLUtils.getChildNode(n, "body") != null)
                            {
                                messages.Add(new MessageMessage(n));
                            }
                        }
                        // Message:
                        else if (XMLUtils.getChildNode(n, "body") != null)
                        {
                            messages.Add(new MessageMessage(n));
                        }
                        // XEP-0045 (MUC room subject):
                        else if (XMLUtils.getChildNode(n, "subject") != null)
                        {
                            messages.Add(new MUCRoomSubjectMessage(n));
                        }
                        break;

                    // Presence:
                    case "presence":
                        foreach (XmlNode n1 in n)
                        {
                            if (string.Equals(n1.Name, "x"))
                            {
                                switch (n1.NamespaceURI)
                                {
                                    case "http://jabber.org/protocol/muc#user":
                                        messages.Add(new MUCMemberPresenceMessage(n));
                                        continue;

                                    case "http://jabber.org/protocol/muc":
                                        messages.Add(new MUCErrorMessage(n));
                                        continue;
                                }
                            }
                        }
                        messages.Add(new PresenceMessage(n));
                        break;

                    // IQ:
                    case "iq":
                        XmlAttribute typeAtt = XMLUtils.getAttribute(n, "type");
                        switch (typeAtt?.InnerText)
                        {
                            case IQMessage.SET:
                                // Rooster:
                                if (XMLUtils.getChildNode(n, "query", Consts.XML_XMLNS, Consts.XML_ROSTER_NAMESPACE) != null)
                                {
                                    messages.Add(new RosterMessage(n));
                                }
                                else
                                {
                                    messages.Add(new IQMessage(n));
                                }
                                break;

                            case IQMessage.RESULT:
                                // XEP-0030 (disco result #info):
                                XmlNode qNode = XMLUtils.getChildNode(n, "query");
                                if (qNode != null)
                                {
                                    switch (qNode.NamespaceURI)
                                    {
                                        case "http://jabber.org/protocol/disco#info":
                                            if (XMLUtils.getChildNode(qNode, "x", Consts.XML_XMLNS, Consts.XML_XEP_0004_NAMESPACE) != null)
                                            {
                                                messages.Add(new ExtendedDiscoResponseMessage(n));
                                            }
                                            // XEP-0045 (MUC discovering reserved room Nicknames):
                                            else if (qNode.Attributes["node"] != null)
                                            {
                                                messages.Add(new DiscoReservedRoomNicknamesResponseMessages(n));
                                            }
                                            else
                                            {
                                                messages.Add(new DiscoResponseMessage(n));
                                            }
                                            break;

                                        // XEP-0030 (disco result #items):
                                        case "http://jabber.org/protocol/disco#items":
                                            messages.Add(new DiscoResponseMessage(n));
                                            break;

                                        // Rooster:
                                        case Consts.XML_ROSTER_NAMESPACE:
                                            messages.Add(new RosterMessage(n));
                                            break;

                                        default:
                                            if (Consts.MUC_ROOM_INFO_NAMESPACE_REGEX.IsMatch(qNode.NamespaceURI))
                                            {
                                                // XEP-0045 (MUC) room info owner:
                                                XmlNode x = XMLUtils.getChildNode(qNode, "x", Consts.XML_XMLNS, Consts.XML_XEP_0004_NAMESPACE);
                                                if (x != null)
                                                {
                                                    messages.Add(new RoomInfoMessage(n));
                                                    break;
                                                }

                                                // XEP-0045 (MUC) ban list:
                                                if (Equals(qNode.NamespaceURI, Consts.XML_XEP_0045_NAMESPACE_ADMIN))
                                                {
                                                    messages.Add(new BanListMessage(n));
                                                    break;
                                                }
                                            }
                                            break;
                                    }
                                }
                                else
                                {
                                    // XEP-0060 (Publish-Subscribe) response:
                                    XmlNode pubSubNode = XMLUtils.getChildNode(n, "pubsub", Consts.XML_XMLNS, Consts.XML_XEP_0060_NAMESPACE);
                                    if (pubSubNode != null)
                                    {
                                        // XEP-0402 (Bookmarks 2) response:
                                        if (XMLUtils.getChildNode(pubSubNode, "items", "node", Consts.XML_XEP_0402_NAMESPACE) != null)
                                        {
                                            messages.Add(new BookmarksResultMessage(n));
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        // XEP-0363 (HTTP File Upload) slot response:
                                        XmlNode slotNode = XMLUtils.getChildNode(n, "slot", Consts.XML_XMLNS, Consts.XML_XEP_0363_NAMESPACE);
                                        if (slotNode != null)
                                        {
                                            messages.Add(new HTTPUploadResponseSlotMessage(n));
                                            break;
                                        }
                                    }
                                }

                                // Default to IQMessage:
                                messages.Add(new IQMessage(n));
                                break;

                            case IQMessage.ERROR:
                                // XEP-0363 (HTTP File Upload) request slot error:
                                XmlNode requestNode = XMLUtils.getChildNode(n, "request", Consts.XML_XMLNS, Consts.XML_XEP_0363_NAMESPACE);
                                if (requestNode != null)
                                {
                                    messages.Add(new HTTPUploadErrorMessage(n));
                                    break;
                                }

                                // Default to IQErrorMessage:
                                messages.Add(new IQErrorMessage(n));
                                break;

                            default:
                                messages.Add(new IQMessage(n));
                                break;
                        }
                        break;

                    // SASL:
                    case "challenge":
                        messages.Add(new ScramSHA1ChallengeMessage(n));
                        break;

                    // SASL failure:
                    case "failure" when Equals(n.NamespaceURI, Consts.XML_SASL_NAMESPACE):
                        messages.Add(new SASLFailureMessage(n));
                        break;

                    // Stream start and end:
                    case "stream:stream":
                        if (hasCloseStream)
                        {
                            messages.Add(new CloseStreamMessage(n));
                        }
                        else
                        {
                            messages.Add(new OpenStreamAnswerMessage(n));
                        }
                        break;

                    // Stream features:
                    case "stream:features":
                        messages.Add(new StreamFeaturesMessage(n));
                        break;

                    // TLS proceed:
                    case "proceed":
                        messages.Add(new ProceedAnswerMessage());
                        break;

                    // SASL success:
                    case "success":
                        messages.Add(new SASLSuccessMessage());
                        break;

                    // XEP-0198 (Stream Management):
                    case "enable":
                        messages.Add(new SMEnableMessage(n));
                        break;

                    // XEP-0198 (Stream Management):
                    case "failed":
                        messages.Add(new SMFailedMessage(n));
                        break;

                    default:
                        Logger.Warn("Unknown message received: " + msg);
                        break;
                }
            }
            return messages;
        }

        /// <summary>
        /// Reads all root nodes from a given xml string and returns them.
        /// </summary>
        /// <param name="msg">A valid xml string.</param>
        /// <returns>A list of XmlNodes read from the given xml string.</returns>
        private List<XmlNode> parseToXmlNodes(string msg)
        {
            List<XmlNode> nodes = new List<XmlNode>();
            XmlDocument doc = new XmlDocument();
            using (XmlReader reader = XmlReader.Create(new StringReader(msg), READER_SETTINGS))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        using (XmlReader fragmentReader = reader.ReadSubtree())
                        {
                            if (fragmentReader.Read())
                            {
                                nodes.Add(toXmlNode(XNode.ReadFrom(fragmentReader) as XElement));
                            }
                        }
                    }
                }
            }
            return nodes;
        }

        #endregion

        #region --Misc Methods (Private)--
        /// <summary>
        /// Translates a given XElement to an XmlNode.
        /// </summary>
        /// <param name="xElement">The XElement that should get translated to an XmlNode.</param>
        /// <returns>The corresponding XmlNode for the given XElement.</returns>
        private XmlNode toXmlNode(XElement xElement)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xElement.ToString());
            return doc.FirstChild;
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
