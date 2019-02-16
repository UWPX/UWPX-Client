using UWPX_UI.Pages.Settings;
using UWPX_UI_Context.Classes.DataContext.Pages;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPX_UI.Pages
{
    public sealed partial class ChatPage : Page
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly ChatsPageContext VIEW_MODEL = new ChatsPageContext();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ChatPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Required;
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

        }

        private void AddChat_mfoi_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AddMix_mfoi_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void AddMuc_mfoi_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void Settings_abb_Click(object sender, RoutedEventArgs e)
        {
            (Window.Current.Content as Frame).Navigate(typeof(SettingsPage));
        }

        private void ChangePresence_abb_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ManageBookmarks_abb_Click(object sender, RoutedEventArgs e)
        {

        }

        #endregion

        private void FilterClear_mfo_Click(object sender, RoutedEventArgs e)
        {
            VIEW_MODEL.CHAT_FILTER.ClearFilter();
        }

        private void FilterChats_tabb_Click(object sender, RoutedEventArgs e)
        {
            filterChats_mfo.ShowAt(filterChats_tabb);
        }

        private void AddChat_abb_Click(object sender, RoutedEventArgs e)
        {
            addChat_mfo.ShowAt(addChat_abb);
        }

        #endregion
    }
}
