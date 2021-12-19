using UWPX_UI.Controls;
using UWPX_UI_Context.Classes.DataContext.Dialogs;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace UWPX_UI.Dialogs
{
    public sealed partial class EditProfileDialog: ContentDialog
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly EditProfileDialogContext VIEW_MODEL = new EditProfileDialogContext();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public EditProfileDialog()
        {
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
        private void OnCancelClicked(IconButtonControl sender, RoutedEventArgs args)
        {
            Hide();
        }

        private void OnSaveClicked(IconProgressButtonControl sender, RoutedEventArgs args)
        {
            Hide();
        }

        private async void OnEditAvatarClicked(object sender, RoutedEventArgs e)
        {
            await VIEW_MODEL.ChangeAvatarAsync();
        }

        private async void OnPersonPictureTapped(object sender, TappedRoutedEventArgs e)
        {
            await VIEW_MODEL.ChangeAvatarAsync();
        }

        private async void OnRemoveAvatarClicked(object sender, RoutedEventArgs e)
        {
            await VIEW_MODEL.RemoveAvatarAsync();
        }

        private void OnAddAccountClicked(AccountSelectionControl sender, System.ComponentModel.CancelEventArgs args)
        {
            Hide();
        }

        private void OnAccountSelectionChanged(AccountSelectionControl sender, Classes.Events.AccountSelectionChangedEventArgs args)
        {
            VIEW_MODEL.MODEL.Client = args.CLIENT;
        }

        #endregion
    }
}
