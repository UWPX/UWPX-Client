using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0060
{
    public abstract class AbstractPubSubRequestMessage : IQMessage
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
        protected AbstractPubSubRequestMessage(string from, string to) : base(from, to, GET, getRandomId())
        {
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        protected override XElement getQuery()
        {
            XNamespace ns = Consts.XML_XEP_0060_NAMESPACE;
            XElement pubsub = new XElement(ns + "pubsub");
            XElement content = getContent(ns);
            if (content != null)
            {
                pubsub.Add(content);
            }
            return pubsub;
        }

        protected abstract XElement getContent(XNamespace ns);

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
