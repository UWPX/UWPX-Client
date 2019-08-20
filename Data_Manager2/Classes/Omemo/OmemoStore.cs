using System;
using System.Collections.Generic;
using Data_Manager2.Classes.DBManager;
using Data_Manager2.Classes.DBManager.Omemo;
using Data_Manager2.Classes.DBTables;
using libsignal;
using libsignal.protocol;
using libsignal.state;
using Logging;
using XMPP_API.Classes.Network;
using XMPP_API.Classes.Network.XML.Messages.XEP_0384;

namespace Data_Manager2.Classes.Omemo
{
    public class OmemoStore: IOmemoStore
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
            ACCOUNT = account;
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

        /// <summary>
        /// DO NOT USE!
        /// Will always return true.
        /// ---
        /// XEP-0384 (OMEMO Encryption) recommends to disable trust management provided by the signal library.
        /// Source: https://xmpp.org/extensions/xep-0384.html#impl
        /// ---
        /// Use <see cref="IsFingerprintTrusted(OmemoFingerprint)"/> instead.
        /// </summary>
        /// <returns>Always true.</returns>
        [Obsolete]
        public bool IsTrustedIdentity(string name, IdentityKey identityKey)
        {
            return true;
        }

        public List<uint> GetSubDeviceSessions(string name)
        {
            return OmemoSignalKeyDBManager.INSTANCE.getDeviceIds(name, ACCOUNT.getBareJid());
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public bool SaveIdentity(string name, IdentityKey identityKey)
        {
            bool contains = OmemoSignalKeyDBManager.INSTANCE.containsIdentityKey(name, ACCOUNT.getBareJid());
            OmemoSignalKeyDBManager.INSTANCE.setIdentityKey(name, identityKey, ACCOUNT.getBareJid());
            return contains;
        }

        public bool ContainsPreKey(uint preKeyId)
        {
            return OmemoSignalKeyDBManager.INSTANCE.containsPreKeyRecord(preKeyId, ACCOUNT.getBareJid());
        }

        public PreKeyRecord LoadPreKey(uint preKeyId)
        {
            PreKeyRecord preKeyRecord = OmemoSignalKeyDBManager.INSTANCE.getPreKeyRecord(preKeyId, ACCOUNT.getBareJid());
            if (preKeyRecord is null)
            {
                throw new InvalidKeyIdException("No such key: " + preKeyId);
            }
            return preKeyRecord;
        }

        public void RemovePreKey(uint preKeyId)
        {
            OmemoSignalKeyDBManager.INSTANCE.deletePreKey(preKeyId, ACCOUNT.getBareJid());
        }

        public void StorePreKey(uint preKeyId, PreKeyRecord preKey)
        {
            OmemoSignalKeyDBManager.INSTANCE.setPreKey(preKeyId, preKey, ACCOUNT.getBareJid());
        }

        public bool ContainsSession(SignalProtocolAddress address)
        {
            SessionRecord session = OmemoSignalKeyDBManager.INSTANCE.getSession(address, ACCOUNT.getBareJid());
            return session != null && session.getSessionState().hasSenderChain() && session.getSessionState().getSessionVersion() == CiphertextMessage.CURRENT_VERSION;
        }

        public void DeleteAllSessions(string name)
        {
            OmemoSignalKeyDBManager.INSTANCE.deleteSessions(name);
        }

        public void DeleteSession(SignalProtocolAddress address)
        {
            OmemoSignalKeyDBManager.INSTANCE.deleteSession(address, ACCOUNT.getBareJid());
        }

        public SessionRecord LoadSession(SignalProtocolAddress address)
        {
            SessionRecord session = OmemoSignalKeyDBManager.INSTANCE.getSession(address, ACCOUNT.getBareJid());
            if (session is null)
            {
                Logger.Warn("No existing libsignal session found for: " + address.ToString());
                session = new SessionRecord();
            }
            return session;
        }

        public void StoreSession(SignalProtocolAddress address, SessionRecord record)
        {
            OmemoSignalKeyDBManager.INSTANCE.setSession(address, record, ACCOUNT.getBareJid());
        }

        public bool ContainsSignedPreKey(uint signedPreKeyId)
        {
            return OmemoSignalKeyDBManager.INSTANCE.containsSignedPreKey(signedPreKeyId, ACCOUNT.getBareJid());
        }

        public SignedPreKeyRecord LoadSignedPreKey(uint signedPreKeyId)
        {
            SignedPreKeyRecord signedPreKeyRecord = OmemoSignalKeyDBManager.INSTANCE.getSignedPreKey(signedPreKeyId, ACCOUNT.getBareJid());
            if (signedPreKeyRecord is null)
            {
                Logger.Error("OmemoStore: No such signed pre key: " + signedPreKeyId);
            }
            return signedPreKeyRecord;
        }

        public List<SignedPreKeyRecord> LoadSignedPreKeys()
        {
            return OmemoSignalKeyDBManager.INSTANCE.getAllSignedPreKeys(ACCOUNT.getBareJid());
        }

        public void RemoveSignedPreKey(uint signedPreKeyId)
        {
            OmemoSignalKeyDBManager.INSTANCE.deleteSignedPreKey(signedPreKeyId, ACCOUNT.getBareJid());
        }

        public void StoreSignedPreKey(uint signedPreKeyId, SignedPreKeyRecord signedPreKey)
        {
            OmemoSignalKeyDBManager.INSTANCE.setSignedPreKey(signedPreKeyId, signedPreKey, ACCOUNT.getBareJid());
        }

        public IList<PreKeyRecord> LoadPreKeys()
        {
            return OmemoSignalKeyDBManager.INSTANCE.getAllPreKeys(ACCOUNT.getBareJid());
        }

        public void StorePreKeys(IList<PreKeyRecord> preKeys)
        {
            OmemoSignalKeyDBManager.INSTANCE.setPreKeys(preKeys, ACCOUNT.getBareJid());
        }

        public void StoreDevices(IList<SignalProtocolAddress> devices)
        {
            OmemoDeviceDBManager.INSTANCE.setDevices(devices, ACCOUNT.getBareJid());
        }

        public void StoreDevice(SignalProtocolAddress device)
        {
            OmemoDeviceDBManager.INSTANCE.setDevice(device, ACCOUNT.getBareJid());
        }

        public IList<SignalProtocolAddress> LoadDevices(string name)
        {
            return OmemoDeviceDBManager.INSTANCE.getDevices(name, ACCOUNT.getBareJid());
        }

        public void StoreDeviceListSubscription(string name, Tuple<OmemoDeviceListSubscriptionState, DateTime> lastUpdate)
        {
            OmemoDeviceDBManager.INSTANCE.setOmemoDeviceListSubscription(name, lastUpdate, ACCOUNT.getBareJid());
        }

        public Tuple<OmemoDeviceListSubscriptionState, DateTime> LoadDeviceListSubscription(string name)
        {
            return OmemoDeviceDBManager.INSTANCE.getOmemoDeviceListSubscription(name, ACCOUNT.getBareJid());
        }

        /// <summary>
        /// Checks if the fingerprint is trusted by the following mechanism:
        /// 1. Does a chat exist for the fingerprint? Yes -> 2. No. -> false
        /// 2. Is "trusted keys only" activated for chats? Yes -> 3. No -> true
        /// 3. Is the key trusted? Yes -> true No -> false
        /// </summary>
        /// <param name="fingerprint">The fingerprint we want to check if it's valid.</param>
        public bool IsFingerprintTrusted(OmemoFingerprint fingerprint)
        {
            string chatId = ChatTable.generateId(fingerprint.ADDRESS.getName(), ACCOUNT.getBareJid());
            ChatTable chat = ChatDBManager.INSTANCE.getChat(chatId);
            return !(chat is null) && (!chat.omemoTrustedKeysOnly || fingerprint.trusted);
        }

        public void StoreFingerprint(OmemoFingerprint fingerprint)
        {
            OmemoSignalKeyDBManager.INSTANCE.setFingerprint(fingerprint, ACCOUNT.getBareJid());
        }

        public OmemoFingerprint LoadFingerprint(SignalProtocolAddress address)
        {
            return OmemoSignalKeyDBManager.INSTANCE.getFingerprint(address, ACCOUNT.getBareJid());
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
