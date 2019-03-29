using UWPX_UI_Context.Classes.DataContext.Dialogs;
using Windows.UI.Xaml.Controls;

namespace UWPX_UI.Dialogs
{
    public sealed partial class AddChatDialog : ContentDialog
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly AddChatDialogContext VIEW_MODEL = new AddChatDialogContext();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public AddChatDialog()
        {
            this.InitializeComponent();
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
        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            VIEW_MODEL.OnCancel();
            Hide();
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            VIEW_MODEL.OnConfirm();
            Hide();
        }

        private void AccountSelectionControl_AddAccountClick(Controls.AccountSelectionControl sender, System.ComponentModel.CancelEventArgs args)
        {
            VIEW_MODEL.OnCancel();
            Hide();
        }

        private void AccountSelectionControl_AccountSelectionChanged(Controls.AccountSelectionControl sender, Classes.Events.AccountSelectionChangedEventArgs args)
        {

        }

        #endregion        
    }
}
