using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace Shared.Classes.Network
{
    public abstract class AbstractDownloadableObject: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }
        [NotMapped]
        private int _id;

        /// <summary>
        /// The current state of the download.
        /// </summary>
        [Required]
        public DownloadState state
        {
            get => _state;
            set => SetProperty(ref _state, value);
        }
        [NotMapped]
        protected DownloadState _state;

        /// <summary>
        /// The download progress in percent.
        /// </summary>
        [Required]
        public double progress
        {
            get => _progress;
            set => SetProperty(ref _progress, value);
        }
        [NotMapped]
        protected double _progress;

        /// <summary>
        /// The URL where the object should get downloaded from.
        /// </summary>
        [Required]
        public string sourceUrl
        {
            get => _sourceUrl;
            set => SetProperty(ref _sourceUrl, value);
        }
        [NotMapped]
        protected string _sourceUrl;

        /// <summary>
        /// The target folder path, where the downloaded object should get saved to.
        /// E.g.: C:\Program Files\Git
        /// </summary>
        public string targetFolderPath
        {
            get => _targetFolderPath;
            set => SetProperty(ref _targetFolderPath, value);
        }
        [NotMapped]
        protected string _targetFolderPath;

        /// <summary>
        /// The name of the downloaded object with extension.
        /// E.g.: file.png
        /// </summary>
        public string targetFileName
        {
            get => _targetFileName;
            set => SetProperty(ref _targetFileName, value);
        }
        [NotMapped]
        protected string _targetFileName;

        /// <summary>
        /// The error code if one occurred.
        /// </summary>
        [Required]
        public DownloadError error
        {
            get => _error;
            set => SetProperty(ref _error, value);
        }
        [NotMapped]
        protected DownloadError _error = DownloadError.NONE;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public AbstractDownloadableObject(string sourceUrl)
        {
            this.sourceUrl = sourceUrl;
            state = DownloadState.NOT_QUEUED;
            error = DownloadError.NONE;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public string GetFullPath()
        {
            return Path.Combine(targetFolderPath, targetFileName);
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        /// <summary>
        /// Removes the current model in the <see cref="DbContext"/> either recursively or not.
        /// </summary>
        /// <param name="ctx">The <see cref="MainDbContext"/> the model should be removed from.</param>
        /// <param name="recursive">Recursively remove the current model.</param>
        public void Remove(DbContext ctx, bool recursive)
        {
            ctx.Remove(this);
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
