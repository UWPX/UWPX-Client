using System.Xml;
using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0045
{
    public class MUCRoomSubjectMessage : MessageMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly string SUBJECT;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 20/02/2018 Created [Fabian Sauter]
        /// </history>
        public MUCRoomSubjectMessage(XmlNode node) : base(node, CarbonCopyType.NONE)
        {
            XmlNode sNode = XMLUtils.getChildNode(node, "subject");
            if (sNode != null)
            {
                this.SUBJECT = sNode.InnerText;
            }
        }

        public MUCRoomSubjectMessage(string from, string to, string subject) : base(from, to, null, TYPE_GROUPCHAT, null, false)
        {
            this.SUBJECT = subject;
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
            node.Add(new XElement("subject", SUBJECT ?? ""));
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
