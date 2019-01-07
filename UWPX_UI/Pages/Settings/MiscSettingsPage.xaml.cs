using UWPX_UI.Extensions;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace UWPX_UI.Pages.Settings
{
    public sealed partial class MiscSettingsPage : Page
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--


        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public MiscSettingsPage()
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
        private void MoreInformation_hlb_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }

        private void OpenAppDataFolder_btn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Main_nview_SelectionChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem is Microsoft.UI.Xaml.Controls.NavigationViewItem item)
            {
                switch (item.Tag)
                {
                    case "Logs":
                        ScrollViewerExtensions.ScrollIntoViewVertically(main_scv, logs_scp, false);
                        break;

                    case "Cache":
                        ScrollViewerExtensions.ScrollIntoViewVertically(main_scv, cache_scp, false);
                        break;

                    case "Analytics":
                        ScrollViewerExtensions.ScrollIntoViewVertically(main_scv, analytics_scp, false);
                        break;

                    case "Misc":
                        ScrollViewerExtensions.ScrollIntoViewVertically(main_scv, misc_scp, false);
                        break;

                    case "About":
                        ScrollViewerExtensions.ScrollIntoViewVertically(main_scv, about_scp, false);
                        break;
                }
            }
        }

        private void Main_nview_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (object item in main_nview.MenuItems)
            {
                if (item is Microsoft.UI.Xaml.Controls.NavigationViewItem navItem && string.Equals(navItem.Tag, "Logs"))
                {
                    main_nview.SelectedItem = item;
                    break;
                }
            }
        }

        #endregion
    }
}
