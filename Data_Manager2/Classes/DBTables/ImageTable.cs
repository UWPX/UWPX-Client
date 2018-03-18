using Data_Manager2.Classes.Events;
using SQLite;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
using System;

namespace Data_Manager2.Classes.DBTables
{
    [Table(DBTableConsts.IMAGE_TABLE)]
    public class ImageTable
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        [PrimaryKey]
        // The id of the message:
        public string messageId { get; set; }
        // The local path:
        public string path { get; set; }
        // The state of the image download:
        public DownloadState state { get; set; }
        // If the image download failed:
        public string errorMessage { get; set; }

        // The image download progress:
        [Ignore]
        public double progress { get; set; }

        public delegate void DownloadStateChangedHandler(ImageTable img, DownloadStateChangedEventArgs args);
        public delegate void DownloadProgressChangedHandler(ImageTable img, DownloadProgressChangedEventArgs args);

        public event DownloadStateChangedHandler DownloadStateChanged;
        public event DownloadProgressChangedHandler DownloadProgressChanged;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 17/11/2017 Created [Fabian Sauter]
        /// </history>
        public ImageTable()
        {
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void onStateChanged()
        {
            DownloadStateChanged?.Invoke(this, new DownloadStateChangedEventArgs(state));
        }

        public void onDownloadProgressChanged()
        {
            DownloadProgressChanged?.Invoke(this, new DownloadProgressChangedEventArgs(progress));
        }

        /// <summary>
        /// Converts the path to an BitmapImage.
        /// This is a workaround to open also images that are stored on a separate drive.
        /// </summary>
        /// <returns>The BitmapImage representation of the current path object.</returns>
        public async Task<BitmapImage> getBitmapImageAsync()
        {
            if(path == null)
            {
                return null;
            }

            try
            {
                StorageFile file = await StorageFile.GetFileFromPathAsync(path);
                if(file == null)
                {
                    return null;
                }

                BitmapImage img = new BitmapImage();
                img.SetSource(await file.OpenReadAsync());
                return img;
            }
            catch (Exception)
            {
                return null;
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
