using Manager.Classes.Chat;
using Shared.Classes;
using UWPX_UI_Context.Classes.DataTemplates.Controls.IoT;

namespace UWPX_UI_Context.Classes.DataTemplates.Controls.Chat.MUC
{
    public class MucConfigurationControlDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private bool _IsLoading;
        public bool IsLoading
        {
            get => _IsLoading;
            set => SetProperty(ref _IsLoading, value);
        }

        private bool _IsAvailable;
        public bool IsAvailable
        {
            get => _IsAvailable;
            set => SetProperty(ref _IsAvailable, value);
        }

        private bool _Success;
        public bool Success
        {
            get => _Success;
            set => SetProperty(ref _Success, value);
        }

        private DataFormDataTemplate _Form;
        public DataFormDataTemplate Form
        {
            get => _Form;
            set => SetProperty(ref _Form, value);
        }

        private string _ErrorMarkdownText;
        public string ErrorMarkdownText
        {
            get => _ErrorMarkdownText;
            set => SetProperty(ref _ErrorMarkdownText, value);
        }

        public ChatDataTemplate chat;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


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
