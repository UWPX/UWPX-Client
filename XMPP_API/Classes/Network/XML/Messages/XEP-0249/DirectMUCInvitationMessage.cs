using System.Xml;
using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0249
{
    // https://xmpp.org/extensions/xep-0249.html
    public class DirectMUCInvitationMessage : MessageMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly string ROOM_JID;
        public readonly string ROOM_PASSWORD;
        public readonly string REASON;

        public const string TYPE_MUC_DIRECT_INVITATION = "muc_direct_invitation";

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 02/03/2018 Created [Fabian Sauter]
        /// </history>
        public DirectMUCInvitationMessage(string from, string to, string roomJid, string roomPassword, string reason) : base(from, to, null, null)
        {
            this.ROOM_JID = roomJid;
            this.ROOM_PASSWORD = roomPassword;
            this.REASON = reason;
        }

        public DirectMUCInvitationMessage(XmlNode node) : base(node, TYPE_MUC_DIRECT_INVITATION)
        {
            XmlNode xNode = XMLUtils.getChildNode(node, "x", Consts.XML_XMLNS, Consts.XML_XEP_0249_NAMESPACE);
            if (xNode != null)
            {
                ROOM_JID = xNode.Attributes["jid"]?.Value;
                ROOM_PASSWORD = xNode.Attributes["password"]?.Value;
                REASON = xNode.Attributes["reason"]?.Value;
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

            XNamespace ns = Consts.XML_XEP_0249_NAMESPACE;
            XElement xNode = new XElement(ns + "x");

            if (ROOM_JID != null)
            {
                xNode.Add(new XAttribute("jid", ROOM_JID));
            }

            if (ROOM_PASSWORD != null)
            {
                xNode.Add(new XAttribute("password", ROOM_PASSWORD));
            }

            if (REASON != null)
            {
                xNode.Add(new XAttribute("reason", REASON));
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
