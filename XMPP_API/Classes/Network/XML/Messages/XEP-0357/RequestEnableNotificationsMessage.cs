using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0357
{
    public class RequestEnableNotificationsMessage: IQMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly string SERVER;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 18/11/2017 Created [Fabian Sauter]
        /// </history>
        public RequestEnableNotificationsMessage(string server) : base(null, null, SET, getRandomId())
        {
            SERVER = server;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        protected override XElement getQuery()
        {
            XNamespace ns = Consts.XML_XEP_0357_NAMESPACE;
            XElement n = new XElement(ns + "enable");
            n.Add(new XAttribute("jid", SERVER));
            n.Add(new XAttribute("node", "yxs32uqsflafdk3iuqo"));
            return n;
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
