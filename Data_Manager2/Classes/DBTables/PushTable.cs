using SQLite;

namespace Data_Manager2.Classes.DBTables
{
    [Table(DBTableConsts.PUSH_TABLE)]
    public class PushTable
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        [PrimaryKey]
        // The id entry of the AccountTable
        public string accountId { get; set; }
        [NotNull]
        // The name of the PubSub node
        public string node { get; set; }
        [NotNull]
        // The secret for the PubSub node
        public string secret { get; set; }
        [NotNull]
        // The bare JID of the push server e.g. 'push@xmpp.example.com'
        public string serverBareJid { get; set; }
        // Has the current node and secret been published to the XMPP server
        public bool published { get; set; }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


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
