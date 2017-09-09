using SQLite.Net.Attributes;
using XMPP_API.Classes;
using XMPP_API.Classes.Network;

namespace Data_Manager.Classes.DBEntries
{
    class UserEntry
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        [PrimaryKey]
        public string id { get; set; }
        public string userAccountEntryId { get; set; }
        public string userId { get; set; }
        public string domain { get; set; }
        public string resource { get; set; }

        public string name { get; set; }
        public string subscription { get; set; }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Construktoren--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 26/08/2017 Created [Fabian Sauter]
        /// </history>
        public UserEntry()
        {
        }

        public UserEntry(XMPPUser user, ServerConnectionConfiguration account)
        {
            this.id = user.userId + '@' + user.domain;
            this.userAccountEntryId = account.getIdAndDomain();
            this.userId = user.userId;
            this.domain = user.domain;
            this.resource = user.resource;
            this.name = user.name;
            this.subscription = user.subscription;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public XMPPUser toXMPPUser()
        {
            XMPPUser user = new XMPPUser(userId, null, domain, resource)
            {
                name = name,
                subscription = subscription
            };
            return user;
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
