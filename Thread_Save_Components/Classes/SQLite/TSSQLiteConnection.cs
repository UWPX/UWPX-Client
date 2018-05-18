using SQLite;
using System.Collections.Generic;

namespace Thread_Save_Components.Classes.SQLite
{
    public class TSSQLiteConnection
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        protected SQLiteConnection dB;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 05/01/2018 Created [Fabian Sauter]
        /// </history>
        public TSSQLiteConnection(string dBPath)
        {
            dB = new SQLiteConnection(dBPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public SQLiteCommand CreateCommand(string cmdText, params object[] args)
        {
            return dB.CreateCommand(cmdText, args);
        }

        public void BeginTransaction()
        {
            dB.BeginTransaction();
        }

        public List<T> ExecuteCommand<T>(bool readOnly, SQLiteCommand cmd) where T : new()
        {
            return cmd.ExecuteQuery<T>();
        }

        public int InsertOrReplace(object obj)
        {
            return dB.InsertOrReplace(obj); ;
        }

        public void Close()
        {
            dB.Close();
        }

        public int Execute(string query, params object[] args)
        {
            return dB.Execute(query, args);
        }

        public void Commit()
        {
            dB.Commit();
        }

        /// <param name="readOnly">Unused/placeholder!</param>
        public List<T> Query<T>(bool readOnly, string query, params object[] args) where T : new()
        {
            return dB.Query<T>(query, args);
        }

        public int CreateTable<T>() where T : new()
        {
            return dB.CreateTable<T>();
        }

        public int DropTable<T>() where T : new()
        {
            return dB.DropTable<T>();
        }

        public int RecreateTable<T>() where T : new()
        {
            dB.DropTable<T>();
            return dB.CreateTable<T>();
        }

        public int Delete(object objectToDelete)
        {
            return dB.Delete(objectToDelete);
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
