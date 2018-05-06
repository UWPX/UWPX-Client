using System.Xml;

namespace XMPP_API.Classes.Network.XML.Messages.Features
{
    class StreamFeature
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly string NAME;
        public readonly string NAMESPACE;
        public readonly bool REQUIRED;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 20/08/2017 Created [Fabian Sauter]
        /// </history>
        public StreamFeature(XmlNode n)
        {
            this.NAME = n.Name;
            this.NAMESPACE = n.NamespaceURI;
            this.REQUIRED = XMLUtils.getChildNode(n, "required") != null;
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


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
