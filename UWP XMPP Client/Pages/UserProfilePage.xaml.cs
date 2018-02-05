using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Core;
using UWP_XMPP_Client.Classes.Events;
using XMPP_API.Classes;
using UWP_XMPP_Client.Classes;
using Data_Manager2.Classes.DBTables;
using Data_Manager2.Classes.DBManager;
using Windows.UI.Xaml;

namespace UWP_XMPP_Client.Pages
{
    public sealed partial class UserProfilePage : Page
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public ChatTable Chat
        {
            get { return (ChatTable)GetValue(chatProperty); }
            set { SetValue(chatProperty, value); }
        }
        public static readonly DependencyProperty chatProperty = DependencyProperty.Register("Chat", typeof(ChatTable), typeof(UserProfilePage), null);

        public XMPPClient Client
        {
            get { return (XMPPClient)GetValue(clientProperty); }
            set { SetValue(clientProperty, value); }
        }
        public static readonly DependencyProperty clientProperty = DependencyProperty.Register("Client", typeof(XMPPClient), typeof(UserProfilePage), null);

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 29/09/2017 Created [Fabian Sauter]
        /// </history>
        public UserProfilePage()
        {
            this.InitializeComponent();
            SystemNavigationManager.GetForCurrentView().BackRequested += UserProfilePage_BackRequested;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private void setChat(ChatTable chat)
        {
            this.Chat = chat;
            showProfile();
        }

        private void setClient(XMPPClient client)
        {
            this.Client = client;
            showClient();
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void showClient()
        {
            if(Client == null)
            {
                return;
            }
        }

        private void showProfile()
        {
            if (Chat == null)
            {
                return;
            }
            name_tblck.Text = Chat.chatJabberId;
            status_tblck.Text = Chat.status ?? "";
            account_tblck.Text = Chat.userAccountId ?? "";
            showSubscriptionStatus(Chat.subscription ?? Chat.ask ?? "none");
        }

        private void showSubscriptionStatus(string status)
        {
            subscriptionStatus_tblck.Text = status;
            switch (status)
            {
                case "both":
                    fromArrow_tblck.Visibility = Visibility.Visible;
                    toArrow_tblck.Visibility = Visibility.Visible;
                    break;

                case "from":
                    fromArrow_tblck.Visibility = Visibility.Visible;
                    toArrow_tblck.Visibility = Visibility.Collapsed;
                    break;

                case "to":
                    fromArrow_tblck.Visibility = Visibility.Collapsed;
                    toArrow_tblck.Visibility = Visibility.Visible;
                    break;

                case "none":
                default:
                    fromArrow_tblck.Visibility = Visibility.Collapsed;
                    toArrow_tblck.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if(e.Parameter is NavigatedToUserProfileEventArgs)
            {
                NavigatedToUserProfileEventArgs args = e.Parameter as NavigatedToUserProfileEventArgs;
                setChat(args.CHAT);
                setClient(args.CLIENT);
                ChatManager.INSTANCE.ChatChanged -= INSTANCE_ChatChanged;
                ChatManager.INSTANCE.ChatChanged += INSTANCE_ChatChanged;
            }
        }

        private async void INSTANCE_ChatChanged(ChatManager handler, Data_Manager.Classes.Events.ChatChangedEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if(Chat != null && Chat.id.Equals(args.CHAT.id))
                {
                    showProfile();
                }
            });
        }

        private void UserProfilePage_BackRequested(object sender, BackRequestedEventArgs e)
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

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            UiUtils.setBackgroundImage(backgroundImage_img);
        }

        #endregion
    }
}
