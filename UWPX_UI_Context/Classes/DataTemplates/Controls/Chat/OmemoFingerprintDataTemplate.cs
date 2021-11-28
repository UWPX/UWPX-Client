using Omemo.Classes;
using Shared.Classes;
using Storage.Classes.Models.Omemo;

namespace UWPX_UI_Context.Classes.DataTemplates.Controls.Chat
{
    public class OmemoFingerprintDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private OmemoFingerprintModel _Fingerprint;
        public OmemoFingerprintModel Fingerprint
        {
            get => _Fingerprint;
            set => SetProperty(ref _Fingerprint, value);
        }

        private SessionState _State;
        public SessionState State
        {
            get => _State;
            set => SetProperty(ref _State, value);
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
