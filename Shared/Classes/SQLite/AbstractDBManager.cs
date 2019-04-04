using Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace Shared.Classes.SQLite
{
    public abstract class AbstractDBManager
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private static readonly string DB_PATH = Path.Combine(ApplicationData.Current.LocalFolder.Path, "data2.db");
        public static TSSQLiteConnection dB = new TSSQLiteConnection(DB_PATH);

        public const bool RESET_DB_ON_STARTUP = false;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 17/11/2017 Created [Fabian Sauter]
        /// </history>
        protected AbstractDBManager()
        {
            if (RESET_DB_ON_STARTUP)
            {
#pragma warning disable CS0162 // Unreachable code detected
                dropTables();
#pragma warning restore CS0162 // Unreachable code detected
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
        /// Inits the manager.
        /// </summary>
        public virtual void initManager()
        {
        }

        /// <summary>
        /// Opens a file picker and exports the DB to the selected path.
        /// </summary>
        public static async Task exportDBAsync()
        {
            // Get the target path/file.
            StorageFile targetFile = await getTargetPathAsync().ConfigureAwait(false);
            if (targetFile is null)
            {
                Logger.Info("Exporting DB canceled.");
                return;
            }
            Logger.Info("Started exporting DB to: " + targetFile.Path);

            StorageFile dbFile = await StorageFile.GetFileFromPathAsync(DB_PATH);
            if (dbFile is null)
            {
                Logger.Error("Failed to export DB - file not found.");
                return;
            }

            try
            {
                await dbFile.CopyAndReplaceAsync(targetFile);
                Logger.Info("Exported DB successfully to:" + targetFile.Path);

            }
            catch (Exception e)
            {
                Logger.Error("Error during exporting DB", e);
            }
        }

        #endregion

        #region --Misc Methods (Private)--
        /// <summary>
        /// Opens a FileSavePicker and lets the user pick the destination.
        /// </summary>
        /// <returns>Returns the selected path.</returns>
        private static async Task<StorageFile> getTargetPathAsync()
        {
            var savePicker = new Windows.Storage.Pickers.FileSavePicker
            {
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary
            };
            savePicker.FileTypeChoices.Add("SQLite DB", new List<string>() { ".db" });
            savePicker.SuggestedFileName = "data";
            return await savePicker.PickSaveFileAsync();
        }

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
            dB = new TSSQLiteConnection(DB_PATH);
        }

        /// <summary>
        /// Drops every table in the db.
        /// </summary>
        protected abstract void dropTables();

        /// <summary>
        /// Creates all required tables.
        /// </summary>
        protected abstract void createTables();

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
