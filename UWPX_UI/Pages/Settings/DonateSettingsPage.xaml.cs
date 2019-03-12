using UWPX_UI.Extensions;
using UWPX_UI_Context.Classes.DataContext.Pages;
using Windows.UI.Xaml.Controls;

namespace UWPX_UI.Pages.Settings
{
    public sealed partial class DonateSettingsPage : Page
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly DonateSettingsPageContext VIEW_MODEL = new DonateSettingsPageContext();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public DonateSettingsPage()
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
        private void Main_nview_SelectionChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem is Microsoft.UI.Xaml.Controls.NavigationViewItem item)
            {
                switch (item.Tag)
                {
                    case "General":
                        ScrollViewerExtensions.ScrollIntoViewVertically(main_scv, general_scp, false);
                        break;

                    case "Bank transfer":
                        ScrollViewerExtensions.ScrollIntoViewVertically(main_scv, bankTransfer_scp, false);
                        break;
                }
            }
        }

        private void Main_nview_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            foreach (object item in main_nview.MenuItems)
            {
                if (item is Microsoft.UI.Xaml.Controls.NavigationViewItem navItem && string.Equals((string)navItem.Tag, "General"))
                {
                    main_nview.SelectedItem = item;
                    break;
                }
            }
        }

        private async void DonatePP_btn_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await VIEW_MODEL.DonateViaPayPalAsync();
        }

        private async void DonateLP_btn_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await VIEW_MODEL.DonateViaLiberapayAsync();
        }

        private async void SendMail_link_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await VIEW_MODEL.SendBankDetailsMailAsync();
        }

        #endregion
    }
}
