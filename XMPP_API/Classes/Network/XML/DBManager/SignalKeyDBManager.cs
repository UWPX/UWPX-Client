using libsignal;
using libsignal.state;
using System.Collections.Generic;
using Thread_Save_Components.Classes.SQLite;
using XMPP_API.Classes.Network.XML.DBEntries;

namespace XMPP_API.Classes.Network.XML.DBManager
{
    class SignalKeyDBManager : AbstractDBManager
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public static readonly SignalKeyDBManager INSTANCE = new SignalKeyDBManager();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 08/08/2018 Created [Fabian Sauter]
        /// </history>
        public SignalKeyDBManager()
        {
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public List<SignedPreKeyRecord> getAllSignedPreKeys(string accountId)
        {
            List<SignedPreKeyRecord> keys = new List<SignedPreKeyRecord>();
            List<SignedPreKeyTable> list = dB.Query<SignedPreKeyTable>(true, "SELECT * FROM " + DBTableConsts.SIGNED_PRE_KEY_TABLE + " WHERE accountId = ?;", accountId);
            foreach (SignedPreKeyTable key in list)
            {
                keys.Add(new SignedPreKeyRecord(key.signedPreKey));
            }
            return keys;
        }

        public List<PreKeyRecord> getAllPreKeys(string accountId)
        {
            List<PreKeyRecord> keys = new List<PreKeyRecord>();
            List<PreKeyTable> list = dB.Query<PreKeyTable>(true, "SELECT * FROM " + DBTableConsts.PRE_KEY_TABLE + " WHERE accountId = ?;", accountId);
            foreach (PreKeyTable key in list)
            {
                keys.Add(new PreKeyRecord(key.preKey));
            }
            return keys;
        }

        public List<IdentityKey> getAllIdentityKeys(string accountId)
        {
            List<IdentityKey> keys = new List<IdentityKey>();
            List<IdentityKeyTable> list = dB.Query<IdentityKeyTable>(true, "SELECT * FROM " + DBTableConsts.IDENTITY_KEY_TABLE + " WHERE accountId = ?;", accountId);
            foreach (IdentityKeyTable key in list)
            {
                keys.Add(new IdentityKey(key.identityKey, 0));
            }
            return keys;
        }

        public SignedPreKeyRecord getSignedPreKey(uint signedPreKeyId, string accountId)
        {
            List<SignedPreKeyTable> list = dB.Query<SignedPreKeyTable>(true, "SELECT * FROM " + DBTableConsts.SIGNED_PRE_KEY_TABLE + " WHERE id = ?;", SignedPreKeyTable.generateId(signedPreKeyId, accountId));
            if (list.Count <= 0)
            {
                return null;
            }
            return new SignedPreKeyRecord(list[0].signedPreKey);
        }

        public PreKeyRecord getPreKeyRecord(uint preKeyId, string accountId)
        {
            List<PreKeyTable> list = dB.Query<PreKeyTable>(true, "SELECT * FROM " + DBTableConsts.PRE_KEY_TABLE + " WHERE id = ?;", PreKeyTable.generateId(preKeyId, accountId));
            if (list.Count <= 0)
            {
                return null;
            }
            return new PreKeyRecord(list[0].preKey);
        }

        public IdentityKey getIdentityKey(string name, string accountId)
        {
            List<IdentityKeyTable> list = dB.Query<IdentityKeyTable>(true, "SELECT * FROM " + DBTableConsts.IDENTITY_KEY_TABLE + " WHERE id = ?;", IdentityKeyTable.generateId(name, accountId));
            if (list.Count <= 0)
            {
                return null;
            }
            return new IdentityKey(list[0].identityKey, 0);
        }

        public SessionRecord getSession(SignalProtocolAddress address, string accountId)
        {
            List<SessionStoreTable> list = dB.Query<SessionStoreTable>(true, "SELECT * FROM " + DBTableConsts.SESSION_STORE_TABLE + " WHERE id = ?;", SessionStoreTable.generateId(address, accountId));
            if (list.Count <= 0)
            {
                return null;
            }
            return new SessionRecord(list[0].session);
        }

        public List<uint> getDeviceIds(string name, string accountId)
        {
            List<SessionStoreTable> sessions = dB.Query<SessionStoreTable>(true, "SELECT * FROM " + DBTableConsts.SESSION_STORE_TABLE + " WHERE name = ? AND accountId = ?;", name, accountId);
            List<uint> deviceIds = new List<uint>();
            foreach (SessionStoreTable session in sessions)
            {
                deviceIds.Add(session.deviceId);
            }
            return deviceIds;
        }

        public void setSignedPreKey(uint signedPreKeyId, SignedPreKeyRecord signedPreKey, string accountId)
        {
            dB.InsertOrReplace(new SignedPreKeyTable()
            {
                id = SignedPreKeyTable.generateId(signedPreKeyId, accountId),
                signedPreKeyId = signedPreKeyId,
                accountId = accountId,
                signedPreKey = signedPreKey.serialize()
            });
        }

        public void setPreKey(uint preKeyId, PreKeyRecord preKey, string accountId)
        {
            dB.InsertOrReplace(new PreKeyTable()
            {
                id = PreKeyTable.generateId(preKeyId, accountId),
                preKeyId = preKeyId,
                accountId = accountId,
                preKey = preKey.serialize()
            });
        }

        public void setIdentityKey(string name, IdentityKey identityKey, string accountId)
        {
            dB.InsertOrReplace(new IdentityKeyTable()
            {
                id = IdentityKeyTable.generateId(name, accountId),
                name = name,
                accountId = accountId,
                identityKey = identityKey.serialize()
            });
        }

        public void setSession(SignalProtocolAddress address, SessionRecord record, string accountId)
        {
            dB.InsertOrReplace(new SessionStoreTable()
            {
                id = SessionStoreTable.generateId(address, accountId),
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
            List<SignedPreKeyTable> list = dB.Query<SignedPreKeyTable>(true, "SELECT * FROM " + DBTableConsts.SIGNED_PRE_KEY_TABLE + " WHERE id = ?;", SignedPreKeyTable.generateId(signedPreKeyId, accountId));
            return list.Count > 0;
        }

        public bool containsPreKeyRecord(uint preKeyId, string accountId)
        {
            List<PreKeyTable> list = dB.Query<PreKeyTable>(true, "SELECT * FROM " + DBTableConsts.PRE_KEY_TABLE + " WHERE id = ?;", PreKeyTable.generateId(preKeyId, accountId));
            return list.Count > 0;
        }

        public bool containsIdentityKey(string name, string accountId)
        {
            List<IdentityKeyTable> list = dB.Query<IdentityKeyTable>(true, "SELECT * FROM " + DBTableConsts.IDENTITY_KEY_TABLE + " WHERE id = ?;", IdentityKeyTable.generateId(name, accountId));
            return list.Count > 0;
        }

        public bool containsSession(SignalProtocolAddress address, string accountId)
        {
            List<SessionStoreTable> list = dB.Query<SessionStoreTable>(true, "SELECT * FROM " + DBTableConsts.SESSION_STORE_TABLE + " WHERE id = ?;", SessionStoreTable.generateId(address, accountId));
            return list.Count > 0;
        }

        public void deleteSignedPreKey(uint signedPreKeyId, string accountId)
        {
            dB.Execute("DELETE FROM " + DBTableConsts.SIGNED_PRE_KEY_TABLE + " WHERE id = ?;", SignedPreKeyTable.generateId(signedPreKeyId, accountId));
        }

        public void deletePreKey(uint preKeyId, string accountId)
        {
            dB.Execute("DELETE FROM " + DBTableConsts.PRE_KEY_TABLE + " WHERE id = ?;", PreKeyTable.generateId(preKeyId, accountId));
        }

        public void deleteIdentityKey(string name, string accountId)
        {
            dB.Execute("DELETE FROM " + DBTableConsts.IDENTITY_KEY_TABLE + " WHERE id = ?;", IdentityKeyTable.generateId(name, accountId));
        }

        public void deleteSessions(string name, string accountId)
        {
            dB.Execute("DELETE FROM " + DBTableConsts.SESSION_STORE_TABLE + " WHERE name = ? AND accountId = ?;", name, accountId);
        }

        public void deleteSession(SignalProtocolAddress address, string accountId)
        {
            dB.Execute("DELETE FROM " + DBTableConsts.SESSION_STORE_TABLE + " WHERE id = ?;", SessionStoreTable.generateId(address, accountId));
        }

        public void deleteSignedPreKey(string accountId)
        {
            dB.Execute("DELETE FROM " + DBTableConsts.SIGNED_PRE_KEY_TABLE + " WHERE accountId = ?;", accountId);
        }

        public void deletePreKey(string accountId)
        {
            dB.Execute("DELETE FROM " + DBTableConsts.PRE_KEY_TABLE + " WHERE accountId = ?;", accountId);
        }

        public void deleteIdentityKey(string accountId)
        {
            dB.Execute("DELETE FROM " + DBTableConsts.IDENTITY_KEY_TABLE + " WHERE accountId = ?;", accountId);
        }

        public void deleteSessions(string accountId)
        {
            dB.Execute("DELETE FROM " + DBTableConsts.SESSION_STORE_TABLE + " WHERE accountId = ?;", accountId);
        }

        public void deleteAllForAccount(string accountId)
        {
            deleteIdentityKey(accountId);
            deletePreKey(accountId);
            deleteSignedPreKey(accountId);
            deleteSessions(accountId);
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--
        protected override void createTables()
        {
            dB.CreateTable<IdentityKeyTable>();
            dB.CreateTable<SignedPreKeyTable>();
            dB.CreateTable<PreKeyTable>();
            dB.CreateTable<SessionStoreTable>();
        }

        protected override void dropTables()
        {
            dB.DropTable<IdentityKeyTable>();
            dB.DropTable<SignedPreKeyTable>();
            dB.DropTable<PreKeyTable>();
            dB.DropTable<SessionStoreTable>();
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
