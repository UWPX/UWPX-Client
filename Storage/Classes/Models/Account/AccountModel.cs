using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Storage.Classes.Contexts;
using Storage.Classes.Models.Omemo;
using XMPP_API.Classes;
using XMPP_API.Classes.Network;

namespace Storage.Classes.Models.Account
{
    public class AccountModel: AbstractModel
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        /// <summary>
        /// The unique bare Jabber ID of the account: user@domain e.g. 'coven@chat.shakespeare.lit'
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)] // Do not automatically let the DB generate this ID
        public string bareJid
        {
            get => _bareJid;
            set => SetProperty(ref _bareJid, value);
        }
        [NotMapped]
        private string _bareJid;

        /// <summary>
        /// The full Jabber ID of the account e.g. 'coven@chat.shakespeare.lit/phone'
        /// </summary>
        [Required]
        public JidModel fullJid
        {
            get => _fullJid;
            set => SetFullJidProperty(value);
        }
        [NotMapped]
        private JidModel _fullJid;

        /// <summary>
        /// The complete server configuration for the account.
        /// </summary>
        [Required]
        public ServerModel server
        {
            get => _server;
            set => SetServerProperty(value);
        }
        [NotMapped]
        private ServerModel _server;

        /// <summary>
        /// The configuration received from our push server.
        /// </summary
        [Required]
        public PushAccountModel push
        {
            get => _push;
            set => SetPushProperty(value);
        }
        [NotMapped]
        private PushAccountModel _push;

        /// <summary>
        /// The presence priority within range -127 to 128 e.g. 0.
        /// </summary>
        [Required]
        public short presencePriorety
        {
            get => _presencePriorety;
            set => SetProperty(ref _presencePriorety, value);
        }
        [NotMapped]
        private short _presencePriorety;

        /// <summary>
        /// Has the account been enabled.
        /// Required for auto connecting accounts.
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
        /// Hex representation of the account color e.g. '#E91E63'.
        /// </summary>
        [Required]
        public string color
        {
            get => _color;
            set => SetProperty(ref _color, value);
        }
        [NotMapped]
        private string _color;

        /// <summary>
        /// The current account presence e.g. 'online'.
        /// </summary>
        [Required]
        public Presence presence
        {
            get => _presence;
            set => SetProperty(ref _presence, value);
        }
        [NotMapped]
        private Presence _presence;

        /// <summary>
        /// The optional account status message e.g. 'My status message!'.
        /// </summary>
        public string status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }
        [NotMapped]
        private string _status;

        /// <summary>
        /// Information about the XEP-0384 (OMEMO Encryption) account status.
        /// </summary>
        [Required]
        public OmemoAccountInformationModel omemoInfo
        {
            get => _omemoInfo;
            set => SetOmemoInfoProperty(value);
        }
        [NotMapped]
        private OmemoAccountInformationModel _omemoInfo;

        /// <summary>
        /// The status of the last time a MAM request happened.
        /// </summary>
        [Required]
        public MamRequestModel mamRequest
        {
            get => _mamRequest;
            set => SetMamRequestProperty(value);
        }
        [NotMapped]
        private MamRequestModel _mamRequest;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public AccountModel()
        {
            omemoInfo = new OmemoAccountInformationModel();
            mamRequest = new MamRequestModel();
        }

        public AccountModel(JidModel fullJid, string color) : this()
        {
            bareJid = fullJid.BareJid();
            this.fullJid = fullJid;
            this.color = color;
            presence = Presence.Online;
            omemoInfo.deviceListSubscription = new OmemoDeviceListSubscriptionModel(bareJid);
            server = new ServerModel(fullJid.domainPart);
            push = new PushAccountModel();
            enabled = true;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private void SetMamRequestProperty(MamRequestModel value)
        {
            MamRequestModel old = _mamRequest;
            if (SetProperty(ref _mamRequest, value, nameof(mamRequest)))
            {
                if (!(old is null))
                {
                    old.PropertyChanged -= OnMamRequestPropertyChanged;
                }
                if (!(value is null))
                {
                    value.PropertyChanged += OnMamRequestPropertyChanged;
                }
            }
        }

        private void SetOmemoInfoProperty(OmemoAccountInformationModel value)
        {
            OmemoAccountInformationModel old = _omemoInfo;
            if (SetProperty(ref _omemoInfo, value, nameof(omemoInfo)))
            {
                if (!(old is null))
                {
                    old.PropertyChanged -= OnOmemoInfoPropertyChanged;
                }
                if (!(value is null))
                {
                    value.PropertyChanged += OnOmemoInfoPropertyChanged;
                }
            }
        }

        private void SetServerProperty(ServerModel value)
        {
            ServerModel old = _server;
            if (SetProperty(ref _server, value, nameof(server)))
            {
                if (!(old is null))
                {
                    old.PropertyChanged -= OnServerPropertyChanged;
                }
                if (!(value is null))
                {
                    value.PropertyChanged += OnServerPropertyChanged;
                }
            }
        }

        private void SetPushProperty(PushAccountModel value)
        {
            PushAccountModel old = _push;
            if (SetProperty(ref _push, value, nameof(push)))
            {
                if (!(old is null))
                {
                    old.PropertyChanged -= OnPushPropertyChanged;
                }
                if (!(value is null))
                {
                    value.PropertyChanged += OnPushPropertyChanged;
                }
            }
        }

        private void SetFullJidProperty(JidModel value)
        {
            JidModel old = _fullJid;
            if (SetProperty(ref _fullJid, value, nameof(fullJid)))
            {
                if (!(old is null))
                {
                    old.PropertyChanged -= OnFullJidPropertyChanged;
                }
                if (!(value is null))
                {
                    value.PropertyChanged += OnFullJidPropertyChanged;
                }
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public XMPPAccount ToXMPPAccount()
        {
            XMPPAccount account = new XMPPAccount(new XMPPUser(fullJid.localPart, fullJid.domainPart, fullJid.resourcePart))
            {
                serverAddress = server.address,
                port = server.port,
                presencePriorety = presencePriorety,
                presence = presence,
                status = status,
                omemoDeviceId = omemoInfo.deviceId,
                omemoIdentityKey = omemoInfo.identityKey,
                omemoBundleInfoAnnounced = omemoInfo.bundleInfoAnnounced,
                omemoSignedPreKey = omemoInfo.signedPreKey,
                omemoDeviceLabel = omemoInfo.deviceLabel,
            };
            account.connectionConfiguration.IGNORED_CERTIFICATE_ERRORS.AddRange(server.ignoredCertificateErrors);
            account.connectionConfiguration.tlsMode = server.tlsMode;
            account.connectionConfiguration.disableMessageCarbons = server.disableMessageCarbons;
            account.connectionConfiguration.disableStreamManagement = server.disableStreamManagement;
            return account;
        }

        public override void Remove(MainDbContext ctx, bool recursive)
        {
            if (recursive)
            {
                fullJid?.Remove(ctx, recursive);
                server?.Remove(ctx, recursive);
                push?.Remove(ctx, recursive);
                omemoInfo?.Remove(ctx, recursive);
                mamRequest?.Remove(ctx, recursive);
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
        private void OnOmemoInfoPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(nameof(omemoInfo) + '.' + e.PropertyName);
        }

        private void OnMamRequestPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(nameof(mamRequest) + '.' + e.PropertyName);
        }

        private void OnServerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(nameof(server) + '.' + e.PropertyName);
        }

        private void OnPushPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(nameof(push) + '.' + e.PropertyName);
        }

        private void OnFullJidPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(nameof(fullJid) + '.' + e.PropertyName);
        }

        #endregion
    }
}
