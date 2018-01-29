using SQLite.Net.Attributes;

namespace Data_Manager2.Classes.DBTables
{
    [Table(DBTableConsts.MUC_MEMBER_TABLE)]
    public class MUCMemberTable
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        [PrimaryKey]
        // Generated in generateId()
        public string id { get; set; }
        [NotNull]
        // The id entry of the ChatTable
        public string chatId { get; set; }
        [NotNull]
        // The user nickname e.g. 'thirdwitch'
        public string nickname { get; set; }
        // The affiliation of the member e.g. 'owner'
        public string affiliation { get; set; }
        // The role of the member e.g. 'moderator'
        public string role { get; set; }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 08/01/2018 Created [Fabian Sauter]
        /// </history>
        public MUCMemberTable()
        {

        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public static string generateId(string chatId, string user)
        {
            return chatId + '_' + user;
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
