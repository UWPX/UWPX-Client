using System;
using System.Collections.Generic;
using System.Linq;
using libsignal;
using libsignal.ecc;
using libsignal.protocol;
using libsignal.state;
using Logging;
using Manager.Classes.Chat;
using Storage.Classes.Contexts;
using Storage.Classes.Models.Account;
using Storage.Classes.Models.Omemo;
using XMPP_API.Classes.Network.XML.Messages.XEP_0384;

namespace Manager.Classes.Client
{
    public class OmemoStore: IOmemoStore
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly Account dbAccount;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public OmemoStore(Account dbAccount)
        {
            this.dbAccount = dbAccount;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public bool ContainsPreKey(uint preKeyId)
        {
            return dbAccount.omemoInfo.preKeys.Any(key => key.keyId == preKeyId);
        }

        public bool ContainsSession(SignalProtocolAddress address)
        {
            List<OmemoDevice> devices = GetDevices(address.getName());
            return devices.Any(d =>
            {
                if (d.session is null)
                {
                    return false;
                }
                SessionRecord session = new SessionRecord(d.session.session);
                return session.getSessionState().hasSenderChain() && session.getSessionState().getSessionVersion() == CiphertextMessage.CURRENT_VERSION;
            });
        }

        public bool ContainsSignedPreKey(uint signedPreKeyId)
        {
            return dbAccount.omemoInfo.signedPreKeys.keyId == signedPreKeyId;
        }

        public void DeleteAllSessions(string name)
        {
            foreach (OmemoDevice device in GetDevices(name))
            {
                device.session = null;
                device.Save();
            }
        }

        public void DeleteSession(SignalProtocolAddress address)
        {
            foreach (OmemoDevice device in GetDevices(address.getName()))
            {
                if (device.deviceId == address.getDeviceId())
                {
                    device.session = null;
                    device.Save();
                }
            }
        }

        public IdentityKeyPair GetIdentityKeyPair()
        {
            return new IdentityKeyPair(dbAccount.omemoInfo.identityKey);
        }

        public uint GetLocalRegistrationId()
        {
            return dbAccount.omemoInfo.deviceId;
        }

        public List<uint> GetSubDeviceSessions(string name)
        {
            return GetDevices(name).Select(d => d.deviceId).ToList();
        }

        public bool IsFingerprintTrusted(XMPP_API.Classes.Network.XML.Messages.XEP_0384.OmemoFingerprint fingerprint)
        {
            // Check for own devices:
            if (string.Equals(fingerprint.ADDRESS.getName(), dbAccount.bareJid))
            {
                // No trust management for own devices right now.
                return true;
            }
            // Check for contact devices:
            else
            {
                Storage.Classes.Models.Chat.Chat chat = ChatHandler.INSTANCE.GetChat(dbAccount, fingerprint.ADDRESS.getName());
                return !(chat is null) && (!chat.omemo.trustedKeysOnly || fingerprint.trusted);
            }
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

        public Tuple<OmemoDeviceListSubscriptionState, DateTime> LoadDeviceListSubscription(string name)
        {
            OmemoDeviceListSubscription subscription = null;
            // Own devices:
            if (string.Equals(name, dbAccount.bareJid))
            {
                subscription = dbAccount.omemoInfo.deviceListSubscription;
            }
            // Remote devices:
            else
            {
                Storage.Classes.Models.Chat.Chat chat = ChatHandler.INSTANCE.GetChat(dbAccount, name);
                if (!(chat is null))
                {
                    subscription = chat.omemo.deviceListSubscription;
                }
            }

            if (subscription is null)
            {
                return new Tuple<OmemoDeviceListSubscriptionState, DateTime>(OmemoDeviceListSubscriptionState.NONE, DateTime.MinValue);
            }
            return new Tuple<OmemoDeviceListSubscriptionState, DateTime>(subscription.state, subscription.lastUpdateReceived);
        }

        public IList<SignalProtocolAddress> LoadDevices(string name)
        {
            return GetDevices(name).Select(d => new SignalProtocolAddress(name, d.deviceId)).ToList();
        }

        public XMPP_API.Classes.Network.XML.Messages.XEP_0384.OmemoFingerprint LoadFingerprint(SignalProtocolAddress address)
        {
            OmemoDevice device = GetDevice(address);
            if (device is null || device.fingerprint is null)
            {
                return null;
            }
            return ToOmemoFingerprint(device.fingerprint, address);
        }

        public IEnumerable<XMPP_API.Classes.Network.XML.Messages.XEP_0384.OmemoFingerprint> LoadFingerprints(string bareJid)
        {
            return GetDevices(bareJid).Where(d => !(d.fingerprint is null)).Select(d => ToOmemoFingerprint(d.fingerprint, new SignalProtocolAddress(bareJid, d.deviceId)));
        }

        public PreKeyRecord LoadPreKey(uint preKeyId)
        {
            PreKeyRecord preKey = dbAccount.omemoInfo.preKeys.Where(k => k.keyId == preKeyId).Select(k => new PreKeyRecord(k.key)).FirstOrDefault();
            if (preKey is null)
            {
                throw new InvalidKeyIdException("No such pre key for account " + dbAccount.bareJid + ": " + preKeyId);
            }
            return preKey;
        }

        public IList<PreKeyRecord> LoadPreKeys()
        {
            return dbAccount.omemoInfo.preKeys.Select(key => new PreKeyRecord(key.key)).ToList();
        }

        public SessionRecord LoadSession(SignalProtocolAddress address)
        {
            OmemoDevice device = GetDevice(address);
            if (device is null || device.session is null)
            {
                Logger.Warn("No existing libsignal session found for: " + address.ToString());
                return new SessionRecord();
            }
            return new SessionRecord(device.session.session);
        }

        public SignedPreKeyRecord LoadSignedPreKey(uint signedPreKeyId)
        {
            SignedPreKeyRecord signedPreKey = dbAccount.omemoInfo.signedPreKeys.Where(k => k.keyId == signedPreKeyId).Select(k => new SignedPreKeyRecord(k.key)).FirstOrDefault();
            if (signedPreKey is null)
            {
                throw new InvalidKeyIdException("No such signed pre key for account " + dbAccount.bareJid + ": " + signedPreKeyId);
            }
            return signedPreKey;
        }

        public List<SignedPreKeyRecord> LoadSignedPreKeys()
        {
            using (AccountDbContext ctx = new AccountDbContext())
            {
                return ctx.SignedPreKeys.Select(k => new SignedPreKeyRecord(k.key)).ToList();
            }
        }

        public void RemovePreKey(uint preKeyId)
        {
            for (int i = 0; i < dbAccount.omemoInfo.preKeys.Count;)
            {
                if (dbAccount.omemoInfo.preKeys[i].keyId == preKeyId)
                {
                    dbAccount.omemoInfo.preKeys.RemoveAt(i);
                }
                else
                {
                    ++i;
                }
            }
            dbAccount.omemoInfo.Save();
        }

        public void RemoveSignedPreKey(uint signedPreKeyId)
        {
            for (int i = 0; i < dbAccount.omemoInfo.signedPreKeys.Count;)
            {
                if (dbAccount.omemoInfo.signedPreKeys[i].keyId == signedPreKeyId)
                {
                    dbAccount.omemoInfo.signedPreKeys.RemoveAt(i);
                }
                else
                {
                    ++i;
                }
            }
            dbAccount.omemoInfo.Save();
        }

        public bool SaveIdentity(string name, IdentityKey identityKey)
        {
            throw new NotImplementedException();
        }

        public void StoreDevice(SignalProtocolAddress device)
        {
            throw new NotImplementedException();
        }

        public void StoreDeviceListSubscription(string name, Tuple<OmemoDeviceListSubscriptionState, DateTime> lastUpdate)
        {
            throw new NotImplementedException();
        }

        public void StoreDevices(IList<SignalProtocolAddress> devices)
        {
            throw new NotImplementedException();
        }

        public void StoreFingerprint(XMPP_API.Classes.Network.XML.Messages.XEP_0384.OmemoFingerprint fingerprint)
        {
            throw new NotImplementedException();
        }

        public void StorePreKey(uint preKeyId, PreKeyRecord record)
        {
            throw new NotImplementedException();
        }

        public void StorePreKeys(IList<PreKeyRecord> preKeys)
        {
            throw new NotImplementedException();
        }

        public void StoreSession(SignalProtocolAddress address, SessionRecord record)
        {
            throw new NotImplementedException();
        }

        public void StoreSignedPreKey(uint signedPreKeyId, SignedPreKeyRecord record)
        {
            dbAccount.omemoInfo.signedPreKeys.Add(new OmemoSignedPreKey()
            {
                key = record.serialize(),
                keyId = signedPreKeyId
            });
            dbAccount.omemoInfo.Save();
        }

        #endregion

        #region --Misc Methods (Private)--
        private List<OmemoDevice> GetDevices(string name)
        {
            // Own device:
            if (string.Equals(name, dbAccount.bareJid))
            {
                return dbAccount.omemoInfo.devices;
            }
            // Remote device:
            else
            {
                Storage.Classes.Models.Chat.Chat chat = ChatHandler.INSTANCE.GetChat(dbAccount, name);
                if (!(chat is null))
                {
                    return chat.omemo.devices;
                }
            }
            return new List<OmemoDevice>();
        }

        public OmemoDevice GetDevice(SignalProtocolAddress address)
        {
            return GetDevices(address.getName()).Where(d => d.deviceId == address.getDeviceId()).FirstOrDefault();
        }

        private XMPP_API.Classes.Network.XML.Messages.XEP_0384.OmemoFingerprint ToOmemoFingerprint(Storage.Classes.Models.Omemo.OmemoFingerprint fingerprint, SignalProtocolAddress address)
        {
            ECPublicKey pubKey = Curve.decodePoint(fingerprint.identityPubKey, 0);
            return new XMPP_API.Classes.Network.XML.Messages.XEP_0384.OmemoFingerprint(pubKey, address, fingerprint.lastSeen, fingerprint.trusted);
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
