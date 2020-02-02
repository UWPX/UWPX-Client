using System.ComponentModel;
using Data_Manager2.Classes.DBTables;
using Windows.UI.Notifications;

namespace Data_Manager2.Classes.Toast
{
    public class OnChatMessageToastEventArgs: CancelEventArgs
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly ToastNotification TOAST;
        public readonly ChatTable CHAT;
        public ChatMessageToasterType toasterTypeOverride;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 06/11/2018 Created [Fabian Sauter]
        /// </history>
        public OnChatMessageToastEventArgs(ToastNotification toast, ChatTable chat)
        {
            TOAST = toast;
            CHAT = chat;
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
