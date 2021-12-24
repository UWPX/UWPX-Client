using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Threading.Tasks;
using Shared.Classes.Image;
using Storage.Classes.Contexts;
using Windows.Graphics.Imaging;

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
        /// SHA-1 hash of the <see cref="data"/>.
        /// </summary>
        [Required]
        public byte[] hash
        {
            get => _hash;
            set => SetProperty(ref _hash, value);
        }
        [NotMapped]
        private byte[] _hash;

        [NotMapped]
        private SoftwareBitmap img;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public async Task<SoftwareBitmap> GetSoftwareBitmapAsync()
        {
            if(img is null)
            {
                img = await ImageUtils.ToBitmapImageAsync(data);
            }
            return img;
        }

        private void SetDataProperty(byte[] value)
        {
            if(SetProperty(ref _data, value))
            {
                img = null;
            }
        }

        public async Task SetImageAsync(SoftwareBitmap img)
        {
            Debug.Assert(!(img is null));
            data = await ImageUtils.ToByteArrayAsync(img);
            hash = ImageUtils.HashImage(data);
        }

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
