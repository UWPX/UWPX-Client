using SQLite.Net.Attributes;

namespace Data_Manager2.Classes.DBTables
{
    [Table(DBTableConsts.DISCO_IDENTITY_TABLE)]
    public class DiscoIdentityTable
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        [PrimaryKey]
        // Generated in generateId()
        public string id { get; set; }
        [NotNull]
        // Who owns this identity? e.g. 'plays.shakespeare.lit'
        public string fromServer { get; set; }
        [NotNull]
        // Identity category e.g. 'directory'
        public string category { get; set; }
        [NotNull]
        // The identity type e.g. 'chatroom'
        public string type { get; set; }
        // A name for this identity e.g. 'Play-Specific Chatrooms'
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
        public DiscoIdentityTable()
        {

        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public static string generateId(string fromServer, string type)
        {
            return fromServer + '_' + type;
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
