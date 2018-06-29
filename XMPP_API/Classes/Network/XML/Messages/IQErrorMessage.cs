using System.Xml;

namespace XMPP_API.Classes.Network.XML.Messages
{
    public class IQErrorMessage : IQMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly Error ERROR_OBJ;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 21/02/2018 Created [Fabian Sauter]
        /// </history>
        public IQErrorMessage(XmlNode n) : base(n)
        {
            XmlNode eNode = XMLUtils.getChildNode(n, ERROR);
            if (eNode == null)
            {
                this.ERROR_OBJ = new Error();
            }
            else
            {
                this.ERROR_OBJ = new Error(eNode);
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
