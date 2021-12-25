using System.Xml;
using XMPP_API.Classes.Network.XML.Messages.XEP_0060;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0084
{
    public class AvatarResponseMessage: AbstractPubSubResultMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public string HASH { get; private set; }
        public string AVATAR_BASE_64 { get; private set; }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public AvatarResponseMessage(XmlNode node) : base(node) { }

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
            foreach (XmlNode itemsNode in content)
            {
                if (string.Equals(itemsNode.Name, "items") && string.Equals(itemsNode.Attributes["node"]?.Value, Consts.XML_XEP_0084_DATA_NAMESPACE))
                {
                    foreach (XmlNode itemNode in itemsNode.ChildNodes)
                    {
                        if (string.Equals(itemNode.Name, "item"))
                        {
                            XmlAttribute idAttr = XMLUtils.getAttribute(itemNode, "id");
                            if (idAttr is not null && !string.IsNullOrEmpty(idAttr.Value))
                            {
                                HASH = idAttr.Value;
                                XmlNode dataNode = XMLUtils.getChildNode(itemNode, "data", Consts.XML_XMLNS, Consts.XML_XEP_0084_DATA_NAMESPACE);
                                if (dataNode is not null)
                                {
                                    AVATAR_BASE_64 = dataNode.InnerText;
                                    return;
                                }
                            }
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
