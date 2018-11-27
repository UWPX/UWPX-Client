using Data_Manager2.Classes.DBManager.Omemo;
using libsignal;
using libsignal.protocol;
using libsignal.state;
using Logging;
using System;
using System.Collections.Generic;
using XMPP_API.Classes.Network;
using XMPP_API.Classes.Network.XML.Messages.XEP_0384;

namespace Data_Manager2.Classes.Omemo
{
    public class OmemoStore : IOmemoStore
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly XMPPAccount ACCOUNT;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 09/11/2018 Created [Fabian Sauter]
        /// </history>
        public OmemoStore(XMPPAccount account)
        {
            this.ACCOUNT = account;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public IdentityKeyPair GetIdentityKeyPair()
        {
            return ACCOUNT.omemoIdentityKeyPair;
        }

        public uint GetLocalRegistrationId()
        {
            return ACCOUNT.omemoDeviceId;
        }

        public bool IsTrustedIdentity(string name, IdentityKey identityKey)
        {
            // XEP-0384 (OMEMO Encryption) recommends to disable trust management provided by the signal library:
            // Source: https://xmpp.org/extensions/xep-0384.html#impl
            return true;
        }

        public List<uint> GetSubDeviceSessions(string name)
        {
            return OmemoSignalKeyDBManager.INSTANCE.getDeviceIds(name, ACCOUNT.getIdAndDomain());
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public bool SaveIdentity(string name, IdentityKey identityKey)
        {
            bool contains = OmemoSignalKeyDBManager.INSTANCE.containsIdentityKey(name, ACCOUNT.getIdAndDomain());
            OmemoSignalKeyDBManager.INSTANCE.setIdentityKey(name, identityKey, ACCOUNT.getIdAndDomain());
            return contains;
        }

        public bool ContainsPreKey(uint preKeyId)
        {
            return OmemoSignalKeyDBManager.INSTANCE.containsPreKeyRecord(preKeyId, ACCOUNT.getIdAndDomain());
        }

        public PreKeyRecord LoadPreKey(uint preKeyId)
        {
            PreKeyRecord preKeyRecord = OmemoSignalKeyDBManager.INSTANCE.getPreKeyRecord(preKeyId, ACCOUNT.getIdAndDomain());
            if (preKeyRecord is null)
            {
                throw new InvalidKeyIdException("No such key: " + preKeyId);
            }
            return preKeyRecord;
        }

        public void RemovePreKey(uint preKeyId)
        {
            OmemoSignalKeyDBManager.INSTANCE.deletePreKey(preKeyId, ACCOUNT.getIdAndDomain());
        }

        public void StorePreKey(uint preKeyId, PreKeyRecord preKey)
        {
            OmemoSignalKeyDBManager.INSTANCE.setPreKey(preKeyId, preKey, ACCOUNT.getIdAndDomain());
        }

        public bool ContainsSession(SignalProtocolAddress address)
        {
            SessionRecord session = OmemoSignalKeyDBManager.INSTANCE.getSession(address, ACCOUNT.getIdAndDomain());
            return session != null && session.getSessionState().hasSenderChain() && session.getSessionState().getSessionVersion() == CiphertextMessage.CURRENT_VERSION;
        }

        public void DeleteAllSessions(string name)
        {
            OmemoSignalKeyDBManager.INSTANCE.deleteSessions(name);
        }

        public void DeleteSession(SignalProtocolAddress address)
        {
            OmemoSignalKeyDBManager.INSTANCE.deleteSession(address, ACCOUNT.getIdAndDomain());
        }

        public SessionRecord LoadSession(SignalProtocolAddress address)
        {
            SessionRecord session = OmemoSignalKeyDBManager.INSTANCE.getSession(address, ACCOUNT.getIdAndDomain());
            if (session is null)
            {
                Logger.Warn("No existing libsignal session found for: " + address.ToString());
                session = new SessionRecord();
            }
            return session;
        }

        public void StoreSession(SignalProtocolAddress address, SessionRecord record)
        {
            OmemoSignalKeyDBManager.INSTANCE.setSession(address, record, ACCOUNT.getIdAndDomain());
        }

        public bool ContainsSignedPreKey(uint signedPreKeyId)
        {
            return OmemoSignalKeyDBManager.INSTANCE.containsSignedPreKey(signedPreKeyId, ACCOUNT.getIdAndDomain());
        }

        public SignedPreKeyRecord LoadSignedPreKey(uint signedPreKeyId)
        {
            SignedPreKeyRecord signedPreKeyRecord = OmemoSignalKeyDBManager.INSTANCE.getSignedPreKey(signedPreKeyId, ACCOUNT.getIdAndDomain());
            if (signedPreKeyRecord is null)
            {
                throw new InvalidKeyIdException("No such key: " + signedPreKeyId);
            }
            return signedPreKeyRecord;
        }

        public List<SignedPreKeyRecord> LoadSignedPreKeys()
        {
            return OmemoSignalKeyDBManager.INSTANCE.getAllSignedPreKeys(ACCOUNT.getIdAndDomain());
        }

        public void RemoveSignedPreKey(uint signedPreKeyId)
        {
            OmemoSignalKeyDBManager.INSTANCE.deleteSignedPreKey(signedPreKeyId, ACCOUNT.getIdAndDomain());
        }

        public void StoreSignedPreKey(uint signedPreKeyId, SignedPreKeyRecord signedPreKey)
        {
            OmemoSignalKeyDBManager.INSTANCE.setSignedPreKey(signedPreKeyId, signedPreKey, ACCOUNT.getIdAndDomain());
        }

        public IList<PreKeyRecord> LoadPreKeys()
        {
            return OmemoSignalKeyDBManager.INSTANCE.getAllPreKeys(ACCOUNT.getIdAndDomain());
        }

        public void StorePreKeys(IList<PreKeyRecord> preKeys)
        {
            OmemoSignalKeyDBManager.INSTANCE.setPreKeys(preKeys, ACCOUNT.getIdAndDomain());
        }

        public void StoreDevices(IList<SignalProtocolAddress> devices)
        {
            OmemoDeviceDBManager.INSTANCE.setDevices(devices, ACCOUNT.getIdAndDomain());
        }

        public void StoreDevice(SignalProtocolAddress device)
        {
            OmemoDeviceDBManager.INSTANCE.setDevice(device, ACCOUNT.getIdAndDomain());
        }

        public IList<SignalProtocolAddress> LoadDevices(string name)
        {
            return OmemoDeviceDBManager.INSTANCE.getDevices(name, ACCOUNT.getIdAndDomain());
        }

        public void StoreDeviceListSubscription(string name, Tuple<OmemoDeviceListSubscriptionState, DateTime> lastUpdate)
        {
            OmemoDeviceDBManager.INSTANCE.setOmemoDeviceListSubscription(name, lastUpdate, ACCOUNT.getIdAndDomain());
        }

        public Tuple<OmemoDeviceListSubscriptionState, DateTime> LoadDeviceListSubscription(string name)
        {
            return OmemoDeviceDBManager.INSTANCE.getOmemoDeviceListSubscription(name, ACCOUNT.getIdAndDomain());
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
