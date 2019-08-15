using libsignal;
using System.Collections.Generic;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0384.Signal.Session
{
    public class OmemoSession
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly Dictionary<uint, SessionCipher> DEVICE_SESSIONS_OWN;
        public readonly Dictionary<uint, SessionCipher> DEVICE_SESSIONS_REMOTE;
        public readonly string CHAT_JID;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 18/08/2018 Created [Fabian Sauter]
        /// </history>
        public OmemoSession(string chatJid)
        {
            CHAT_JID = chatJid;
            DEVICE_SESSIONS_OWN = new Dictionary<uint, SessionCipher>();
            DEVICE_SESSIONS_REMOTE = new Dictionary<uint, SessionCipher>();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


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
