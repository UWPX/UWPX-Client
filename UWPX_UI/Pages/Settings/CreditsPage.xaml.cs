using System;
using UWPX_UI.Extensions;
using UWPX_UI_Context.Classes;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace UWPX_UI.Pages.Settings
{
    public sealed partial class CreditsPage: Page
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--


        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public CreditsPage()
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
        private async void MarkdownTextBlock_LinkClicked(object sender, Microsoft.Toolkit.Uwp.UI.Controls.LinkClickedEventArgs e)
        {
            await UiUtils.LaunchUriAsync(new Uri(e.Link));
        }

        private void Main_nview_SelectionChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem is Microsoft.UI.Xaml.Controls.NavigationViewItem item)
            {
                switch (item.Tag)
                {
                    case "General":
                        ScrollViewerExtensions.ScrollIntoViewVertically(main_scv, general_scp, false);
                        break;

                    case "UI":
                        ScrollViewerExtensions.ScrollIntoViewVertically(main_scv, ui_scp, false);
                        break;

                    case "Code":
                        ScrollViewerExtensions.ScrollIntoViewVertically(main_scv, code_scp, false);
                        break;

                    case "Misc":
                        ScrollViewerExtensions.ScrollIntoViewVertically(main_scv, misc_scp, false);
                        break;

                    case "Images":
                        ScrollViewerExtensions.ScrollIntoViewVertically(main_scv, images_scp, false);
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            titleBar.OnPageNavigatedTo();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            titleBar.OnPageNavigatedFrom();
        }

        #endregion
    }
}
