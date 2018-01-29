using SQLite.Net.Attributes;

namespace Data_Manager2.Classes.DBTables
{
    [Table(DBTableConsts.DISCO_ITEM_TABLE)]
    public class DiscoItemTable
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        [PrimaryKey]
        // Generated in generateId()
        public string id { get; set; }
        [NotNull]
        // Who owns this item? e.g. 'shakespeare.lit'
        public string fromServer { get; set; }
        [NotNull]
        // The JID of the item e.g. 'chat.shakespeare.lit'
        public string jid { get; set; }
        // A name for the item e.g. 'Chatroom Service'
        public string name { get; set; }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 03/01/2018 Created [Fabian Sauter]
        /// </history>
        public DiscoItemTable()
        {

        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public static string generateId(string fromServer, string jid)
        {
            return fromServer + '_' + jid;
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
