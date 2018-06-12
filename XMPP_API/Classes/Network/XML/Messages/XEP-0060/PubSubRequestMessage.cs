using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0060
{
    public abstract class PubSubRequestMessage : IQMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        protected static readonly XNamespace NS = Consts.XML_XEP_0060_NAMESPACE;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 12/06/2018 Created [Fabian Sauter]
        /// </history>
        public PubSubRequestMessage(string from, XElement content) : base(from, null, GET, getRandomId(), getQuery(content))
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
        private static XElement getQuery(XElement content)
        {
            XNamespace ns = Consts.XML_XEP_0060_NAMESPACE;
            XElement pubsub = new XElement(ns + "pubsub");
            pubsub.Add(content);
            return pubsub;
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
