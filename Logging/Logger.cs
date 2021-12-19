using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using NLog.Config;
using NLog.Targets;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Search;

namespace Logging
{
    public static class Logger
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public static LogLevel logLevel;
        private static readonly NLog.Logger NLOGGER;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        static Logger()
        {
            Target.Register<ConsoleTarget>(nameof(ConsoleTarget));
            Target.Register<WindowsLogChannel>(nameof(WindowsLogChannel));
            LogManager.Configuration = new XmlLoggingConfiguration(Path.Combine(Package.Current.InstalledLocation.Path, @"Logging\NLog.config"));
            LogManager.Configuration.Variables["LogPath"] = getLogsFolderPath();
            LogManager.Configuration.Variables["LogArchivePath"] = GetLogsArchivePath();
            NLOGGER = LogManager.GetCurrentClassLogger();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        /// <summary>
        /// Opens a FileSavePicker and lets the user pick the destination.
        /// </summary>
        /// <returns>Returns the selected path.</returns>
        private static Task<StorageFile> GetTargetPathAsync()
        {
            FileSavePicker savePicker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };
            savePicker.FileTypeChoices.Add("Logs", new List<string>() { ".zip" });
            savePicker.SuggestedFileName = "Logs";
            return savePicker.PickSaveFileAsync();
        }

        /// <summary>
        /// Calculates the size of the "Logs" folder.
        /// </summary>
        /// <returns>Returns the "Logs" folder size in KB.</returns>
        public static async Task<long> GetLogFolderSizeAsync()
        {
            StorageFolder logsFolder = await GetLogFolderAsync();
            StorageFileQueryResult result = logsFolder.CreateFileQuery();

            IEnumerable<Task<ulong>> fileSizeTasks = (await result.GetFilesAsync()).Select(async file => (await file.GetBasicPropertiesAsync()).Size);
            ulong[] sizes = await Task.WhenAll(fileSizeTasks);

            return sizes.Sum(l => (long)l) / 1024;
        }

        /// <summary>
        /// Returns the "Logs" folder and creates it, if it does not exist.
        /// </summary>
        /// <returns>Returns the "Logs" folder.</returns>
        public static Task<StorageFolder> GetLogFolderAsync()
        {
            return ApplicationData.Current.LocalFolder.CreateFolderAsync("Logs", CreationCollisionOption.OpenIfExists);
        }

        private static string getLogsFolderPath()
        {
            return Path.Combine(ApplicationData.Current.LocalFolder.Path, "Logs");
        }

        private static string GetExportedLogsPath()
        {
            return Path.Combine(ApplicationData.Current.LocalFolder.Path, "LogsExport.zip");
        }

        private static string GetLogsArchivePath()
        {
            return Path.Combine(getLogsFolderPath(), "Archive");
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        /// <summary>
        /// Adds a Trace message to the log
        /// </summary>
        public static void Trace(string message)
        {
            if (logLevel >= LogLevel.TRACE)
            {
                NLOGGER.Trace(message);
            }
        }

        /// <summary>
        /// Adds a Debug message to the log
        /// </summary>
        public static void Debug(string message)
        {
            if (logLevel >= LogLevel.DEBUG)
            {
                NLOGGER.Debug(message);
            }
        }

        /// <summary>
        /// Adds a Info message to the log
        /// </summary>
        public static void Info(string message)
        {
            if (logLevel >= LogLevel.INFO)
            {
                NLOGGER.Info(message);
            }
        }

        /// <summary>
        /// Adds a Warn message to the log
        /// </summary>
        public static void Warn(string message)
        {
            if (logLevel >= LogLevel.WARNING)
            {
                NLOGGER.Warn(message);
            }
        }

        /// <summary>
        /// Adds a Error message to the log
        /// </summary>
        public static void Error(string message, Exception e)
        {
            if (logLevel >= LogLevel.ERROR)
            {
                if (e != null)
                {
                    NLOGGER.Error(e, message);
                }
                else
                {
                    NLOGGER.Error(message);
                }
            }
        }

        /// <summary>
        /// Adds a Error message to the log
        /// </summary>
        public static void Error(string message)
        {
            Error(message, null);
        }

        /// <summary>
        /// Opens the log folder.
        /// </summary>
        /// <returns>An async Task.</returns>
        public static Task OpenLogFolderAsync()
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            return Windows.System.Launcher.LaunchFolderAsync(folder);
        }

        /// <summary>
        /// Deletes all files inside the "Logs" folder, if they aren't opened by anything at the moment.
        /// </summary>
        public static async Task DeleteLogsAsync()
        {
            StorageFolder logsFolder = await GetLogFolderAsync();
            if (logsFolder != null)
            {
                foreach (IStorageItem item in await logsFolder.GetItemsAsync())
                {
                    try
                    {
                        await item.DeleteAsync(StorageDeleteOption.PermanentDelete);
                    }
                    catch
                    {
                        //ignore exception - could happen if some file is currently open
                    }
                }
            }

            Info("Deleted logs!");
        }

        /// <summary>
        /// Exports all logs as a .zip file to the selected path.
        /// </summary>
        public static async Task ExportLogsAsync()
        {
            // Get the target path/file:
            StorageFile targetFile = await GetTargetPathAsync().ConfigureAwait(false);
            if (targetFile is null)
            {
                Info("Exporting logs canceled.");
                return;
            }
            Info("Started exporting logs to: " + targetFile.Path);

            // Delete existing log export zip files:
            IStorageItem zipItem = await ApplicationData.Current.LocalFolder.TryGetItemAsync("LogsExport.zip");
            if (zipItem != null)
            {
                await zipItem.DeleteAsync(StorageDeleteOption.PermanentDelete);
            }

            StorageFolder logsFolder = await GetLogFolderAsync().ConfigureAwait(false);
            ZipFile.CreateFromDirectory(logsFolder.Path, GetExportedLogsPath(), CompressionLevel.Fastest, true);

            try
            {
                zipItem = await ApplicationData.Current.LocalFolder.TryGetItemAsync("LogsExport.zip");
                if (zipItem is null)
                {
                    Error("Failed to export logs - zipItem is null");
                }
                else if (zipItem is StorageFile zipFile)
                {
                    await zipFile.MoveAndReplaceAsync(targetFile);
                    Info("Exported logs successfully to:" + targetFile.Path);
                }
                else
                {
                    Error("Failed to export logs - zipItem is no StorageFile");
                }

            }
            catch (Exception e)
            {
                Error("Error during exporting logs", e);
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
