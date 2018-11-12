using Data_Manager2.Classes.DBTables;
using SQLite;

namespace Data_Manager2.Classes.DBTables.Omemo
{
    [Table(DBTableConsts.OMEMO_SIGNED_PRE_KEY_TABLE)]
    public class OmemoSignedPreKeyTable
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        [PrimaryKey]
        public string id { get; set; }
        [NotNull]
        public uint signedPreKeyId { get; set; }
        [NotNull]
        public string accountId { get; set; }
        [NotNull]
        public byte[] signedPreKey { get; set; }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 03/11/2018 Created [Fabian Sauter]
        /// </history>
        public OmemoSignedPreKeyTable()
        {
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public static string generateId(uint signedPreKeyId, string accountId)
        {
            return signedPreKeyId + "_" + accountId;
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
