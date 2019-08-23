using System.Text.RegularExpressions;
using UWPX_UI_Context.Classes.DataTemplates.Controls;
using Windows.UI.Xaml;

namespace UWPX_UI_Context.Classes.DataContext.Controls
{
    public class QrCodeScannerControlContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly QrCodeScannerControlDataTemplate MODEL = new QrCodeScannerControlDataTemplate();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void UpdateView(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is string s)
            {
                MODEL.ValidQrCodeRegex = new Regex(s);
            }
            else
            {
                // Fall back to every QR Code is valid:
                MODEL.ValidQrCodeRegex = new Regex(".*");
            }
        }

        public bool IsValidQrCode(string text)
        {
            return MODEL.ValidQrCodeRegex.IsMatch(text);
        }

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
