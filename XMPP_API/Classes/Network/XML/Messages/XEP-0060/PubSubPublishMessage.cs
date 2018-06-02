using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0060
{
    class PubSubPublishMessage : IQMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly string NODE_NAME;
        public readonly PubSubItem ITEM;
        public readonly PubSubPublishOptions OPTIONS;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 02/06/2018 Created [Fabian Sauter]
        /// </history>
        public PubSubPublishMessage(string from, string nodeName, PubSubItem item, PubSubPublishOptions options) : base(from, null, SET, getRandomId(), getPubsubNode(nodeName, item, options))
        {
            this.NODE_NAME = nodeName;
            this.ITEM = item;
            this.OPTIONS = options;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private static XElement getPubsubNode(string nodeName, PubSubItem item, PubSubPublishOptions options)
        {
            XNamespace ns = Consts.XML_XEP_0060_NAMESPACE;
            XElement pubsubNode = new XElement(ns + "pubsub");
            pubsubNode.Add(new XAttribute("node", nodeName));
            pubsubNode.Add(item.toXElement());
            pubsubNode.Add(options.toXElement());

            return pubsubNode;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


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
