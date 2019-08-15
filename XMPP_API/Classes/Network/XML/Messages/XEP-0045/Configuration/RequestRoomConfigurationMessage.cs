using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0045.Configuration
{
    public class RequestRoomConfigurationMessage : IQMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly MUCAffiliation SENDER_AFFILIATION;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 07/02/2018 Created [Fabian Sauter]
        /// </history>
        public RequestRoomConfigurationMessage(string to, MUCAffiliation senderAffiliation) : base(null, to, GET, getRandomId())
        {
            SENDER_AFFILIATION = senderAffiliation;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        protected override XElement getQuery()
        {
            XNamespace ns = Consts.XML_XEP_0045_NAMESPACE + '#' + Utils.mucAffiliationToString(SENDER_AFFILIATION);
            return new XElement(ns + "query");
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
