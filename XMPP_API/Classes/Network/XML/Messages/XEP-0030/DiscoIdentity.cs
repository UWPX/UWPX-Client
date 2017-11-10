using System.Xml;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0030
{
    class DiscoIdentity
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly string CATEGORY;
        public readonly string TYPE;
        public readonly string NAME;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 20/09/2017 Created [Fabian Sauter]
        /// </history>
        public DiscoIdentity(XmlNode n)
        {
            if (n != null)
            {
                CATEGORY = n.Attributes["category"]?.Value;
                TYPE = n.Attributes["type"]?.Value;
                NAME = n.Attributes["name"]?.Value;
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


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
