using Data_Manager2.Classes.DBTables;
using System;
using XMPP_API.Classes;

namespace UWP_XMPP_Client.Classes.Events
{
    public class NavigatedToUserProfileEventArgs : EventArgs
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly ChatTable CHAT;
        public readonly XMPPClient CLIENT;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 20/09/2017 Created [Fabian Sauter]
        /// </history>
        public NavigatedToUserProfileEventArgs(ChatTable chat, XMPPClient client)
        {
            CHAT = chat;
            CLIENT = client;
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
