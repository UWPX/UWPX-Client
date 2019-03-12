namespace Shared.Classes.Network
{
    public enum DownloadState
    {
        /// <summary>
        /// The object is not in the download queue.
        /// </summary>
        NOT_QUEUED,
        /// <summary>
        /// The object is in the download queue and ready to be downloaded.
        /// </summary>
        QUEUED,
        /// <summary>
        /// The object is being downloaded.
        /// </summary>
        DOWNLOADING,
        /// <summary>
        /// The download finished without any errors.
        /// </summary>
        DONE,
        /// <summary>
        /// The download ended with an error.
        /// </summary>
        ERROR,
        /// <summary>
        /// The download has been canceled.
        /// </summary>
        CANCELED,
    }
}
