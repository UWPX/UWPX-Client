using SQLite;
using XMPP_API.Classes.Network;
using XMPP_API.Classes.Network.TCP;

namespace Data_Manager2.Classes.DBTables
{
    [Table(DBTableConsts.CONNECTION_OPTIONS_TABLE)]
    public class ConnectionOptionsTable
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        [PrimaryKey]
        public string accountId { get; set; }
        public TLSConnectionMode tlsMode { get; set; }
        public bool disableStreamManagement { get; set; }
        public bool disableMessageCarbons { get; set; }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 13/04/2018 Created [Fabian Sauter]
        /// </history>
        public ConnectionOptionsTable()
        {
        }

        public ConnectionOptionsTable(ConnectionConfiguration configuration)
        {
            tlsMode = configuration.tlsMode;
            disableStreamManagement = configuration.disableStreamManagement;
            disableMessageCarbons = configuration.disableMessageCarbons;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void toConnectionConfiguration(ConnectionConfiguration configuration)
        {
            configuration.tlsMode = tlsMode;
            configuration.disableStreamManagement = disableStreamManagement;
            configuration.disableMessageCarbons = disableMessageCarbons;
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
