using System.Xml;

namespace XMPP_API.Classes.Network.XML
{
    public class RosterItem
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly string JID;
        public readonly string NAME;
        public readonly string SUBSCRIPTION;
        public readonly string ASK;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 30/08/2017 Created [Fabian Sauter]
        /// </history>
        public RosterItem(XmlNode n)
        {
            JID = n.Attributes["jid"].Value;
            NAME = n.Attributes["name"]?.Value;
            SUBSCRIPTION = n.Attributes["subscription"]?.Value ?? "none";
            ASK = n.Attributes["ask"]?.Value;
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
