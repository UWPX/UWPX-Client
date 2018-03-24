using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using XMPP_API.Classes;
using System;
using System.Threading.Tasks;
using XMPP_API.Classes.Network.XML.Messages;
using UWP_XMPP_Client.Classes;
using UWP_XMPP_Client.Pages;
using Microsoft.Toolkit.Uwp.UI.Controls;
using UWP_XMPP_Client.Classes.Events;
using Data_Manager2.Classes;
using XMPP_API.Classes.Network.XML.Messages.XEP_0085;
using Data_Manager2.Classes.DBTables;
using Data_Manager2.Classes.DBManager;
using UWP_XMPP_Client.DataTemplates;
using System.Collections.Generic;
using Windows.UI.Xaml.Input;
using Data_Manager2.Classes.Events;

namespace UWP_XMPP_Client.Controls
{
    public sealed partial class ChatDetailsControl : UserControl
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
                    Client.NewChatState -= Client_NewChatState;
                }
                SetValue(ClientProperty, value);
                showClient();
                if (Client != null)
                {
                    Client.NewChatState -= Client_NewChatState;
                    Client.NewChatState += Client_NewChatState;
                }
            }
        }
        public static readonly DependencyProperty ClientProperty = DependencyProperty.Register("Client", typeof(XMPPClient), typeof(ChatDetailsControl), null);

        public ChatTable Chat
        {
            get { return (ChatTable)GetValue(ChatProperty); }
            set
            {
                showChatMessages(value, Chat);
                SetValue(ChatProperty, value);
                showChat();
                showMUCInfo();
            }
        }
        public static readonly DependencyProperty ChatProperty = DependencyProperty.Register("Chat", typeof(ChatTable), typeof(ChatDetailsControl), null);

        public MUCChatInfoTable MUCInfo
        {
            get { return (MUCChatInfoTable)GetValue(MUCInfoProperty); }
            set
            {
                SetValue(MUCInfoProperty, value);
                showMUCInfo();
            }
        }
        public static readonly DependencyProperty MUCInfoProperty = DependencyProperty.Register("MUCInfo", typeof(MUCChatInfoTable), typeof(ChatDetailsControl), null);

        private CustomObservableCollection<ChatMessageDataTemplate> chatMessages;

        private static readonly char[] TRIM_CHARS = new char[] { ' ', '\t', '\n', '\r' };

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 29/08/2017 Created [Fabian Sauter]
        /// </history>
        public ChatDetailsControl()
        {
            this.chatMessages = new CustomObservableCollection<ChatMessageDataTemplate>();
            this.InitializeComponent();

            // Disable the test button on release builds:
#if !DEBUG
            test_bnt.Visibility = Visibility.Collapsed;
#endif
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private string getChatType()
        {
            switch (Chat.chatType)
            {
                case ChatType.CHAT:
                    return MessageMessage.TYPE_CHAT;
                case ChatType.MUC:
                    return MessageMessage.TYPE_GROUPCHAT;
                default:
                    // For backwards compatibility with older versions of the app:
                    return MessageMessage.TYPE_CHAT;
            }
        }

        private bool shouldSendChatState()
        {
            return !Settings.getSettingBoolean(SettingsConsts.DONT_SEND_CHAT_STATE) && Chat != null && Chat.chatType == ChatType.CHAT;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void showChatMessages(ChatTable newChat, ChatTable oldChat)
        {
            if (newChat != null)
            {
                // Only show chat messages if the chat changed:
                if (oldChat != null && Equals(newChat.id, oldChat.id))
                {
                    return;
                }

                ChatTable newChat_cpy = newChat.clone();

                // Show all chat messages:
                Task.Run(async () =>
                {
                    List<ChatMessageDataTemplate> msgs = new List<ChatMessageDataTemplate>();
                    foreach (ChatMessageTable msg in ChatDBManager.INSTANCE.getAllChatMessagesForChat(newChat_cpy.id))
                    {
                        msgs.Add(new ChatMessageDataTemplate()
                        {
                            message = msg,
                            chat = newChat_cpy
                        });
                    }
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        chatMessages.Clear();
                        chatMessages.AddRange(msgs);
                    });
                });

                // Mark all unread messages as read for this chat:
                Task.Run(() => ChatDBManager.INSTANCE.markAllMessagesAsRead(newChat_cpy));
            }
        }

        private void showChat()
        {
            if (Chat != null)
            {
                switch (Chat.chatType)
                {
                    case ChatType.CHAT:
                        chatName_tblck.Text = Chat.chatJabberId ?? "";
                        chatState_tblck.Text = Chat.chatState ?? "";
                        join_mfo.Visibility = Visibility.Collapsed;
                        leave_mfo.Visibility = Visibility.Collapsed;
                        break;
                }
            }
        }

        private void showClient()
        {
            if (Client != null)
            {
                accountName_tblck.Text = Client.getXMPPAccount().getIdAndDomain();
            }
        }

        private void showMUCInfo()
        {
            if (MUCInfo != null && Chat != null && Equals(MUCInfo.chatId, Chat.id))
            {
                chatName_tblck.Text = string.IsNullOrWhiteSpace(MUCInfo.name) ? Chat.chatJabberId : MUCInfo.name;
                chatState_tblck.Text = MUCInfo.subject ?? "";
            }
        }

        private async Task sendMessageAsync()
        {
            if (!String.IsNullOrWhiteSpace(message_tbx.Text))
            {
                MessageMessage sendMessage;

                string messageText = message_tbx.Text;
                // Remove all tailing whitespaces, tabs and newlines:
                messageText = messageText.TrimEnd(TRIM_CHARS);

                // For MUC messages also pass the nickname:
                if (Chat.chatType == ChatType.MUC && MUCInfo != null)
                {
                    sendMessage = await Client.sendAsync(Chat.chatJabberId, messageText, getChatType(), MUCInfo.nickname);
                }
                else
                {
                    sendMessage = await Client.sendAsync(Chat.chatJabberId, messageText, getChatType());
                }
                ChatDBManager.INSTANCE.setChatMessage(new ChatMessageTable(sendMessage, Chat) { state = MessageState.SENDING }, true, false);
                Chat.lastActive = DateTime.Now;
                ChatDBManager.INSTANCE.setChat(Chat, false, false);

                message_tbx.Text = "";
            }
        }

        private void showBackgroundForViewState(MasterDetailsViewState state)
        {
            backgroundImage_img.Visibility = state == MasterDetailsViewState.Both ? Visibility.Collapsed : Visibility.Visible;
        }

        private void storeChatState(ChatState state)
        {
            switch (state)
            {
                case ChatState.ACTIVE:
                    Chat.chatState = "Active";
                    break;
                case ChatState.COMPOSING:
                    Chat.chatState = "Typing...";
                    break;
                case ChatState.PAUSED:
                    Chat.chatState = "Paused";
                    break;
                case ChatState.INACTIVE:
                    Chat.chatState = "Inactive";
                    break;
                case ChatState.GONE:
                    Chat.chatState = "Gone";
                    break;
                default:
                    Chat.chatState = "";
                    break;
            }
            chatState_tblck.Text = Chat.chatState;
        }

        private void showProfile()
        {
            if (Chat != null && Chat.chatType == ChatType.MUC)
            {
                (Window.Current.Content as Frame).Navigate(typeof(MUCInfoPage), new NavigatedToMUCInfoEventArgs(Chat, Client, MUCInfo));
            }
            else
            {
                (Window.Current.Content as Frame).Navigate(typeof(UserProfilePage), new NavigatedToUserProfileEventArgs(Chat, Client));
            }
        }

        private async Task leaveRoomAsync()
        {
            if (Client != null && MUCInfo != null && Chat != null)
            {
                await MUCHandler.INSTANCE.leaveRoomAsync(Client, Chat, MUCInfo);
            }
        }

        private async Task joinRoomAsync()
        {
            if (Client != null && MUCInfo != null && Chat != null)
            {
                await MUCHandler.INSTANCE.enterMUCAsync(Client, Chat, MUCInfo);
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private async void INSTANCE_NewChatMessage(ChatDBManager handler, NewChatMessageEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                ChatMessageTable msg = args.MESSAGE;
                if (Chat != null && Chat.id.Equals(msg.chatId))
                {
                    msg.state = MessageState.READ;
                    ChatDBManager.INSTANCE.setChatMessage(msg, false, true);
                    chatMessages.Add(new ChatMessageDataTemplate()
                    {
                        message = msg,
                        chat = Chat
                    });
                }
            });
        }

        private async void send_btn_Click(object sender, RoutedEventArgs e)
        {
            await sendMessageAsync();
        }

        private void invertedListView_lstv_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            invertedListView_lstv.SelectedIndex = -1;
        }

        private void clip_btn_Click(object sender, RoutedEventArgs e)
        {
            //TODO Add clip menu
        }

        private void message_tbx_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(message_tbx.Text))
            {
                send_btn.IsEnabled = false;
            }
            else
            {
                send_btn.IsEnabled = true;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            UiUtils.setBackgroundImage(backgroundImage_img);
            object o = (Window.Current.Content as Frame).Content;
            if (o is ChatPage)
            {
                ChatPage chatPage = o as ChatPage;
                MasterDetailsView masterDetailsView = chatPage.getMasterDetailsView();
                if (masterDetailsView != null)
                {
                    masterDetailsView.ViewStateChanged -= MasterDetailsView_ViewStateChanged;
                    masterDetailsView.ViewStateChanged += MasterDetailsView_ViewStateChanged;
                    showBackgroundForViewState(masterDetailsView.ViewState);
                }
            }

            // Subscribe to chat and chat message changed events:
            ChatDBManager.INSTANCE.NewChatMessage -= INSTANCE_NewChatMessage;
            ChatDBManager.INSTANCE.NewChatMessage += INSTANCE_NewChatMessage;
            ChatDBManager.INSTANCE.ChatMessageChanged += INSTANCE_ChatMessageChanged;
            ChatDBManager.INSTANCE.ChatMessageChanged -= INSTANCE_ChatMessageChanged;
        }

        private void MasterDetailsView_ViewStateChanged(object sender, MasterDetailsViewState e)
        {
            showBackgroundForViewState(e);
        }

        private async void Client_NewChatState(XMPPClient client, XMPP_API.Classes.Network.Events.NewChatStateEventArgs args)
        {
            if (args.Cancel)
            {
                return;
            }
            string chatId = ChatTable.generateId(args.FROM, args.TO);
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (string.Equals(chatId, Chat.id))
                {
                    storeChatState(args.STATE);
                    args.Cancel = true;
                }
            });
        }

        private async void message_tbx_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                if (Settings.getSettingBoolean(SettingsConsts.ENTER_TO_SEND_MESSAGES))
                {
                    await sendMessageAsync();
                }
                else
                {
                    int selectionStart = message_tbx.SelectionStart;
                    message_tbx.Text += "\r";
                    message_tbx.SelectionStart = selectionStart + 1;
                }
                e.Handled = true;
            }
        }

        private async void test_bnt_Click(object sender, RoutedEventArgs e)
        {
            //await Logging.Logger.openLogFolderAsync();
            //await Client.requestVCardAsync(Chat.chatJabberId);
            //await Client.createDiscoAsync(Client.getXMPPAccount().user.domain, XMPP_API.Classes.Network.XML.Messages.XEP_0030.DiscoType.ITEMS);
            //await Client.createDiscoAsync(Client.getXMPPAccount().user.domain, XMPP_API.Classes.Network.XML.Messages.XEP_0030.DiscoType.INFO);
        }

        private void message_tbx_GotFocus(object sender, RoutedEventArgs e)
        {
            if (shouldSendChatState())
            {
                Task t = Client.sendChatStateAsync(Chat.chatJabberId, ChatState.COMPOSING);
            }
        }

        private void message_tbx_LostFocus(object sender, RoutedEventArgs e)
        {
            if (shouldSendChatState())
            {
                Task t = Client.sendChatStateAsync(Chat.chatJabberId, ChatState.ACTIVE);
            }
        }

        private async void INSTANCE_ChatMessageChanged(ChatDBManager handler, ChatMessageChangedEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (Chat != null && Equals(args.MESSAGE.chatId, Chat.id))
                {
                    Task.Run(async () =>
                    {
                        for (int i = 0; i < chatMessages.Count; i++)
                        {
                            if (chatMessages[i].message != null && Equals(chatMessages[i].message.id, args.MESSAGE.id))
                            {
                                // Only the main thread should update the list to prevent problems:
                                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => chatMessages[i].message = args.MESSAGE);
                            }
                        }
                    });
                }
            });
        }

        private void AccountImageWithPresenceControl_Tapped(object sender, TappedRoutedEventArgs e)
        {
            showProfile();
        }

        private void StackPanel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            showProfile();
        }

        private async void leave_mfo_Click(object sender, RoutedEventArgs e)
        {
            await leaveRoomAsync();
        }

        private async void join_mfo_Click(object sender, RoutedEventArgs e)
        {
            await joinRoomAsync();
        }

        private void info_mfo_Click(object sender, RoutedEventArgs e)
        {
            showProfile();
        }

        private void MenuFlyout_Opening(object sender, object e)
        {
            if (MUCInfo != null)
            {
                switch (MUCInfo.state)
                {
                    case MUCState.ERROR:
                    case MUCState.DISCONNECTED:
                    case MUCState.KICKED:
                    case MUCState.BANED:
                        join_mfo.Visibility = Visibility.Visible;
                        leave_mfo.Visibility = Visibility.Collapsed;
                        break;

                    case MUCState.DISCONNECTING:
                        join_mfo.Visibility = Visibility.Collapsed;
                        leave_mfo.Visibility = Visibility.Collapsed;
                        break;

                    case MUCState.ENTERING:
                    case MUCState.ENTERD:
                        join_mfo.Visibility = Visibility.Collapsed;
                        leave_mfo.Visibility = Visibility.Visible;
                        break;
                }
            }
            else
            {
                join_mfo.Visibility = Visibility.Collapsed;
                leave_mfo.Visibility = Visibility.Collapsed;
            }
        }

        #endregion
    }
}
