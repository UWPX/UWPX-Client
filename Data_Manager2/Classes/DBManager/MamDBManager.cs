using System.Collections.Generic;
using Data_Manager2.Classes.DBTables;
using Shared.Classes.SQLite;

namespace Data_Manager2.Classes.DBManager
{
    public class MamDBManager: AbstractDBManager
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public static readonly MamDBManager INSTANCE = new MamDBManager();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public MamRequestTable getLastRequest(string userAccountId)
        {
            List<MamRequestTable> results = dB.Query<MamRequestTable>(true, "SELECT * FROM " + DBTableConsts.MAM_TABLE + " WHERE " + nameof(MamRequestTable.accountId) + " = ?;", userAccountId);
            return results.Count > 0 ? results[0] : null;
        }

        public void setLastRequest(MamRequestTable request)
        {
            dB.InsertOrReplace(request);
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--
        protected override void CreateTables()
        {
            dB.CreateTable<MamRequestTable>();
        }

        protected override void DropTables()
        {
            dB.DropTable<MamRequestTable>();
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
