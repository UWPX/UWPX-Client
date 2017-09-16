using System.Xml;

namespace XMPP_API.Classes.Network.XML.Messages
{
    class RemoveFromRosterMessage : IQMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--


        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Construktoren--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 09/09/2017 Created [Fabian Sauter]
        /// </history>
        public RemoveFromRosterMessage(XmlNode answer) : base(answer)
        {
        }

        public RemoveFromRosterMessage(string fullJabberId, string target) : base(fullJabberId, null, SET, getRandomId(), "<query xmlns='jabber:iq:roster'><item jid='" + target + "' subscription='remove'/></query>")
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
