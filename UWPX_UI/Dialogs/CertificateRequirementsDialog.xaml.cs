using UWPX_UI.Controls;
using UWPX_UI_Context.Classes.DataContext.Dialogs;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using XMPP_API.Classes.Network;

namespace UWPX_UI.Dialogs
{
    public sealed partial class CertificateRequirementsDialog: ContentDialog
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly CertificateRequirementsDialogContext VIEW_MODEL;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public CertificateRequirementsDialog(XMPPAccount account)
        {
            VIEW_MODEL = new CertificateRequirementsDialogContext(account);
            InitializeComponent();
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
        private void accept_btn_Click(IconButtonControl sender, RoutedEventArgs args)
        {
            VIEW_MODEL.Confirm();
            Hide();
        }

        private void cancel_btn_Click(IconButtonControl sender, RoutedEventArgs args)
        {
            VIEW_MODEL.Cancel();
            Hide();
        }

        #endregion
    }
}
