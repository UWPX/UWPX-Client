using System;
using SQLite;

namespace Data_Manager2.Classes.DBTables
{
    [Table(DBTableConsts.MAM_TABLE)]
    public class MamRequestTable
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        [PrimaryKey]
        // The id entry of the AccountTable
        public string accountId { get; set; }
        [NotNull]
        // The date and time of the last refresh
        public DateTime lastUpdate { get; set; }
        // The message ID of the last message
        public string lastMsgId { get; set; }

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
