using Data_Manager2.Classes.DBTables;
using System;
using XMPP_API.Classes;

namespace UWP_XMPP_Client.Classes.Events
{
    public class NavigatedToMUCInfoEventArgs : EventArgs
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly ChatTable CHAT;
        public readonly MUCChatInfoTable MUC_INFO;
        public readonly XMPPClient CLIENT;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 05/02/2018 Created [Fabian Sauter]
        /// </history>
        public NavigatedToMUCInfoEventArgs(ChatTable chat, XMPPClient client, MUCChatInfoTable mucInfo)
        {
            this.CHAT = chat;
            this.CLIENT = client;
            this.MUC_INFO = mucInfo;
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
