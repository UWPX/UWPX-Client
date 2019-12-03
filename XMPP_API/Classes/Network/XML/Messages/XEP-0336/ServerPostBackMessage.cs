using System.Xml;
using System.Xml.Linq;
using XMPP_API.Classes.Network.XML.Messages.XEP_0004;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0336
{
    public class ServerPostBackMessage: IQMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly DataForm FORM;
        public readonly string LANG;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ServerPostBackMessage(string from, string to, DataForm form) : this(from, to, form, "en") { }

        public ServerPostBackMessage(string from, string to, DataForm form, string lang) : base(from, to, SET, getRandomId())
        {
            FORM = form;
            LANG = lang;
        }

        public ServerPostBackMessage(XmlNode node) : base(node)
        {
            XmlNode submitNode = XMLUtils.getChildNode(node, "submit", Consts.XML_XMLNS, Consts.XML_XEP_0336_NAMESPACE);
            if (!(submitNode is null))
            {
                LANG = submitNode.Attributes["xml:lang"]?.Value;
                XmlNode xNode = XMLUtils.getChildNode(submitNode, "x", Consts.XML_XMLNS, Consts.XML_XEP_0004_NAMESPACE);
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
            XElement submitNode = new XElement(ns + "submit");
            submitNode.Add(new XAttribute("xml:lang", LANG));
            FORM.addToXElement(submitNode);
            return submitNode;
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
