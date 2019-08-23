using Shared.Classes;

namespace UWPX_UI_Context.Classes.DataTemplates.Dialogs
{
    public class QrCodeScannerDialogDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private bool _Success;
        public bool Success
        {
            get => _Success;
            set => SetProperty(ref _Success, value);
        }

        private string _QrCode;
        public string QrCode
        {
            get => _QrCode;
            set => SetProperty(ref _QrCode, value);
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
