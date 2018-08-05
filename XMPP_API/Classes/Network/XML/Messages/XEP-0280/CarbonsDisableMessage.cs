using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0280
{
    public class CarbonsDisableMessage : IQMessage
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
        /// 05/08/2018 Created [Fabian Sauter]
        /// </history>
        public CarbonsDisableMessage(string from) : base(from, null, SET, getRandomId())
        {
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        protected override XElement getQuery()
        {
            XNamespace ns = Consts.XML_XEP_0280_NAMESPACE;
            XElement enableNode = new XElement(ns + "disable");
            return enableNode;
        }

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
