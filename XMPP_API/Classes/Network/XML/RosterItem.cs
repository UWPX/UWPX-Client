using System.Xml;

namespace XMPP_API.Classes.Network.XML
{
    public class RosterItem
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly string JABBER_ID;
        private readonly string NAME;
        private readonly string SUBSCRIPTION;
        private readonly string ASK;

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
            this.JABBER_ID = n.Attributes["jid"].Value;
            this.NAME = n.Attributes["name"]?.Value;
            this.SUBSCRIPTION = n.Attributes["subscription"]?.Value ?? "none";
            this.ASK = n.Attributes["ask"]?.Value;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public string getJabberId()
        {
            return JABBER_ID;
        }

        public string getName()
        {
            return NAME;
        }

        public string getSubscription()
        {
            return SUBSCRIPTION;
        }

        public string getAsk()
        {
            return ASK;
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
