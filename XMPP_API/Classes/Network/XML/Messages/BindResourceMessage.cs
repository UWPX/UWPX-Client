using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages
{
    public class BindResourceMessage : IQMessage
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
        /// 03/02/2018 Created [Fabian Sauter]
        /// </history>
        public BindResourceMessage(string resource) : base(null, null, SET, getRandomId(), getResourceQuery(resource))
        {
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private static XElement getResourceQuery(string resource)
        {
            XNamespace ns = XNamespace.Get("urn:ietf:params:xml:ns:xmpp-bind");
            XElement node = new XElement(ns + "bind");
            node.Add(new XElement(ns + "resource", resource));
            return node;
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
