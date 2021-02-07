using System.ComponentModel.DataAnnotations;
using System.IO;

namespace Shared.Classes.Network
{
    public abstract class AbstractDownloadableObject: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        protected DownloadState _State;
        /// <summary>
        /// The current state of the download.
        /// </summary>
        [Required]
        public DownloadState State
        {
            get => _State;
            set => SetProperty(ref _State, value);
        }

        protected double _Progress;
        /// <summary>
        /// The download progress in percent.
        /// </summary>
        [Required]
        public double Progress
        {
            get => _Progress;
            set => SetProperty(ref _Progress, value);
        }

        protected string _SourceUrl;
        /// <summary>
        /// The URL where the object should get downloaded from.
        /// </summary>
        [Required]
        public string SourceUrl
        {
            get => _SourceUrl;
            set => SetProperty(ref _SourceUrl, value);
        }

        protected string _TargetFolderPath;
        /// <summary>
        /// The target folder path, where the downloaded object should get saved to.
        /// E.g.: C:\Program Files\Git
        /// </summary>
        [Required]
        public string TargetFolderPath
        {
            get => _TargetFolderPath;
            set => SetProperty(ref _TargetFolderPath, value);
        }

        protected string _TargetFileName;
        /// <summary>
        /// The name of the downloaded object with extension.
        /// E.g.: file.png
        /// </summary>
        [Required]
        public string TargetFileName
        {
            get => _TargetFileName;
            set => SetProperty(ref _TargetFileName, value);
        }

        protected DownloadError _Error = DownloadError.NONE;
        /// <summary>
        /// The error code if one occurred.
        /// </summary>
        [Required]
        public DownloadError Error
        {
            get => _Error;
            set => SetProperty(ref _Error, value);
        }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public AbstractDownloadableObject()
        {
            State = DownloadState.NOT_QUEUED;
            Error = DownloadError.NONE;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public string GetFullPath()
        {
            return Path.Combine(TargetFolderPath, TargetFileName);
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


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
