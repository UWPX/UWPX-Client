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
        private bool _Confirmed;
        public bool Confirmed
        {
            get { return _Confirmed; }
            set { SetProperty(ref _Confirmed, value); }
        }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void OnYes()
        {
            Confirmed = true;
        }

        public void OnNo()
        {
            Confirmed = false;
        }

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
