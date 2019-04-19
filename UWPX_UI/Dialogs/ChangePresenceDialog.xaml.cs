using UWPX_UI_Context.Classes.DataContext.Dialogs;
using Windows.UI.Xaml.Controls;

namespace UWPX_UI.Dialogs
{
    public sealed partial class ChangePresenceDialog: ContentDialog
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly ChangePresenceDialogContext VIEW_MODEL = new ChangePresenceDialogContext();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ChangePresenceDialog() => InitializeComponent();

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
        }

        private async void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            args.Cancel = true;
            IsEnabled = false;
            await VIEW_MODEL.SavePresenceAsync();
            IsEnabled = true;
            Hide();
        }

        private void AccountSelectionControl_AddAccountClick(Controls.AccountSelectionControl sender, System.ComponentModel.CancelEventArgs args)
        {
            Hide();
        }

        private void AccountSelectionControl_AccountSelectionChanged(Controls.AccountSelectionControl sender, Classes.Events.AccountSelectionChangedEventArgs args)
        {
            VIEW_MODEL.MODEL.Client = args.CLIENT;
        }

        #endregion
    }
}
