using System.Collections.Generic;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0384.Session
{
    public class OmemoDeviceSession
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly Dictionary<uint, OmemoDeviceSession> DEVICE_SESSIONS_OWN;
        public readonly Dictionary<uint, OmemoDeviceSession> DEVICE_SESSIONS_REMOTE;
        public readonly string CHAT_JID;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public OmemoDeviceSession(string chatJid)
        {
            CHAT_JID = chatJid;
            DEVICE_SESSIONS_OWN = new Dictionary<uint, OmemoDeviceSession>();
            DEVICE_SESSIONS_REMOTE = new Dictionary<uint, OmemoDeviceSession>();
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
