using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Omemo.Classes;
using Storage.Classes.Contexts;

namespace Storage.Classes.Models.Omemo
{
    /// <summary>
    /// Represents a single OMEMO capable device.
    /// </summary>
    public class OmemoDeviceModel: AbstractModel
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
        /// The unique ID of the device
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
        /// Not null in case there exists a session with this device.
        /// </summary>
        public OmemoSessionModel session
        {
            get => _session;
            set => SetSessionProperty(value);
        }
        [NotMapped]
        private OmemoSessionModel _session;

        /// <summary>
        /// The fingerprint of this device.
        /// </summary>
        public OmemoFingerprintModel fingerprint
        {
            get => _fingerprint;
            set => SetFingerprintProperty(value);
        }
        [NotMapped]
        private OmemoFingerprintModel _fingerprint;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public OmemoDeviceModel() { }

        public OmemoDeviceModel(OmemoProtocolAddress address)
        {
            deviceId = address.DEVICE_ID;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private void SetFingerprintProperty(OmemoFingerprintModel value)
        {
            OmemoFingerprintModel old = _fingerprint;
            if (SetProperty(ref _fingerprint, value, nameof(fingerprint)))
            {
                if (!(old is null))
                {
                    old.PropertyChanged -= OnFingerprintPropertyChanged;
                }
                if (!(value is null))
                {
                    value.PropertyChanged += OnFingerprintPropertyChanged;
                }
            }
        }

        private void SetSessionProperty(OmemoSessionModel value)
        {
            OmemoSessionModel old = _session;
            if (SetProperty(ref _session, value, nameof(session)))
            {
                if (!(old is null))
                {
                    old.PropertyChanged -= OnSessionPropertyChanged;
                }
                if (!(value is null))
                {
                    value.PropertyChanged += OnSessionPropertyChanged;
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
                session?.Remove(ctx, recursive);
                fingerprint?.Remove(ctx, recursive);
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
        private void OnFingerprintPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(nameof(fingerprint) + '.' + e.PropertyName);
        }

        private void OnSessionPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(nameof(session) + '.' + e.PropertyName);
        }

        #endregion
    }
}
