using Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;

namespace Shared.Classes.Network
{
    public class DownloadHandler : IDisposable
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public const int MAX_CONCURRENT_DOWNLOADS = 2;
        private readonly Task[] DOWNLOADER = new Task[MAX_CONCURRENT_DOWNLOADS];
        private readonly CancellationTokenSource[] DOWNLOADER_CANCELLATION_TOKENS = new CancellationTokenSource[MAX_CONCURRENT_DOWNLOADS];

        private readonly List<AbstractDownloadableObject> TO_DOWNLOAD = new List<AbstractDownloadableObject>();
        private readonly List<AbstractDownloadableObject> DOWNLOADING = new List<AbstractDownloadableObject>();
        private readonly SemaphoreSlim DOWNLOADING_SEMA = new SemaphoreSlim(1);
        private readonly SemaphoreSlim TO_DOWNLOAD_SEMA = new SemaphoreSlim(1);
        private readonly SemaphoreSlim TO_DOWNLOAD_COUNT_SEMA = new SemaphoreSlim(0);

        public delegate void DownloadStateChangedHandler(AbstractDownloadableObject o, DownloadStateChangedEventArgs args);

        public event DownloadStateChangedHandler DownloadStateChanged;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public DownloadHandler()
        {
            StartDownloaderTasks();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public async Task<AbstractDownloadableObject> FindAsync(Predicate<AbstractDownloadableObject> predicate)
        {
            AbstractDownloadableObject result = null;
            await TO_DOWNLOAD_SEMA.WaitAsync();
            result = TO_DOWNLOAD.Find(predicate);
            TO_DOWNLOAD_SEMA.Release();
            if (!(result is null))
            {
                return result;
            }

            DOWNLOADING_SEMA.Wait();
            result = DOWNLOADING.Find(predicate);
            DOWNLOADING_SEMA.Release();
            if (!(result is null))
            {
                return result;
            }
            return null;
        }

        private void SetDownloadState(AbstractDownloadableObject o, DownloadState newState)
        {
            if (o.State != newState)
            {
                DownloadStateChangedEventArgs args = new DownloadStateChangedEventArgs(o.State, newState);
                o.State = newState;
                DownloadStateChanged?.Invoke(o, args);
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public async Task EnqueueDownloadAsync(AbstractDownloadableObject o)
        {
            SetDownloadState(o, DownloadState.QUEUED);
            await TO_DOWNLOAD_SEMA.WaitAsync();
            TO_DOWNLOAD.Add(o);
            TO_DOWNLOAD_SEMA.Release();
            TO_DOWNLOAD_COUNT_SEMA.Release();
        }

        public void CancelDownload(AbstractDownloadableObject o)
        {
            SetDownloadState(o, DownloadState.CANCELED);
        }

        #endregion

        #region --Misc Methods (Private)--
        private void StartDownloaderTasks()
        {
            for (int i = 0; i < DOWNLOADER.Length; i++)
            {
                DOWNLOADER_CANCELLATION_TOKENS[i] = new CancellationTokenSource();
                DOWNLOADER[i] = StartDownlaoderTask(i);
            }
        }

        /// <summary>
        /// Starts a new downloader Task and returns it.
        /// </summary>
        /// <param name="index">The index of the downloader Task.</param>
        /// <returns>Returns a new downloader Task.</returns>
        private Task StartDownlaoderTask(int index)
        {
            return Task.Run(async () =>
            {
                while (!DOWNLOADER_CANCELLATION_TOKENS[index].IsCancellationRequested)
                {
                    try
                    {
                        // Wait until downloads are available:
                        await TO_DOWNLOAD_COUNT_SEMA.WaitAsync();
                        Logger.Debug("Downloader task " + index + " started job.");

                        // Remove one download:
                        await TO_DOWNLOAD_SEMA.WaitAsync();
                        AbstractDownloadableObject o = TO_DOWNLOAD[0];
                        TO_DOWNLOAD.RemoveAt(0);
                        TO_DOWNLOAD_SEMA.Release();

                        if (o.State != DownloadState.CANCELED)
                        {
                            SetDownloadState(o, DownloadState.DOWNLOADING);
                            o.Progress = 0;

                            // Add to currently downloading:
                            await DOWNLOADING_SEMA.WaitAsync();
                            DOWNLOADING.Add(o);
                            DOWNLOADING_SEMA.Release();

                            // Download:
                            await DownloadAsync(o);

                            // Remove since the download finished:
                            await DOWNLOADING_SEMA.WaitAsync();
                            DOWNLOADING.Remove(o);
                            DOWNLOADING_SEMA.Release();
                        }
                        Logger.Debug("Downloader task " + index + " finished job.");
                    }
                    catch (Exception e)
                    {
                        Logger.Error("Downloader task " + index + " job failed with: ", e);
                    }
                }

            }, DOWNLOADER_CANCELLATION_TOKENS[index].Token);
        }

        private void CancelDownloaderTasks()
        {
            for (int i = 0; i < DOWNLOADER.Length; i++)
            {
                if (!(DOWNLOADER[i] is null))
                {
                    DOWNLOADER_CANCELLATION_TOKENS[i].Cancel();
                    DOWNLOADER[i] = null;
                }
            }
        }

        private async Task DownloadAsync(AbstractDownloadableObject o)
        {
            Logger.Info("Started downloading <" + o.TargetFileName + "> from: " + o.SourceUrl);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(o.SourceUrl);
            HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
            long bytesReadTotal = 0;

            // Check that the remote file was found.
            // The ContentType check is performed since a request for a non-existent image file might be redirected to a 404-page, which would yield the StatusCode "OK", even though the image was not found.
            if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Moved || response.StatusCode == HttpStatusCode.Redirect)
            {
                StorageFile file = await GenFilePathAsync(o.TargetFolderPath, o.TargetFileName);
                if (file is null)
                {
                    o.Error = DownloadError.FAILED_TO_CREATE_LOCAL_PATH;
                    SetDownloadState(o, DownloadState.ERROR);
                    return;
                }

                using (Stream inputStream = response.GetResponseStream())
                using (Stream outputStream = await file.OpenStreamForWriteAsync())
                {
                    byte[] buffer = new byte[4096];
                    int bytesRead;
                    do
                    {
                        bytesRead = await inputStream.ReadAsync(buffer, 0, buffer.Length);
                        await outputStream.WriteAsync(buffer, 0, bytesRead);

                        // Update progress:
                        bytesReadTotal += bytesRead;
                        UpdateDownloadProgress(o, response.ContentLength, bytesReadTotal);

                    } while (bytesRead != 0 && o.State != DownloadState.CANCELED);
                }

                if (o.State == DownloadState.CANCELED)
                {
                    Logger.Info("Canceled downloading <" + o.TargetFileName + "> from: " + o.SourceUrl);
                    return;
                }

                SetDownloadState(o, DownloadState.DONE);
                Logger.Info("Finished downloading <" + o.TargetFileName + "> from: " + o.SourceUrl);
            }
            else
            {
                o.Error = DownloadError.INVALID_STATUS_CODE;
                SetDownloadState(o, DownloadState.ERROR);
                Logger.Error("Unable to download image <" + o.TargetFileName + "> from: " + o.SourceUrl + "- Status code check failed: " + response.StatusCode + "(" + response.StatusDescription + ')');
            }
        }

        private async Task<StorageFile> GenFilePathAsync(string folderPath, string fileName)
        {
            //CreateDirectoryRecursively(folderPath);
            StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(folderPath);
            if (!(folder is null))
            {
                return await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            }
            Logger.Error("Failed to get folder from path: " + folderPath);
            return null;
        }

        /// <summary>
        /// Creates all directories for the given path recursively.
        /// Based on: https://stackoverflow.com/questions/10941657/creating-files-recursively-creating-directories
        /// </summary>
        /// <param name="path">The absolute directory path.</param>
        private void CreateDirectoryRecursively(string path)
        {
            string[] pathParts = path.Split('\\');

            for (int i = 0; i < pathParts.Length; i++)
            {
                if (i > 0)
                {
                    pathParts[i] = Path.Combine(pathParts[i - 1], pathParts[i]);
                }

                if (!Directory.Exists(pathParts[i]))
                {
                    Directory.CreateDirectory(pathParts[i]);
                    Logger.Debug("Created directory: " + pathParts[i]);
                }
            }
        }

        /// <summary>
        /// Calculates the new download progress and sets it for the given AbstractDownloadableObject.
        /// </summary>
        /// <param name="o">The AbstractDownloadableObject.</param>
        /// <param name="totalSize">The total size of the download in bytes.</param>
        /// <param name="bytesDownloadedTotal">How many bytes have been downloaded?</param>
        /// <returns>Returns the last progress update percentage.</returns>
        private double UpdateDownloadProgress(AbstractDownloadableObject o, long totalSize, long bytesDownloadedTotal)
        {
            o.Progress = bytesDownloadedTotal / ((double)totalSize);
            return o.Progress;
        }

        public void Dispose()
        {
            // Cancel all downloader tasks:
            CancelDownloaderTasks();

            // Clear download queue:
            TO_DOWNLOAD_SEMA.Wait();
            foreach (AbstractDownloadableObject o in TO_DOWNLOAD)
            {
                SetDownloadState(o, DownloadState.NOT_QUEUED);
            }
            TO_DOWNLOAD.Clear();
            TO_DOWNLOAD_SEMA.Release();

            // Stop all downloads:
            DOWNLOADING_SEMA.Wait();
            foreach (AbstractDownloadableObject o in DOWNLOADING)
            {
                if (o.State != DownloadState.DONE && o.State != DownloadState.ERROR)
                {
                    SetDownloadState(o, DownloadState.NOT_QUEUED);
                }
            }
            DOWNLOADING.Clear();
            DOWNLOADING_SEMA.Release();
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
