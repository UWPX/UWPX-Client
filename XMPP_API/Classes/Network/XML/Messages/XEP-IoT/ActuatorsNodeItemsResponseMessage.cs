using System.Xml;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_IoT
{
    public class ActuatorsNodeItemsResponseMessage: AbstractNodeItemsResponseMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--


        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ActuatorsNodeItemsResponseMessage(XmlNode node) : base(node) { }

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
                if (string.Equals(itemsNode.Name, "items") && string.Equals(itemsNode.Attributes["node"]?.Value, IoTConsts.NODE_NAME_ACTUATORS))
                {
                    foreach (XmlNode itemNode in itemsNode.ChildNodes)
                    {
                        if (string.Equals(itemNode.Name, "item"))
                        {
                            XmlNode valNode = XMLUtils.getChildNode(itemNode, "val", Consts.XML_XMLNS, Consts.XML_XEP_IOT_NAMESPACE);
                            if (!(valNode is null))
                            {
                                VALUES.Add(new IoTValue(itemNode.Attributes["id"]?.Value, valNode));
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
