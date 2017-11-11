using Data_Manager.Classes;
using Data_Manager.Classes.Managers;
using System;
using System.Linq;
using System.Threading.Tasks;
using UWP_XMPP_Client.Classes;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using XMPP_API.Classes;
using XMPP_API.Classes.Network;

namespace UWP_XMPP_Client.Pages
{
    public sealed partial class AddAccountPage : Page
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
        /// 25/08/2017 Created [Fabian Sauter]
        /// </history>
        public AddAccountPage()
        {
            this.InitializeComponent();
            Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested += AbstractBackRequestPage_BackRequested;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public XMPPAccount getAccount()
        {
            XMPPAccount account = new XMPPAccount(getUser(), serverAddress_tbx.Text, int.Parse(serverPort_tbx.Text));
            account.presencePriorety = (int)presencePriorety_slider.Value;
            account.color = color_tbx.Text;
            return account;
        }

        public XMPPUser getUser()
        {
            string userId = jabberId_tbx.Text.Substring(0, jabberId_tbx.Text.IndexOf('@'));
            string domain = jabberId_tbx.Text.Substring(jabberId_tbx.Text.IndexOf('@') + 1);
            return new XMPPUser(userId, password_pwb.Password, domain, resource_tbx.Text);
        }

        #endregion

        #region --Misc Methods (Private)--
        private async Task<bool> areEntriesValidAsync()
        {
            if (!Utils.isValidJabberId(jabberId_tbx.Text))
            {
                MessageDialog messageDialog = new MessageDialog(Localisation.getLocalizedString("invalid_jabber_id_text"), Localisation.getLocalizedString("error_text"));
                await messageDialog.ShowAsync();
                return false;
            }
            if (resource_tbx.Text == "")
            {
                MessageDialog messageDialog = new MessageDialog(Localisation.getLocalizedString("invalid_resource_text"), Localisation.getLocalizedString("error_text"));
                await messageDialog.ShowAsync();
                return false;
            }
            if (serverAddress_tbx.Text == "")
            {
                MessageDialog messageDialog = new MessageDialog(Localisation.getLocalizedString("invalid_server_text"), Localisation.getLocalizedString("error_text"));
                await messageDialog.ShowAsync();
                return false;
            }
            if (serverPort_tbx.Text == "")
            {
                MessageDialog messageDialog = new MessageDialog(Localisation.getLocalizedString("invalid_port_text"), Localisation.getLocalizedString("error_text"));
                await messageDialog.ShowAsync();
                return false;
            }
            return true;
        }

        private async Task nextAsync()
        {
            if (await areEntriesValidAsync())
            {
                UserManager.INSTANCE.setAccount(getAccount());
                Settings.setSetting(SettingsConsts.INITIALLY_STARTED, true);
                if (Window.Current.Content is Frame rootFrame && rootFrame.CanGoBack)
                {
                    rootFrame.GoBack();
                    return;
                }
                (Window.Current.Content as Frame).Navigate(typeof(ChatPage));
            }
        }

        private void updateColor(string color)
        {
            if (UiUtils.isHexColor(color))
            {
                color_tbx.Header = "Hex color:";
                color_rcta.Fill = UiUtils.convertHexColorToBrush(color);
                color_rcta.Visibility = Visibility.Visible;
            }
            else
            {
                color_tbx.Header = "Hex color (invalid):";
                color_rcta.Fill = new SolidColorBrush(Colors.Transparent);
                color_rcta.Visibility = Visibility.Collapsed;
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void jabberId_tbx_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (jabberId_tbx.Text.Contains('@'))
            {
                serverAddress_tbx.Text = jabberId_tbx.Text.Substring(jabberId_tbx.Text.IndexOf('@') + 1);
            }
        }

        private void skip_btn_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void next_btn_Click(object sender, RoutedEventArgs e)
        {
            await nextAsync();
        }

        private async void resource_tbx_KeyUp(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                await nextAsync();
            }
        }

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

        private void color_tbx_TextChanged(object sender, TextChangedEventArgs e)
        {
            updateColor(color_tbx.Text);
        }

        private void randomColor_btn_Click(object sender, RoutedEventArgs e)
        {
            color_tbx.Text = UiUtils.getRandomMaterialColor();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            color_tbx.Text = UiUtils.getRandomMaterialColor();
            updateColor(color_tbx.Text);
        }
        #endregion
    }
}
