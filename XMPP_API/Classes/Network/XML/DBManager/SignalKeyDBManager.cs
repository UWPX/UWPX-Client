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
        /// 17/03/2018 Created [Fabian Sauter]
        /// </history>
        public SignalKeyDBManager()
        {
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public List<SignedPreKeyRecord> getAllSignedPreKeys()
        {
            List<SignedPreKeyRecord> keys = new List<SignedPreKeyRecord>();
            List<SignedPreKeyTable> list = dB.Query<SignedPreKeyTable>(true, "SELECT * FROM " + DBTableConsts.SIGNED_PRE_KEY_TABLE + ";");
            foreach (SignedPreKeyTable key in list)
            {
                keys.Add(new SignedPreKeyRecord(key.signedPreKey));
            }
            return keys;
        }

        public List<PreKeyRecord> getAllPreKeys()
        {
            List<PreKeyRecord> keys = new List<PreKeyRecord>();
            List<PreKeyTable> list = dB.Query<PreKeyTable>(true, "SELECT * FROM " + DBTableConsts.PRE_KEY_TABLE + ";");
            foreach (PreKeyTable key in list)
            {
                keys.Add(new PreKeyRecord(key.preKey));
            }
            return keys;
        }

        public List<IdentityKey> getAllIdentityKeys()
        {
            List<IdentityKey> keys = new List<IdentityKey>();
            List<IdentityKeyTable> list = dB.Query<IdentityKeyTable>(true, "SELECT * FROM " + DBTableConsts.IDENTITY_KEY_TABLE + ";");
            foreach (IdentityKeyTable key in list)
            {
                keys.Add(new IdentityKey(key.identityKey, 0));
            }
            return keys;
        }

        public SignedPreKeyRecord getSignedPreKey(uint signedPreKeyId)
        {
            List<SignedPreKeyTable> list = dB.Query<SignedPreKeyTable>(true, "SELECT * FROM " + DBTableConsts.SIGNED_PRE_KEY_TABLE + " WHERE signedPreKeyId = ?;", signedPreKeyId);
            if (list.Count <= 0)
            {
                return null;
            }
            return new SignedPreKeyRecord(list[0].signedPreKey);
        }

        public PreKeyRecord getPreKeyRecord(uint preKeyId)
        {
            List<PreKeyTable> list = dB.Query<PreKeyTable>(true, "SELECT * FROM " + DBTableConsts.PRE_KEY_TABLE + " WHERE preKeyId = ?;", preKeyId);
            if (list.Count <= 0)
            {
                return null;
            }
            return new PreKeyRecord(list[0].preKey);
        }

        public IdentityKey getIdentityKey(string name)
        {
            List<IdentityKeyTable> list = dB.Query<IdentityKeyTable>(true, "SELECT * FROM " + DBTableConsts.IDENTITY_KEY_TABLE + " WHERE name = ?;", name);
            if (list.Count <= 0)
            {
                return null;
            }
            return new IdentityKey(list[0].identityKey, 0);
        }

        public SessionRecord getSession(SignalProtocolAddress address)
        {
            List<SessionStoreTable> list = dB.Query<SessionStoreTable>(true, "SELECT * FROM " + DBTableConsts.SESSION_STORE_TABLE + " WHERE id = ?;", SessionStoreTable.generateId(address));
            if (list.Count <= 0)
            {
                return null;
            }
            return new SessionRecord(list[0].session);
        }

        public List<uint> getDeviceIds(string name)
        {
            List<SessionStoreTable> sessions = dB.Query<SessionStoreTable>(true, "SELECT * FROM " + DBTableConsts.SESSION_STORE_TABLE + " WHERE name = ?;", name);
            List<uint> deviceIds = new List<uint>();
            foreach (SessionStoreTable session in sessions)
            {
                deviceIds.Add(session.deviceId);
            }
            return deviceIds;
        }

        public void setSignedPreKey(uint signedPreKeyId, SignedPreKeyRecord signedPreKey)
        {
            dB.InsertOrReplace(new SignedPreKeyTable()
            {
                signedPreKeyId = signedPreKeyId,
                signedPreKey = signedPreKey.serialize()
            });
        }

        public void setPreKey(uint preKeyId, PreKeyRecord preKey)
        {
            dB.InsertOrReplace(new PreKeyTable()
            {
                preKeyId = preKeyId,
                preKey = preKey.serialize()
            });
        }

        public void setIdentityKey(string name, IdentityKey identityKey, bool nonBlockingApproval)
        {
            dB.InsertOrReplace(new IdentityKeyTable()
            {
                name = name,
                identityKey = identityKey.serialize(),
                nonBlockingApproval = nonBlockingApproval
            });
        }

        public void setSession(SignalProtocolAddress address, SessionRecord record)
        {
            dB.InsertOrReplace(new SessionStoreTable()
            {
                id = SessionStoreTable.generateId(address),
                deviceId = address.getDeviceId(),
                name = address.getName(),
                session = record.serialize()
            });
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public bool containsSignedPreKey(uint signedPreKeyId)
        {
            List<SignedPreKeyTable> list = dB.Query<SignedPreKeyTable>(true, "SELECT * FROM " + DBTableConsts.SIGNED_PRE_KEY_TABLE + " WHERE signedPreKeyId = ?;", signedPreKeyId);
            return list.Count > 0;
        }

        public bool containsPreKeyRecord(uint preKeyId)
        {
            List<PreKeyTable> list = dB.Query<PreKeyTable>(true, "SELECT * FROM " + DBTableConsts.PRE_KEY_TABLE + " WHERE preKeyId = ?;", preKeyId);
            return list.Count > 0;
        }

        public bool containsIdentityKey(string name)
        {
            List<IdentityKeyTable> list = dB.Query<IdentityKeyTable>(true, "SELECT * FROM " + DBTableConsts.IDENTITY_KEY_TABLE + " WHERE name = ?;", name);
            return list.Count > 0;
        }

        public bool containsSession(SignalProtocolAddress address)
        {
            List<SessionStoreTable> list = dB.Query<SessionStoreTable>(true, "SELECT * FROM " + DBTableConsts.SESSION_STORE_TABLE + " WHERE id = ?;", SessionStoreTable.generateId(address));
            return list.Count > 0;
        }

        public void deleteSignedPreKey(uint signedPreKeyId)
        {
            dB.Execute("DELETE FROM " + DBTableConsts.SIGNED_PRE_KEY_TABLE + " WHERE signedPreKeyId = ?;", signedPreKeyId);
        }

        public void deletePreKeyRecord(uint preKeyId)
        {
            dB.Execute("DELETE FROM " + DBTableConsts.PRE_KEY_TABLE + " WHERE preKeyId = ?;", preKeyId);
        }

        public void deleteIdentityKey(string name)
        {
            dB.Execute("DELETE FROM " + DBTableConsts.IDENTITY_KEY_TABLE + " WHERE name = ?;", name);
        }

        public void deleteSessions(string name)
        {
            dB.Execute("DELETE FROM " + DBTableConsts.SESSION_STORE_TABLE + " WHERE name = ?;", name);
        }

        public void deleteSession(SignalProtocolAddress address)
        {
            dB.Execute("DELETE FROM " + DBTableConsts.SESSION_STORE_TABLE + " WHERE id = ?;", SessionStoreTable.generateId(address));
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
