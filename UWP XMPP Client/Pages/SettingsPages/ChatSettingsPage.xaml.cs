using Data_Manager2.Classes;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWP_XMPP_Client.Pages.SettingsPages
{
    public sealed partial class ChatSettingsPage : Page
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--


        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 29/01/2017 Created [Fabian Sauter]
        /// </history>
        public ChatSettingsPage()
        {
            this.InitializeComponent();
            Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested += AbstractBackRequestPage_BackRequested;
            loadSettings();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void loadSettings()
        {
            enterToSend_tgls.IsOn = Settings.getSettingBoolean(SettingsConsts.ENTER_TO_SEND_MESSAGES);
            sendChatState_tgls.IsOn = !Settings.getSettingBoolean(SettingsConsts.DONT_SEND_CHAT_STATE);
            sendChatMessageReceivedMarkers_tgls.IsOn = !Settings.getSettingBoolean(SettingsConsts.DONT_SEND_CHAT_MESSAGE_RECEIVED_MARKERS);
            storeImagesInLibary_tgls.IsOn = !Settings.getSettingBoolean(SettingsConsts.DISABLE_DOWNLOAD_IMAGES_TO_LIBARY);
            autoJoinMUC_tgls.IsOn = !Settings.getSettingBoolean(SettingsConsts.DISABLE_AUTO_JOIN_MUC);
            advancedChatMsgProcessing_tgls.IsOn = !Settings.getSettingBoolean(SettingsConsts.DISABLE_ADVANCED_CHAT_MESSAGE_PROCESSING);
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void AbstractBackRequestPage_BackRequested(object sender, Windows.UI.Core.BackRequestedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame is null)
            {
                return;
            }
            if (rootFrame.CanGoBack && e.Handled == false)
            {
                e.Handled = true;
                rootFrame.GoBack();
            }
        }

        private void enterToSend_tgls_Toggled(object sender, RoutedEventArgs e)
        {
            Settings.setSetting(SettingsConsts.ENTER_TO_SEND_MESSAGES, enterToSend_tgls.IsOn);
        }

        private void sendChatState_tgls_Toggled(object sender, RoutedEventArgs e)
        {
            Settings.setSetting(SettingsConsts.DONT_SEND_CHAT_STATE, !sendChatState_tgls.IsOn);
        }

        private void storeImagesInLibary_tgls_Toggled(object sender, RoutedEventArgs e)
        {
            Settings.setSetting(SettingsConsts.DISABLE_DOWNLOAD_IMAGES_TO_LIBARY, !storeImagesInLibary_tgls.IsOn);
        }

        private void clearCache_hlb_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            (Window.Current.Content as Frame).Navigate(typeof(MiscSettingsPage));
        }

        private void autoJoinMUC_tgls_Toggled(object sender, RoutedEventArgs e)
        {
            Settings.setSetting(SettingsConsts.DISABLE_AUTO_JOIN_MUC, !autoJoinMUC_tgls.IsOn);
        }

        private void sendChatMarkers_tgls_Toggled(object sender, RoutedEventArgs e)
        {
            Settings.setSetting(SettingsConsts.DONT_SEND_CHAT_MESSAGE_RECEIVED_MARKERS, !sendChatMessageReceivedMarkers_tgls.IsOn);
        }

        private void AdvancedChatMsgProcessing_tgls_Toggled(object sender, RoutedEventArgs e)
        {
            Settings.setSetting(SettingsConsts.DISABLE_ADVANCED_CHAT_MESSAGE_PROCESSING, !advancedChatMsgProcessing_tgls.IsOn);
        }
        #endregion
    }
}
