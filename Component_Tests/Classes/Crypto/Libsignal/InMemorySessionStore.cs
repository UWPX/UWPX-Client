using libsignal;
using libsignal.state;
using System.Collections.Generic;
using System.Linq;

namespace Component_Tests.Classes.Crypto.Libsignal
{
    class InMemorySessionStore : SessionStore
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly Dictionary<SignalProtocolAddress, SessionRecord> SESSIONS;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 07/11/2018 Created [Fabian Sauter]
        /// </history>
        public InMemorySessionStore()
        {
            this.SESSIONS = new Dictionary<SignalProtocolAddress, SessionRecord>();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public List<uint> GetSubDeviceSessions(string name)
        {
            List<uint> ids = new List<uint>();
            foreach (var kv in SESSIONS)
            {
                if (string.Equals(kv.Key.getName(), name))
                {
                    ids.Add(kv.Key.getDeviceId());
                }
            }
            return ids;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public SessionRecord LoadSession(SignalProtocolAddress address)
        {
            if(ContainsSession(address))
            {
                return SESSIONS[address];
            }
            return new SessionRecord();
        }

        public void StoreSession(SignalProtocolAddress address, SessionRecord record)
        {
            SESSIONS[address] = record;
        }

        public bool ContainsSession(SignalProtocolAddress address)
        {
            return SESSIONS.ContainsKey(address);
        }

        public void DeleteAllSessions(string name)
        {
            foreach (var s in SESSIONS.Where(kv => string.Equals(kv.Key.getName(), name)).ToList())
            {
                SESSIONS.Remove(s.Key);
            }
        }

        public void DeleteSession(SignalProtocolAddress address)
        {
            SESSIONS.Remove(address);
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
