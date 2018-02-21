using System.Xml;

namespace XMPP_API.Classes.Network.XML.Messages
{
    public class IQErrorMessage : IQMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly string ERROR_MESSAGE;
        public readonly string ERROR_TYPE;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 21/02/2018 Created [Fabian Sauter]
        /// </history>
        public IQErrorMessage(XmlNode answer) : base(answer)
        {
            XmlNode eNode = XMLUtils.getChildNode(answer, ERROR);
            if (eNode == null)
            {
                this.ERROR_MESSAGE = null;
                this.ERROR_TYPE = null;
            }
            else
            {
                this.ERROR_MESSAGE = eNode.InnerText;
                this.ERROR_TYPE = eNode.Attributes["type"]?.Value;
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
