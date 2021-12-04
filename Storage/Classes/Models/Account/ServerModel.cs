using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shared.Classes.Collections;
using Storage.Classes.Contexts;
using Windows.Security.Cryptography.Certificates;
using XMPP_API.Classes.Network.TCP;

namespace Storage.Classes.Models.Account
{
    public class ServerModel: AbstractModel
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
        /// The address of the server e.g. 'xmpp.jabber.org'.
        /// </summary>
        [Required]
        public string address
        {
            get => _address;
            set => SetProperty(ref _address, value);
        }
        [NotMapped]
        private string _address;

        /// <summary>
        /// The server port e.g. '5222'.
        /// </summary>
        [Required]
        public ushort port
        {
            get => _port;
            set => SetProperty(ref _port, value);
        }
        [NotMapped]
        private ushort _port = 5222;

        /// <summary>
        /// Defines how to handle the TLS connection.
        /// </summary>
        [Required]
        public TLSConnectionMode tlsMode
        {
            get => _tlsMode;
            set => SetProperty(ref _tlsMode, value);
        }
        [NotMapped]
        private TLSConnectionMode _tlsMode;

        /// <summary>
        /// True in case XEP-0198 (Stream Management) should be disabled.
        /// </summary>
        [Required]
        public bool disableStreamManagement
        {
            get => _disableStreamManagement;
            set => SetProperty(ref _disableStreamManagement, value);
        }
        [NotMapped]
        private bool _disableStreamManagement;

        /// <summary>
        /// True in case XEP-0280 (Message Carbons) should be disabled.
        /// </summary>
        [Required]
        public bool disableMessageCarbons
        {
            get => _disableMessageCarbons;
            set => SetProperty(ref _disableMessageCarbons, value);
        }
        [NotMapped]
        private bool _disableMessageCarbons;

        /// <summary>
        /// A collection of certificate errors that should be ignored during connecting to a server.
        /// </summary>
        [Required]
        public CustomObservableCollection<ChainValidationResult> ignoredCertificateErrors
        {
            get => _ignoredCertificateErrors;
            set => SetIgnoredCertificateErrorsProperty(value);
        }
        [NotMapped]
        private CustomObservableCollection<ChainValidationResult> _ignoredCertificateErrors = new CustomObservableCollection<ChainValidationResult>(true);


        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ServerModel()
        {
            tlsMode = TLSConnectionMode.FORCE;
        }

        public ServerModel(string address) : this()
        {
            this.address = address;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private void SetIgnoredCertificateErrorsProperty(CustomObservableCollection<ChainValidationResult> value)
        {
            CustomObservableCollection<ChainValidationResult> old = _ignoredCertificateErrors;
            if (SetProperty(ref _ignoredCertificateErrors, value, nameof(ignoredCertificateErrors)))
            {
                if (!(old is null))
                {
                    old.PropertyChanged -= OnIgnoredCertificateErrorsPropertyChanged;
                }
                if (!(value is null))
                {
                    value.PropertyChanged += OnIgnoredCertificateErrorsPropertyChanged;
                }
            }
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
        private void OnIgnoredCertificateErrorsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(nameof(ignoredCertificateErrors) + '.' + e.PropertyName);
        }

        #endregion
    }
}
