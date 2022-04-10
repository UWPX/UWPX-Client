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

        [NotMapped]
        private SoftwareBitmap img;
        [NotMapped]
        private SoftwareBitmapSource imgSrc;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public async Task<SoftwareBitmap> GetSoftwareBitmapAsync()
        {
            if (img is null)
            {
                if (!(data is null) && data.Length > 0)
                {
                    img = await ImageUtils.ToBitmapImageAsync(data);
                }
                if (!(img is null))
                {
                    imgSrc = new SoftwareBitmapSource();
                    await imgSrc.SetBitmapAsync(img);
                }
                else
                {
                    imgSrc = null;
                }
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
                if (!(img is null) && !(imgSrc is null))
                {
                    imgSrc = new SoftwareBitmapSource();
                    await imgSrc.SetBitmapAsync(img);
                }
                else
                {
                    imgSrc = null;
                }
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

        public override bool Equals(object obj)
        {
            return obj is ImageModel img && string.Equals(hash, img.hash);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
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
