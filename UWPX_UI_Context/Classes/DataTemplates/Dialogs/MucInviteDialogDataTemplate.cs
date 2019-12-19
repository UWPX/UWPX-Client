using Shared.Classes;

namespace UWPX_UI_Context.Classes.DataTemplates.Dialogs
{
    public class MucInviteDialogDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private bool _IsInputValid;
        public bool IsInputValid
        {
            get => _IsInputValid;
            set => SetIsInputValidProperty(value);
        }

        private string _TargetBareJid;
        public string TargetBareJid
        {
            get => _TargetBareJid;
            set => SetProperty(ref _TargetBareJid, value);
        }

        private string _Reason;
        public string Reason
        {
            get => _Reason;
            set => SetProperty(ref _Reason, value);
        }

        private bool _IncludePassword;
        public bool IncludePassword
        {
            get => _IncludePassword;
            set => SetProperty(ref _IncludePassword, value);
        }

        private bool _RoomIsPasswordProtected;
        public bool RoomIsPasswordProtected
        {
            get => _RoomIsPasswordProtected;
            set => SetProperty(ref _RoomIsPasswordProtected, value);
        }

        private string _Title;
        public string Title
        {
            get => _Title;
            set => SetProperty(ref _Title, value);
        }

        private bool _IsInviting;
        public bool IsInviting
        {
            get => _IsInviting;
            set => SetIsInvitingProperty(value);
        }

        private bool _CanInvite;
        public bool CanInvite
        {
            get => _CanInvite;
            set => SetProperty(ref _CanInvite, value);
        }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private void SetIsInputValidProperty(bool value)
        {
            if (SetProperty(ref _IsInputValid, value, nameof(IsInputValid)))
            {
                CanInvite = IsInputValid && !IsInviting;
            }
        }

        private void SetIsInvitingProperty(bool value)
        {
            if (SetProperty(ref _IsInviting, value, nameof(IsInviting)))
            {
                CanInvite = IsInputValid && !IsInviting;
            }
        }

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
