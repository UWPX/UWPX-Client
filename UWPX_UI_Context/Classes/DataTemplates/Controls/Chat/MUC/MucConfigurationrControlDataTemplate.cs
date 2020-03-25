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

        private bool _IsEnabled;
        public bool IsEnabled
        {
            get => _IsEnabled;
            set => SetProperty(ref _IsEnabled, value);
        }

        private DataFormDataTemplate _Form;
        public DataFormDataTemplate Form
        {
            get => _Form;
            set => SetProperty(ref _Form, value);
        }

        private bool _HasError;
        public bool HasError
        {
            get => _HasError;
            set => SetProperty(ref _HasError, value);
        }

        private string _ErrorText;
        public string ErrorText
        {
            get => _ErrorText;
            set => SetProperty(ref _ErrorText, value);
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
