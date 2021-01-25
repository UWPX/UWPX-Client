using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DnsClient.Protocol;
using Logging;
using Omemo.Classes.Keys;
using Shared.Classes;
using Shared.Classes.Collections;
using XMPP_API.Classes.Network.TCP;
using XMPP_API.Classes.Network.XML.Messages.XEP_0384;

namespace XMPP_API.Classes.Network
{
    public class XMPPAccount: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private int _port;
        public int port
        {
            get => _port;
            set => SetProperty(ref _port, value);
        }
        private XMPPUser _user;
        public XMPPUser user
        {
            get => _user;
            set => setXMPPUserProperty(value);
        }
        private string _serverAddress;
        public string serverAddress
        {
            get => _serverAddress;
            set => SetProperty(ref _serverAddress, value);
        }
        private short _presencePriorety;
        public short presencePriorety
        {
            get => _presencePriorety;
            set => SetProperty(ref _presencePriorety, value);
        }
        private bool _disabled;
        public bool disabled
        {
            get => _disabled;
            set => SetProperty(ref _disabled, value);
        }
        private string _color;
        public string color
        {
            get => _color;
            set => SetProperty(ref _color, value);
        }
        private Presence _presence;
        public Presence presence
        {
            get => _presence;
            set => SetProperty(ref _presence, value);
        }
        private string _status;
        public string status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }
        private ConnectionConfiguration _connectionConfiguration;
        public ConnectionConfiguration connectionConfiguration
        {
            get => _connectionConfiguration;
            set => setConnectionConfigurationProperty(value);
        }

        public readonly ConnectionInformation CONNECTION_INFO;

        // XEP-0384 (OMEMO Encryption):
        private bool _omemoKeysGenerated;
        public bool omemoKeysGenerated
        {
            get => _omemoKeysGenerated;
            set => SetProperty(ref _omemoKeysGenerated, value);
        }
        private IdentityKeyPairModel _omemoIdentityKey;
        public IdentityKeyPairModel omemoIdentityKey
        {
            get => _omemoIdentityKey;
            set => SetProperty(ref _omemoIdentityKey, value);
        }
        private SignedPreKeyModel _omemoSignedPreKey;
        public SignedPreKeyModel omemoSignedPreKey
        {
            get => _omemoSignedPreKey;
            set => SetProperty(ref _omemoSignedPreKey, value);
        }
        private uint _omemoDeviceId;
        public uint omemoDeviceId
        {
            get => _omemoDeviceId;
            set => SetProperty(ref _omemoDeviceId, value);
        }
        private string _omemoDeviceLabel;
        public string omemoDeviceLabel
        {
            get => _omemoDeviceLabel;
            set => SetProperty(ref _omemoDeviceLabel, value);
        }
        private bool _omemoBundleInfoAnnounced;
        public bool omemoBundleInfoAnnounced
        {
            get => _omemoBundleInfoAnnounced;
            set => SetProperty(ref _omemoBundleInfoAnnounced, value);
        }
        public readonly CustomObservableCollection<PreKeyModel> OMEMO_PRE_KEYS;

        // XEP-0357 (Push Notifications):
        private string _pushNode;
        public string pushNode
        {
            get => _pushNode;
            set => SetProperty(ref _pushNode, value);
        }
        private string _pushNodeSecret;
        public string pushNodeSecret
        {
            get => _pushNodeSecret;
            set => SetProperty(ref _pushNodeSecret, value);
        }
        private string _pushServerBareJid;
        public string pushServerBareJid
        {
            get => _pushServerBareJid;
            set => SetProperty(ref _pushServerBareJid, value);
        }
        private bool _pushNodePublished;
        public bool pushNodePublished
        {
            get => _pushNodePublished;
            set => SetProperty(ref _pushNodePublished, value);
        }
        private bool _pushEnabled;
        public bool pushEnabled
        {
            get => _pushEnabled;
            set => SetProperty(ref _pushEnabled, value);
        }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public XMPPAccount(XMPPUser user) : this(user, user?.domainPart ?? "", 5222, new ConnectionConfiguration()) { }

        public XMPPAccount(XMPPUser user, string serverAddress, int port, ConnectionConfiguration connectionConfiguration)
        {
            invokeInUiThread = false;
            this.user = user;
            this.serverAddress = serverAddress;
            this.port = port;
            this.connectionConfiguration = connectionConfiguration;
            presencePriorety = 0;
            disabled = false;
            color = null;
            presence = Presence.Online;
            status = null;
            CONNECTION_INFO = new ConnectionInformation();
            CONNECTION_INFO.PropertyChanged += CONNECTION_INFO_PropertyChanged;
            omemoIdentityKey = null;
            omemoSignedPreKey = null;
            OMEMO_PRE_KEYS = new CustomObservableCollection<PreKeyModel>(false);
            OMEMO_PRE_KEYS.CollectionChanged += OMEMO_PRE_KEYS_CollectionChanged;
            omemoDeviceId = 0;
            omemoBundleInfoAnnounced = false;
            pushNode = null;
            pushNodeSecret = null;
            pushServerBareJid = null;
            pushEnabled = false;
            pushNodePublished = false;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public bool checkOmemoKeys()
        {
            return omemoKeysGenerated && !(omemoIdentityKey is null || OMEMO_PRE_KEYS.Count <= 0 || omemoSignedPreKey is null);
        }

        public string getBareJid()
        {
            return user.getBareJid();
        }

        public string getFullJid()
        {
            return user.getFullJid();
        }

        public OmemoBundleInformation getOmemoBundleInformation()
        {
            Bundle bundle = new Bundle()
            {
                identityKey = omemoIdentityKey.pubKey,
                preKeys = OMEMO_PRE_KEYS.Select(key => new PreKeyModel(null, key.pubKey, key.keyId)).ToList(),
                preKeySignature = omemoSignedPreKey.signature,
                signedPreKey = omemoSignedPreKey.preKey.pubKey,
                signedPreKeyId = omemoSignedPreKey.preKey.keyId
            };
            return new OmemoBundleInformation(bundle, omemoDeviceId);
        }

        private void setXMPPUserProperty(XMPPUser value)
        {
            if (!(user is null))
            {
                value.PropertyChanged -= XMPPUser_PropertyChanged;
            }
            SetProperty(ref _user, value, nameof(user));
            if (!(user is null))
            {
                value.PropertyChanged += XMPPUser_PropertyChanged;
            }
        }

        private void setConnectionConfigurationProperty(ConnectionConfiguration value)
        {
            if (!(connectionConfiguration is null))
            {
                connectionConfiguration.PropertyChanged -= ConnectionConfiguration_PropertyChanged;
            }
            SetProperty(ref _connectionConfiguration, value, nameof(connectionConfiguration));
            if (!(connectionConfiguration is null))
            {
                connectionConfiguration.PropertyChanged += ConnectionConfiguration_PropertyChanged;
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public override bool Equals(object obj)
        {
            if (obj is XMPPAccount)
            {
                XMPPAccount o = obj as XMPPAccount;
                return o.disabled == disabled &&
                    o.port == port &&
                    o.presencePriorety == presencePriorety &&
                    string.Equals(o.serverAddress, serverAddress) &&
                    Equals(o.user, user) &&
                    string.Equals(o.color, color) &&
                    o.presence == presence &&
                    string.Equals(o.status, status) &&
                    connectionConfiguration.Equals(o.connectionConfiguration) &&
                    o.omemoDeviceId == omemoDeviceId &&
                    string.Equals(o.omemoDeviceLabel, omemoDeviceLabel) &&
                    o.omemoKeysGenerated == omemoKeysGenerated &&
                    o.omemoBundleInfoAnnounced == omemoBundleInfoAnnounced &&
                    Equals(o.omemoIdentityKey, omemoIdentityKey) &&
                    Equals(o.omemoSignedPreKey, omemoSignedPreKey) &&
                    o.OMEMO_PRE_KEYS.SequenceEqual(OMEMO_PRE_KEYS) &&
                    string.Equals(o.pushNode, pushNode) &&
                    string.Equals(o.pushNodeSecret, pushNodeSecret) &&
                    string.Equals(o.pushServerBareJid, pushServerBareJid) &&
                    o.pushNodePublished == pushNodePublished &&
                    o.pushEnabled == pushEnabled;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Performs a DNS lookup and sets the first entry (highest priority).
        /// Updates <see cref="port"/> and <see cref="serverAddress"/>.
        /// </summary>
        /// <returns>Returns true if the request was successful and the new values differ from the old values.</returns>
        public async Task<bool> dnsSrvLookupAsync()
        {
            List<SrvRecord> records = await TcpConnection.DnsSrvLookupAsync(user.domainPart);
            if (records.Count <= 0)
            {
                return false;
            }

            SrvRecord record = records[0];
            string serverAddress = record.Target.Value;
            if (serverAddress.EndsWith("."))
            {
                serverAddress = serverAddress.Substring(0, serverAddress.Length - 1);
            }
            if ((ushort)port == record.Port && this.serverAddress.Equals(serverAddress))
            {
                return false;
            }
            else
            {
                port = record.Port;
                this.serverAddress = serverAddress;
                Logger.Info("Updated the port and server address for: " + user.domainPart + " to: " + this.serverAddress + ":" + port);
                return true;
            }
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void XMPPUser_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(user));
        }

        private void OMEMO_PRE_KEYS_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(OMEMO_PRE_KEYS));
        }

        private void CONNECTION_INFO_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(CONNECTION_INFO));
        }

        private void ConnectionConfiguration_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(connectionConfiguration));
        }

        #endregion
    }
}
