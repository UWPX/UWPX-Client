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
        private ushort _port;
        public ushort port
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

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public XMPPAccount(XMPPUser user) : this(user, user?.domainPart ?? "", 5222, new ConnectionConfiguration()) { }

        public XMPPAccount(XMPPUser user, string serverAddress, ushort port, ConnectionConfiguration connectionConfiguration)
        {
            invokeInUiThread = false;
            this.user = user;
            this.serverAddress = serverAddress;
            this.port = port;
            this.connectionConfiguration = connectionConfiguration;
            presencePriorety = 0;
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
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
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
                return o.port == port &&
                    o.presencePriorety == presencePriorety &&
                    string.Equals(o.serverAddress, serverAddress) &&
                    Equals(o.user, user) &&
                    o.presence == presence &&
                    string.Equals(o.status, status) &&
                    connectionConfiguration.Equals(o.connectionConfiguration) &&
                    o.omemoDeviceId == omemoDeviceId &&
                    string.Equals(o.omemoDeviceLabel, omemoDeviceLabel) &&
                    o.omemoBundleInfoAnnounced == omemoBundleInfoAnnounced &&
                    Equals(o.omemoIdentityKey, omemoIdentityKey) &&
                    Equals(o.omemoSignedPreKey, omemoSignedPreKey) &&
                    o.OMEMO_PRE_KEYS.SequenceEqual(OMEMO_PRE_KEYS);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Performs a DNS lookup for the XMPP SRV record and returns the result of the first (highest priority) entry.
        /// </summary>
        /// <param name="host">The address to perform the SRV lookup for.</param>
        public static async Task<SRVLookupResult> dnsSrvLookupAsync(string host)
        {
            List<SrvRecord> records = await TcpConnection.DnsSrvLookupAsync(host);
            if (records.Count <= 0)
            {
                return new SRVLookupResult();
            }

            SrvRecord record = records[0];
            string serverAddress = record.Target.Value;
            if (serverAddress.EndsWith("."))
            {
                serverAddress = serverAddress.Substring(0, serverAddress.Length - 1);
            }
            Logger.Info("Updated the port and server address for: " + host + " to: " + serverAddress + ":" + record.Port);
            return new SRVLookupResult(serverAddress, record.Port);
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
