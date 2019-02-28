using Data_Manager2.Classes;
using Data_Manager2.Classes.DBManager;
using System;
using System.Threading.Tasks;
using UWP_XMPP_Client.Classes;
using UWP_XMPP_Client.Dialogs;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
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


        #endregion

        #region --Misc Methods (Private)--
        private async Task acceptAsync()
        {
            accept_pgr.Visibility = Visibility.Visible;
            IsEnabled = false;
            if (await account_ac.isAccountVaildAsync())
            {
                XMPPAccount account = account_ac.getAccount();
                Task t = Task.Run(async () =>
                {
                    if (account != null)
                    {
                        account.generateOmemoKeys();
                        AccountDBManager.INSTANCE.setAccount(account, true);
                        await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, moveOn);
                    }
                    else
                    {
                        await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                        {
                            await showErrorDialogAsync(Localisation.getLocalizedString("invalid_jabber_id_text"));
                            accept_pgr.Visibility = Visibility.Collapsed;
                            IsEnabled = true;
                        });
                    }
                });
            }
            else
            {
                accept_pgr.Visibility = Visibility.Collapsed;
                IsEnabled = true;
            }
        }

        private async Task showErrorDialogAsync(string text)
        {
            TextDialog dialog = new TextDialog(text, Localisation.getLocalizedString("error_text"));
            await UiUtils.showDialogAsyncQueue(dialog);
        }

        private void moveOn()
        {
            Settings.setSetting(SettingsConsts.INITIALLY_STARTED, true);
            if (Window.Current.Content is Frame rootFrame && rootFrame.CanGoBack)
            {
                rootFrame.GoBack();
                return;
            }
            //(Window.Current.Content as Frame).Navigate(typeof(ChatPage), "AddAccountPage.xaml.cs");
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void cancel_btn_Click_1(object sender, RoutedEventArgs e)
        {
            moveOn();
        }

        private async void accept_btn_Click_1(object sender, RoutedEventArgs e)
        {
            await acceptAsync();
        }

        private void AbstractBackRequestPage_BackRequested(object sender, Windows.UI.Core.BackRequestedEventArgs e)
        {
            if (!(Window.Current.Content is Frame rootFrame))
            {
                return;
            }
            if (rootFrame.CanGoBack && e.Handled == false)
            {
                e.Handled = true;
                rootFrame.GoBack();
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.New && e.Parameter is string && (e.Parameter as string).Equals("App.xaml.cs"))
            {
                await UiUtils.showInitialStartDialogAsync();
            }
        }

        private async void account_ac_AccountAccepted(Controls.AccountControl sender, EventArgs args)
        {
            await acceptAsync();
        }

        #endregion
    }
}
