using UWPX_UI.Extensions;
using UWPX_UI_Context.Classes;
using UWPX_UI_Context.Classes.DataContext.Pages;
using Windows.UI.Xaml.Controls;

namespace UWPX_UI.Pages.Settings
{
    public sealed partial class ChatSettingsPage : Page
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly ChatSettingsPageContext VIEW_MODEL = new ChatSettingsPageContext();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ChatSettingsPage()
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

                    case "MUC":
                        ScrollViewerExtensions.ScrollIntoViewVertically(main_scv, muc_scp, false);
                        break;

                    case "Media":
                        ScrollViewerExtensions.ScrollIntoViewVertically(main_scv, media_scp, false);
                        break;

                    case "OMEMO":
                        ScrollViewerExtensions.ScrollIntoViewVertically(main_scv, omemo_scp, false);
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

        private void ClearCache_hlb_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            UiUtils.NavigateToPage(typeof(MiscSettingsPage), "Cache");
        }

        private async void OmemoInfo_hlb_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await VIEW_MODEL.OnWhatIsOmemoClickedAsync();
        }

        #endregion
    }
}
