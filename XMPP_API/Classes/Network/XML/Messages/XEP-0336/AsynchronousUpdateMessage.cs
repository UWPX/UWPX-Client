using System.Xml;
using System.Xml.Linq;
using XMPP_API.Classes.Network.XML.Messages.XEP_0004;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0336
{
    public class AsynchronousUpdateMessage: AbstractAddressableMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly string SESSION_VARIABLE;
        public readonly DataForm FORM;
        public readonly string LANG;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public AsynchronousUpdateMessage(string from, string to, DataForm form, string sessionVariable, string lang) : base(from, to)
        {
            FORM = form;
            SESSION_VARIABLE = sessionVariable;
            LANG = lang;
        }

        public AsynchronousUpdateMessage(string from, string to, DataForm form, string sessionVariable) : this(from, to, form, sessionVariable, Consts.XML_LANG) { }

        public AsynchronousUpdateMessage(XmlNode node) : base(node?.Attributes["from"]?.Value, node?.Attributes["to"]?.Value, node.Attributes["id"]?.Value)
        {
            XmlNode updateNode = XMLUtils.getChildNode(node, "updated", Consts.XML_XMLNS, Consts.XML_XEP_0336_NAMESPACE);
            if (!(updateNode is null))
            {
                LANG = updateNode.Attributes["xml:lang"]?.Value;
                SESSION_VARIABLE = updateNode.Attributes["sessionVariable"]?.Value;
                XmlNode xNode = XMLUtils.getChildNode(updateNode, "x", Consts.XML_XMLNS, Consts.XML_XEP_0004_NAMESPACE);
                if (!(xNode is null))
                {
                    FORM = new DataForm(xNode);
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
            XElement messageNode = new XElement("message");
            messageNode.Add(new XAttribute("from", FROM));
            messageNode.Add(new XAttribute("to", TO));

            XNamespace ns = Consts.XML_XEP_0336_NAMESPACE;
            XElement updateNode = new XElement(ns + "updated");
            updateNode.Add(new XAttribute("sessionVariable", SESSION_VARIABLE));
            updateNode.Add(new XAttribute("xml:lang", LANG));
            FORM.addToXElement(updateNode);
            messageNode.Add(updateNode);

            return messageNode;
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
