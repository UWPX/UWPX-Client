using System.Xml;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0060
{
    public abstract class PubSubResultMessage : IQMessage
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
        public PubSubResultMessage(XmlNode node) : base(node)
        {
            XmlNode content = XMLUtils.getChildNode(node, "pubsub", Consts.XML_XMLNS, Consts.XML_XEP_0060_NAMESPACE);
            if (content != null)
            {
                foreach (XmlNode n in content.ChildNodes)
                {
                    loadContent(n);
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
        protected abstract void loadContent(XmlNode content);

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
