using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DnsClient.Protocol;
using libsignal;
using libsignal.ecc;
using libsignal.state;
using libsignal.util;
using Logging;
using Shared.Classes;
using Shared.Classes.Collections;
using XMPP_API.Classes.Crypto;
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
            get { return _port; }
            set { SetProperty(ref _port, value); }
        }
        private XMPPUser _user;
        public XMPPUser user
        {
            get { return _user; }
            set { setXMPPUserProperty(value); }
        }
        private string _serverAddress;
        public string serverAddress
        {
            get { return _serverAddress; }
            set { SetProperty(ref _serverAddress, value); }
        }
        private int _presencePriorety;
        public int presencePriorety
        {
            get { return _presencePriorety; }
            set { SetProperty(ref _presencePriorety, value); }
        }
        private bool _disabled;
        public bool disabled
        {
            get { return _disabled; }
            set { SetProperty(ref _disabled, value); }
        }
        private string _color;
        public string color
        {
            get { return _color; }
            set { SetProperty(ref _color, value); }
        }
        private Presence _presence;
        public Presence presence
        {
            get { return _presence; }
            set { SetProperty(ref _presence, value); }
        }
        private string _status;
        public string status
        {
            get { return _status; }
            set { SetProperty(ref _status, value); }
        }
        private ConnectionConfiguration _connectionConfiguration;
        public ConnectionConfiguration connectionConfiguration
        {
            get { return _connectionConfiguration; }
            set { setConnectionConfigurationProperty(value); }
        }

        public readonly ConnectionInformation CONNECTION_INFO;

        // XEP-0384 (OMEMO Encryption):
        private bool _omemoKeysGenerated;
        public bool omemoKeysGenerated
        {
            get { return _omemoKeysGenerated; }
            set { SetProperty(ref _omemoKeysGenerated, value); }
        }
        private IdentityKeyPair _omemoIdentityKeyPair;
        public IdentityKeyPair omemoIdentityKeyPair
        {
            get { return _omemoIdentityKeyPair; }
            set { SetProperty(ref _omemoIdentityKeyPair, value); }
        }
        private uint _omemoSignedPreKeyId;
        public uint omemoSignedPreKeyId
        {
            get { return _omemoSignedPreKeyId; }
            set { SetProperty(ref _omemoSignedPreKeyId, value); }
        }
        private SignedPreKeyRecord _omemoSignedPreKeyPair;
        public SignedPreKeyRecord omemoSignedPreKeyPair
        {
            get { return _omemoSignedPreKeyPair; }
            set { SetProperty(ref _omemoSignedPreKeyPair, value); }
        }
        private uint _omemoDeviceId;
        public uint omemoDeviceId
        {
            get { return _omemoDeviceId; }
            set { SetProperty(ref _omemoDeviceId, value); }
        }
        private bool _omemoBundleInfoAnnounced;
        public bool omemoBundleInfoAnnounced
        {
            get { return _omemoBundleInfoAnnounced; }
            set { SetProperty(ref _omemoBundleInfoAnnounced, value); }
        }
        public readonly CustomObservableCollection<PreKeyRecord> OMEMO_PRE_KEYS;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 17/08/2017 Created [Fabian Sauter]
        /// </history>
        public XMPPAccount(XMPPUser user) : this(user, user?.domainPart ?? "", 5222, new ConnectionConfiguration())
        {
        }

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
            omemoIdentityKeyPair = null;
            omemoSignedPreKeyPair = null;
            OMEMO_PRE_KEYS = new CustomObservableCollection<PreKeyRecord>(false);
            OMEMO_PRE_KEYS.CollectionChanged += OMEMO_PRE_KEYS_CollectionChanged;
            omemoDeviceId = 0;
            omemoBundleInfoAnnounced = false;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public bool checkOmemoKeys()
        {
            return omemoKeysGenerated && !(omemoIdentityKeyPair is null || OMEMO_PRE_KEYS.Count <= 0 || omemoSignedPreKeyPair is null);
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
            List<Tuple<uint, ECPublicKey>> pubPreKeys = new List<Tuple<uint, ECPublicKey>>();
            foreach (PreKeyRecord key in OMEMO_PRE_KEYS)
            {
                pubPreKeys.Add(new Tuple<uint, ECPublicKey>(key.getId(), key.getKeyPair().getPublicKey()));
            }
            return new OmemoBundleInformation(omemoIdentityKeyPair.getPublicKey(), omemoSignedPreKeyPair.getKeyPair().getPublicKey(), omemoSignedPreKeyId, omemoSignedPreKeyPair.getSignature(), pubPreKeys);
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
                ;
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        /// <summary>
        /// Loads all OMEMO keys from the given store.
        /// </summary>
        /// <param name="omemoStore">A persistent store for all the OMEMO related data (e.g. device ids and keys).</param>
        /// <returns>Returns true on success.</returns>
        public bool loadOmemoKeys(IOmemoStore omemoStore)
        {
            if (!omemoKeysGenerated)
            {
                Logger.Error("Failed to load OMEMO keys for: " + getBareJid() + " - run generateOmemoKeys() first!");
                return false;
            }

            OMEMO_PRE_KEYS.Clear();
            OMEMO_PRE_KEYS.AddRange(omemoStore.LoadPreKeys());
            if (OMEMO_PRE_KEYS.Count <= 0)
            {
                Logger.Error("Failed to load OMEMO prekeys for: " + getBareJid());
                return false;
            }

            omemoSignedPreKeyPair = omemoStore.LoadSignedPreKey(omemoSignedPreKeyId);
            if (omemoSignedPreKeyPair is null)
            {
                Logger.Error("Failed to load OMEMO signed prekey pair for: " + getBareJid());
                return false;
            }

            Logger.Info("Successfully loaded OMEMO keys for: " + getBareJid());
            return true;
        }

        /// <summary>
        /// Stores all OMEMO keys in the given store.
        /// </summary>
        /// <param name="omemoStore">A persistent store for all the OMEMO related data (e.g. device ids and keys).</param>
        /// <returns>Returns true on success.</returns>
        public bool storeOmemoKeys(IOmemoStore omemoStore)
        {
            if (!checkOmemoKeys())
            {
                Logger.Error("Failed to save OMEMO keys for: " + getBareJid());
                return false;
            }
            omemoStore.StoreSignedPreKey(omemoSignedPreKeyId, omemoSignedPreKeyPair);
            omemoStore.StorePreKeys(OMEMO_PRE_KEYS);
            return true;
        }

        /// <summary>
        /// Generates a new omemoIdentityKeyPair, omemoSignedPreKeyPair, omemoPreKeys.
        /// Sets omemoDeviceId to 0.
        /// Sets omemoBundleInfoAnnounced to false.
        /// Sets omemoKeysGenerated to true.
        /// </summary>
        public void generateOmemoKeys()
        {
            omemoDeviceId = 0;
            omemoBundleInfoAnnounced = false;
            omemoIdentityKeyPair = CryptoUtils.generateOmemoIdentityKeyPair();
            OMEMO_PRE_KEYS.Clear();
            OMEMO_PRE_KEYS.AddRange(CryptoUtils.generateOmemoPreKeys());
            omemoSignedPreKeyPair = CryptoUtils.generateOmemoSignedPreKey(omemoIdentityKeyPair);
            omemoSignedPreKeyId = omemoSignedPreKeyPair.getId();
            omemoKeysGenerated = true;
        }

        public void replaceOmemoPreKey(uint preKeyId, IOmemoStore omemoStore)
        {
            // Remove key:
            foreach (PreKeyRecord key in OMEMO_PRE_KEYS)
            {
                if (key.getId() == preKeyId)
                {
                    OMEMO_PRE_KEYS.Remove(key);
                    omemoStore.RemovePreKey(preKeyId);
                    break;
                }
            }

            // Generate new key:
            PreKeyRecord newKey = KeyHelper.generatePreKeys(preKeyId, 1)[0];
            OMEMO_PRE_KEYS.Add(newKey);
            omemoStore.StorePreKey(newKey.getId(), newKey);
            omemoBundleInfoAnnounced = false;
        }

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
                    o.omemoKeysGenerated == omemoKeysGenerated &&
                    Equals(o.omemoIdentityKeyPair.serialize(), omemoIdentityKeyPair.serialize()) &&
                    Equals(o.omemoSignedPreKeyPair.serialize(), omemoSignedPreKeyPair.serialize()) &&
                    Equals(o.OMEMO_PRE_KEYS, OMEMO_PRE_KEYS) &&
                    o.omemoBundleInfoAnnounced == omemoBundleInfoAnnounced;
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
            List<SrvRecord> records = await TCPConnection2.dnsSrvLookupAsync(user.domainPart);
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
