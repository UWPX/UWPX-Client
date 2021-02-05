using Shared.Classes;
using Storage.Classes.Models.Chat;

namespace UWPX_UI_Context.Classes.DataTemplates.Dialogs
{
    public sealed class DeleteChatConfirmDialogDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private bool _KeepChatMessages;
        public bool KeepChatMessages
        {
            get => _KeepChatMessages;
            set => SetProperty(ref _KeepChatMessages, value);
        }
        private bool _RemoveFromRoster;
        public bool RemoveFromRoster
        {
            get => _RemoveFromRoster;
            set => SetProperty(ref _RemoveFromRoster, value);
        }
        private bool _RemoveFromRosterVisible;
        public bool RemoveFromRosterVisible
        {
            get => _RemoveFromRosterVisible;
            set => SetProperty(ref _RemoveFromRosterVisible, value);
        }
        private bool _Confirmed;
        public bool Confirmed
        {
            get => _Confirmed;
            set => SetProperty(ref _Confirmed, value);
        }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public DeleteChatConfirmDialogDataTemplate(ChatModel chat)
        {
            KeepChatMessages = false;
            Confirmed = false;
            RemoveFromRoster = chat.chatType == ChatType.CHAT;
            RemoveFromRosterVisible = RemoveFromRoster && chat.inRoster;
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
