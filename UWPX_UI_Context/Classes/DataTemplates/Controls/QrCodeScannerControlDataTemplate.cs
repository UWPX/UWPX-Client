using System.Text.RegularExpressions;
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
        private Regex _ValidQrCodeRegex;
        public Regex ValidQrCodeRegex
        {
            get => _ValidQrCodeRegex;
            set => SetProperty(ref _ValidQrCodeRegex, value);
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
        public QrCodeScannerControlDataTemplate()
        {
            HasAccess = true;
            Loading = true;

            // By default every QR Code is valid:
            ValidQrCodeRegex = new Regex(".*");
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
