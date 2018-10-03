using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0060
{
    public abstract class AbstractPubSubPublishMessage : AbstractPubSubMessage
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
        protected AbstractPubSubPublishMessage(string from, string to, string nodeName) : base(from, to)
        {
            this.NODE_NAME = nodeName;
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
        protected override void addContent(XElement node, XNamespace ns)
        {
            XElement publishNode = new XElement(ns + "publish");
            publishNode.Add(new XAttribute("node", NODE_NAME));
            AbstractPubSubItem item = getPubSubItem();
            if (item != null)
            {
                publishNode.Add(item.toXElement(ns));
            }
            node.Add(publishNode);

            PubSubPublishOptions options = getPublishOptions();
            if (options != null)
            {
                node.Add(options.toXElement(ns));
            }
        }

        protected abstract PubSubPublishOptions getPublishOptions();
        protected abstract AbstractPubSubItem getPubSubItem();

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
