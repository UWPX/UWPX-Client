using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Logging;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;

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
        protected AbstractDBManager()
        {
            if (RESET_DB_ON_STARTUP)
            {
#pragma warning disable CS0162 // Unreachable code detected
                DropTables();
#pragma warning restore CS0162 // Unreachable code detected
            }
            CreateTables();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public virtual void initManager()
        {
        }

        /// <summary>
        /// Opens a file picker and exports the DB to the selected path.
        /// </summary>
        public static async Task ExportDBAsync()
        {
            // Get the target path/file.
            StorageFile targetFile = await GetTargetSavePathAsync();
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

        /// <summary>
        /// Opens a file picker and imports the DB from the selected path.
        /// </summary>
        public static async Task ImportDBAsync()
        {
            // Get the source file:
            StorageFile sourceFile = await GetTargetOpenPathAsync();
            if (sourceFile is null)
            {
                Logger.Info("Importing DB canceled.");
                return;
            }
            Logger.Info("Started importing DB to: " + sourceFile.Path);

            // Close the DB connection:
            dB.Close();

            // Delete all existing DB files:
            await DeleteDBFilesAsync();

            // Import DB:
            StorageFile dbFile = await StorageFile.GetFileFromPathAsync(DB_PATH);
            try
            {
                await sourceFile.CopyAndReplaceAsync(dbFile);
                Logger.Info("Imported DB successfully from:" + sourceFile.Path);

            }
            catch (Exception e)
            {
                Logger.Error("Error during importing DB", e);
            }

            // Open the new DB:
            dB = new TSSQLiteConnection(DB_PATH);
        }

        #endregion

        #region --Misc Methods (Private)--
        /// <summary>
        /// Opens a FileSavePicker and lets the user pick the destination.
        /// </summary>
        /// <returns>Returns the selected path.</returns>
        private static IAsyncOperation<StorageFile> GetTargetSavePathAsync()
        {
            FileSavePicker savePicker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };
            savePicker.FileTypeChoices.Add("SQLite DB", new List<string>() { ".db" });
            savePicker.SuggestedFileName = "data";
            return savePicker.PickSaveFileAsync();
        }

        /// <summary>
        /// Opens a FileSavePicker and lets the user pick the destination.
        /// </summary>
        /// <returns>Returns the selected path.</returns>
        private static IAsyncOperation<StorageFile> GetTargetOpenPathAsync()
        {
            FileOpenPicker openPicker = new FileOpenPicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };
            openPicker.FileTypeFilter.Add(".db");
            return openPicker.PickSingleFileAsync();
        }

        /// <summary>
        /// Deletes all DB files.
        /// </summary>
        private static async Task DeleteDBFilesAsync()
        {
            try
            {
                StorageFile targetFile = await StorageFile.GetFileFromPathAsync(DB_PATH);
                if (targetFile is null)
                {
                    Logger.Warn("Unable to delete DB files - DB file not found.");
                    return;
                }

                StorageFolder folder = await targetFile.GetParentAsync();
                if (folder is null)
                {
                    Logger.Warn("Unable to delete DB files - folder not found.");
                    return;
                }

                foreach (StorageFile file in await folder.GetFilesAsync())
                {
                    if (file.Name.StartsWith(targetFile.Name))
                    {
                        await file.DeleteAsync();
                        Logger.Info("Deleted DB file: " + file.Name);
                    }
                }
                Logger.Info("Finished deleting DB files.");
            }
            catch (Exception e)
            {
                Logger.Error("Failed to delete DB files!", e);
            }
        }

        #endregion

        #region --Misc Methods (Protected)--
        /// <summary>
        /// Deletes the whole db and recreates an empty one.
        /// Only for testing use resetDB() instead!
        /// </summary>
        protected void DeleteDB()
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
        protected abstract void DropTables();

        /// <summary>
        /// Creates all required tables.
        /// </summary>
        protected abstract void CreateTables();

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
