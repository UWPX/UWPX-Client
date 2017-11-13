using XMPP_API.Classes.Network;

namespace Data_Manager.Classes.DBEntries
{
    class UserAccountEntry : UserEntry
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public string server { get; set; }
        public int port { get; set; }
        public int presencePriorety { get; set; }
        public bool disabled { get; set; }
        public string color { get; set; }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 26/08/2017 Created [Fabian Sauter]
        /// </history>
        public UserAccountEntry()
        {

        }

        public UserAccountEntry(XMPPAccount account) : base(account.user, account)
        {
            this.server = account.serverAddress;
            this.port = account.port;
            this.presencePriorety = account.presencePriorety;
            this.disabled = account.disabled;
            this.color = account.color;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public XMPPAccount toXMPPAccount()
        {
            XMPPAccount account = new XMPPAccount(toXMPPUser(), server, port)
            {
                presencePriorety = presencePriorety,
                disabled = disabled,
                color = color
            };
            return account;
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
