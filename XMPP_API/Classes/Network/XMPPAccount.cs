using libsignal;
using libsignal.state;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        public IdentityKeyPair omemoIdentityKeyPair;
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
            string bas64IdentKeyPair = Convert.ToBase64String(omemoIdentityKeyPair.getPublicKey().serialize());
            string bas64SignedPreKey = Convert.ToBase64String(omemoSignedPreKeyPair.getKeyPair().getPublicKey().serialize());
            string bas64SignedPreKeySig = Convert.ToBase64String(omemoSignedPreKeyPair.getSignature());
            List<string> base64PreKeys = new List<string>();
            foreach (PreKeyRecord key in omemoPreKeys)
            {
                base64PreKeys.Add(Convert.ToBase64String(key.serialize()));
            }

            return new OmemoBundleInformation(bas64IdentKeyPair, bas64SignedPreKey, bas64SignedPreKeySig, base64PreKeys);
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        /// <summary>
        /// Generates a new omemoIdentityKeyPair, omemoSignedPreKeyPair, omemoPreKeys.
        /// Sets omemoDeviceId to 0.
        /// Sets omemoBundleInfoAnnounced to false.
        /// </summary>
        public void generateOmemoKeys()
        {
            omemoDeviceId = 0;
            omemoBundleInfoAnnounced = false;
            omemoIdentityKeyPair = OmemoUtils.generateIdentityKeyPair();
            omemoPreKeys = OmemoUtils.generatePreKeys();
            omemoSignedPreKeyPair = OmemoUtils.generateSignedPreKey(omemoIdentityKeyPair);
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
