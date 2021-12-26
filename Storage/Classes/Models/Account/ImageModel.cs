using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Threading.Tasks;
using Shared.Classes.Image;
using Storage.Classes.Contexts;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml.Media.Imaging;

namespace Storage.Classes.Models.Account
{
    public class ImageModel: AbstractModel
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
        /// The actual image data.
        /// </summary>
        [Required]
        public byte[] data
        {
            get => _data;
            set => SetDataProperty(value);
        }
        [NotMapped]
        private byte[] _data;

        /// <summary>
        /// SHA-1 hash of the <see cref="data"/> in hex.
        /// </summary>
        [Required]
        public string hash
        {
            get => _hash;
            set => SetProperty(ref _hash, value);
        }
        [NotMapped]
        private string _hash;

        /// <summary>
        /// When was the last time the image got updated.
        /// </summary>
        [Required]
        public DateTime lastUpdate
        {
            get => _lastUpdate;
            set => SetProperty(ref _lastUpdate, value);
        }
        [NotMapped]
        private DateTime _lastUpdate;

        /// <summary>
        /// The IANA media type of the image.
        /// https://www.iana.org/assignments/media-types/media-types.xhtml#image
        /// https://developer.mozilla.org/en-US/docs/Web/HTTP/Basics_of_HTTP/MIME_types
        /// </summary>
        [Required]
        public string type
        {
            get => _type;
            set => SetProperty(ref _type, value);
        }
        [NotMapped]
        private string _type;

        /// <summary>
        /// The state of the subscription to the metadata PEP node.
        /// </summary>
        [Required]
        public AvatarMetadataSubscriptionState subscriptionState
        {
            get => _subscriptionState;
            set => SetProperty(ref _subscriptionState, value);
        }
        [NotMapped]
        private AvatarMetadataSubscriptionState _subscriptionState;

        [NotMapped]
        private SoftwareBitmap img;
        [NotMapped]
        private SoftwareBitmapSource imgSrc;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ImageModel()
        {
            lastUpdate = DateTime.MinValue;
            subscriptionState = AvatarMetadataSubscriptionState.UNKNOWN;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public async Task<SoftwareBitmap> GetSoftwareBitmapAsync()
        {
            if (img is null)
            {
                img = await ImageUtils.ToBitmapImageAsync(data);
                imgSrc = new SoftwareBitmapSource();
                await imgSrc.SetBitmapAsync(img);
            }
            return img;
        }

        public async Task<SoftwareBitmapSource> GetSoftwareBitmapSourceAsync()
        {
            if (imgSrc is null)
            {
                if (img is null)
                {
                    _ = await GetSoftwareBitmapAsync();
                }
                imgSrc = new SoftwareBitmapSource();
                await imgSrc.SetBitmapAsync(img);
            }
            return imgSrc;
        }

        private void SetDataProperty(byte[] value)
        {
            if (SetProperty(ref _data, value))
            {
                img = null;
                imgSrc = null;
            }
        }

        public async Task SetImageAsync(SoftwareBitmap img, bool isAnimated)
        {
            Debug.Assert(!(img is null));
            data = await ImageUtils.ToByteArrayAsync(img, isAnimated);
            hash = ImageUtils.HashImage(data);
            this.img = img;
            imgSrc = new SoftwareBitmapSource();
            await imgSrc.SetBitmapAsync(img);
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public override void Remove(MainDbContext ctx, bool recursive)
        {
            ctx.Remove(this);
        }

        /// <summary>
        /// Returns true in case the <see cref="subscriptionState"/> is set to <see cref="AvatarMetadataSubscriptionState.UNKNOWN"/> or the subscription is 30 days old.
        /// </summary>
        public bool ShouldCheckSubscription()
        {
            return subscriptionState == AvatarMetadataSubscriptionState.UNKNOWN || (DateTime.Now - lastUpdate).TotalDays > 30;
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
