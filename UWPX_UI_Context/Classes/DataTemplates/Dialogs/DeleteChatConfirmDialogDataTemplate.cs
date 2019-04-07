using Data_Manager2.Classes;
using Data_Manager2.Classes.DBTables;
using Shared.Classes;

namespace UWPX_UI_Context.Classes.DataTemplates.Dialogs
{
    public sealed class DeleteChatConfirmDialogDataTemplate : AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private bool _KeepChatMessages;
        public bool KeepChatMessages
        {
            get { return _KeepChatMessages; }
            set { SetProperty(ref _KeepChatMessages, value); }
        }
        private bool _RemoveFromRoster;
        public bool RemoveFromRoster
        {
            get { return _RemoveFromRoster; }
            set { SetProperty(ref _RemoveFromRoster, value); }
        }
        private bool _RemoveFromRosterVisible;
        public bool RemoveFromRosterVisible
        {
            get { return _RemoveFromRosterVisible; }
            set { SetProperty(ref _RemoveFromRosterVisible, value); }
        }
        private bool _Confirmed;
        public bool Confirmed
        {
            get { return _Confirmed; }
            set { SetProperty(ref _Confirmed, value); }
        }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public DeleteChatConfirmDialogDataTemplate(ChatTable chat)
        {
            this.KeepChatMessages = false;
            this.Confirmed = false;
            this.RemoveFromRoster = chat.chatType == ChatType.CHAT;
            this.RemoveFromRosterVisible = this.RemoveFromRoster && chat.inRoster;
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
