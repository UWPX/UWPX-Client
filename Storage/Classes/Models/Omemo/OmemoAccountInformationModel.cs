using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Omemo.Classes;
using Omemo.Classes.Keys;
using Shared.Classes;
using Shared.Classes.Collections;

namespace Storage.Classes.Models.Omemo
{
    /// <summary>
    /// Information about the XEP-0384 (OMEMO Encryption) account status.
    /// </summary>
    public class OmemoAccountInformationModel: AbstractDataTemplate
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
        /// The local unique device id.
        /// </summary>
        [Required]
        public uint deviceId
        {
            get => _deviceId;
            set => SetProperty(ref _deviceId, value);
        }
        [NotMapped]
        private uint _deviceId;

        /// <summary>
        /// The local name for this device.
        /// </summary>
        public string deviceLabel
        {
            get => _deviceLabel;
            set => SetProperty(ref _deviceLabel, value);
        }
        [NotMapped]
        private string _deviceLabel;

        /// <summary>
        /// Did we announce our bundle information?
        /// </summary>
        [Required]
        public bool bundleInfoAnnounced
        {
            get => _bundleInfoAnnounced;
            set => SetProperty(ref _bundleInfoAnnounced, value);
        }
        [NotMapped]
        private bool _bundleInfoAnnounced;

        /// <summary>
        /// The identity key pair.
        /// </summary>
        [Required]
        public IdentityKeyPairModel identityKey
        {
            get => _identityKey;
            set => SetProperty(ref _identityKey, value);
        }
        [NotMapped]
        private IdentityKeyPairModel _identityKey;

        /// <summary>
        /// The signed PreKey.
        /// </summary>
        [Required]
        public SignedPreKeyModel signedPreKey
        {
            get => _signedPreKey;
            set => SetSignedPreKeyProperty(value);
        }
        [NotMapped]
        private SignedPreKeyModel _signedPreKey;

        /// <summary>
        /// A collection of PreKeys to publish.
        /// </summary>
        [Required]
        public CustomObservableCollection<PreKeyModel> preKeys
        {
            get => _preKeys;
            set => SetPreKeysProperty(value);
        }
        [NotMapped]
        private CustomObservableCollection<PreKeyModel> _preKeys = new CustomObservableCollection<PreKeyModel>(true);

        /// <summary>
        /// The highest PreKey id used until now.
        /// </summary>
        public uint maxPreKeyId
        {
            get => _maxPreKeyId;
            set => SetProperty(ref _maxPreKeyId, value);
        }
        [NotMapped]
        private uint _maxPreKeyId;

        /// <summary>
        /// A collection of OMEMO capable devices.
        /// </summary>
        [Required]
        public CustomObservableCollection<OmemoDeviceModel> devices
        {
            get => _devices;
            set => SetDevicesProperty(value);
        }
        [NotMapped]
        private CustomObservableCollection<OmemoDeviceModel> _devices;

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

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


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

        private void SetDevicesProperty(CustomObservableCollection<OmemoDeviceModel> value)
        {
            CustomObservableCollection<OmemoDeviceModel> old = _devices;
            if (SetProperty(ref _devices, value, nameof(devices)))
            {
                if (!(old is null))
                {
                    old.PropertyChanged -= OnDevicesPropertyChanged;
                }
                if (!(value is null))
                {
                    value.PropertyChanged += OnDevicesPropertyChanged;
                }
            }
        }

        private void SetPreKeysProperty(CustomObservableCollection<PreKeyModel> value)
        {
            CustomObservableCollection<PreKeyModel> old = _preKeys;
            if (SetProperty(ref _preKeys, value, nameof(preKeys)))
            {
                if (!(old is null))
                {
                    old.PropertyChanged -= OnPreKeysPropertyChanged;
                }
                if (!(value is null))
                {
                    value.PropertyChanged += OnPreKeysPropertyChanged;
                }
            }
        }

        private void SetSignedPreKeyProperty(SignedPreKeyModel value)
        {
            SignedPreKeyModel old = _signedPreKey;
            if (SetProperty(ref _signedPreKey, value, nameof(signedPreKey)))
            {
                if (!(old is null))
                {
                    old.PropertyChanged -= OnSignedPreKeyPropertyChanged;
                }
                if (!(value is null))
                {
                    value.PropertyChanged += OnSignedPreKeyPropertyChanged;
                }
            }
        }

        private void SetIdentityKeyProperty(IdentityKeyPairModel value)
        {
            IdentityKeyPairModel old = _identityKey;
            if (SetProperty(ref _identityKey, value, nameof(identityKey)))
            {
                if (!(old is null))
                {
                    old.PropertyChanged -= OnIdentityKeyPropertyChanged;
                }
                if (!(value is null))
                {
                    value.PropertyChanged += OnIdentityKeyPropertyChanged;
                }
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        /// <summary>
        /// Generates all OMEMO keys and sets <see cref="keysGenerated"/> to true, as well as <see cref="bundleInfoAnnounced"/> to false.
        /// Should only be called once, when a new account has been added to prevent overlapping PreKeys and DB inconsistencies.
        /// </summary>
        public void GenerateOmemoKeys()
        {
            deviceId = 0;
            bundleInfoAnnounced = false;
            identityKey = KeyHelper.GenerateIdentityKeyPair();
            signedPreKey = KeyHelper.GenerateSignedPreKey(0, identityKey.privKey);
            preKeys.Clear();
            preKeys.AddRange(KeyHelper.GeneratePreKeys(maxPreKeyId, 100));
            maxPreKeyId += 100;
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

        private void OnDevicesPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(nameof(devices) + '.' + e.PropertyName);
        }

        private void OnPreKeysPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(nameof(preKeys) + '.' + e.PropertyName);
        }

        private void OnSignedPreKeyPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(nameof(signedPreKey) + '.' + e.PropertyName);
        }

        private void OnIdentityKeyPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(nameof(identityKey) + '.' + e.PropertyName);
        }

        #endregion
    }
}
