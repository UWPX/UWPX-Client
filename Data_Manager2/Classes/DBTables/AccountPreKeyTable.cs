using SQLite;

namespace Data_Manager2.Classes.DBTables
{
    [Table(DBTableConsts.ACCOUNT_PRE_KEY_TABLE)]
    public class AccountPreKeyTable
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        [PrimaryKey]
        public string id { get; set; }
        [NotNull]
        public string accountId { get; set; }
        [NotNull]
        public byte[] preKey { get; set; }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 06/08/2018 Created [Fabian Sauter]
        /// </history>
        public AccountPreKeyTable()
        {
        }

        public AccountPreKeyTable(string accountId, byte[] preKey)
        {
            this.id = generateId(accountId, System.Text.Encoding.ASCII.GetString(preKey));
            this.accountId = accountId;
            this.preKey = preKey;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public static string generateId(string accountId, string preKey)
        {
            return accountId + "_" + preKey;
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
