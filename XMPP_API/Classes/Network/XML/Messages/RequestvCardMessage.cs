using System.Xml;

namespace XMPP_API.Classes.Network.XML.Messages
{
    public class RequestvCardMessage : IQMessage
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
        /// 08/09/2017 Created [Fabian Sauter]
        /// </history>
        public RequestvCardMessage(string target, string from) : base(from, target, IQMessage.GET, getRandomId(), "<vCard xmlns='vcard-temp'/>")
        {
        }

        public RequestvCardMessage(XmlNode answer) : base(answer)
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


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
