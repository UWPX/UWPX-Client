using UWPX_UI.Extensions;
using UWPX_UI_Context.Classes.DataContext;
using Windows.UI.Xaml.Controls;

namespace UWPX_UI.Pages.Settings
{
    public sealed partial class AccountsSettingsPage : Page
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly AccountSettingsPageContext VIEW_MODEL = new AccountSettingsPageContext();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public AccountsSettingsPage()
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
                    case "Accounts":
                        ScrollViewerExtensions.ScrollIntoViewVertically(main_scv, accounts_scp, false);
                        break;

                    case "Manage":
                        ScrollViewerExtensions.ScrollIntoViewVertically(main_scv, manage_scp, false);
                        break;
                }
            }
        }

        private void Main_nview_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            foreach (object item in main_nview.MenuItems)
            {
                if (item is Microsoft.UI.Xaml.Controls.NavigationViewItem navItem && string.Equals((string)navItem.Tag, "Manage"))
                {
                    main_nview.SelectedItem = item;
                    break;
                }
            }
        }

        #endregion
    }
}
