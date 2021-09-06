using System;
using Microsoft.Toolkit.Uwp.UI.Controls;
using UWPX_UI.Controls;
using UWPX_UI_Context.Classes;
using UWPX_UI_Context.Classes.DataContext.Dialogs;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPX_UI.Dialogs
{
    public sealed partial class DeprecationInfoDialog: ContentDialog
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly DeprecationInfoDialogContext VIEW_MODEL = new DeprecationInfoDialogContext();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public DeprecationInfoDialog()
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
        private async void OnMarkdownLinkClicked(object sender, LinkClickedEventArgs e)
        {
            await UiUtils.LaunchUriAsync(new Uri(e.Link));
        }

        private async void OnMSStoreClicked(object sender, RoutedEventArgs e)
        {
            await UiUtils.LaunchUriAsync(new Uri(Localisation.GetLocalizedString("MSStoreLink_Url")));
        }

        private void OnCloseClicked(IconButtonControl sender, RoutedEventArgs args)
        {
            Hide();
        }

        #endregion
    }
}
