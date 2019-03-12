using SQLite;
using System;

namespace Data_Manager2.Classes.DBTables
{
    [Table(DBTableConsts.SPAM_MESSAGE_TABLE)]
    public class SpamMessageTable
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        [AutoIncrement, PrimaryKey]
        public int id { get; set; }
        public DateTime lastReceived { get; set; }
        public int count { get; set; }
        public string text { get; set; }

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
