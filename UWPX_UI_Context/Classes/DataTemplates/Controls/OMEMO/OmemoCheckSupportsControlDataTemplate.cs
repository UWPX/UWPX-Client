using Manager.Classes.Chat;
using Shared.Classes;

namespace UWPX_UI_Context.Classes.DataTemplates.Controls.OMEMO
{
    public enum OmemoSupportedStatus
    {
        UNKNOWN,
        CHECKING,
        SUPPORTED,
        ERROR,
        OLD_VERSION,
        UNSUPPORTED
    }

    public class OmemoCheckSupportsControlDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private OmemoSupportedStatus _Status;
        public OmemoSupportedStatus Status
        {
            get => _Status;
            set => SetProperty(ref _Status, value);
        }

        private ChatDataTemplate _Chat;
        public ChatDataTemplate Chat
        {
            get => _Chat;
            set => SetProperty(ref _Chat, value);
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
        public OmemoCheckSupportsControlDataTemplate()
        {
            Status = OmemoSupportedStatus.UNKNOWN;
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
