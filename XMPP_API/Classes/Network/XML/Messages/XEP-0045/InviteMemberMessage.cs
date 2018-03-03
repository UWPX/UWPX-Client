using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0045
{
    class InviteMemberMessage : MessageMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly string ROOM_JID;
        public readonly string ROOM_PASSWORD;
        public readonly string REASON;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 02/03/2018 Created [Fabian Sauter]
        /// </history>
        public InviteMemberMessage(string from, string to, string roomJid, string roomPassword, string reason) : base(from, to, null)
        {
            this.ROOM_JID = roomJid;
            this.ROOM_PASSWORD = roomPassword;
            this.REASON = reason;
        }

        public InviteMemberMessage(XmlNode node) : base(node)
        {
            XmlNode xNode = XMLUtils.getChildNode(node, "x", Consts.XML_XMLNS, Consts.XML_XEP_0045_NAMESPACE_USER);
            if (xNode != null)
            {
                XmlNode inviteNode = XMLUtils.getChildNode(xNode, "invite");
                if (inviteNode != null)
                {
                    ROOM_JID = inviteNode.Attributes["from"].Value;
                    XmlNode reasonNode = XMLUtils.getChildNode(inviteNode, "reason");
                    if (reasonNode != null)
                    {
                        REASON = reasonNode.InnerText;
                    }
                }
                XmlNode passwordNode = XMLUtils.getChildNode(xNode, "password");
                if (inviteNode != null)
                {
                    ROOM_JID = passwordNode.InnerText;
                }
            }
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public override XElement toXElement()
        {
            XElement node = base.toXElement();

            XNamespace ns = Consts.XML_XEP_0045_NAMESPACE_USER;
            XElement xNode = new XElement(ns + "x");
            XElement invNode = new XElement(ns + "invite");
            if (ROOM_JID != null)
            {
                invNode.Add(new XAttribute("to", ROOM_JID));
                if (REASON != null)
                {
                    invNode.Add(new XElement(ns + "reason")
                    {
                        Value = REASON
                    });
                }
            }
            xNode.Add(invNode);

            if (ROOM_PASSWORD != null)
            {
                xNode.Add(new XElement(ns + "password")
                {
                    Value = ROOM_PASSWORD
                });
            }
            node.Add(xNode);

            return node;
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
