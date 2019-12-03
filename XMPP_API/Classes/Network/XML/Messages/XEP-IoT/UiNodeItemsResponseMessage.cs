using System.Xml;
using XMPP_API.Classes.Network.XML.Messages.XEP_0004;
using XMPP_API.Classes.Network.XML.Messages.XEP_0060;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_IoT
{
    public class UiNodeItemsResponseMessage: AbstractPubSubResultMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public DataForm form;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public UiNodeItemsResponseMessage(XmlNode node) : base(node) { }

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
        protected override void loadContent(XmlNodeList content)
        {
            foreach (XmlNode n in content)
            {
                if (string.Equals(n.Name, "items"))
                {
                    XmlNode itemNode = XMLUtils.getChildNode(n, "item");
                    if (!(itemNode is null))
                    {
                        XmlNode xNode = XMLUtils.getChildNode(itemNode, "x", Consts.XML_XMLNS, Consts.XML_XEP_0004_NAMESPACE);
                        if (!(xNode is null))
                        {
                            form = new DataForm(xNode);
                            return;
                        }
                    }
                }
            }
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
