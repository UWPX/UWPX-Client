using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Data_Manager2.Classes.DBTables;
using Logging;
using Shared.Classes.Network;
using Shared.Classes.SQLite;
using Windows.Storage;

namespace Data_Manager2.Classes.DBManager
{
    public class ImageDBManager: AbstractDBManager
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public static readonly ImageDBManager INSTANCE = new ImageDBManager();
        private readonly SemaphoreSlim GET_IMAGE_SEMA = new SemaphoreSlim(1);

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public async Task<ImageTable> getImageAsync(ChatMessageTable msg)
        {
            await GET_IMAGE_SEMA.WaitAsync();

            // Is already downloading:
            ImageTable img = (ImageTable)await ConnectionHandler.INSTANCE.IMAGE_DOWNLOAD_HANDLER.FindAsync((x) => { return x is ImageTable imageTable && string.Equals(imageTable.messageId, msg.id); });
            if (img is null)
            {
                // Is in the DB:
                List<ImageTable> list = dB.Query<ImageTable>(true, "SELECT * FROM " + DBTableConsts.IMAGE_TABLE + " WHERE " + nameof(ImageTable.messageId) + " = ?;", msg.id);
                if (list.Count > 0)
                {
                    img = list[0];
                }
                else
                {
                    // Create a new image entry:
                    StorageFolder folder = await ConnectionHandler.INSTANCE.IMAGE_DOWNLOAD_HANDLER.GetCacheFolderAsync();
                    img = new ImageTable()
                    {
                        messageId = msg.id,
                        SourceUrl = msg.message,
                        TargetFileName = ConnectionHandler.INSTANCE.IMAGE_DOWNLOAD_HANDLER.CreateUniqueFileName(msg.message),
                        TargetFolderPath = folder.Path,
                        State = DownloadState.NOT_QUEUED,
                        Progress = 0,
                    };
                    setImage(img);
                }
            }
            GET_IMAGE_SEMA.Release();
            return img;
        }

        public void setImage(ImageTable image)
        {
            dB.InsertOrReplace(image);
        }

        public List<ImageTable> getAllUndownloadedImages()
        {
            return dB.Query<ImageTable>(true, "SELECT * FROM " + DBTableConsts.IMAGE_TABLE + " WHERE " + nameof(ImageTable.State) + " != ? AND " + nameof(ImageTable.State) + " != ?;", (int)DownloadState.DONE, (int)DownloadState.ERROR);
        }

        public async Task deleteImageAsync(ChatMessageTable msg)
        {
            ImageTable image = await getImageAsync(msg);
            if (!(image is null))
            {
                // Cancel download:
                ConnectionHandler.INSTANCE.IMAGE_DOWNLOAD_HANDLER.CancelDownload(image);

                // Try to delete local file:
                try
                {
                    if (!string.IsNullOrEmpty(image.TargetFolderPath) && !string.IsNullOrEmpty(image.TargetFileName))
                    {
                        string path = image.GetFullPath();
                        StorageFile file = await StorageFile.GetFileFromPathAsync(path);
                        if (!(file is null))
                        {
                            await file.DeleteAsync();
                        }
                    }
                    Logger.Info("Deleted: " + image.TargetFileName);
                }
                catch (Exception e)
                {
                    Logger.Error("Failed to delete image: " + image.TargetFileName, e);
                }

                // Delete DB entry:
                dB.Delete(image);
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--
        protected override void CreateTables()
        {
            dB.CreateTable<ImageTable>();
        }

        protected override void DropTables()
        {
            dB.DropTable<ImageTable>();
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
