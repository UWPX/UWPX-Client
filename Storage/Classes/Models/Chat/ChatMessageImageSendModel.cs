using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Storage.Classes.Contexts;

namespace Storage.Classes.Models.Chat
{
    public class ChatMessageImageSendModel: AbstractModel
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
        /// The byte data representing the image to upload.
        /// </summary>
        [Required]
        public byte[] data
        {
            get => _data;
            set => SetProperty(ref _data, value);
        }
        [NotMapped]
        protected byte[] _data;

        [Required]
        public string fileName
        {
            get => _fileName;
            set => SetProperty(ref _fileName, value);
        }
        [NotMapped]
        protected string _fileName;

        /// <summary>
        /// The IANA media content type e.g. 'image/jpeg' or 'image/png'.
        /// </summary>
        [Required]
        public string contentType
        {
            get => _contentType;
            set => SetProperty(ref _contentType, value);
        }
        [NotMapped]
        protected string _contentType;

        [Required]
        public UploadState state
        {
            get => _state;
            set => SetProperty(ref _state, value);
        }
        [NotMapped]
        protected UploadState _state;

        /// <summary>
        /// The URL for image upload.
        /// Only valid in case <see cref="state"/> is set to <see cref="UploadState.SLOT_RECEIVED"/>.
        /// </summary>
        public string uploadSlot
        {
            get => _uploadSlot;
            set => SetProperty(ref _uploadSlot, value);
        }
        [NotMapped]
        protected string _uploadSlot;

        /// <summary>
        /// The URL for image download.
        /// Only valid in case <see cref="state"/> is set to <see cref="UploadState.SLOT_RECEIVED"/>.
        /// </summary>
        public string url
        {
            get => _url;
            set => SetProperty(ref _url, value);
        }
        [NotMapped]
        protected string _url;

        /// <summary>
        /// The error in case something went wrong.
        /// </summary>
        [Required]
        public UploadError error
        {
            get => _error;
            set => SetProperty(ref _error, value);
        }
        [NotMapped]
        protected UploadError _error;

        /// <summary>
        /// The max file size in case the upload failed because the file size is too large.
        /// Only valid in case it is larger than 0 and <see cref="state"/> is set to <see cref="UploadState.ERROR"/>.
        /// </summary>
        public uint maxFileSize
        {
            get => _maxFileSize;
            set => SetProperty(ref _maxFileSize, value);
        }
        [NotMapped]
        protected uint _maxFileSize;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ChatMessageImageSendModel()
        {
            error = UploadError.NONE;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public override void Remove(MainDbContext ctx, bool recursive)
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
