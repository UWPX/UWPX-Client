using System.Xml;
using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0184
{
    public class DeliveryReceiptMessage: AbstractAddressableMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly string RECEIPT_ID;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 31/07/2018 Created [Fabian Sauter]
        /// </history>
        public DeliveryReceiptMessage(string from, string to, string receiptId) : base(from, to, getRandomId())
        {
            RECEIPT_ID = receiptId;
        }

        public DeliveryReceiptMessage(XmlNode node) : base(node.Attributes["from"]?.Value, node.Attributes["to"]?.Value, (node.Attributes["id"]?.Value) ?? getRandomId())
        {
            XmlNode recNode = XMLUtils.getChildNode(node, "received", Consts.XML_XMLNS, Consts.XML_XEP_0184_NAMESPACE);
            RECEIPT_ID = recNode?.Attributes["id"]?.Value;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public override XElement toXElement()
        {
            XElement msgNode = new XElement("message");

            if (FROM != null)
            {
                msgNode.Add(new XAttribute("from", FROM));
            }

            if (TO != null)
            {
                msgNode.Add(new XAttribute("to", TO));
            }

            if (ID != null)
            {
                msgNode.Add(new XAttribute("id", ID));
            }

            XNamespace ns = Consts.XML_XEP_0184_NAMESPACE;
            XElement recNode = new XElement(ns + "received");
            recNode.Add(new XAttribute("id", RECEIPT_ID));
            msgNode.Add(recNode);
            return msgNode;
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
