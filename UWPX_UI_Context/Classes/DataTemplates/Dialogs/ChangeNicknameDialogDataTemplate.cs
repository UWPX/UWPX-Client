using Shared.Classes;

namespace UWPX_UI_Context.Classes.DataTemplates.Dialogs
{
    public class ChangeNicknameDialogDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private string _MucName;
        public string MucName
        {
            get => _MucName;
            set => SetProperty(ref _MucName, value);
        }

        private string _Nickname;
        public string Nickname
        {
            get => _Nickname;
            set => SetNicknameProperty(value);
        }

        private bool _IsSaving;
        public bool IsSaving
        {
            get => _IsSaving;
            set => SetIsSavingProperty(value);
        }

        private bool _IsSaveEnabled;
        public bool IsSaveEnabled
        {
            get => _IsSaveEnabled;
            set => SetProperty(ref _IsSaveEnabled, value);
        }

        private bool _Error;
        public bool Error
        {
            get => _Error;
            set => SetProperty(ref _Error, value);
        }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private void SetNicknameProperty(string value)
        {
            if (SetProperty(ref _Nickname, value, nameof(Nickname)))
            {
                IsSaveEnabled = !string.IsNullOrEmpty(value);
            }
        }

        private void SetIsSavingProperty(bool value)
        {
            if (SetProperty(ref _IsSaving, value, nameof(IsSaving)))
            {
                IsSaveEnabled = !value && !string.IsNullOrEmpty(Nickname);
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
