using System;
using Microsoft.Toolkit.Uwp.UI.Controls;
using UWPX_UI.Extensions;
using UWPX_UI_Context.Classes;
using UWPX_UI_Context.Classes.DataContext.Controls;
using UWPX_UI_Context.Classes.DataContext.Pages;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace UWPX_UI.Pages.Settings
{
    public sealed partial class AccountsSettingsPage: Page
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly AccountSettingsPageContext VIEW_MODEL = new AccountSettingsPageContext();
        public AccountsListControlContext accountsListViewModel = null;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public AccountsSettingsPage()
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

                    case "Push":
                        ScrollViewerExtensions.ScrollIntoViewVertically(main_scv, push_scp, false);
                        break;
                }
            }
        }

        private void Main_nview_Loaded(object sender, RoutedEventArgs e)
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

        private void AccountsListControl_Loaded(object sender, RoutedEventArgs e)
        {
            accountsListViewModel = accounts_alc.VIEW_MODEL;
        }

        private void AddAccount_ibtn_Click(Controls.IconButtonControl sender, RoutedEventArgs args)
        {
            UiUtils.NavigateToPage(typeof(RegisterPage));
        }

        private void ReconnectAll_ibtn_Click(object sender, RoutedEventArgs args)
        {
            reconnectAll_ibtn.ProgressRingVisibility = Visibility.Visible;
            reconnectAll_ibtn.IsEnabled = false;
            VIEW_MODEL.ReconnectAll();
            reconnectAll_ibtn.ProgressRingVisibility = Visibility.Collapsed;
            reconnectAll_ibtn.IsEnabled = true;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            titleBar.OnPageNavigatedTo();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            titleBar.OnPageNavigatedFrom();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            VIEW_MODEL.OnLoaded();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            VIEW_MODEL.OnUnloaded();
        }

        private async void RequestTestPush_btn_Click(object sender, RoutedEventArgs e)
        {
            requestTestPush_btn.IsEnabled = false;
            requestTestPush_btn.ProgressRingVisibility = Visibility.Visible;
            await VIEW_MODEL.RequestTestPushAsync();
            requestTestPush_btn.IsEnabled = true;
            requestTestPush_btn.ProgressRingVisibility = Visibility.Collapsed;
        }

        private void InitPush_btn_Click(object sender, RoutedEventArgs e)
        {
            VIEW_MODEL.InitPush();
        }

        private async void InitPushForAccounts_btn_Click(object sender, RoutedEventArgs e)
        {
            initPushForAccounts_btn.IsEnabled = false;
            initPushForAccounts_btn.ProgressRingVisibility = Visibility.Visible;
            await VIEW_MODEL.InitPushForAccountsAsync();
            initPushForAccounts_btn.IsEnabled = true;
            initPushForAccounts_btn.ProgressRingVisibility = Visibility.Collapsed;
        }

        private async void MarkdownTextBlock_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            await UiUtils.LaunchUriAsync(new Uri(e.Link));
        }

        #endregion
    }
}
