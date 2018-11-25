using libsignal;
using libsignal.ecc;
using libsignal.state;
using libsignal.util;
using Logging;
using org.whispersystems.libsignal.fingerprint;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using XMPP_API.Classes.Crypto;
using XMPP_API.Classes.Network.XML.Messages.XEP_0384;

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
        public bool omemoKeysGenerated;
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
        public bool checkOmemoKeys()
        {
            return omemoKeysGenerated && omemoIdentityKeyPair != null && omemoPreKeys != null && omemoSignedPreKeyPair != null;
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
        /// <summary>
        /// Loads all OMEMO keys from the given store.
        /// </summary>
        /// <param name="omemoStore">A persistent store for all the OMEMO related data (e.g. device ids and keys).</param>
        /// <returns>Returns true on success.</returns>
        public bool loadOmemoKeys(IOmemoStore omemoStore)
        {
            if (!omemoKeysGenerated)
            {
                Logger.Error("Failed to load OMEMO keys for: " + getIdAndDomain() + " - run generateOmemoKeys() first!");
                return false;
            }

            omemoPreKeys = omemoStore.LoadPreKeys();
            if (omemoPreKeys is null || omemoPreKeys.Count <= 0)
            {
                Logger.Error("Failed to load OMEMO prekeys for: " + getIdAndDomain());
                return false;
            }

            omemoSignedPreKeyPair = omemoStore.LoadSignedPreKey(omemoSignedPreKeyId);
            if (omemoSignedPreKeyPair is null)
            {
                Logger.Error("Failed to load OMEMO signed prekey pair for: " + getIdAndDomain());
                return false;
            }

            Logger.Info("Successfully loaded OMEMO keys for: " + getIdAndDomain());
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
                Logger.Error("Failed to save OMEMO keys for: " + getIdAndDomain());
                return false;
            }
            omemoStore.StoreSignedPreKey(omemoSignedPreKeyId, omemoSignedPreKeyPair);
            omemoStore.StorePreKeys(omemoPreKeys);
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
            omemoPreKeys = CryptoUtils.generateOmemoPreKeys();
            omemoSignedPreKeyPair = CryptoUtils.generateOmemoSignedPreKey(omemoIdentityKeyPair);
            omemoSignedPreKeyId = omemoSignedPreKeyPair.getId();
            omemoKeysGenerated = true;
        }

        public void replaceOmemoPreKey(uint preKeyId, IOmemoStore omemoStore)
        {
            // Remove key:
            foreach (PreKeyRecord key in omemoPreKeys)
            {
                if(key.getId() == preKeyId)
                {
                    omemoPreKeys.Remove(key);
                    omemoStore.RemovePreKey(preKeyId);
                    break;
                }
            }

            // Generate new key:
            PreKeyRecord newKey = KeyHelper.generatePreKeys(preKeyId, 1)[0];
            omemoPreKeys.Add(newKey);
            omemoStore.StorePreKey(newKey.getId(), newKey);
            omemoBundleInfoAnnounced = false;
            onPropertyChanged(nameof(omemoBundleInfoAnnounced));
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
