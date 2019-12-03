using System.Xml;
using System.Xml.Linq;
using XMPP_API.Classes.Network.XML.Messages.XEP_0004;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0336
{
    public class CancelFormMessage: IQMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly DataForm FORM;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public CancelFormMessage(string from, string to, DataForm form) : base(from, to, SET, getRandomId())
        {
            FORM = form;
        }

        public CancelFormMessage(XmlNode node) : base(node)
        {
            XmlNode cancelNode = XMLUtils.getChildNode(node, "cancel", Consts.XML_XMLNS, Consts.XML_XEP_0336_NAMESPACE);
            if (!(cancelNode is null))
            {
                XmlNode xNode = XMLUtils.getChildNode(cancelNode, "x", Consts.XML_XMLNS, Consts.XML_XEP_0004_NAMESPACE);
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


        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--
        protected override XElement getQuery()
        {
            XNamespace ns = Consts.XML_XEP_0336_NAMESPACE;
            XElement cancelNode = new XElement(ns + "cancel");
            FORM.addToXElement(cancelNode);
            return cancelNode;
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
