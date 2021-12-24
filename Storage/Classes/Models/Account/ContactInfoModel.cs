using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Storage.Classes.Contexts;

namespace Storage.Classes.Models.Account
{
    public class ContactInfoModel: AbstractModel
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
        /// The <see cref="ImageModel"/> representing the contact's XEP-0084 avatar.
        /// </summary>
        public ImageModel avatar
        {
            get => _avatar;
            set => SetAvatarProperty(value);
        }
        [NotMapped]
        private ImageModel _avatar;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private void SetAvatarProperty(ImageModel value)
        {
            ImageModel old = _avatar;
            if (SetProperty(ref _avatar, value, nameof(avatar)))
            {
                if (!(old is null))
                {
                    old.PropertyChanged -= OnAvatarPropertyChanged;
                }
                if (!(value is null))
                {
                    value.PropertyChanged += OnAvatarPropertyChanged;
                }
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public override void Remove(MainDbContext ctx, bool recursive)
        {
            if(recursive)
            {
                avatar?.Remove(ctx, recursive);
            }
            ctx.Remove(this);
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void OnAvatarPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(nameof(avatar) + '.' + e.PropertyName);
        }

        #endregion
    }
}
