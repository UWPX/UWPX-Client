using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using XMPP_API.Classes;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI;
using UWP_XMPP_Client.Classes;
using Data_Manager2.Classes.DBTables;
using Data_Manager2.Classes.DBManager;
using UWP_XMPP_Client.Dialogs;
using UWP_XMPP_Client.Pages;
using UWP_XMPP_Client.Classes.Events;
using XMPP_API.Classes.Network.XML.Messages.XEP_0048_1_0;

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
                if (Client != null)
                {
                    ChatManager.INSTANCE.NewChatMessage -= INSTANCE_NewChatMessage;
                }
                SetValue(ClientProperty, value);
                if (Client != null)
                {
                    ChatManager.INSTANCE.NewChatMessage -= INSTANCE_NewChatMessage;
                    ChatManager.INSTANCE.NewChatMessage += INSTANCE_NewChatMessage;
                }
                showChat();
            }
        }
        public static readonly DependencyProperty ClientProperty = DependencyProperty.Register("Client", typeof(XMPPClient), typeof(ChatMasterControl), null);

        public ChatTable Chat
        {
            get { return (ChatTable)GetValue(ChatProperty); }
            set
            {
                SetValue(ChatProperty, value);
                showChat();
            }
        }
        public static readonly DependencyProperty ChatProperty = DependencyProperty.Register("Chat", typeof(ChatTable), typeof(ChatMasterControl), null);

        public MUCChatInfoTable MUCInfo
        {
            get { return (MUCChatInfoTable)GetValue(MUCInfoProperty); }
            set
            {
                SetValue(MUCInfoProperty, value);
                showMUCInfo();
            }
        }
        public static readonly DependencyProperty MUCInfoProperty = DependencyProperty.Register("MUCInfo", typeof(MUCChatInfoTable), typeof(ChatMasterControl), null);

        private bool subscriptionRequest;
        private ChatMessageTable lastChatMessage;

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
            this.lastChatMessage = null;
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
            accountAction_tblck.Text = Chat.status ?? Chat.chatJabberId + "  has requested to subscribe to your presence!";
            accountActionRefuse_btn.Content = "Refuse";
            accountActionAccept_btn.Content = "Accept";
            subscriptionRequest = true;
        }

        private void showRemovedChat()
        {
            accountAction_grid.Visibility = Visibility.Visible;
            accountAction_tblck.Text = Chat.chatJabberId + " has removed you from his roster and/or has unsubscribed you from his presence. Do you = to unsubscribe him from your presence?";
            accountActionRefuse_btn.Content = "Keep";
            accountActionAccept_btn.Content = "Remove";
            subscriptionRequest = false;
        }

        private void showMUCInfo()
        {
            if (MUCInfo != null && Chat != null)
            {
                // Chat jabber id:
                name_tblck.Text = string.IsNullOrWhiteSpace(MUCInfo.name) ? Chat.chatJabberId : MUCInfo.name;

                // Menu Flyout:
                muteMUC_tmfo.Text = Chat.muted ? "Unmute" : "Mute";
                muteMUC_tmfo.IsChecked = Chat.muted;
                bookmark_tmfo.Text = Chat.inRoster ? "Remove bookmark" : "Bookmark";
                autoEnter_tmfo.IsChecked = autoEnter_tmfo.IsChecked = MUCInfo.autoEnterRoom;
            }
        }

        private void showChat()
        {
            if (Chat != null && Client != null)
            {
                if (Chat.chatType != Data_Manager2.Classes.ChatType.MUC)
                {
                    // Chat jabber id:
                    name_tblck.Text = Chat.chatJabberId;

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

                    // Menu Flyout:
                    mute_tmfo.Text = Chat.muted ? "Unmute" : "Mute";
                    mute_tmfo.IsChecked = Chat.muted;
                    removeFromRoster_mfo.Text = Chat.inRoster ? "Remove from roster" : "Add to roster";
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

                // Last action date:
                if (Chat.lastActive != null)
                {
                    DateTime lastActiveLocal = Chat.lastActive.ToLocalTime();
                    if (lastActiveLocal.Date.CompareTo(DateTime.Now.Date) == 0)
                    {
                        lastAction_tblck.Text = lastActiveLocal.ToString("HH:mm");
                    }
                    else
                    {
                        lastAction_tblck.Text = lastActiveLocal.ToString("dd.MM.yyyy");
                    }
                }
                else
                {
                    lastAction_tblck.Text = "";
                }

                // Chat color:
                if (UiUtils.isHexColor(Client.getXMPPAccount().color))
                {
                    color_rcta.Fill = UiUtils.convertHexColorToBrush(Client.getXMPPAccount().color);
                }
                else
                {
                    color_rcta.Fill = new SolidColorBrush(Colors.Transparent);
                }

                // Last chat message:
                showLastChatMessage(ChatManager.INSTANCE.getLastChatMessageForChat(Chat.id));

                // Status icons:
                muted_tbck.Visibility = Chat.muted ? Visibility.Visible : Visibility.Collapsed;
                inRooster_tbck.Visibility = Chat.inRoster ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void showLastChatMessage(ChatMessageTable chatMessage)
        {
            // Remove the event subscription for the last message:
            if (lastChatMessage != null)
            {
                lastChatMessage.ChatMessageChanged -= ChatMessage_ChatMessageChanged;
            }

            if (chatMessage != null)
            {
                lastChatMessage = chatMessage;
                chatMessage.ChatMessageChanged += ChatMessage_ChatMessageChanged;
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
                if (Chat.id.Equals(args.MESSAGE.chatId))
                {
                    showLastChatMessage(args.MESSAGE);
                }
            });
        }

        private async Task presenceSubscriptionRequestClickedAsync(bool accepted)
        {
            await Client.answerPresenceSubscriptionRequest(Chat.chatJabberId, accepted);
            Chat.ask = null;
            ChatManager.INSTANCE.setChat(Chat, false, false);
        }

        private void resetAsk()
        {
            Chat.ask = null;
            ChatManager.INSTANCE.setChat(Chat, false, false);
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void Grid_RightTapped(object sender, Windows.UI.Xaml.Input.RightTappedRoutedEventArgs e)
        {
            if (Chat != null)
            {
                Grid grid = (Grid)sender;
                switch (Chat.chatType)
                {
                    case Data_Manager2.Classes.ChatType.CHAT:
                        chat_mfo.ShowAt(grid, e.GetPosition(grid));
                        break;
                    case Data_Manager2.Classes.ChatType.MUC:
                        muc_mfo.ShowAt(grid, e.GetPosition(grid));
                        break;
                    default:
                        break;
                }
            }
        }

        private void mute_tmfo_Click(object sender, RoutedEventArgs e)
        {
            if (Chat != null && Chat.muted != mute_tmfo.IsChecked)
            {
                Chat.muted = mute_tmfo.IsChecked;
                ChatManager.INSTANCE.setChat(Chat, false, false);
            }
        }

        private void muteMUC_tmfo_Click(object sender, RoutedEventArgs e)
        {
            if (Chat != null && Chat.muted != muteMUC_tmfo.IsChecked)
            {
                Chat.muted = muteMUC_tmfo.IsChecked;
                ChatManager.INSTANCE.setChat(Chat, false, false);
            }
        }

        private async void accountActionAccept_btn_Click(object sender, RoutedEventArgs e)
        {
            if (subscriptionRequest)
            {
                await presenceSubscriptionRequestClickedAsync(false);
            }
            else
            {
                Chat.subscription = "none";
                ChatManager.INSTANCE.setChat(Chat, false, false);
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
                Chat.subscription = "none";
                ChatManager.INSTANCE.setChat(Chat, false, false);
            }
        }

        private async void deleteChat_mfo_Click(object sender, RoutedEventArgs e)
        {
            DeleteChatDialog deleteChatDialog = new DeleteChatDialog();
            await deleteChatDialog.ShowAsync();
            if (deleteChatDialog.deleteChat)
            {
                if (Chat.inRoster)
                {
                    await Client.removeFromRosterAsync(Chat.chatJabberId);
                }
                if (Chat.chatType == Data_Manager2.Classes.ChatType.MUC && MUCInfo != null)
                {
                    // ToDo remove MUC from bookmarks
                    ChatManager.INSTANCE.setMUCChatInfo(MUCInfo, true, false);
                }
                ChatManager.INSTANCE.setChat(Chat, true, true);
                if (!deleteChatDialog.keepChat)
                {
                    ChatManager.INSTANCE.deleteAllChatMessagesForAccount(Chat);
                }
            }
        }

        private async void removeFromRoster_mfo_Click(object sender, RoutedEventArgs e)
        {
            if (Chat.inRoster)
            {
                await Client.removeFromRosterAsync(Chat.chatJabberId);
            }
            else
            {
                await Client.addToRosterAsync(Chat.chatJabberId);
            }
        }

        private async void requestPresenceSubscription_mfo_Click(object sender, RoutedEventArgs e)
        {
            await Client.requestPresenceSubscriptionAsync(Chat.chatJabberId);
        }

        private async void cancelPresenceSubscription_mfo_Click(object sender, RoutedEventArgs e)
        {
            await Client.unsubscribeFromPresence(Chat.chatJabberId);
            resetAsk();
        }

        private async void rejectPresenceSubscription_mfo_Click(object sender, RoutedEventArgs e)
        {
            await Client.answerPresenceSubscriptionRequest(Chat.chatJabberId, false);
            resetAsk();
        }

        private async void cancelPresenceSubscriptionRequest_Click(object sender, RoutedEventArgs e)
        {
            await Client.unsubscribeFromPresence(Chat.chatJabberId);
            resetAsk();
        }

        private void ChatMessage_ChatMessageChanged(object sender, EventArgs e)
        {
            showLastChatMessage(lastChatMessage);
        }

        private async void INSTANCE_ChatMessageChanged(ChatManager handler, Data_Manager.Classes.Events.ChatMessageChangedEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (Chat != null && lastChatMessage != null && Equals(args.MESSAGE.chatId, Chat.id) && Equals(lastChatMessage.id, args.MESSAGE.id))
                {
                    showLastChatMessage(args.MESSAGE);
                }
            });
        }

        private void autoEnter_tmfo_Click(object sender, RoutedEventArgs e)
        {
            if (MUCInfo != null && MUCInfo.autoEnterRoom != autoEnter_tmfo.IsChecked)
            {
                MUCInfo.autoEnterRoom = autoEnter_tmfo.IsChecked;
                ConferenceItem cI = null;
                if (Chat.inRoster)
                {
                    cI = MUCInfo.toConferenceItem(Chat);
                }

                Task.Factory.StartNew(async () =>
                {
                    ChatManager.INSTANCE.setMUCChatInfo(MUCInfo, false, false);
                    if (cI != null)
                    {
                        await Client.setBookmarkAsync(cI);
                    }
                });
            }
        }

        private void showInfo_mfo_Click(object sender, RoutedEventArgs e)
        {
        }

        private void showProfile_mfo_Click(object sender, RoutedEventArgs e)
        {
            (Window.Current.Content as Frame).Navigate(typeof(UserProfilePage), new NavigatedToUserProfileEventArgs(Chat, Client));
        }

        private void bookmark_tmfo_Click(object sender, RoutedEventArgs e)
        {
            if (MUCInfo != null && Chat != null)
            {
                Chat.inRoster = !Chat.inRoster;
                if (Chat.inRoster)
                {
                    ConferenceItem cI = MUCInfo.toConferenceItem(Chat);
                    Task t = Client.setBookmarkAsync(cI);
                }
                // ToDo remove MUC from bookmarks
                ChatManager.INSTANCE.setChat(Chat, false, false);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Subscribe to the chat message changed event:
            ChatManager.INSTANCE.ChatMessageChanged -= INSTANCE_ChatMessageChanged;
            ChatManager.INSTANCE.ChatMessageChanged += INSTANCE_ChatMessageChanged;
        }

        #endregion
    }
}
