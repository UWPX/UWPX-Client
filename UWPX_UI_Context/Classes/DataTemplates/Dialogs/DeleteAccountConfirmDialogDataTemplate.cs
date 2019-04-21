using Shared.Classes;

namespace UWPX_UI_Context.Classes.DataTemplates.Dialogs
{
    public sealed class DeleteAccountConfirmDialogDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private bool _KeepChatMessages;
        public bool KeepChatMessages
        {
            get => _KeepChatMessages;
            set => SetProperty(ref _KeepChatMessages, value);
        }
        private bool _KeepChats;
        public bool KeepChats
        {
            get => _KeepChats;
            set => SetProperty(ref _KeepChats, value);
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
        public DeleteAccountConfirmDialogDataTemplate()
        {
            KeepChatMessages = false;
            KeepChats = false;
            Confirmed = false;
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
