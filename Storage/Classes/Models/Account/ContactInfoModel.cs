using System;
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
        [ForeignKey(nameof(avatarId))]
        public ImageModel avatar
        {
            get => _avatar;
            set => SetAvatarProperty(value);
        }
        [NotMapped]
        private ImageModel _avatar;
        public int? avatarId { get; set; }

        /// <summary>
        /// When was the last time the avatar got updated.
        /// </summary>
        [Required]
        public DateTime lastAvatarUpdate
        {
            get => _lastAvatarUpdate;
            set => SetProperty(ref _lastAvatarUpdate, value);
        }
        [NotMapped]
        private DateTime _lastAvatarUpdate;

        /// <summary>
        /// The state of the subscription to the avatar metadata PEP node.
        /// </summary>
        [Required]
        public AvatarMetadataSubscriptionState avatarSubscriptionState
        {
            get => _avatarSubscriptionState;
            set => SetProperty(ref _avatarSubscriptionState, value);
        }
        [NotMapped]
        private AvatarMetadataSubscriptionState _avatarSubscriptionState;

        /// <summary>
        /// A custom name for the chat/account.
        /// </summary>
        public string name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
        [NotMapped]
        private string _name;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ContactInfoModel()
        {
            lastAvatarUpdate = DateTime.MinValue;
            avatarSubscriptionState = AvatarMetadataSubscriptionState.UNKNOWN;
        }

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
            if (recursive)
            {
                avatar?.Remove(ctx, recursive);
            }
            ctx.Remove(this);
        }

        /// <summary>
        /// Returns true in case the <see cref="avatarSubscriptionState"/> is set to <see cref="AvatarMetadataSubscriptionState.UNKNOWN"/> or the subscription is 30 days old.
        /// </summary>
        public bool ShouldCheckAvatarSubscription()
        {
            return avatarSubscriptionState == AvatarMetadataSubscriptionState.UNKNOWN || (DateTime.Now - lastAvatarUpdate).TotalDays > 30;
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
