using System;
using Microsoft.UI.Xaml.Controls;
using UWPX_UI.Extensions;
using UWPX_UI_Context.Classes;
using UWPX_UI_Context.Classes.DataContext.Pages;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace UWPX_UI.Pages.Settings
{
    public sealed partial class ChatSettingsPage: Page
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly ChatSettingsPageContext VIEW_MODEL = new ChatSettingsPageContext();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ChatSettingsPage()
        {
            InitializeComponent();
            UiUtils.ApplyBackground(this);
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

                    case "MAM":
                        ScrollViewerExtensions.ScrollIntoViewVertically(main_scv, mam_scp, false);
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            titleBar.OnPageNavigatedTo();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            titleBar.OnPageNavigatedFrom();
        }

        private void MamDaysNumberBox_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            if (args.NewValue < -1)
            {
                sender.Value = -1;
            }
            else if (args.NewValue > 999)
            {
                sender.Value = 999;
            }
            else if (double.IsNaN(args.NewValue))
            {
                sender.Value = 30;
            }
        }

        #endregion
    }
}
