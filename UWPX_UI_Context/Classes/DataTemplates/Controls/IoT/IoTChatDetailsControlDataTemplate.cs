using Manager.Classes.Chat;
using Shared.Classes;

namespace UWPX_UI_Context.Classes.DataTemplates.Controls.IoT
{
    public sealed class IoTChatDetailsControlDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private bool _IsLoading;
        public bool IsLoading
        {
            get => _IsLoading;
            set => SetProperty(ref _IsLoading, value);
        }

        private DataFormDataTemplate _Form;
        public DataFormDataTemplate Form
        {
            get => _Form;
            set => SetProperty(ref _Form, value);
        }

        private ChatDataTemplate _Chat;
        public ChatDataTemplate Chat
        {
            get => _Chat;
            set => SetProperty(ref _Chat, value);
        }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public IoTChatDetailsControlDataTemplate()
        {
            IsLoading = true;
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
