using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Logging;
using Shared.Classes.Network;
using Storage.Classes;
using Storage.Classes.Contexts;
using Storage.Classes.Models.Chat;
using Windows.Foundation;
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
            DOWNLOAD_HANDLER.DownloadStateChanged += OnDownloadStateChanged;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public IAsyncOperation<StorageFolder> GetCacheFolderAsync()
        {
            if (Settings.GetSettingBoolean(SettingsConsts.DISABLE_DOWNLOAD_IMAGES_TO_LIBARY))
            {
                return ApplicationData.Current.LocalFolder.CreateFolderAsync("cachedImages", CreationCollisionOption.OpenIfExists);
            }
            return KnownFolders.PicturesLibrary.CreateFolderAsync("UWPX", CreationCollisionOption.OpenIfExists);
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public async Task StartDownloadAsync(ChatMessageImageModel image)
        {
            await DOWNLOAD_SEMA.WaitAsync();
            if (image.state != DownloadState.DOWNLOADING && image.state != DownloadState.QUEUED)
            {
                await DOWNLOAD_HANDLER.EnqueueDownloadAsync(image);
            }
            DOWNLOAD_SEMA.Release();
        }

        public Task RedownloadAsync(ChatMessageImageModel image)
        {
            return DOWNLOAD_HANDLER.EnqueueDownloadAsync(image);
        }

        public void CancelDownload(ChatMessageImageModel image)
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
            if (!Settings.GetSettingBoolean(SettingsConsts.DISABLE_IMAGE_AUTO_DOWNLOAD))
            {
                IEnumerable<ChatMessageImageModel> images;
                using (MainDbContext ctx = new MainDbContext())
                {
                    images = ctx.ChatMessageImages.Where(i => i.state != DownloadState.DONE && i.state != DownloadState.ERROR && i.state != DownloadState.CANCELED).ToList();
                }
                foreach (ChatMessageImageModel image in images)
                {
                    if (image.state == DownloadState.DOWNLOADING || image.state == DownloadState.QUEUED)
                    {
                        image.state = DownloadState.NOT_QUEUED;
                        image.Update();
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
        private void OnDownloadStateChanged(AbstractDownloadableObject o, DownloadStateChangedEventArgs args)
        {
            if (o is ChatMessageImageModel image)
            {
                using (MainDbContext ctx = new MainDbContext())
                {
                    ctx.Update(image);
                }
            }
        }

        #endregion
    }
}
