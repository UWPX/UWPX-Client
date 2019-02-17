using Data_Manager2.Classes.DBTables;
using Logging;
using Shared.Classes.SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;

namespace Data_Manager2.Classes.DBManager
{
    public class ImageDBManager : AbstractDBManager
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public static readonly ImageDBManager INSTANCE = new ImageDBManager();

        // The interval for how often the ImageTable onDownloadProgressChanged() should get triggered (e.g 0.1 = every 10%):
        private const double DOWNLOAD_PROGRESS_REPORT_INTERVAL = 0.05;

        // A list of all currently downloading images:
        private readonly List<ImageTable> DOWNLOADING;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 15/12/2017 Created [Fabian Sauter]
        /// </history>
        public ImageDBManager()
        {
            DOWNLOADING = new List<ImageTable>();
            contiuneAllDownloads();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        /// <summary>
        /// Calculates the size of the "cachedImages" folder.
        /// </summary>
        /// <returns>Returns the "cachedImages" folder size in KB.</returns>
        public async Task<long> getCachedImagesFolderSizeAsync()
        {
            StorageFolder f = await getCachedImagesFolderAsync();
            StorageFileQueryResult result = f.CreateFileQuery(CommonFileQuery.OrderByName);

            var fileSizeTasks = (await result.GetFilesAsync()).Select(async file => (await file.GetBasicPropertiesAsync()).Size);
            var sizes = await Task.WhenAll(fileSizeTasks);

            return sizes.Sum(l => (long)l) / 1024;
        }

        /// <summary>
        /// Returns the image container from a given ChatMessageTable.
        /// If none exist will create a new one.
        /// If the download is still outstanding/paused it will continue/start the download.
        /// </summary>
        /// <param name="msg">The ChatMessageTable.</param>
        /// <returns>The corresponding image container.</returns>
        public async Task<ImageTable> getImageForMessageAsync(ChatMessageTable msg)
        {
            ImageTable img = DOWNLOADING.Find(x => string.Equals(x.messageId, msg.id));
            if (img is null)
            {
                List<ImageTable> list = dB.Query<ImageTable>(true, "SELECT * FROM " + DBTableConsts.IMAGE_TABLE + " WHERE messageId = ?;", msg.id);
                if (list.Count > 0)
                {
                    img = list[0];
                }

                if (img is null)
                {
                    return await downloadImageAsync(msg);
                }

                // Start the image download if not already downloaded or error:
                if (img.state == DownloadState.WAITING || img.state == DownloadState.DOWNLOADING)
                {
                    await downloadImageAsync(img, msg.message);
                }
            }
            return img;
        }

        /// <summary>
        /// Checks if the image is already available locally, if yes returns the path to it.
        /// Else tries to download the image and stores it. Also returns the path of the downloaded image.
        /// </summary>
        /// <param name="img">The image container.</param>
        /// <param name="url">The image url.</param>
        /// <returns>Returns the local path to the downloaded image.</returns>
        public async Task<string> getImagePathAsync(ImageTable img, string url)
        {
            try
            {
                string fileName = createUniqueFileName(url);
                string path = await getLocalImageAsync(fileName);
                if (path is null)
                {
                    path = await downloadImageAsync(img, url, fileName);
                }
                return path;
            }
            catch (Exception e)
            {
                Logger.Error("Error during downloading image: " + e.Message);
                img.errorMessage = e.Message;
                update(img);
                return null;
            }
        }

        /// <summary>
        /// Returns the path for the given file name if the file exits.
        /// Else returns null.
        /// </summary>
        /// <param name="name">The file name.</param>
        /// <returns>The path to the file if it exists else null.</returns>
        private async Task<string> getLocalImageAsync(string name)
        {
            StorageFolder f = await getCachedImagesFolderAsync();
            if (f != null)
            {
                FileInfo fI = new FileInfo(f.Path + '\\' + name);
                if (fI.Exists)
                {
                    return fI.FullName;
                }
            }
            return null;
        }

        public async Task<StorageFolder> getCachedImagesFolderAsync()
        {
            if (Settings.getSettingBoolean(SettingsConsts.DISABLE_DOWNLOAD_IMAGES_TO_LIBARY))
            {
                return await ApplicationData.Current.LocalFolder.CreateFolderAsync("cachedImages", CreationCollisionOption.OpenIfExists);
            }
            return await KnownFolders.PicturesLibrary.CreateFolderAsync("UWPX", CreationCollisionOption.OpenIfExists);
        }

        /// <summary>
        /// Continues all outstanding downloads.
        /// </summary>
        private void contiuneAllDownloads()
        {
            List<ImageTable> list = dB.Query<ImageTable>(true, "SELECT * FROM " + DBTableConsts.IMAGE_TABLE + " WHERE state = ? OR state = ?;", (int)DownloadState.WAITING, (int)DownloadState.DOWNLOADING);
            foreach (ImageTable img in list)
            {
                Task.Run(async () =>
                {
                    // Reset image progress:
                    img.progress = 0;

                    ChatMessageTable msg = ChatDBManager.INSTANCE.getChatMessageById(img.messageId);
                    await downloadImageAsync(img, msg.message);
                });
            }
        }

        /// <summary>
        /// Downloads the image from the given message and stores it locally.
        /// </summary>
        /// <param name="img">The image container.</param>
        /// <param name="msg">The image url.</param>
        /// <returns></returns>
        private async Task downloadImageAsync(ImageTable img, string msg)
        {
            DOWNLOADING.Add(img);

            updateImageState(img, DownloadState.DOWNLOADING);
            string path = await getImagePathAsync(img, msg);

            img.path = path;
            if (path is null)
            {
                updateImageState(img, DownloadState.ERROR);
            }
            else
            {
                updateImageState(img, DownloadState.DONE);
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        /// <summary>
        /// Retries to download the image from the given ChatMessageTable.
        /// </summary>
        /// <param name="msg">The ChatMessageTable containing the image url.</param>
        /// <returns>The image container.</returns>
        public async Task<ImageTable> retryImageDownloadAsync(ChatMessageTable msg)
        {
            ImageTable img = await getImageForMessageAsync(msg);

            img.state = DownloadState.WAITING;
            img.progress = 0;
            img.path = null;

            await Task.Run(async () =>
            {
                update(img);
                await downloadImageAsync(img, msg.message);
            });

            return img;
        }

        /// <summary>
        /// Deletes the "cachedImages" folder and creates a new empty one.
        /// </summary>
        public async Task deleteImageCacheAsync()
        {
            StorageFolder folder = await getCachedImagesFolderAsync();
            if (folder != null)
            {
                await folder.DeleteAsync();
            }
            await getCachedImagesFolderAsync();
            Logger.Info("Deleted image cache!");
        }

        /// <summary>
        /// Opens the current folder containing the cached images.
        /// </summary>
        public async Task openCachedImagesFolderAsync()
        {
            StorageFolder folder = await getCachedImagesFolderAsync();
            await Windows.System.Launcher.LaunchFolderAsync(folder);
        }

        /// <summary>
        /// Creates a new task, downloads the image from the given message and stores it locally.
        /// </summary>
        /// <param name="msg">The ChatMessageTable containing the image url.</param>
        /// <returns>Returns the created Task.</returns>
        public Task downloadImage(ChatMessageTable msg)
        {
            return downloadImageAsync(msg);
        }

        #endregion

        #region --Misc Methods (Private)--
        private async Task<ImageTable> downloadImageAsync(ChatMessageTable msg)
        {
            ImageTable img = new ImageTable()
            {
                messageId = msg.id,
                path = null,
                state = DownloadState.WAITING,
                progress = 0
            };
            update(img);
            await downloadImageAsync(img, msg.message);
            return img;
        }

        /// <summary>
        /// Tries to download the image from the given url.
        /// </summary>
        /// <param name="url">The image url.</param>
        /// <param name="name"><The image name for storing the image./param>
        /// <returns>Returns null if it fails, else the local path.</returns>
        private async Task<string> downloadImageAsync(ImageTable img, string url, string name)
        {
            Logger.Info("Started downloading image <" + name + "> from: " + url);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
            long bytesReadTotal = 0;
            double lastProgressUpdatePercent = 0;

            // Check that the remote file was found. The ContentType
            // check is performed since a request for a non-existent
            // image file might be redirected to a 404-page, which would
            // yield the StatusCode "OK", even though the image was not
            // found.
            if (response.StatusCode == HttpStatusCode.OK ||
                response.StatusCode == HttpStatusCode.Moved ||
                response.StatusCode == HttpStatusCode.Redirect)
            {
                // if the remote file was found, download it
                StorageFile f = await createImageStorageFileAsync(name);
                using (Stream inputStream = response.GetResponseStream())
                using (Stream outputStream = await f.OpenStreamForWriteAsync())
                {
                    byte[] buffer = new byte[4096];
                    int bytesRead;
                    do
                    {
                        bytesRead = await inputStream.ReadAsync(buffer, 0, buffer.Length);
                        await outputStream.WriteAsync(buffer, 0, bytesRead);

                        // Update progress:
                        bytesReadTotal += bytesRead;
                        lastProgressUpdatePercent = updateProgress(img, response.ContentLength, bytesReadTotal, lastProgressUpdatePercent);

                    } while (bytesRead != 0);
                }
                Logger.Info("Finished downloading image <" + name + "> from: " + url);
                return f.Path;
            }
            else
            {
                img.errorMessage = "Status code check failed: " + response.StatusCode + " (" + response.StatusDescription + ')';
                update(img);
            }
            Logger.Error("Unable to download image <" + name + "> from: " + url + " Status code: " + response.StatusCode);
            return null;
        }

        /// <summary>
        /// Updates the progress of the given ImageTable and triggers if necessary the onDownloadProgressChanged event.
        /// </summary>
        /// <param name="img">The image container.</param>
        /// <param name="totalLengt">The total length of the download in bytes.</param>
        /// <param name="bytesReadTotal">How many bytes got read already?</param>
        /// <param name="lastProgressUpdatePercent">When was the last onDownloadProgressChanged event triggered?</param>
        /// <returns>Returns the last progress update percentage.</returns>
        private double updateProgress(ImageTable img, long totalLengt, long bytesReadTotal, double lastProgressUpdatePercent)
        {
            img.progress = bytesReadTotal / ((double)totalLengt);
            if ((img.progress - lastProgressUpdatePercent) >= DOWNLOAD_PROGRESS_REPORT_INTERVAL)
            {
                img.onDownloadProgressChanged();
                return img.progress;
            }

            return lastProgressUpdatePercent;
        }

        private async Task<StorageFile> createImageStorageFileAsync(string name)
        {
            StorageFolder f = await getCachedImagesFolderAsync();
            return await f.CreateFileAsync(name, CreationCollisionOption.ReplaceExisting);
        }

        /// <summary>
        /// Creates an unique file name, bases on the given url and the current time.
        /// </summary>
        /// <param name="url">The url of the image.</param>
        /// <returns>Returns an unique file name.</returns>
        private string createUniqueFileName(string url)
        {
            string name = DateTime.Now.ToString("dd.MM.yyyy_HH.mm.ss.ffff");
            int index = url.LastIndexOf('.');
            string ending = url.Substring(index, url.Length - index);
            return name + ending;
        }

        /// <summary>
        /// Updates the DownloadState of the given ImageTable and triggers the onStateChanged() event.
        /// </summary>
        /// <param name="img">The ImageTable, that should get updated.</param>
        /// <param name="state">The new DownloadState.</param>
        private void updateImageState(ImageTable img, DownloadState state)
        {
            img.state = state;
            img.onStateChanged();
            update(img);
            switch (state)
            {
                case DownloadState.DONE:
                case DownloadState.ERROR:
                    DOWNLOADING.Remove(img);
                    break;

                default:
                    break;
            }
        }

        #endregion

        #region --Misc Methods (Protected)--
        protected override void createTables()
        {
            dB.CreateTable<ImageTable>();
        }

        protected override void dropTables()
        {
            dB.DropTable<ImageTable>();
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
