using Data_Manager.Classes.Events;
using SQLite.Net.Attributes;

namespace Data_Manager2.Classes.DBTables
{
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
