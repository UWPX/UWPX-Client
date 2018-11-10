using System;
using System.Xml;
using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0060
{
    public class PubSubSubscriptionMessage : AbstractPubSubMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly string NODE_NAME;
        public readonly string JID;
        public readonly string SUBID;
        public readonly PubSubSubscriptionState SUBSCRIPTION;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 17/07/2018 Created [Fabian Sauter]
        /// </history>
        public PubSubSubscriptionMessage(XmlNode n) : base(n)
        {
            XmlNode subNode = XMLUtils.getChildNode(n, "subscription");
            if (subNode != null)
            {
                this.NODE_NAME = subNode.Attributes["node"]?.Value;
                this.JID = subNode.Attributes["jid"]?.Value;
                this.SUBID = subNode.Attributes["subid"]?.Value;
                if (!Enum.TryParse(subNode.Attributes["subscription"]?.Value?.ToUpper(), out this.SUBSCRIPTION))
                {
                    this.SUBSCRIPTION = PubSubSubscriptionState.NONE;
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
        protected override void addContent(XElement node, XNamespace ns)
        {
            throw new NotImplementedException();
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
