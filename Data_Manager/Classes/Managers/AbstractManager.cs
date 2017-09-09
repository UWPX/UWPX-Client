using System;
using System.IO;
using Windows.Storage;
using SQLite.Net;
using SQLite.Net.Platform.WinRT;
using Logging;

namespace Data_Manager.Classes.Managers
{
    public abstract class AbstractManager
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private static readonly string DB_PATH = Path.Combine(ApplicationData.Current.LocalFolder.Path, "data.db");
        protected static SQLiteConnection dB = new SQLiteConnection(new SQLitePlatformWinRT(), DB_PATH);

        public static readonly bool RESET_DB_ON_STARTUP = false;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Construktoren--
        /// <summary>
        /// Initalises the object and creates all required tables for this object. 
        /// </summary>
        /// <history>
        /// 26/08/2017 Created [Fabian Sauter]
        /// </history>
        public AbstractManager()
        {
            if(RESET_DB_ON_STARTUP)
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
        /// <summary>
        /// Delets the whole db and recreates an empty one.
        /// Only for testing use resetDB() instead!
        /// </summary>
        protected void deleteDB()
        {
            try
            {
                dB.Close();
                File.Delete(AbstractManager.DB_PATH);
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
        protected void update(object obj)
        {
            dB.InsertOrReplace(obj);
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
