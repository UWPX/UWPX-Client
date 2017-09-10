using Data_Manager.Classes.Managers;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using XMPP_API.Classes;
using System;
using Data_Manager.Classes.DBEntries;
using Windows.UI.Xaml.Media;
using Windows.UI;

namespace UWP_XMPP_Client.Controls
{
    public sealed partial class ChatMasterControl : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public XMPPClient Client
        {
            get { return (XMPPClient)GetValue(ClientProperty); }
            set
            {
                SetValue(ClientProperty, value);
                showChatDescription();
                linkEvents();
            }
        }
        public static readonly DependencyProperty ClientProperty = DependencyProperty.Register("Client", typeof(XMPPClient), typeof(ChatMasterControl), null);

        public ChatEntry Chat
        {
            get { return (ChatEntry)GetValue(ChatProperty); }
            set
            {
                SetValue(ChatProperty, value);
                showChatDescription();
            }
        }
        public static readonly DependencyProperty ChatProperty = DependencyProperty.Register("Chat", typeof(ChatEntry), typeof(ChatMasterControl), null);
        
        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Construktoren--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 27/08/2017 Created [Fabian Sauter]
        /// </history>
        public ChatMasterControl()
        {
            this.InitializeComponent();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void showChatDescription()
        {
            if (Chat != null && Client != null)
            {
                // Chat name
                if (Chat.name == null)
                {
                    name_tblck.Text = Chat.id;
                }
                else
                {
                    name_tblck.Text = Chat.name + " (" + Chat.id + ')';
                }

                // Status icons
                lastChat_tblck.Text = ChatManager.INSTANCE.getLastChatMessageForChat(Chat) ?? "";
                muted_tbck.Visibility = Chat.muted ? Visibility.Visible : Visibility.Collapsed;
                inRooster_tbck.Visibility = Chat.inRooster ? Visibility.Visible : Visibility.Collapsed;

                // Subscription pending
                if(Chat.ask != null && Chat.ask.Equals("subscribe")) {
                    presence_tblck.Text = "Subscription pending...";
                }

                // subscription state
                requestPresenceSubscription_grid.Visibility = Visibility.Collapsed;
                removedPresenceSubscription_grid.Visibility = Visibility.Collapsed;
                switch (Chat.subscription)
                {
                    case "from":
                        subscription_tbck.Visibility = Visibility.Visible;
                        subscription_tbck.Foreground = new SolidColorBrush(Color.FromArgb(255, 235, 140, 16));
                        break;
                    case "both":
                        subscription_tbck.Visibility = Visibility.Visible;
                        subscription_tbck.Foreground = new SolidColorBrush(Color.FromArgb(255, 16, 124, 16));
                        break;
                    case "pending":
                        subscription_tbck.Visibility = Visibility.Visible;
                        subscription_tbck.Foreground = new SolidColorBrush(Color.FromArgb(255, 76, 74, 72));
                        break;
                    case "subscribe":
                        requestPresenceSubscription_grid.Visibility = Visibility.Visible;
                        subscription_tbck.Visibility = Visibility.Collapsed;
                        presenceSubscriptionRequestText_tblck.Text = Chat.status ?? (Chat.name ?? Chat.id) + "  has requested to subscribe to your presence!";
                        break;
                    case "unsubscribe":
                    case "remove":
                        removedPresenceSubscription_grid.Visibility = Visibility.Visible;
                        subscription_tbck.Visibility = Visibility.Collapsed;
                        presenceSubscriptionRemovedText_tblck.Text = (Chat.name ?? Chat.id) + " has removed you from his roster and/or has unsubscribed you from his presence. Do you like to unsubscribe him from yor presence?";
                        break;
                    default:
                        subscription_tbck.Visibility = Visibility.Collapsed;
                        break;
                }
            }
        }

        private void linkEvents()
        {
            if (Client != null)
            {
                Client.NewChatMessage += Client_NewChatMessage;
                Client.ConnectionStateChanged += Client_ConnectionStateChanged;
            }
        }
        
        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void Client_ConnectionStateChanged(XMPPClient client, XMPP_API.Classes.Network.ConnectionState state)
        {
        }

        private async void Client_NewChatMessage(XMPPClient client, XMPP_API.Classes.Network.Events.NewChatMessageEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                showChatDescription();
            });
        }

        private void Grid_RightTapped(object sender, Windows.UI.Xaml.Input.RightTappedRoutedEventArgs e)
        {
            Grid grid = (Grid)sender;
            menuFlyout.ShowAt(grid, e.GetPosition(grid));
            var a = ((FrameworkElement)e.OriginalSource).DataContext;
        }

        #endregion

        private void presenceSubscriptionAccept_btn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void presenceSubscriptionRefuse_btn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void presenceSubscriptionRemovedAccept_btn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void presenceSubscriptionRemovedRefuse_btn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
