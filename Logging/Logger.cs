using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
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
            LogManager.ThrowExceptions = true;
            Target.Register<ConsoleTarget>(nameof(ConsoleTarget));
            LogManager.Configuration = new XmlLoggingConfiguration(Path.Combine(Package.Current.InstalledLocation.Path, @"Logging\NLog.config"));
            LogManager.Configuration.Variables["LogPath"] = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Logs");
            NLOGGER = LogManager.GetCurrentClassLogger();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        /// <summary>
        /// Returns the current log file name.
        /// </summary>
        /// <returns>Returns the current log file name.</returns>
        private static string getFilename()
        {
            return "Log-" + DateTime.Now.ToString("dd.MM.yyyy") + ".log";
        }

        /// <summary>
        /// Returns the current time stamp as a string.
        /// </summary>
        /// <returns>Returns the current time stamp as a string.</returns>
        private static string getTimeStamp()
        {
            return DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
        }

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
            savePicker.FileTypeChoices.Add("Logs", new List<string>() { ".zip" });
            savePicker.SuggestedFileName = "Logs";
            return await savePicker.PickSaveFileAsync();
        }

        /// <summary>
        /// Calculates the size of the "Logs" folder.
        /// </summary>
        /// <returns>Returns the "Logs" folder size in KB.</returns>
        public static async Task<long> getLogFolderSizeAsync()
        {
            StorageFolder f = await getLogFolderAsync();
            StorageFileQueryResult result = f.CreateFileQuery(CommonFileQuery.OrderByName);

            var fileSizeTasks = (await result.GetFilesAsync()).Select(async file => (await file.GetBasicPropertiesAsync()).Size);
            var sizes = await Task.WhenAll(fileSizeTasks);

            return sizes.Sum(l => (long)l) / 1024;
        }

        /// <summary>
        /// Returns the "Logs" folder and creates it, if it does not exist.
        /// </summary>
        /// <returns>Returns the "Logs" folder.</returns>
        private static async Task<StorageFolder> getLogFolderAsync()
        {
            await ApplicationData.Current.LocalFolder.CreateFolderAsync("Logs", CreationCollisionOption.OpenIfExists);
            return await ApplicationData.Current.LocalFolder.GetFolderAsync("Logs");
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        /// <summary>
        /// Adds a Debug message to the log
        /// </summary>
        /// <history>
        /// 01/01/2017 Created [Fabian Sauter]
        /// </history>
        public static void Debug(string message)
        {
            if(logLevel >= LogLevel.DEBUG)
            {
                NLOGGER.Debug(message);
            }
        }

        /// <summary>
        /// Adds a Info message to the log
        /// </summary>
        /// <history>
        /// 01/01/2017 Created [Fabian Sauter]
        /// </history>
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
        /// <history>
        /// 01/01/2017 Created [Fabian Sauter]
        /// </history>
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
        /// <history>
        /// 01/01/2017 Created [Fabian Sauter]
        /// </history>
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
        /// <history>
        /// 01/01/2017 Created [Fabian Sauter]
        /// </history>
        public static void Error(string message)
        {
            Error(message, null);
        }

        /// <summary>
        /// Opens the log folder.
        /// </summary>
        /// <returns>An async Task.</returns>
        public static async Task openLogFolderAsync()
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            await Windows.System.Launcher.LaunchFolderAsync(folder);
        }

        /// <summary>
        /// Deletes the "Logs" folder and creates a new empty one.
        /// </summary>
        /// <returns>An async Task.</returns>
        public static async Task deleteLogsAsync()
        {
            StorageFolder folder = (StorageFolder)await ApplicationData.Current.LocalFolder.TryGetItemAsync("Logs");
            if (folder != null)
            {
                await folder.DeleteAsync();
            }
            await ApplicationData.Current.LocalFolder.CreateFolderAsync("Logs", CreationCollisionOption.OpenIfExists);

            Info("Deleted logs!");
        }

        /// <summary>
        /// Exports all logs as a .zip file to the selected path.
        /// </summary>
        /// <returns>An async Task.</returns>
        public static async Task exportLogs()
        {
            StorageFolder folder = await ApplicationData.Current.LocalFolder.GetFolderAsync("Logs");
            if (folder != null)
            {
                StorageFile target = await getTargetPathAsync();
                if (target == null)
                {
                    return;
                }
                await Task.Run(async () =>
                {
                    try
                    {
                        IStorageItem file = await ApplicationData.Current.LocalFolder.TryGetItemAsync("Logs.zip");
                        if (file != null)
                        {
                            await file.DeleteAsync();
                        }
                        ZipFile.CreateFromDirectory(folder.Path, ApplicationData.Current.LocalFolder.Path + @"\Logs.zip", CompressionLevel.Optimal, false);
                        file = await ApplicationData.Current.LocalFolder.GetFileAsync("Logs.zip");
                        if (file != null && file is StorageFile)
                        {
                            StorageFile f = file as StorageFile;
                            await f.CopyAndReplaceAsync(target);
                        }
                        Info("Exported logs successfully to:" + target.Path);
                    }
                    catch (Exception e)
                    {
                        Error("Error during exporting logs", e);
                    }
                });
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
