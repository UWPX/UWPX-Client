using SQLite.Net.Attributes;
using XMPP_API.Classes.Network;
using XMPP_API.Classes;

namespace Data_Manager2.Classes.DBTables
{
    public class AccountTable
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        [PrimaryKey]
        // An unique id: jabberId @ domain
        public string id { get; set; }
        // The account jabber id 
        public string jabberId { get; set; }
        // XMPP domain e.g. phone or W10PC
        public string domain { get; set; }
        // Some device name
        public string resource { get; set; }
        // The XMPP server address e.g. xmpp.jabber.org
        public string serverAddress { get; set; }
        // Server port e.g. 5222
        public int port { get; set; }
        // The presence priority default = 0
        public int presencePriorety { get; set; }
        // Connect to the account true or false
        public bool disabled { get; set; }
        // A color in hex format e.g. #E91E63
        public string color { get; set; }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 17/11/2017 Created [Fabian Sauter]
        /// </history>
        public AccountTable()
        {

        }

        public AccountTable(XMPPAccount account)
        {
            this.id = account.getIdAndDomain();
            this.jabberId = account.user.userId;
            this.domain = account.user.domain;
            this.resource = account.user.resource;
            this.serverAddress = account.serverAddress;
            this.port = account.port;
            this.disabled = account.disabled;
            this.color = account.color;
        }

        internal XMPPAccount toXMPPAccount()
        {
            return new XMPPAccount(new XMPPUser(jabberId, domain, resource), serverAddress, port)
            {
                color = color,
                presencePriorety = presencePriorety,
                serverAddress = serverAddress
            };
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
