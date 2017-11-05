using Data_Manager.Classes.Managers;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using XMPP_API.Classes;
using System;
using Data_Manager.Classes.DBEntries;
using System.Threading.Tasks;
using Windows.UI.Popups;
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
                showChat();
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
                showChat();
                if (Chat != null)
                {
                    Chat.ChatChanged -= Chat_ChatChanged;
                    Chat.ChatChanged += Chat_ChatChanged;
                }
            }
        }
        public static readonly DependencyProperty ChatProperty = DependencyProperty.Register("Chat", typeof(ChatEntry), typeof(ChatMasterControl), null);

        private bool subscriptionRequest;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 27/08/2017 Created [Fabian Sauter]
        /// </history>
        public ChatMasterControl()
        {
            this.InitializeComponent();
            this.subscriptionRequest = false;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void showPresenceSubscriptionRequest()
        {
            accountAction_grid.Visibility = Visibility.Visible;
            accountAction_tblck.Text = Chat.status ?? (Chat.name ?? Chat.id) + "  has requested to subscribe to your presence!";
            accountActionRefuse_btn.Content = "Refuse";
            accountActionAccept_btn.Content = "Accept";
            subscriptionRequest = true;
        }

        private void showRemovedChat()
        {
            accountAction_grid.Visibility = Visibility.Visible;
            accountAction_tblck.Text = (Chat.name ?? Chat.id) + " has removed you from his roster and/or has unsubscribed you from his presence. Do you like to unsubscribe him from your presence?";
            accountActionRefuse_btn.Content = "Keep";
            accountActionAccept_btn.Content = "Remove";
            subscriptionRequest = false;
        }

        private void showChat()
        {
            if (Chat != null && Client != null)
            {
                // Chat name:
                if (Chat.name == null)
                {
                    name_tblck.Text = Chat.id;
                }
                else
                {
                    name_tblck.Text = Chat.name + " (" + Chat.id + ')';
                }

                // Last action date:
                if (Chat.lastActive != null)
                {
                    if (Chat.lastActive.Date.CompareTo(DateTime.Now.Date) == 0)
                    {
                        lastAction_tblck.Text = Chat.lastActive.ToString("HH:mm");
                    }
                    else
                    {
                        lastAction_tblck.Text = Chat.lastActive.ToString("dd.MM.yyyy");
                    }
                }
                else
                {
                    lastAction_tblck.Text = "";
                }

                // Last chat message:
                showLastChatMessage(ChatManager.INSTANCE.getLastChatMessageForChat(Chat));

                // Status icons:
                muted_tbck.Visibility = Chat.muted ? Visibility.Visible : Visibility.Collapsed;
                inRooster_tbck.Visibility = Chat.inRoster ? Visibility.Visible : Visibility.Collapsed;

                // Subscription state:
                accountAction_grid.Visibility = Visibility.Collapsed;
                requestPresenceSubscription_mfo.Visibility = Visibility.Collapsed;
                cancelPresenceSubscription_mfo.Visibility = Visibility.Collapsed;
                rejectPresenceSubscription_mfo.Visibility = Visibility.Collapsed;
                cancelPresenceSubscriptionRequest.Visibility = Visibility.Collapsed;

                switch (Chat.subscription)
                {
                    case "to":
                        cancelPresenceSubscription_mfo.Visibility = Visibility.Visible;
                        break;
                    case "both":
                        cancelPresenceSubscription_mfo.Visibility = Visibility.Visible;
                        rejectPresenceSubscription_mfo.Visibility = Visibility.Visible;
                        break;
                    case "pending":
                        cancelPresenceSubscriptionRequest.Visibility = Visibility.Visible;
                        break;
                    case "subscribe":
                        showPresenceSubscriptionRequest();
                        break;
                    case "unsubscribe":
                        requestPresenceSubscription_mfo.Visibility = Visibility.Visible;
                        showRemovedChat();
                        break;
                    case "from":
                        requestPresenceSubscription_mfo.Visibility = Visibility.Visible;
                        rejectPresenceSubscription_mfo.Visibility = Visibility.Visible;
                        break;
                    case "none":
                    default:
                        requestPresenceSubscription_mfo.Visibility = Visibility.Visible;
                        break;
                }

                // Chat status:
                image_aciwp.Presence = Chat.presence;
            }

            // Subscription pending:
            if (Chat.ask != null && Chat.ask.Equals("subscribe"))
            {
                presence_tblck.Visibility = Visibility.Visible;
                cancelPresenceSubscription_mfo.Visibility = Visibility.Visible;
                requestPresenceSubscription_mfo.Visibility = Visibility.Collapsed;
            }
            else
            {
                presence_tblck.Visibility = Visibility.Collapsed;
            }

            // Menu Flyout:
            mute_tmfo.Text = Chat.muted ? "Unmute" : "Mute";
            mute_tmfo.IsChecked = Chat.muted;
            removeFromRoster_mfo.Text = Chat.inRoster ? "Remove from roster" : "Add to roster";
        }

        private void linkEvents()
        {
            if (Client != null)
            {
                ChatManager.INSTANCE.NewChatMessage -= INSTANCE_NewChatMessage;
                ChatManager.INSTANCE.NewChatMessage += INSTANCE_NewChatMessage;
                Client.ConnectionStateChanged -= Client_ConnectionStateChanged;
                Client.ConnectionStateChanged += Client_ConnectionStateChanged;
                Client.NewPresence -= Client_NewPresence;
                Client.NewPresence += Client_NewPresence;
            }
        }

        private void showLastChatMessage(ChatMessageEntry chatMessage)
        {
            if (chatMessage != null)
            {
                switch (chatMessage.state)
                {
                    case Data_Manager.Classes.MessageState.UNREAD:
                        lastChat_tblck.Foreground = new SolidColorBrush((Color)Resources["SystemAccentColor"]);
                        break;
                    case Data_Manager.Classes.MessageState.SENDING:
                    case Data_Manager.Classes.MessageState.SEND:
                    case Data_Manager.Classes.MessageState.READ:
                    default:
                        lastChat_tblck.Foreground = (SolidColorBrush)Resources["SystemControlBackgroundBaseMediumBrush"];
                        break;
                }
                lastChat_tblck.Text = chatMessage.message ?? "";
            }
            else
            {
                lastChat_tblck.Text = "";
            }
        }

        private async void INSTANCE_NewChatMessage(ChatManager handler, Data_Manager.Classes.Events.NewChatMessageEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (Chat.id.Equals(args.getMessage().chatId))
                {
                    showLastChatMessage(args.getMessage());
                }
            });
        }

        private async Task removeChatRequestClickedAsync(bool remove)
        {
            if (remove)
            {
                MessageDialog dialog = new MessageDialog("Do you also want to delete all chat messages from this chat?");
                dialog.Commands.Add(new UICommand { Label = "No", Id = 0 });
                dialog.Commands.Add(new UICommand { Label = "Yes", Id = 1 });
                IUICommand command = await dialog.ShowAsync();
                if ((int)command.Id == 1)
                {
                    ChatManager.INSTANCE.deleteChat(Chat, true);
                    return;
                }
                ChatManager.INSTANCE.deleteChat(Chat, false);
            }
            else
            {
                Chat.subscription = "none";
                ChatManager.INSTANCE.setChatEntry(Chat, false);
                showChat();
            }
        }

        private async Task presenceSubscriptionRequestClickedAsync(bool accepted)
        {
            await Client.answerPresenceSubscriptionRequest(Chat.id, accepted);
            Chat.ask = null;
            ChatManager.INSTANCE.setChatEntry(Chat, false);
            showChat();
        }

        private async Task<bool> showShouldRemoveChat()
        {
            MessageDialog dialog = new MessageDialog("Do you really want to delete this chat?");
            dialog.Commands.Add(new UICommand { Label = "No", Id = 0 });
            dialog.Commands.Add(new UICommand { Label = "Yes", Id = 1 });
            IUICommand command = await dialog.ShowAsync();
            return (int)command.Id == 1;
        }

        private void resetAsk()
        {
            Chat.ask = null;
            ChatManager.INSTANCE.setChatEntry(Chat, true);
            showChat();
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void Client_ConnectionStateChanged(XMPPClient client, XMPP_API.Classes.Network.ConnectionState state)
        {
        }

        private void Grid_RightTapped(object sender, Windows.UI.Xaml.Input.RightTappedRoutedEventArgs e)
        {
            Grid grid = (Grid)sender;
            menuFlyout.ShowAt(grid, e.GetPosition(grid));
            var a = ((FrameworkElement)e.OriginalSource).DataContext;
        }

        private void mute_tmfo_Click(object sender, RoutedEventArgs e)
        {
            Chat.muted = mute_tmfo.IsChecked;
            ChatManager.INSTANCE.setChatEntry(Chat, false);
            showChat();
        }

        private async void accountActionAccept_btn_Click(object sender, RoutedEventArgs e)
        {
            if (subscriptionRequest)
            {
                await presenceSubscriptionRequestClickedAsync(true);
            }
            else
            {
                await removeChatRequestClickedAsync(true);
            }
        }

        private async void accountActionRefuse_btn_Click(object sender, RoutedEventArgs e)
        {
            if (subscriptionRequest)
            {
                await presenceSubscriptionRequestClickedAsync(false);
            }
            else
            {
                await removeChatRequestClickedAsync(false);
            }
        }

        private async void deleteChat_mfo_Click(object sender, RoutedEventArgs e)
        {
            if (await showShouldRemoveChat())
            {
                if (Chat.inRoster)
                {
                    await Client.removeFromRosterAsync(Chat.id);
                }
                await removeChatRequestClickedAsync(true);
            }
        }

        private async void removeFromRoster_mfo_Click(object sender, RoutedEventArgs e)
        {
            if (Chat.inRoster)
            {
                await Client.removeFromRosterAsync(Chat.id);
            }
            else
            {
                await Client.addToRosterAsync(Chat.id);
            }
        }

        private async void Chat_ChatChanged(object sender, EventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                showChat();
            });
        }

        private async void Client_NewPresence(XMPPClient client, XMPP_API.Classes.Events.NewPresenceEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (args.getFrom().Equals(Chat.id))
                {
                    image_aciwp.Presence = args.getPresence();
                }
            });
        }

        private async void requestPresenceSubscription_mfo_Click(object sender, RoutedEventArgs e)
        {
            await Client.requestPresenceSubscriptionAsync(Chat.id);
        }

        private async void cancelPresenceSubscription_mfo_Click(object sender, RoutedEventArgs e)
        {
            await Client.unsubscribeFromPresence(Chat.id);
            resetAsk();
        }

        private async void rejectPresenceSubscription_mfo_Click(object sender, RoutedEventArgs e)
        {
            await Client.answerPresenceSubscriptionRequest(Chat.id, false);
            resetAsk();
        }

        private async void cancelPresenceSubscriptionRequest_Click(object sender, RoutedEventArgs e)
        {
            await Client.unsubscribeFromPresence(Chat.id);
            resetAsk();
        }

        #endregion
    }
}
