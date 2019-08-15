using System.Collections.Generic;
using System.ComponentModel;
using XMPP_API.Classes.Network.XML.Messages.XEP_0384;
using XMPP_API.Classes.Network.XML.Messages.XEP_0384.Signal.Session;

namespace XMPP_API.Classes.Events
{
    public class OmemoSessionBuildErrorEventArgs : CancelEventArgs
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly string CHAT_JID;
        public readonly OmemoSessionBuildError ERROR;
        public readonly IList<OmemoMessageMessage> MESSAGES;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 04/12/2018 Created [Fabian Sauter]
        /// </history>
        public OmemoSessionBuildErrorEventArgs(string chatJid, OmemoSessionBuildError error, IList<OmemoMessageMessage> messages)
        {
            CHAT_JID = chatJid;
            ERROR = error;
            MESSAGES = messages;
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
