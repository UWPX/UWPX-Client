using System.ComponentModel;
using Storage.Classes.Models.Chat;
using Windows.UI.Notifications;

namespace Manager.Classes.Toast
{
    public class OnChatMessageToastEventArgs: CancelEventArgs
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly ToastNotification TOAST;
        public readonly ChatModel CHAT;
        public ChatMessageToasterType toasterTypeOverride;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public OnChatMessageToastEventArgs(ToastNotification toast, ChatModel chat)
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
