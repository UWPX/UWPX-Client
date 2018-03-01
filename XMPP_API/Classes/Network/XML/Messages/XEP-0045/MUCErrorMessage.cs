using System.Xml;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0045
{
    public class MUCErrorMessage : PresenceMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly int ERROR_CODE;
        public readonly string ERROR_TYPE;
        public readonly string ERROR_MESSAGE;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 01/03/2018 Created [Fabian Sauter]
        /// </history>
        public MUCErrorMessage(XmlNode node) : base(node)
        {
            XmlNode errorNode = XMLUtils.getChildNode(node, "error");
            if (errorNode != null)
            {
                string code = errorNode.Attributes["code"]?.Value;
                int.TryParse(code, out ERROR_CODE);
                ERROR_TYPE = errorNode.Attributes["type"]?.Value;
                ERROR_MESSAGE = errorNode.InnerXml;
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
