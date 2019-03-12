using Data_Manager2.Classes.DBTables;
using Logging;
using Shared.Classes.Network;
using Shared.Classes.SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace Data_Manager2.Classes.DBManager
{
    public class ImageDBManager : AbstractDBManager
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public static readonly ImageDBManager INSTANCE = new ImageDBManager();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public async Task<ImageTable> getImageAsync(ChatMessageTable msg)
        {
            ImageTable img = (ImageTable)await ConnectionHandler.INSTANCE.IMAGE_DOWNLOAD_HANDLER.FindAsync((x) => { return x is ImageTable imageTable && string.Equals(imageTable.messageId, msg.id); });
            if (img is null)
            {
                List<ImageTable> list = dB.Query<ImageTable>(true, "SELECT * FROM " + DBTableConsts.IMAGE_TABLE + " WHERE " + nameof(ImageTable.messageId) + " = ?;", msg.id);
                if (list.Count > 0)
                {
                    img = list[0];
                }
            }
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
                        string path = Path.Combine(image.TargetFolderPath, image.TargetFileName);
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
