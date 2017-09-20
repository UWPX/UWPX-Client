using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Threading.Tasks;
using Windows.Storage;

namespace Logging
{
    public class Logger
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private static readonly Object thisLock = new Object();

        #endregion
        //--------------------------------------------------------Construktor:----------------------------------------------------------------\\
        #region --Constructors--


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
            var savePicker = new Windows.Storage.Pickers.FileSavePicker()
            {
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary
            };
            savePicker.FileTypeChoices.Add("Logs", new List<string>() { ".zip" });
            savePicker.SuggestedFileName = "Logs";
            return await savePicker.PickSaveFileAsync(); ;
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
            addToLog(message, null, "DEBUG");
        }

        /// <summary>
        /// Adds a Info message to the log
        /// </summary>
        /// <history>
        /// 01/01/2017 Created [Fabian Sauter]
        /// </history>
        public static void Info(string message)
        {
            addToLog(message, null, "INFO");
        }

        /// <summary>
        /// Adds a Warn message to the log
        /// </summary>
        /// <history>
        /// 01/01/2017 Created [Fabian Sauter]
        /// </history>
        public static void Warn(string message)
        {
            addToLog(message, null, "WARN");
        }

        /// <summary>
        /// Adds a Error message to the log
        /// </summary>
        /// <history>
        /// 01/01/2017 Created [Fabian Sauter]
        /// </history>
        public static void Error(string message, Exception e)
        {
            addToLog(message, e, "ERROR");
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
            if(folder != null)
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
                await Task.Factory.StartNew(async () =>
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
                        Logger.Info("Exported logs successfully to:" + target.Path);
                    }
                    catch (Exception e)
                    {
                        Logger.Error("Error during exporting loggs", e);
                    }
                });
            }
        }

        #endregion

        #region --Misc Methods (Private)--
        /// <summary>
        /// Creates a task that adds the given message and exception to the log.
        /// </summary>
        /// <param name="message">The log message.</param>
        /// <param name="e">The thrown exception.</param>
        /// <param name="code">The log code (INFO, DEBUG, ...)</param>
        private static void addToLog(string message, Exception e, string code)
        {
            Task t = addToLogAsync(message, e, code);
        }

        /// <summary>
        /// Adds the given message and exception to the log.
        /// </summary>
        /// <param name="message">The log message.</param>
        /// <param name="e">The thrown exception.</param>
        /// <param name="code">The log code (INFO, DEBUG, ...)</param>
        private static async Task addToLogAsync(string message, Exception e, string code)
        {
            await ApplicationData.Current.LocalFolder.CreateFolderAsync("Logs", CreationCollisionOption.OpenIfExists);
            StorageFile logFile = await (await ApplicationData.Current.LocalFolder.GetFolderAsync("Logs")).CreateFileAsync(getFilename(), CreationCollisionOption.OpenIfExists);
            string s = "[" + code + "][" + getTimeStamp() + "]: " + message;
            if (e != null)
            {
                s += ":\n" + e.Message + "\n" + e.StackTrace;
            }
            lock (thisLock)
            {
                System.Diagnostics.Debug.WriteLine(s);
                Task.WaitAny(FileIO.AppendTextAsync(logFile, s + Environment.NewLine).AsTask());
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
