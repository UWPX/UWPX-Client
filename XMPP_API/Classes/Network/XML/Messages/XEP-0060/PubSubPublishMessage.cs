using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0060
{
    public abstract class PubSubPublishMessage : PubSubMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly string NODE_NAME;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 02/06/2018 Created [Fabian Sauter]
        /// </history>
        public PubSubPublishMessage(string from, string nodeName) : base(from)
        {
            this.NODE_NAME = nodeName;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        protected override XElement getContent(XNamespace ns)
        {
            XElement publishNode = new XElement(ns + "publish");
            publishNode.Add(new XAttribute("node", NODE_NAME));
            AbstractPubSubItem item = getPubSubItem();
            if (item != null)
            {
                publishNode.Add(item.toXElement(ns));
            }

            PubSubPublishOptions options = getPublishOptions();

            if (options != null)
            {
                publishNode.Add(options.toXElement());
            }
            return publishNode;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--
        protected abstract PubSubPublishOptions getPublishOptions();
        protected abstract AbstractPubSubItem getPubSubItem();

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
