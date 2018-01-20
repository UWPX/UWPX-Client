using SQLite.Net;
using SQLite.Net.Platform.WinRT;
using System.Collections.Generic;

namespace Thread_Save_Components.Classes.SQLite
{
    public class TSSQLiteConnection
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        protected SQLiteConnection dB;

        private static readonly object _dBLocker = new object();

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
            dB = new SQLiteConnection(new SQLitePlatformWinRT(), dBPath);
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public int InsertOrReplace(object obj)
        {
            lock (_dBLocker)
            {
                return dB.InsertOrReplace(obj);
            }
        }

        public void Close()
        {
            lock (_dBLocker)
            {
                dB.Close();
            }
        }

        public int Execute(string query, params object[] args)
        {
            lock (_dBLocker)
            {
                return dB.Execute(query, args);
            }
        }

        public List<T> Query<T>(string query, params object[] args) where T : class
        {
            lock (_dBLocker)
            {
                return dB.Query<T>(query, args);
            }
        }

        public int CreateTable<T>() where T : class
        {
            lock (_dBLocker)
            {
                return dB.CreateTable<T>();
            }
        }

        public int DropTable<T>() where T : class
        {
            lock (_dBLocker)
            {
                return dB.DropTable<T>();
            }
        }

        public int RecreateTable<T>() where T : class
        {
            lock (_dBLocker)
            {
                dB.DropTable<T>();
                return dB.CreateTable<T>();
            }
        }

        public int Delete(object objectToDelete)
        {
            lock (_dBLocker)
            {
                return dB.Delete(objectToDelete);
            }
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
