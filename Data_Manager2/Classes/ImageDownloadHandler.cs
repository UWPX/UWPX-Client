using Data_Manager2.Classes.DBManager;
using Data_Manager2.Classes.DBTables;
using Logging;
using Shared.Classes.Network;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;

namespace Data_Manager2.Classes
{
    public class ImageDownloadHandler
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly DownloadHandler DOWNLOAD_HANDLER;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ImageDownloadHandler(DownloadHandler downloadHandler)
        {
            this.DOWNLOAD_HANDLER = downloadHandler;
            this.DOWNLOAD_HANDLER.DownloadStateChanged += DOWNLOAD_HANDLER_DownloadStateChanged;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public async Task<StorageFolder> GetImageCacheFolderAsync()
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
        public async Task DownloadImageAsync(ImageTable image)
        {
            if (image.State != DownloadState.DOWNLOADING && image.State != DownloadState.QUEUED)
            {
                await DOWNLOAD_HANDLER.EnqueueDownloadAsync(image);
            }
        }

        public async Task<ImageTable> DownloadImageAsync(ChatMessageTable msg)
        {
            ImageTable image = await ImageDBManager.INSTANCE.getImageAsync(msg);
            if (image is null)
            {
                StorageFolder folder = await GetImageCacheFolderAsync();
                image = new ImageTable()
                {
                    messageId = msg.id,
                    SourceUrl = msg.message,
                    TargetFileName = CreateUniqueFileName(msg.message),
                    TargetFolderPath = folder.Path,
                    State = DownloadState.NOT_QUEUED,
                    Progress = 0,
                };
                ImageDBManager.INSTANCE.setImage(image);
            }
            await DownloadImageAsync(image);
            return image;
        }

        public async Task RedownloadImageAsync(ImageTable image)
        {
            await DOWNLOAD_HANDLER.EnqueueDownloadAsync(image);
        }

        public void CancelDownload(ImageTable image)
        {
            DOWNLOAD_HANDLER.CancelDownload(image);
        }

        public async Task<AbstractDownloadableObject> FindAsync(Predicate<AbstractDownloadableObject> predicate)
        {
            return await DOWNLOAD_HANDLER.FindAsync(predicate);
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
                    await DownloadImageAsync(image);
                }
            }
        }

        public async Task ClearImageCacheAsync()
        {
            StorageFolder folder = await GetImageCacheFolderAsync();
            if (folder != null)
            {
                await folder.DeleteAsync();
            }
            // Recreate the image cache folder:
            await GetImageCacheFolderAsync();
            Logger.Info("Image cache cleared!");
        }

        public async Task OpenImageCacheFolderAsync()
        {
            StorageFolder folder = await GetImageCacheFolderAsync();
            await Windows.System.Launcher.LaunchFolderAsync(folder);
        }

        #endregion

        #region --Misc Methods (Private)--
        /// <summary>
        /// Creates an unique file name, bases on the given url and the current time.
        /// </summary>
        /// <param name="url">The url of the image.</param>
        /// <returns>Returns an unique file name.</returns>
        private string CreateUniqueFileName(string url)
        {
            string name = DateTime.Now.ToString("dd.MM.yyyy_HH.mm.ss.ffff");
            int index = url.LastIndexOf('.');
            string ending = url.Substring(index, url.Length - index);
            return name + ending;
        }

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
