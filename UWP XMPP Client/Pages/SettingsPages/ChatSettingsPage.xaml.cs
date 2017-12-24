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
            storeImagesInLibary_tgls.IsOn = !Settings.getSettingBoolean(SettingsConsts.DISABLE_DOWNLOAD_IMAGES_TO_LIBARY);
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void AbstractBackRequestPage_BackRequested(object sender, Windows.UI.Core.BackRequestedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame == null)
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
        #endregion
    }
}
