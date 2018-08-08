using libsignal;
using libsignal.protocol;
using libsignal.state;
using Logging;
using System.Collections.Generic;
using XMPP_API.Classes.Network.XML.DBManager;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0384.Signal
{
    // https://github.com/signalapp/Signal-Android/blob/2beb1dd8d9e77c0b2773c483133977b9bbe5319a/src/org/thoughtcrime/securesms/crypto/storage/TextSecureSessionStore.java
    public class OmemoSessionStore : SessionStore
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--


        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 08/08/2018 Created [Fabian Sauter]
        /// </history>
        public OmemoSessionStore()
        {
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public List<uint> GetSubDeviceSessions(string name)
        {
            return SignalKeyDBManager.INSTANCE.getDeviceIds(name);
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public bool ContainsSession(SignalProtocolAddress address)
        {
            SessionRecord session = SignalKeyDBManager.INSTANCE.getSession(address);
            return session != null && session.getSessionState().hasSenderChain() && session.getSessionState().getSessionVersion() == CiphertextMessage.CURRENT_VERSION;
        }

        public void DeleteAllSessions(string name)
        {
            SignalKeyDBManager.INSTANCE.deleteSessions(name);
        }

        public void DeleteSession(SignalProtocolAddress address)
        {
            SignalKeyDBManager.INSTANCE.deleteSession(address);
        }

        public SessionRecord LoadSession(SignalProtocolAddress address)
        {
            SessionRecord session = SignalKeyDBManager.INSTANCE.getSession(address);
            if (session == null)
            {
                Logger.Warn("No existing session information found.");
                session = new SessionRecord();
            }
            return session;
        }

        public void StoreSession(SignalProtocolAddress address, SessionRecord record)
        {
            SignalKeyDBManager.INSTANCE.setSession(address, record);
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
