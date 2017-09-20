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

        public UserAccountEntry(ServerConnectionConfiguration account) : base(account.user, account)
        {
            this.server = account.serverAddress;
            this.port = account.port;
            this.presencePriorety = account.presencePriorety;
            this.disabled = account.disabled;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public ServerConnectionConfiguration toServerConnectionConfiguration()
        {
            ServerConnectionConfiguration account = new ServerConnectionConfiguration(toXMPPUser(), server, port)
            {
                presencePriorety = presencePriorety,
                disabled = disabled
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
