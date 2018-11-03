using libsignal;
using libsignal.ecc;
using libsignal.state;
using org.whispersystems.libsignal.fingerprint;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using XMPP_API.Classes.Crypto;
using XMPP_API.Classes.Network.XML.DBManager;
using XMPP_API.Classes.Network.XML.Messages.XEP_0384;
using XMPP_API.Classes.Network.XML.Messages.XEP_0384.Signal;

namespace XMPP_API.Classes.Network
{
    public class XMPPAccount : INotifyPropertyChanged
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public int port;
        public XMPPUser user;
        public string serverAddress;
        public int presencePriorety;
        public bool disabled;
        public string color;
        public Presence presence;
        public string status;
        public ConnectionConfiguration connectionConfiguration;
        public readonly ConnectionInformation CONNECTION_INFO;
        // XEP-0384 (OMEMO Encryption):
        public IdentityKeyPair omemoIdentityKeyPair;
        public uint omemoSignedPreKeyId;
        public SignedPreKeyRecord omemoSignedPreKeyPair;
        public IList<PreKeyRecord> omemoPreKeys;
        public uint omemoDeviceId;
        public bool omemoBundleInfoAnnounced;

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 17/08/2017 Created [Fabian Sauter]
        /// </history>
        public XMPPAccount(XMPPUser user, string serverAddress, int port) : this(user, serverAddress, port, new ConnectionConfiguration())
        {
        }

        public XMPPAccount(XMPPUser user, string serverAddress, int port, ConnectionConfiguration connectionConfiguration)
        {
            this.user = user;
            this.serverAddress = serverAddress;
            this.port = port;
            this.connectionConfiguration = connectionConfiguration;
            this.presencePriorety = 0;
            this.disabled = false;
            this.color = null;
            this.presence = Presence.Online;
            this.status = null;
            this.CONNECTION_INFO = new ConnectionInformation();
            this.omemoIdentityKeyPair = null;
            this.omemoSignedPreKeyPair = null;
            this.omemoPreKeys = null;
            this.omemoDeviceId = 0;
            this.omemoBundleInfoAnnounced = false;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public bool hasOmemoKeys()
        {
            return omemoIdentityKeyPair != null && omemoPreKeys != null && omemoSignedPreKeyPair != null;
        }

        public string getIdAndDomain()
        {
            return user.getIdAndDomain();
        }

        public string getIdDomainAndResource()
        {
            return user.getIdDomainAndResource();
        }

        public OmemoBundleInformation getOmemoBundleInformation()
        {
            List<Tuple<uint, ECPublicKey>> pubPreKeys = new List<Tuple<uint, ECPublicKey>>();
            foreach (PreKeyRecord key in omemoPreKeys)
            {
                pubPreKeys.Add(new Tuple<uint, ECPublicKey>(key.getId(), key.getKeyPair().getPublicKey()));
            }
            return new OmemoBundleInformation(omemoIdentityKeyPair.getPublicKey(), omemoSignedPreKeyPair.getKeyPair().getPublicKey(), omemoSignedPreKeyId, omemoSignedPreKeyPair.getSignature(), pubPreKeys);
        }

        public Fingerprint getOmemoFingerprint()
        {
            if (omemoIdentityKeyPair != null)
            {
                return CryptoUtils.generateOmemoFingerprint(getIdAndDomain(), omemoIdentityKeyPair.getPublicKey());
            }
            return null;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void loadPreKeys(ISignalKeyDBManager signalKeyDBManager)
        {
            omemoPreKeys = signalKeyDBManager.getAllPreKeys(getIdAndDomain());
        }

        public void loadSignedPreKey(ISignalKeyDBManager signalKeyDBManager)
        {
            omemoSignedPreKeyPair = signalKeyDBManager.getSignedPreKey(omemoSignedPreKeyId, getIdAndDomain());
        }

        public void savePreKeys(ISignalKeyDBManager signalKeyDBManager)
        {
            if (omemoPreKeys != null)
            {
                string accountId = getIdAndDomain();
                foreach (PreKeyRecord key in omemoPreKeys)
                {
                    signalKeyDBManager.setPreKey(key.getId(), key, accountId);
                }
            }
        }

        public void deleteAccountSignedPreKey(ISignalKeyDBManager signalKeyDBManager)
        {
            if (omemoSignedPreKeyPair != null)
            {
                signalKeyDBManager.deleteSignedPreKey(omemoSignedPreKeyPair.getId(), getIdAndDomain());
            }
        }

        public void deleteAccountPreKeys(ISignalKeyDBManager signalKeyDBManager)
        {
            if (omemoPreKeys != null)
            {
                string accountId = getIdAndDomain();
                foreach (PreKeyRecord key in omemoPreKeys)
                {
                    signalKeyDBManager.deleteSignedPreKey(key.getId(), accountId);
                }
            }
        }

        public void saveSignedPreKey(ISignalKeyDBManager signalKeyDBManager)
        {
            if (omemoSignedPreKeyPair != null)
            {
                signalKeyDBManager.setSignedPreKey(omemoSignedPreKeyId, omemoSignedPreKeyPair, getIdAndDomain());
            }
        }

        public void deleteOmemoKeysAndDevices(ISignalKeyDBManager signalKeyDBManager)
        {
            signalKeyDBManager.deleteAllForAccount(getIdAndDomain());
            OmemoDeviceDBManager.INSTANCE.deleteAllForAccount(getIdAndDomain());
        }

        /// <summary>
        /// Generates a new omemoIdentityKeyPair, omemoSignedPreKeyPair, omemoPreKeys.
        /// Sets omemoDeviceId to 0.
        /// Sets omemoBundleInfoAnnounced to false.
        /// </summary>
        public void generateOmemoKeys()
        {
            omemoDeviceId = 0;
            omemoBundleInfoAnnounced = false;
            omemoIdentityKeyPair = CryptoUtils.generateOmemoIdentityKeyPair();
            omemoPreKeys = CryptoUtils.generateOmemoPreKeys();
            omemoSignedPreKeyPair = CryptoUtils.generateOmemoSignedPreKey(omemoIdentityKeyPair);
            omemoSignedPreKeyId = omemoSignedPreKeyPair.getId();
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
                    Equals(o.omemoIdentityKeyPair.serialize(), omemoIdentityKeyPair.serialize()) &&
                    Equals(o.omemoSignedPreKeyPair.serialize(), omemoSignedPreKeyPair.serialize()) &&
                    Equals(o.omemoPreKeys, omemoPreKeys) &&
                    o.omemoBundleInfoAnnounced == omemoBundleInfoAnnounced;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Called once an important property has changed and the account should get written back to the DB.
        /// </summary>
        /// <param name="name">The name of the property that changed.</param>
        public void onPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
