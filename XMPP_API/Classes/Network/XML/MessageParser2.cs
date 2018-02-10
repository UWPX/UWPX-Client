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
using XMPP_API.Classes.Network.XML.Messages.XEP_0048_1_0;
using XMPP_API.Classes.Network.XML.Messages.XEP_0085;

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
            else if((msg.Contains(Consts.XML_STREAM_CLOSE)))
            {
                string s = msg.Replace(Consts.XML_STREAM_CLOSE, "");
                if(string.IsNullOrEmpty(s))
                {
                    return new List<AbstractMessage>() { new CloseStreamMessage((XmlNode)null) };
                }
                else
                {
                    if(!s.Contains(Consts.XML_STREAM_START))
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

            // Pars each node:
            foreach (XmlNode n in parseToXmlNodes(msg))
            {
                if(n.Name == null)
                {
                    continue;
                }

                switch (n.Name)
                {
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

                    // Messages:
                    case "message":
                        // Message:
                        if(XMLUtils.getChildNode(n, "body") != null)
                        {
                            messages.Add(new MessageMessage(n));
                        }
                        // XEP-0085 (chat state):
                        else
                        {
                            ChatStateMessage chatStateMessage = new ChatStateMessage(n);
                            // Check if containing a valid chat state:
                            if (chatStateMessage.getState() != ChatState.UNKNOWN)
                            {
                                messages.Add(chatStateMessage);
                            }
                        }
                        break;

                    // Presence:
                    case "presence":
                        XmlNode xNode = XMLUtils.getChildNode(n, "x", "xmlns", "http://jabber.org/protocol/muc#user");
                        if(xNode != null)
                        {
                            messages.Add(new MUCMemberPresenceMessage(n));
                        }
                        else
                        {
                            messages.Add(new PresenceMessage(n));
                        }
                        break;

                    // IQ:
                    case "iq":
                        XmlAttribute typeAtt = XMLUtils.getAttribute(n, "type");
                        if(typeAtt != null && Equals(typeAtt.InnerText, "result"))
                        {
                            // XEP-0030 (disco result #info):
                            XmlNode qNode = XMLUtils.getChildNode(n, "query", "xmlns", "http://jabber.org/protocol/disco#info");
                            if(qNode != null)
                            {
                                if (XMLUtils.getChildNode(qNode, "x", "xmlns", "jabber:x:data") != null)
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
                            }
                            // XEP-0030 (disco result #items):
                            else if (XMLUtils.getChildNode(n, "query", "xmlns", "http://jabber.org/protocol/disco#items") != null)
                            {
                                messages.Add(new DiscoResponseMessage(n));
                            }
                            // Rooster:
                            else if (XMLUtils.getChildNode(n, "query", "xmlns", "jabber:iq:roster") != null)
                            {
                                messages.Add(new RosterMessage(n));
                            }
                            // XEP-0048-1.0 (bookmarks result):
                            else if (XMLUtils.getChildNode(n, "query", "xmlns", "jabber:iq:private") != null)
                            {
                                messages.Add(new BookmarksResultMessage(n));
                            }
                            // XEP-0045 (MUC) room info owner:
                            else if (XMLUtils.getChildNode(n, "query", "xmlns", Consts.MUC_ROOM_INFO_NAMESPACE_REGEX) != null)
                            {
                                messages.Add(new RoomInfoResponseMessage(n));
                            }
                            else
                            {
                                messages.Add(new IQMessage(n));
                            }
                        }
                        else
                        {
                            messages.Add(new IQMessage(n));
                        }
                        break;

                    // SASL:
                    case "challenge":
                        messages.Add(new ScramSha1ChallengeMessage(n));
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
                while(reader.Read())
                {
                    if(reader.NodeType == XmlNodeType.Element)
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
