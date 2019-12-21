using Data_Manager2.Classes;
using UWP_XMPP_Client.Pages;
using UWPX_UI.Dialogs;
using UWPX_UI.Pages.Settings;
using UWPX_UI_Context.Classes;
using UWPX_UI_Context.Classes.DataContext.Pages;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace UWPX_UI.Pages
{
    public sealed partial class ChatPage: Page
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly ChatsPageContext VIEW_MODEL = new ChatsPageContext();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ChatPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;
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
        #region --Master_Command_Bar--
        private void Master_cmdb_Opening(object sender, object e)
        {
            changePresence_abb.IsEnabled = ConnectionHandler.INSTANCE.getClients().Count > 0;
        }

        private async void AddChat_mfoi_Click(object sender, RoutedEventArgs e)
        {
            AddChatDialog dialog = new AddChatDialog();
            await UiUtils.ShowDialogAsync(dialog);
            await VIEW_MODEL.OnAddChatAsync(dialog.VIEW_MODEL.MODEL);
        }

        private void AddMix_mfoi_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Add MIX support.
        }

        private async void AddMuc_mfoi_Click(object sender, RoutedEventArgs e)
        {
            AddMucDialog dialog = new AddMucDialog();
            await UiUtils.ShowDialogAsync(dialog);
        }

        private void RegisterIoTDevice_mfoi_Click(object sender, RoutedEventArgs e)
        {
            UiUtils.NavigateToPage(typeof(RegisterIoTDevicePage));
        }

        private void Settings_abb_Click(object sender, RoutedEventArgs e)
        {
            UiUtils.NavigateToPage(typeof(SettingsPage));
        }

        private async void ChangePresence_abb_Click(object sender, RoutedEventArgs e)
        {
            ChangePresenceDialog dialog = new ChangePresenceDialog();
            await UiUtils.ShowDialogAsync(dialog);
        }

        private void ManageBookmarks_abb_Click(object sender, RoutedEventArgs e)
        {
            UiUtils.NavigateToPage(typeof(ManageBookmarksPage));
        }

        #endregion

        private void FilterClear_mfo_Click(object sender, RoutedEventArgs e)
        {
            VIEW_MODEL.MODEL.CHAT_FILTER.ClearFilter();
        }

        private void FilterChats_tabb_Click(object sender, RoutedEventArgs e)
        {
            filterChats_mfo.ShowAt(filterChats_tabb);
        }

        private void AddChat_abb_Click(object sender, RoutedEventArgs e)
        {
            addChat_mfo.ShowAt(addChat_abb);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            VIEW_MODEL.OnNavigatedFrom();
            titleBar.OnPageNavigatedFrom();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            VIEW_MODEL.OnNavigatedTo(e.Parameter);
            titleBar.OnPageNavigatedTo();
        }
        #endregion
    }
}
