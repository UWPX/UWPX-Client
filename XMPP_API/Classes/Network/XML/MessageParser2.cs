using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Logging;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.Features;
using XMPP_API.Classes.Network.XML.Messages.Features.SASL;
using XMPP_API.Classes.Network.XML.Messages.Features.SASL.SCRAM;
using XMPP_API.Classes.Network.XML.Messages.Features.TLS;
using XMPP_API.Classes.Network.XML.Messages.XEP_0030;
using XMPP_API.Classes.Network.XML.Messages.XEP_0045;
using XMPP_API.Classes.Network.XML.Messages.XEP_0045.Configuration;
using XMPP_API.Classes.Network.XML.Messages.XEP_0060;
using XMPP_API.Classes.Network.XML.Messages.XEP_0085;
using XMPP_API.Classes.Network.XML.Messages.XEP_0184;
using XMPP_API.Classes.Network.XML.Messages.XEP_0198;
using XMPP_API.Classes.Network.XML.Messages.XEP_0199;
using XMPP_API.Classes.Network.XML.Messages.XEP_0249;
using XMPP_API.Classes.Network.XML.Messages.XEP_0313;
using XMPP_API.Classes.Network.XML.Messages.XEP_0336;
using XMPP_API.Classes.Network.XML.Messages.XEP_0363;
using XMPP_API.Classes.Network.XML.Messages.XEP_0384;
using XMPP_API.Classes.Network.XML.Messages.XEP_0402;
using XMPP_API.Classes.Network.XML.Messages.XEP_IoT;

namespace XMPP_API.Classes.Network.XML
{
    public class MessageParser2
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly MessageParserStats STATS = new MessageParserStats();
        private static readonly XmlReaderSettings READER_SETTINGS = new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Fragment };

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public List<AbstractMessage> parseMessages(ref string msg)
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            List<AbstractMessage> messages = parseMessageInternal(ref msg);
            stopwatch.Stop();
            STATS.onMeasurement(stopwatch.ElapsedMilliseconds);
            return messages;
        }

        /// <summary>
        /// Reads all root nodes from a given XML string and returns them.
        /// </summary>
        /// <param name="msg">A valid XML string.</param>
        /// <returns>A list of XmlNodes read from the given XML string.</returns>
        public static List<XmlNode> parseToXmlNodes(in string msg)
        {
            List<XmlNode> nodes = new List<XmlNode>();
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

        /// <summary>
        /// Translates a given XElement to an XmlNode.
        /// </summary>
        /// <param name="xElement">The XElement that should get translated to an XmlNode.</param>
        /// <returns>The corresponding XmlNode for the given XElement.</returns>
        public static XmlNode toXmlNode(XElement xElement)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xElement.ToString());
            return doc.FirstChild;
        }

        #endregion

        #region --Misc Methods (Private)--
        private List<AbstractMessage> parseMessageInternal(ref string msg)
        {
            List<AbstractMessage> messages = new List<AbstractMessage>();
            // Fix non valid XML strings:
            bool hasCloseStream = msg.Contains(Consts.XML_STREAM_CLOSE);
            bool hasopenStream = msg.Contains(Consts.XML_STREAM_START);
            if (!hasCloseStream)
            {
                if (hasopenStream)
                {
                    msg += Consts.XML_STREAM_CLOSE;
                }
            }
            else if (!hasopenStream)
            {
                msg = msg.Replace(Consts.XML_STREAM_CLOSE, "<stream:stream xmlns:stream='http://etherx.jabber.org/streams' />");
            }

            // Fix missing namespace for features:
            // https://github.com/ubiety/xmpp/blob/6abea007d09bdd45313773bdab1c4f920fbf8fbb/ubiety/Common/Namespaces.cs
            msg = msg.Replace(Consts.XML_STREAM_FEATURE_START, "<stream:features " + Consts.XML_STREAM_NAMESPACE + ">");
            msg = msg.Replace(Consts.XML_STREAM_ERROR_START, "<stream:error " + Consts.XML_STREAM_NAMESPACE + ">");

            // Fix for compatibility with the 'smartsupp.com' XMPP server:
            msg = msg.Replace("xmlns=\"http://etherx.jabber.org/streams\"", Consts.XML_STREAM_NAMESPACE);

            List<XmlNode> nodes;
            try
            {
                nodes = parseToXmlNodes(msg);
            }
            catch (Exception e)
            {
                Logger.Debug($"Failed to parse message as XML with: {e.Message}");
                nodes = null;
            }

            if (nodes is null)
            {
                if (msg.Contains(Consts.XML_STREAM_CLOSE))
                {
                    string s = msg.Replace(Consts.XML_STREAM_CLOSE, "");
                    if (string.IsNullOrEmpty(s))
                    {
                        return new List<AbstractMessage> { new CloseStreamMessage((XmlNode)null) };
                    }
                }
                return messages;
            }

            // Pars each node:
            foreach (XmlNode n in nodes)
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
                        parseMessageMessage(messages, n, CarbonCopyType.NONE);
                        break;

                    // Presence:
                    case "presence":
                        foreach (XmlNode n1 in n)
                        {
                            if (string.Equals(n1.Name, "x"))
                            {
                                switch (n1.NamespaceURI)
                                {
                                    case Consts.XML_XEP_0045_NAMESPACE_USER:
                                        messages.Add(new MUCMemberPresenceMessage(n));
                                        continue;

                                    case Consts.XML_XEP_0045_NAMESPACE:
                                        // messages.Add(new MUCErrorMessage(n)); // Issue #58
                                        continue;

                                    default:
                                        break;
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
                                    messages.Add(new RosterResultMessage(n));
                                }
                                // XEP-0336 (Data Forms - Dynamic Forms):
                                else if (XMLUtils.getChildNode(n, "submit", Consts.XML_XMLNS, Consts.XML_XEP_0336_NAMESPACE) != null)
                                {
                                    messages.Add(new ServerPostBackMessage(n));
                                }
                                // XEP-0336 (Data Forms - Dynamic Forms):
                                else if (XMLUtils.getChildNode(n, "cancel", Consts.XML_XMLNS, Consts.XML_XEP_0336_NAMESPACE) != null)
                                {
                                    messages.Add(new CancelFormMessage(n));
                                }
                                // Fallback to a default IQ Message:
                                else
                                {
                                    messages.Add(new IQMessage(n));
                                }
                                break;

                            case IQMessage.RESULT:
                                XmlNode qNode = XMLUtils.getChildNode(n, "query");
                                bool fondNode = false;
                                if (qNode != null)
                                {
                                    switch (qNode.NamespaceURI)
                                    {
                                        // XEP-0030 (disco result #info):
                                        case Consts.XML_XEP_0030_INFO_NAMESPACE:
                                            if (XMLUtils.getChildNode(qNode, "x", Consts.XML_XMLNS, Consts.XML_XEP_0004_NAMESPACE) != null)
                                            {
                                                if (qNode.Attributes["node"] != null)
                                                {
                                                    // XEP-0060 (PubSub node metadata response):
                                                    messages.Add(new PubSubNodeMetadataResultMessage(n));
                                                }
                                                else
                                                {
                                                    // XEP-0045 (MUC extended disco response):
                                                    messages.Add(new ExtendedDiscoResponseMessage(n));
                                                }
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
                                            fondNode = true;
                                            break;

                                        // XEP-0030 (disco result #items):
                                        case Consts.XML_XEP_0030_ITEMS_NAMESPACE:
                                            messages.Add(new DiscoResponseMessage(n));
                                            fondNode = true;
                                            break;

                                        // Rooster:
                                        case Consts.XML_ROSTER_NAMESPACE:
                                            messages.Add(new RosterResultMessage(n));
                                            fondNode = true;
                                            break;

                                        default:
                                            if (Consts.MUC_ROOM_INFO_NAMESPACE_REGEX.IsMatch(qNode.NamespaceURI))
                                            {
                                                // XEP-0045 (MUC) room info owner:
                                                XmlNode x = XMLUtils.getChildNode(qNode, "x", Consts.XML_XMLNS, Consts.XML_XEP_0004_NAMESPACE);
                                                if (x != null)
                                                {
                                                    messages.Add(new RoomConfigMessage(n));
                                                    fondNode = true;
                                                    break;
                                                }

                                                // XEP-0045 (MUC) ban list:
                                                if (Equals(qNode.NamespaceURI, Consts.XML_XEP_0045_NAMESPACE_ADMIN))
                                                {
                                                    messages.Add(new BanListMessage(n));
                                                    fondNode = true;
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
                                        foreach (XmlNode contentNode in pubSubNode)
                                        {
                                            switch (contentNode.Name)
                                            {
                                                case "items":
                                                    string nodeAttr = contentNode.Attributes["node"]?.Value;
                                                    if (nodeAttr != null)
                                                    {
                                                        // XEP-0048 (Bookmarks) response:
                                                        if (nodeAttr.Equals(Consts.XML_XEP_0048_NAMESPACE))
                                                        {
                                                            messages.Add(new Messages.XEP_0048.BookmarksResultMessage(n));
                                                            fondNode = true;
                                                        }
                                                        // XEP-0384 (OMEMO Encryption) device list:
                                                        else if (nodeAttr.Equals(Consts.XML_XEP_0384_DEVICE_LIST_NODE))
                                                        {
                                                            messages.Add(new OmemoDeviceListResultMessage(n));
                                                            fondNode = true;
                                                        }
                                                        // XEP-0384 (OMEMO Encryption) bundle information:
                                                        else if (nodeAttr.StartsWith(Consts.XML_XEP_0384_BUNDLES_NODE))
                                                        {
                                                            messages.Add(new OmemoBundleInformationResultMessage(n));
                                                            fondNode = true;
                                                        }
                                                        // XEP-0402 (Bookmarks 2) response:
                                                        else if (nodeAttr.Equals(Consts.XML_XEP_0402_NAMESPACE))
                                                        {
                                                            messages.Add(new BookmarksResultMessage(n));
                                                            fondNode = true;
                                                        }
                                                        // XEP-IoT:
                                                        else if (nodeAttr.Equals(IoTConsts.NODE_NAME_UI))
                                                        {
                                                            messages.Add(new UiNodeItemsResponseMessage(n));
                                                            fondNode = true;
                                                        }
                                                        // XEP-IoT:
                                                        else if (nodeAttr.Equals(IoTConsts.NODE_NAME_SENSORS))
                                                        {
                                                            messages.Add(new SensorsNodeItemsResponseMessage(n));
                                                            fondNode = true;
                                                        }
                                                        // XEP-IoT:
                                                        else if (nodeAttr.Equals(IoTConsts.NODE_NAME_ACTUATORS))
                                                        {
                                                            messages.Add(new ActuatorsNodeItemsResponseMessage(n));
                                                            fondNode = true;
                                                        }
                                                    }
                                                    break;

                                                case "publish":
                                                    messages.Add(new PubSubPublishResultMessage(n));
                                                    fondNode = true;
                                                    break;

                                                case "subscription":
                                                    messages.Add(new PubSubSubscriptionMessage(n));
                                                    fondNode = true;
                                                    break;

                                                case "subscriptions":
                                                    messages.Add(new PubSubSubscriptionsResultMessage(n));
                                                    fondNode = true;
                                                    break;

                                                default:
                                                    break;
                                            }
                                            if (fondNode)
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        // XEP-0313 (Message Archive Management):
                                        if (XMLUtils.getChildNode(n, "fin", Consts.XML_XMLNS, Consts.XML_XEP_0313_NAMESPACE) != null)
                                        {
                                            messages.Add(new QueryArchiveFinishMessage(n));
                                            fondNode = true;
                                            break;
                                        }
                                        // XEP-0363 (HTTP File Upload) slot response:
                                        if (XMLUtils.getChildNode(n, "slot", Consts.XML_XMLNS, Consts.XML_XEP_0363_NAMESPACE) != null)
                                        {
                                            messages.Add(new HTTPUploadResponseSlotMessage(n));
                                            fondNode = true;
                                            break;
                                        }
                                    }
                                }

                                if (!fondNode)
                                {
                                    // Default to IQMessage:
                                    messages.Add(new IQMessage(n));
                                }
                                break;

                            case IQMessage.GET:
                                // XEP-0199 (XMPP Ping):
                                XmlNode ping = XMLUtils.getChildNode(n, "ping", Consts.XML_XMLNS, Consts.XML_XEP_0199_NAMESPACE);
                                if (ping != null)
                                {
                                    messages.Add(new PingMessage(n));
                                    break;
                                }

                                // XEP-0030 (disco request)
                                XmlNode query = XMLUtils.getChildNode(n, "query");
                                if (!(query is null))
                                {
                                    if (string.Equals(query.NamespaceURI, Consts.XML_XEP_0030_INFO_NAMESPACE) || string.Equals(query.NamespaceURI, Consts.XML_XEP_0030_ITEMS_NAMESPACE))
                                    {
                                        messages.Add(new DiscoRequestMessage(n));
                                        break;
                                    }
                                }

                                // Default to IQErrorMessage:
                                messages.Add(new IQErrorMessage(n));
                                break;

                            case IQMessage.ERROR:
                                // XEP-0363 (HTTP File Upload) request slot error:
                                XmlNode requestNode = XMLUtils.getChildNode(n, "request", Consts.XML_XMLNS, Consts.XML_XEP_0363_NAMESPACE);
                                if (requestNode != null)
                                {
                                    messages.Add(new HTTPUploadErrorMessage(n));
                                    break;
                                }

                                // XEP-0199 (XMPP Ping) ping failed:
                                XmlNode pingError = XMLUtils.getChildNode(n, "ping", Consts.XML_XMLNS, Consts.XML_XEP_0199_NAMESPACE);
                                if (pingError != null)
                                {
                                    messages.Add(new PingErrorMessage(n));
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
                        messages.Add(new ScramSHAChallengeMessage(n));
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

                    // Stream error:
                    case "stream:error":
                        messages.Add(new StreamErrorMessage(n));
                        break;

                    default:
                        Logger.Warn("Unknown message received: " + msg);
                        break;
                }
            }
            return messages;
        }

        private void parseMamResultMessage(List<AbstractMessage> messages, XmlNode n)
        {
            XmlNode resultNode = XMLUtils.getChildNode(n, "result", Consts.XML_XMLNS, Consts.XML_XEP_0313_NAMESPACE);
            XmlNode forwardedNode = XMLUtils.getChildNode(resultNode, "forwarded", Consts.XML_XMLNS, Consts.XML_XEP_0297_NAMESPACE);
            XmlNode messageNode = XMLUtils.getChildNode(forwardedNode, "message");
            List<AbstractMessage> innerMessages = new List<AbstractMessage>();
            parseMessageMessage(innerMessages, messageNode, CarbonCopyType.NONE);
            if (innerMessages.Count > 0)
            {
                messages.Add(new QueryArchiveResultMessage(n, resultNode, forwardedNode, innerMessages));
            }
        }

        private void parseMessageMessage(List<AbstractMessage> messages, XmlNode n, CarbonCopyType ccType)
        {
            // XEP-0085 (chat state):
            if (XMLUtils.getChildNode(n, Consts.XML_XMLNS, Consts.XML_XEP_0085_NAMESPACE) != null)
            {
                messages.Add(new ChatStateMessage(n));
            }

            // XEP-0384 (OMEMO Encryption):
            if (XMLUtils.getChildNode(n, "encrypted", Consts.XML_XMLNS, Consts.XML_XEP_0384_NAMESPACE) != null)
            {
                messages.Add(new OmemoEncryptedMessage(n, ccType));
            }
            // XEP-0184 (Message Delivery Receipts):
            else if (XMLUtils.getChildNode(n, "received", Consts.XML_XMLNS, Consts.XML_XEP_0184_NAMESPACE) != null)
            {
                messages.Add(new DeliveryReceiptMessage(n));
            }
            // XEP-0313 (Message Archive Management):
            else if (XMLUtils.getChildNode(n, "result", Consts.XML_XMLNS, Consts.XML_XEP_0313_NAMESPACE) != null)
            {
                parseMamResultMessage(messages, n);
            }
            // XEP-0249 (Direct MUC Invitations):
            else if (XMLUtils.getChildNode(n, "x", Consts.XML_XMLNS, Consts.XML_XEP_0249_NAMESPACE) != null)
            {
                messages.Add(new DirectMUCInvitationMessage(n));
            }
            // XEP-0336 (Data Forms - Dynamic Forms):
            else if (XMLUtils.getChildNode(n, "updated", Consts.XML_XMLNS, Consts.XML_XEP_0336_NAMESPACE) != null)
            {
                messages.Add(new AsynchronousUpdateMessage(n));
            }
            // Message:
            else if (XMLUtils.getChildNode(n, "body") != null)
            {
                messages.Add(new MessageMessage(n, ccType));
            }
            // XEP-0045 (MUC room subject):
            else if (XMLUtils.getChildNode(n, "subject") != null)
            {
                messages.Add(new MUCRoomSubjectMessage(n));
            }
            else
            {
                // XEP-0280 (Message Carbons):
                bool sendCC = false;
                XmlNode carbNode = XMLUtils.getChildNode(n, "received", Consts.XML_XMLNS, Consts.XML_XEP_0280_NAMESPACE);
                if (carbNode is null)
                {
                    sendCC = true;
                    carbNode = XMLUtils.getChildNode(n, "sent", Consts.XML_XMLNS, Consts.XML_XEP_0280_NAMESPACE);
                }
                if (carbNode != null)
                {
                    XmlNode forwardedNode = XMLUtils.getChildNode(carbNode, "forwarded", Consts.XML_XMLNS, Consts.XML_XEP_0280_NAMESPACE_FORWARDED);
                    if (forwardedNode != null)
                    {
                        XmlNode messageNode = XMLUtils.getChildNode(forwardedNode, "message");
                        if (messageNode != null)
                        {
                            parseMessageMessage(messages, messageNode, sendCC ? CarbonCopyType.SENT : CarbonCopyType.RECEIVED);
                        }
                    }
                }

                // XEP-0060 (Publish-Subscribe) events:
                XmlNode eventNode = XMLUtils.getChildNode(n, "event", Consts.XML_XMLNS, Consts.XML_XEP_0060_NAMESPACE_EVENT);
                if (eventNode != null)
                {
                    // XEP-0384 (OMEMO Encryption) device list:
                    if (XMLUtils.getChildNode(eventNode, "items", "node", Consts.XML_XEP_0384_DEVICE_LIST_NODE) != null)
                    {
                        /**
                         * Disabled for now since we can not distinguish from whom they are coming from.
                         * For example if the from='pubsub.example.com' and we have multiple chats with this server.
                         **/
                        // messages.Add(new OmemoDeviceListEventMessage(n));
                    }
                    // XEP-IoT sensor changed:
                    else if (XMLUtils.getChildNode(eventNode, "items", "node", IoTConsts.NODE_NAME_SENSORS) != null)
                    {
                        messages.Add(new SensorsNodeEventMessage(n));
                    }
                    // XEP-IoT actuator changed:
                    else if (XMLUtils.getChildNode(eventNode, "items", "node", IoTConsts.NODE_NAME_ACTUATORS) != null)
                    {
                        messages.Add(new ActuatorsNodeEventMessage(n));
                    }
                    // XEP-IoT UI changed:
                    else if (XMLUtils.getChildNode(eventNode, "items", "node", IoTConsts.NODE_NAME_UI) != null)
                    {
                        messages.Add(new UiNodeEventMessage(n));
                    }
                }
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
