using System.Xml;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0060
{
    public abstract class AbstractPubSubResultMessage : IQMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--


        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 12/06/2018 Created [Fabian Sauter]
        /// </history>
        protected AbstractPubSubResultMessage(XmlNode node) : base(node)
        {
            XmlNode content = XMLUtils.getChildNode(node, "pubsub", Consts.XML_XMLNS, Consts.XML_XEP_0060_NAMESPACE);
            if (content != null)
            {
                loadContent(content.ChildNodes);
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
        protected abstract void loadContent(XmlNodeList content);

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
