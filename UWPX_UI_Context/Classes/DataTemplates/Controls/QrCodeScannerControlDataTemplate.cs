using Shared.Classes;

namespace UWPX_UI_Context.Classes.DataTemplates.Controls
{
    public class QrCodeScannerControlDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private bool _Loading;
        public bool Loading
        {
            get => _Loading;
            set => SetProperty(ref _Loading, value);
        }
        private bool _HasAccess;
        public bool HasAccess
        {
            get => _HasAccess;
            set => SetProperty(ref _HasAccess, value);
        }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public QrCodeScannerControlDataTemplate()
        {
            HasAccess = true;
            Loading = true;
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
