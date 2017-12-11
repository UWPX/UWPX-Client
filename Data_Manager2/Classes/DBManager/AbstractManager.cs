using Data_Manager2.Classes.DBTables;
using Logging;
using SQLite.Net;
using SQLite.Net.Platform.WinRT;
using System;
using System.IO;
using Windows.Storage;

namespace Data_Manager2.Classes.DBManager
{
    public abstract class AbstractManager
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private static readonly string DB_PATH = Path.Combine(ApplicationData.Current.LocalFolder.Path, "data2.db");
        protected static SQLiteConnection dB = new SQLiteConnection(new SQLitePlatformWinRT(), DB_PATH);

        public static readonly bool RESET_DB_ON_STARTUP = false;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 17/11/2017 Created [Fabian Sauter]
        /// </history>
        public AbstractManager()
        {
            if (RESET_DB_ON_STARTUP)
            {
                dropTables();
            }
            createTables();
        }

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
        /// <summary>
        /// Deletes the whole db and recreates an empty one.
        /// Only for testing use resetDB() instead!
        /// </summary>
        protected void deleteDB()
        {
            try
            {
                dB.Close();
                File.Delete(DB_PATH);
            }
            catch (Exception e)
            {
                Logger.Error("Unable to close or delete the DB", e);
            }
            dB = new SQLiteConnection(new SQLitePlatformWinRT(), DB_PATH);
        }

        /// <summary>
        /// Drops every table in the db
        /// </summary>
        protected abstract void dropTables();

        /// <summary>
        /// Creates all required tables.
        /// </summary>
        protected abstract void createTables();

        /// <summary>
        /// Inserts or replaces the given object into the db
        /// </summary>
        protected virtual void update(object obj)
        {
            try
            {
                dB.InsertOrReplace(obj);
            }
            catch (Exception e)
            {
                Logger.Error("Error in update", e);
            }          
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
