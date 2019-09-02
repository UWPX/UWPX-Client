using System.Collections.Generic;
using System.Linq;
using Data_Manager2.Classes.DBTables;
using Data_Manager2.Classes.DBTables.Omemo;
using libsignal;
using libsignal.state;
using Shared.Classes.SQLite;
using XMPP_API.Classes.Network.XML.Messages.XEP_0384;

namespace Data_Manager2.Classes.DBManager.Omemo
{
    public class OmemoSignalKeyDBManager: AbstractDBManager
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public static readonly OmemoSignalKeyDBManager INSTANCE = new OmemoSignalKeyDBManager();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 03/11/2018 Created [Fabian Sauter]
        /// </history>
        public OmemoSignalKeyDBManager()
        {
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public List<SignedPreKeyRecord> getAllSignedPreKeys(string accountId)
        {
            List<SignedPreKeyRecord> keys = new List<SignedPreKeyRecord>();
            List<OmemoSignedPreKeyTable> list = dB.Query<OmemoSignedPreKeyTable>(true, "SELECT * FROM " + DBTableConsts.OMEMO_SIGNED_PRE_KEY_TABLE + " WHERE accountId = ?;", accountId);
            foreach (OmemoSignedPreKeyTable key in list)
            {
                keys.Add(new SignedPreKeyRecord(key.signedPreKey));
            }
            return keys;
        }

        public List<PreKeyRecord> getAllPreKeys(string accountId)
        {
            List<PreKeyRecord> keys = new List<PreKeyRecord>();
            List<OmemoPreKeyTable> list = dB.Query<OmemoPreKeyTable>(true, "SELECT * FROM " + DBTableConsts.OMEMO_PRE_KEY_TABLE + " WHERE accountId = ?;", accountId);
            foreach (OmemoPreKeyTable key in list)
            {
                keys.Add(new PreKeyRecord(key.preKey));
            }
            return keys;
        }

        public List<IdentityKey> getAllIdentityKeys(string accountId)
        {
            List<IdentityKey> keys = new List<IdentityKey>();
            List<OmemoIdentityKeyTable> list = dB.Query<OmemoIdentityKeyTable>(true, "SELECT * FROM " + DBTableConsts.OMEMO_IDENTITY_KEY_TABLE + " WHERE accountId = ?;", accountId);
            foreach (OmemoIdentityKeyTable key in list)
            {
                keys.Add(new IdentityKey(key.identityKey, 0));
            }
            return keys;
        }

        public SignedPreKeyRecord getSignedPreKey(uint signedPreKeyId, string accountId)
        {
            List<OmemoSignedPreKeyTable> list = dB.Query<OmemoSignedPreKeyTable>(true, "SELECT * FROM " + DBTableConsts.OMEMO_SIGNED_PRE_KEY_TABLE + " WHERE id = ?;", OmemoSignedPreKeyTable.generateId(signedPreKeyId, accountId));
            return list.Count <= 0 ? null : new SignedPreKeyRecord(list[0].signedPreKey);
        }

        public PreKeyRecord getPreKeyRecord(uint preKeyId, string accountId)
        {
            List<OmemoPreKeyTable> list = dB.Query<OmemoPreKeyTable>(true, "SELECT * FROM " + DBTableConsts.OMEMO_PRE_KEY_TABLE + " WHERE id = ?;", OmemoPreKeyTable.generateId(preKeyId, accountId));
            if (list.Count <= 0)
            {
                return null;
            }
            return new PreKeyRecord(list[0].preKey);
        }

        public IdentityKey getIdentityKey(string name, string accountId)
        {
            List<OmemoIdentityKeyTable> list = dB.Query<OmemoIdentityKeyTable>(true, "SELECT * FROM " + DBTableConsts.OMEMO_IDENTITY_KEY_TABLE + " WHERE id = ?;", OmemoIdentityKeyTable.generateId(name, accountId));
            if (list.Count <= 0)
            {
                return null;
            }
            return new IdentityKey(list[0].identityKey, 0);
        }

        public SessionRecord getSession(SignalProtocolAddress address, string accountId)
        {
            List<OmemoSessionStoreTable> list = dB.Query<OmemoSessionStoreTable>(true, "SELECT * FROM " + DBTableConsts.OMEMO_SESSION_STORE_TABLE + " WHERE id = ?;", OmemoSessionStoreTable.generateId(address, accountId));
            return list.Count <= 0 ? null : new SessionRecord(list[0].session);
        }

        public List<uint> getDeviceIds(string name, string accountId)
        {
            List<OmemoSessionStoreTable> sessions = dB.Query<OmemoSessionStoreTable>(true, "SELECT * FROM " + DBTableConsts.OMEMO_SESSION_STORE_TABLE + " WHERE name = ? AND accountId = ?;", name, accountId);
            List<uint> deviceIds = new List<uint>();
            foreach (OmemoSessionStoreTable session in sessions)
            {
                deviceIds.Add(session.deviceId);
            }
            return deviceIds;
        }

        public OmemoFingerprint getFingerprint(SignalProtocolAddress address, string accountId)
        {
            string chatId = ChatTable.generateId(address.getName(), accountId);
            string id = OmemoFingerprintTable.generateId(chatId, address);
            List<OmemoFingerprintTable> list = dB.Query<OmemoFingerprintTable>(true, "SELECT * FROM " + DBTableConsts.OMEMO_FINGERPRINT_TABLE + " WHERE id = ?;", id);
            return list.Count <= 0 ? null : list[0].toOmemoFingerprint();
        }

        public IEnumerable<OmemoFingerprint> getFingerprints(string chatId)
        {
            List<OmemoFingerprintTable> list = dB.Query<OmemoFingerprintTable>(true, "SELECT * FROM " + DBTableConsts.OMEMO_FINGERPRINT_TABLE + " WHERE chatId = ?;", chatId);
            return list.Select(x => x.toOmemoFingerprint());
        }

        public void setFingerprint(OmemoFingerprint fingerprint, string accountId)
        {
            string chatId = ChatTable.generateId(fingerprint.ADDRESS.getName(), accountId);
            dB.InsertOrReplace(new OmemoFingerprintTable(fingerprint, chatId));
        }

        public void setSignedPreKey(uint signedPreKeyId, SignedPreKeyRecord signedPreKey, string accountId)
        {
            dB.InsertOrReplace(new OmemoSignedPreKeyTable
            {
                id = OmemoSignedPreKeyTable.generateId(signedPreKeyId, accountId),
                signedPreKeyId = signedPreKeyId,
                accountId = accountId,
                signedPreKey = signedPreKey.serialize()
            });
        }

        public void setPreKeys(IList<PreKeyRecord> preKeys, string accountId)
        {
            dB.BeginTransaction();
            deletePreKeys(accountId);
            foreach (PreKeyRecord key in preKeys)
            {
                dB.Insert(new OmemoPreKeyTable
                {
                    id = OmemoPreKeyTable.generateId(key.getId(), accountId),
                    preKeyId = key.getId(),
                    accountId = accountId,
                    preKey = key.serialize()
                });
            }
            dB.Commit();
        }

        public void setPreKey(uint preKeyId, PreKeyRecord preKey, string accountId)
        {
            dB.InsertOrReplace(new OmemoPreKeyTable
            {
                id = OmemoPreKeyTable.generateId(preKeyId, accountId),
                preKeyId = preKeyId,
                accountId = accountId,
                preKey = preKey.serialize()
            });
        }

        public void setIdentityKey(string name, IdentityKey identityKey, string accountId)
        {
            dB.InsertOrReplace(new OmemoIdentityKeyTable
            {
                id = OmemoIdentityKeyTable.generateId(name, accountId),
                name = name,
                accountId = accountId,
                identityKey = identityKey.serialize()
            });
        }

        public void setSession(SignalProtocolAddress address, SessionRecord record, string accountId)
        {
            dB.InsertOrReplace(new OmemoSessionStoreTable
            {
                id = OmemoSessionStoreTable.generateId(address, accountId),
                accountId = accountId,
                deviceId = address.getDeviceId(),
                name = address.getName(),
                session = record.serialize()
            });
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public bool containsSignedPreKey(uint signedPreKeyId, string accountId)
        {
            List<OmemoSignedPreKeyTable> list = dB.Query<OmemoSignedPreKeyTable>(true, "SELECT * FROM " + DBTableConsts.OMEMO_SIGNED_PRE_KEY_TABLE + " WHERE id = ?;", OmemoSignedPreKeyTable.generateId(signedPreKeyId, accountId));
            return list.Count > 0;
        }

        public bool containsPreKeyRecord(uint preKeyId, string accountId)
        {
            List<OmemoPreKeyTable> list = dB.Query<OmemoPreKeyTable>(true, "SELECT * FROM " + DBTableConsts.OMEMO_PRE_KEY_TABLE + " WHERE id = ?;", OmemoPreKeyTable.generateId(preKeyId, accountId));
            return list.Count > 0;
        }

        public bool containsIdentityKey(string name, string accountId)
        {
            List<OmemoIdentityKeyTable> list = dB.Query<OmemoIdentityKeyTable>(true, "SELECT * FROM " + DBTableConsts.OMEMO_IDENTITY_KEY_TABLE + " WHERE id = ?;", OmemoIdentityKeyTable.generateId(name, accountId));
            return list.Count > 0;
        }

        public bool containsSession(SignalProtocolAddress address, string accountId)
        {
            List<OmemoSessionStoreTable> list = dB.Query<OmemoSessionStoreTable>(true, "SELECT * FROM " + DBTableConsts.OMEMO_SESSION_STORE_TABLE + " WHERE id = ?;", OmemoSessionStoreTable.generateId(address, accountId));
            return list.Count > 0;
        }

        public void deleteSignedPreKey(uint signedPreKeyId, string accountId)
        {
            dB.Execute("DELETE FROM " + DBTableConsts.OMEMO_SIGNED_PRE_KEY_TABLE + " WHERE id = ?;", OmemoSignedPreKeyTable.generateId(signedPreKeyId, accountId));
        }

        public void deletePreKeys(string accountId)
        {
            dB.Execute("DELETE FROM " + DBTableConsts.OMEMO_PRE_KEY_TABLE + " WHERE accountId = ?;", accountId);
        }

        public void deletePreKey(uint preKeyId, string accountId)
        {
            dB.Execute("DELETE FROM " + DBTableConsts.OMEMO_PRE_KEY_TABLE + " WHERE id = ?;", OmemoPreKeyTable.generateId(preKeyId, accountId));
        }

        public void deleteIdentityKey(string name, string accountId)
        {
            dB.Execute("DELETE FROM " + DBTableConsts.OMEMO_IDENTITY_KEY_TABLE + " WHERE id = ?;", OmemoIdentityKeyTable.generateId(name, accountId));
        }

        public void deleteSessions(string name, string accountId)
        {
            dB.Execute("DELETE FROM " + DBTableConsts.OMEMO_SESSION_STORE_TABLE + " WHERE name = ? AND accountId = ?;", name, accountId);
        }

        public void deleteSession(SignalProtocolAddress address, string accountId)
        {
            dB.Execute("DELETE FROM " + DBTableConsts.OMEMO_SESSION_STORE_TABLE + " WHERE id = ?;", OmemoSessionStoreTable.generateId(address, accountId));
        }

        public void deleteSignedPreKey(string accountId)
        {
            dB.Execute("DELETE FROM " + DBTableConsts.OMEMO_SIGNED_PRE_KEY_TABLE + " WHERE accountId = ?;", accountId);
        }

        public void deletePreKey(string accountId)
        {
            dB.Execute("DELETE FROM " + DBTableConsts.OMEMO_PRE_KEY_TABLE + " WHERE accountId = ?;", accountId);
        }

        public void deleteIdentityKey(string accountId)
        {
            dB.Execute("DELETE FROM " + DBTableConsts.OMEMO_IDENTITY_KEY_TABLE + " WHERE accountId = ?;", accountId);
        }

        public void deleteSessions(string accountId)
        {
            dB.Execute("DELETE FROM " + DBTableConsts.OMEMO_SESSION_STORE_TABLE + " WHERE accountId = ?;", accountId);
        }

        public void deleteFingerprints(string chatId)
        {
            dB.Execute("DELETE FROM " + DBTableConsts.OMEMO_FINGERPRINT_TABLE + " WHERE chatId = ?;", chatId);
        }

        public void deleteAllForAccount(string accountId)
        {
            deleteIdentityKey(accountId);
            deletePreKey(accountId);
            deleteSignedPreKey(accountId);
            deleteSessions(accountId);
            deleteFingerprints(ChatTable.generateId(accountId, accountId));
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--
        protected override void CreateTables()
        {
            dB.CreateTable<OmemoIdentityKeyTable>();
            dB.CreateTable<OmemoSignedPreKeyTable>();
            dB.CreateTable<OmemoPreKeyTable>();
            dB.CreateTable<OmemoSessionStoreTable>();
            dB.CreateTable<OmemoFingerprintTable>();
        }

        protected override void DropTables()
        {
            dB.DropTable<OmemoIdentityKeyTable>();
            dB.DropTable<OmemoSignedPreKeyTable>();
            dB.DropTable<OmemoPreKeyTable>();
            dB.DropTable<OmemoSessionStoreTable>();
            dB.DropTable<OmemoFingerprintTable>();
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
