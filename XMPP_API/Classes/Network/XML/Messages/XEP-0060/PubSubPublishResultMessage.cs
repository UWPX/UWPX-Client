using System.Xml;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0060
{
    public class PubSubPublishResultMessage : AbstractPubSubResultMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public string NODE_NAME { get; private set; }
        public string ITEM_ID { get; private set; }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 07/08/2018 Created [Fabian Sauter]
        /// </history>
        public PubSubPublishResultMessage(XmlNode node) : base(node)
        {
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
        protected override void loadContent(XmlNodeList content)
        {
            foreach (XmlNode n in content)
            {
                if (string.Equals(n.Name, "publish"))
                {
                    NODE_NAME = n.Attributes["node"]?.Value;
                    XmlNode itemNode = XMLUtils.getChildNode(n, "item");
                    ITEM_ID = itemNode?.Attributes["id"]?.Value;
                }
            }
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
