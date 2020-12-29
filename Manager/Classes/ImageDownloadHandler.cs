using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Logging;
using Shared.Classes.Network;
using Windows.Storage;

namespace Manager.Classes
{
    public class ImageDownloadHandler
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly DownloadHandler DOWNLOAD_HANDLER;
        private readonly SemaphoreSlim DOWNLOAD_SEMA = new SemaphoreSlim(1);

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ImageDownloadHandler(DownloadHandler downloadHandler)
        {
            DOWNLOAD_HANDLER = downloadHandler;
            DOWNLOAD_HANDLER.DownloadStateChanged += DOWNLOAD_HANDLER_DownloadStateChanged;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public async Task<StorageFolder> GetCacheFolderAsync()
        {
            if (Settings.getSettingBoolean(SettingsConsts.DISABLE_DOWNLOAD_IMAGES_TO_LIBARY))
            {
                return await ApplicationData.Current.LocalFolder.CreateFolderAsync("cachedImages", CreationCollisionOption.OpenIfExists);
            }
            return await KnownFolders.PicturesLibrary.CreateFolderAsync("UWPX", CreationCollisionOption.OpenIfExists);
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public async Task StartDownloadAsync(ImageTable image)
        {
            await DOWNLOAD_SEMA.WaitAsync();
            if (image.State != DownloadState.DOWNLOADING && image.State != DownloadState.QUEUED)
            {
                await DOWNLOAD_HANDLER.EnqueueDownloadAsync(image);
            }
            DOWNLOAD_SEMA.Release();
        }

        public async Task RedownloadAsync(ImageTable image)
        {
            await DOWNLOAD_HANDLER.EnqueueDownloadAsync(image);
        }

        public void CancelDownload(ImageTable image)
        {
            DOWNLOAD_HANDLER.CancelDownload(image);
        }

        public async Task<AbstractDownloadableObject> FindAsync(Predicate<AbstractDownloadableObject> predicate)
        {
            await DOWNLOAD_SEMA.WaitAsync();
            AbstractDownloadableObject result = await DOWNLOAD_HANDLER.FindAsync(predicate);
            DOWNLOAD_SEMA.Release();
            return result;
        }

        public async Task ContinueDownloadsAsync()
        {
            if (!Settings.getSettingBoolean(SettingsConsts.DISABLE_IMAGE_AUTO_DOWNLOAD))
            {
                List<ImageTable> images = ImageDBManager.INSTANCE.getAllUndownloadedImages();
                foreach (ImageTable image in images)
                {
                    if (image.State == DownloadState.DOWNLOADING || image.State == DownloadState.QUEUED)
                    {
                        image.State = DownloadState.NOT_QUEUED;
                        ImageDBManager.INSTANCE.setImage(image);
                    }
                    await StartDownloadAsync(image);
                }
            }
        }

        public async Task ClearCacheAsync()
        {
            StorageFolder folder = await GetCacheFolderAsync();
            if (folder != null)
            {
                await folder.DeleteAsync();
            }
            // Recreate the image cache folder:
            await GetCacheFolderAsync();
            Logger.Info("Image cache cleared!");
        }

        public async Task OpenCacheFolderAsync()
        {
            StorageFolder folder = await GetCacheFolderAsync();
            await Windows.System.Launcher.LaunchFolderAsync(folder);
        }

        /// <summary>
        /// Creates an unique file name, bases on the given url and the current time.
        /// </summary>
        /// <param name="url">The url of the image.</param>
        /// <returns>Returns an unique file name.</returns>
        public string CreateUniqueFileName(string url)
        {
            string name = DateTime.Now.ToString("dd.MM.yyyy_HH.mm.ss.ffff");
            int index = url.LastIndexOf('.');
            string ending = url.Substring(index, url.Length - index);
            return name + ending;
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void DOWNLOAD_HANDLER_DownloadStateChanged(AbstractDownloadableObject o, DownloadStateChangedEventArgs args)
        {
            if (o is ImageTable image)
            {
                ImageDBManager.INSTANCE.setImage(image);
            }
        }

        #endregion
    }
}
