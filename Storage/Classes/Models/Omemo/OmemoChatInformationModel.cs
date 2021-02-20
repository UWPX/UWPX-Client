using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shared.Classes;
using Storage.Classes.Contexts;

namespace Storage.Classes.Models.Omemo
{
    /// <summary>
    /// Information about the XEP-0384 (OMEMO Encryption) chat status.
    /// </summary>
    public class OmemoChatInformationModel: AbstractDataTemplate
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
        /// The device list subscription states for this chat.
        /// </summary>
        [Required]
        public OmemoDeviceListSubscriptionModel deviceListSubscription
        {
            get => _deviceListSubscription;
            set => SetDeviceListSubscriptionProperty(value);
        }
        [NotMapped]
        private OmemoDeviceListSubscriptionModel _deviceListSubscription;

        /// <summary>
        /// A collection of devices involved in this chat.
        /// </summary>
        [Required]
        public List<OmemoDeviceModel> devices
        {
            get => _devices;
            set => SetProperty(ref _devices, value);
        }
        [NotMapped]
        private List<OmemoDeviceModel> _devices;

        /// <summary>
        /// Has the OMEMO encryption been enabled for this chat.
        /// </summary>
        [Required]
        public bool enabled
        {
            get => _enabled;
            set => SetProperty(ref _enabled, value);
        }
        [NotMapped]
        private bool _enabled;

        /// <summary>
        /// Allow and send messages from and to trusted devices only.
        /// </summary>
        [Required]
        public bool trustedKeysOnly
        {
            get => _trustedKeysOnly;
            set => SetProperty(ref _trustedKeysOnly, value);
        }
        [NotMapped]
        private bool _trustedKeysOnly;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public OmemoChatInformationModel()
        {
            devices = new List<OmemoDeviceModel>();
            deviceListSubscription = new OmemoDeviceListSubscriptionModel();
            enabled = Settings.GetSettingBoolean(SettingsConsts.ENABLE_OMEMO_BY_DEFAULT_FOR_NEW_CHATS);
            trustedKeysOnly = false;
        }

        public OmemoChatInformationModel(string chatBareJid)
        {
            devices = new List<OmemoDeviceModel>();
            deviceListSubscription = new OmemoDeviceListSubscriptionModel
            {
                bareJid = chatBareJid
            };
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private void SetDeviceListSubscriptionProperty(OmemoDeviceListSubscriptionModel value)
        {
            OmemoDeviceListSubscriptionModel old = _deviceListSubscription;
            if (SetProperty(ref _deviceListSubscription, value, nameof(deviceListSubscription)))
            {
                if (!(old is null))
                {
                    old.PropertyChanged -= OnDeviceListSubscriptionPropertyChanged;
                }
                if (!(value is null))
                {
                    value.PropertyChanged += OnDeviceListSubscriptionPropertyChanged;
                }
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void Save()
        {
            using (MainDbContext ctx = new MainDbContext())
            {
                ctx.Update(this);
            }
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void OnDeviceListSubscriptionPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(nameof(deviceListSubscription) + '.' + e.PropertyName);
        }

        #endregion
    }
}
