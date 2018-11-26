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
using Windows.UI.Xaml.Media;
using Windows.UI;
using Windows.UI.Xaml.Controls.Primitives;
using XMPP_API.Classes.Network.XML.Messages.XEP_0384;
using Data_Manager2.Classes.Toast;
using UWP_XMPP_Client.Classes.Collections;
using System.ComponentModel;

namespace UWP_XMPP_Client.Controls.Chat
{
    public sealed partial class ChatDetailsControl : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public bool IsDummy
        {
            get { return (bool)GetValue(IsDummyProperty); }
            set
            {
                SetValue(IsDummyProperty, value);
                if (!IsDummy)
                {
                    showChatMessages(Chat);
                }
            }
        }
        public static readonly DependencyProperty IsDummyProperty = DependencyProperty.Register(nameof(IsDummy), typeof(bool), typeof(ChatDetailsControl), null);

        public ChatTemplate ChatTemp
        {
            get { return (ChatTemplate)GetValue(ChatTempProperty); }
            set
            {
                ChatTemplate cur = (ChatTemplate)GetValue(ChatTempProperty);
                if (value != cur)
                {
                    if (cur != null)
                    {
                        cur.PropertyChanged -= Value_PropertyChanged;
                    }
                    if (value != null)
                    {
                        value.PropertyChanged += Value_PropertyChanged;
                    }
                    SetValue(ChatTempProperty, value);

                    onChatTemplateChanged(value);
                }
            }
        }
        public static readonly DependencyProperty ChatTempProperty = DependencyProperty.Register(nameof(ChatTemp), typeof(ChatTemplate), typeof(ChatDetailsControl), new PropertyMetadata(null));

        private ChatTable Chat
        {
            get { return ChatTemp?.chat; }
            set { if (ChatTemp == null) { throw new InvalidOperationException("Can't set ChatTemp.chat - ChatTemp is null in ChatDetailsControl."); } ChatTemp.chat = value; }
        }

        private XMPPClient Client
        {
            get { return ChatTemp?.client; }
            set { if (ChatTemp == null) { throw new InvalidOperationException("Can't set ChatTemp.client - ChatTemp is null in ChatDetailsControl."); } ChatTemp.client = value; }
        }

        private MUCChatInfoTable MUCInfo
        {
            get { return ChatTemp?.mucInfo; }
            set { if (ChatTemp == null) { throw new InvalidOperationException("Can't set ChatTemp.MUCInfo - ChatTemp is null in ChatDetailsControl."); } ChatTemp.mucInfo = value; }
        }

        private readonly CustomObservableCollection<ChatMessageDataTemplate> CHAT_MESSAGES;

        private int sendDummyMessages;
        private string curChatId;
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
            this.sendDummyMessages = 0;
            this.CHAT_MESSAGES = new CustomObservableCollection<ChatMessageDataTemplate>();
            this.curChatId = null;
            this.InitializeComponent();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private string getChatType()
        {
            switch (Chat.chatType)
            {
                case ChatType.MUC:
                    return MessageMessage.TYPE_GROUPCHAT;
                case ChatType.CHAT:
                default:
                    // For backwards compatibility with older versions of the app:
                    return MessageMessage.TYPE_CHAT;
            }
        }

        private bool shouldSendChatState()
        {
            return !IsDummy && !Settings.getSettingBoolean(SettingsConsts.DONT_SEND_CHAT_STATE) && Chat != null && Chat.chatType == ChatType.CHAT;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void loadBackgrundImage()
        {
            UiUtils.setBackgroundImage(backgroundImage_img);
        }

        public void loadDummyContent()
        {
            ChatTemp = new ChatTemplate
            {
                chat = new ChatTable
                {
                    chatJabberId = "dave@example.com",
                    userAccountId = "kevin@example.com",
                    chatType = ChatType.CHAT,
                    presence = Presence.Away
                }
            };

            addDummyMessage("Hi", Chat.userAccountId, MessageState.READ);
            addDummyMessage("Hey, what's up?", Chat.chatJabberId, MessageState.SEND);
            addDummyMessage("That's a great app.", Chat.userAccountId, MessageState.READ);
            message_tbx.Text = "Yes, its awesome :D !";

            invertedListView_lstv.Visibility = Visibility.Visible;
            loading_ldng.IsLoading = false;
        }

        #endregion

        #region --Misc Methods (Private)--
        private void onChatTemplateChanged(ChatTemplate chatTemp)
        {
            showClient(chatTemp.client);
            showChat(chatTemp.chat);
            showMUCInfo(chatTemp.mucInfo, chatTemp.chat);

            if (!IsDummy)
            {
                showChatMessages(Chat);
            }
        }

        private void addDummyMessage(string msg, string fromUser, MessageState state)
        {
            addDummyMessage(msg, fromUser, state, false);
        }

        private void addDummyMessage(string msg, string fromUser, MessageState state, bool isImage)
        {
            CHAT_MESSAGES.Add(new ChatMessageDataTemplate
            {
                chat = Chat,
                message = new ChatMessageTable
                {
                    message = msg,
                    chatId = Chat.id,
                    fromUser = fromUser,
                    date = DateTime.Now,
                    state = state,
                    type = MessageMessage.TYPE_CHAT,
                    isImage = isImage,
                    isDummyMessage = true
                }
            });
        }

        private void showChatMessages(ChatTable chat)
        {
            if (chat != null)
            {
                // Only show chat messages if the chat changed:
                if (curChatId != null && Equals(chat.id, curChatId))
                {
                    return;
                }
                curChatId = chat.id;

                // Show loading:
                loading_ldng.IsLoading = true;
                invertedListView_lstv.Visibility = Visibility.Collapsed;

                // Create a copy to prevent multi threading issues:
                ChatTable chatCpy = chat;

                Task.Run(async () =>
                {
                    // Show all chat messages:
                    List<ChatMessageDataTemplate> msgs = new List<ChatMessageDataTemplate>();
                    foreach (ChatMessageTable msg in ChatDBManager.INSTANCE.getAllChatMessagesForChat(chatCpy.id))
                    {
                        msgs.Add(new ChatMessageDataTemplate
                        {
                            message = msg,
                            chat = chatCpy
                        });
                    }

                    // Mark all unread messages as read for this chat:
                    ChatDBManager.INSTANCE.markAllMessagesAsRead(chatCpy.id);
                    // Remove notification group:
                    ToastHelper.removeToastGroup(chatCpy.id);

                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        CHAT_MESSAGES.Clear();
                        CHAT_MESSAGES.AddRange(msgs);
                        invertedListView_lstv.Visibility = Visibility.Visible;
                        loading_ldng.IsLoading = false;
                    });
                });
            }
        }

        private void showChat(ChatTable chat)
        {
            if (chat != null)
            {
                if (chat.chatType == ChatType.CHAT)
                {
                    chatName_tblck.Text = chat.chatJabberId ?? "";
                    chatState_tblck.Text = chat.chatState ?? "";
                    join_mfo.Visibility = Visibility.Collapsed;
                    leave_mfo.Visibility = Visibility.Collapsed;
                }

                omemoIndicator_tbx.Visibility = chat.omemoEnabled ? Visibility.Visible : Visibility.Collapsed;
                omemo_tmfo.IsChecked = chat.omemoEnabled;
            }
        }

        private void showClient(XMPPClient client)
        {
            if (client != null)
            {
                accountName_tblck.Text = client.getXMPPAccount().getIdAndDomain();
            }
        }

        private void showMUCInfo(MUCChatInfoTable mucInfo, ChatTable chat)
        {
            if (mucInfo != null && chat != null && Equals(mucInfo.chatId, chat.id))
            {
                chatName_tblck.Text = string.IsNullOrWhiteSpace(mucInfo.name) ? chat.chatJabberId : mucInfo.name;
                chatState_tblck.Text = mucInfo.subject ?? "";
            }
        }

        private void sendBotMessage(string text, bool isImage, int millisecondsDelay)
        {
            storeChatState(ChatState.COMPOSING);
            Task.Run(async () =>
            {
                await Task.Delay(millisecondsDelay);
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    addDummyMessage(text, Chat.chatJabberId, MessageState.READ, isImage);
                    storeChatState(ChatState.ACTIVE);
                });
            });
        }

        private void sendMessage()
        {
            if (!string.IsNullOrWhiteSpace(message_tbx.Text))
            {
                if (IsDummy)
                {
                    addDummyMessage(message_tbx.Text, Chat.userAccountId, MessageState.SEND);
                    sendDummyMessages++;


                    switch (sendDummyMessages)
                    {
                        case 1:
                            accImg_aiwp.PresenceP = Presence.Online;
                            break;

                        case 3:
                            sendBotMessage(Localisation.getLocalizedString("chat_details_dummy_answer_3_img"), true, 3000);
                            sendBotMessage(Localisation.getLocalizedString("chat_details_dummy_answer_3"), false, 4000);
                            accImg_aiwp.PresenceP = Presence.Chat;
                            break;

                        case 4:
                            sendBotMessage(Localisation.getLocalizedString("chat_details_dummy_answer_4"), false, 3000);
                            accImg_aiwp.PresenceP = Presence.Online;
                            break;

                        case 7:
                            sendBotMessage(Localisation.getLocalizedString("chat_details_dummy_answer_7"), false, 3000);
                            break;

                        case 11:
                            sendBotMessage(Localisation.getLocalizedString("chat_details_dummy_answer_11"), false, 3000);
                            break;

                        case 15:
                            sendBotMessage(Localisation.getLocalizedString("chat_details_dummy_answer_15"), false, 3000);
                            accImg_aiwp.PresenceP = Presence.Xa;
                            break;

                        case 20:
                            sendBotMessage(Localisation.getLocalizedString("chat_details_dummy_answer_20"), false, 3000);
                            break;

                        case 30:
                            sendBotMessage(Localisation.getLocalizedString("chat_details_dummy_answer_30"), false, 3000);
                            break;

                        case 50:
                            sendBotMessage(Localisation.getLocalizedString("chat_details_dummy_answer_50_1"), false, 3000);
                            sendBotMessage(Localisation.getLocalizedString("chat_details_dummy_answer_50_2"), false, 4000);
                            sendBotMessage(Localisation.getLocalizedString("chat_details_dummy_answer_50_3"), false, 5000);
                            sendBotMessage(Localisation.getLocalizedString("chat_details_dummy_answer_50_4"), false, 6000);
                            sendBotMessage(Localisation.getLocalizedString("chat_details_dummy_answer_50_5"), false, 7000);
                            sendBotMessage(Localisation.getLocalizedString("chat_details_dummy_answer_50_6"), false, 8000);
                            sendBotMessage(Localisation.getLocalizedString("chat_details_dummy_answer_50_7"), true, 9000);
                            Task.Run(async () =>
                            {
                                await Task.Delay(9000);
                                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                                {
                                    storeChatState(ChatState.GONE);
                                    accImg_aiwp.PresenceP = Presence.Unavailable;
                                });
                            });
                            break;
                    }
                }
                else
                {
                    MessageMessage sendMessage;

                    string messageText = message_tbx.Text;
                    // Remove all tailing whitespaces, tabs and newlines:
                    messageText = messageText.TrimEnd(UiUtils.TRIM_CHARS).TrimStart(UiUtils.TRIM_CHARS);

                    // For MUC messages also pass the nickname:
                    bool toEncrypt = false;
                    if (Chat.chatType == ChatType.MUC && MUCInfo != null)
                    {
                        sendMessage = new MessageMessage(Client.getXMPPAccount().getIdAndDomain(), Chat.chatJabberId, messageText, getChatType(), MUCInfo.nickname, false);
                    }
                    else
                    {
                        if (Chat.omemoEnabled)
                        {
                            sendMessage = new OmemoMessageMessage(Client.getXMPPAccount().getIdAndDomain(), Chat.chatJabberId, messageText, getChatType(), true);
                            toEncrypt = true;
                        }
                        else
                        {
                            sendMessage = new MessageMessage(Client.getXMPPAccount().getIdAndDomain(), Chat.chatJabberId, messageText, getChatType(), true);
                        }
                    }
                    ChatMessageTable sendMessageTable = new ChatMessageTable(sendMessage, Chat)
                    {
                        state = toEncrypt ? MessageState.TO_ENCRYPT : MessageState.SENDING
                    };

                    // Set chatMessageId:
                    sendMessage.chatMessageId = sendMessageTable.id;

                    // Add message to DB and update chat last active:
                    Chat.lastActive = DateTime.Now;
                    ChatTable chatCpy = Chat;
                    Task.Run(() =>
                    {
                        ChatDBManager.INSTANCE.setChatMessage(sendMessageTable, true, false);
                        ChatDBManager.INSTANCE.setChat(chatCpy, false, true);
                    });
                    if (sendMessage is OmemoMessageMessage omemoMsg)
                    {
                        Client.sendOmemoMessage(omemoMsg, Chat.chatJabberId, Client.getXMPPAccount().getIdAndDomain());
                    }
                    else
                    {
                        Client.sendMessage(sendMessage);
                    }
                }

                message_tbx.Text = "";
            }
        }

        private void showBackgroundForViewState(MasterDetailsViewState state)
        {
            if (state == MasterDetailsViewState.Both)
            {
                backgroundImage_img.Visibility = Visibility.Collapsed;
                main_grid.Background = new SolidColorBrush(Colors.Transparent);
            }
            else
            {
                backgroundImage_img.Visibility = Visibility.Visible;
                main_grid.Background = (SolidColorBrush)Application.Current.Resources["ApplicationPageBackgroundThemeBrush"];
            }
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
                await MUCHandler.INSTANCE.leaveRoomAsync(Client, Chat, MUCInfo).ConfigureAwait(false);
            }
        }

        private async Task joinRoomAsync()
        {
            if (Client != null && MUCInfo != null && Chat != null)
            {
                await MUCHandler.INSTANCE.enterMUCAsync(Client, Chat, MUCInfo).ConfigureAwait(false);
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
                if (Chat != null && Chat.id.Equals(args.MESSAGE.chatId))
                {
                    // Only update for unread messages:
                    if (args.MESSAGE.state == MessageState.UNREAD)
                    {
                        Task.Run(() => ChatDBManager.INSTANCE.markMessageAsRead(args.MESSAGE));
                    }

                    CHAT_MESSAGES.Add(new ChatMessageDataTemplate()
                    {
                        message = args.MESSAGE,
                        chat = Chat
                    });
                }
            });
        }

        private void send_btn_Click(object sender, RoutedEventArgs e)
        {
            sendMessage();
        }

        private void invertedListView_lstv_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            invertedListView_lstv.SelectedIndex = -1;
        }

        private void clip_btn_Click(object sender, RoutedEventArgs e)
        {
            if (!IsDummy)
            {
                //TODO Add clip menu
            }
        }

        private void message_tbx_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(message_tbx.Text))
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
            loadBackgrundImage();

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

            if (!IsDummy)
            {
                // Subscribe to chat and chat message changed events:
                ChatDBManager.INSTANCE.NewChatMessage -= INSTANCE_NewChatMessage;
                ChatDBManager.INSTANCE.NewChatMessage += INSTANCE_NewChatMessage;
                ChatDBManager.INSTANCE.ChatMessageChanged -= INSTANCE_ChatMessageChanged;
                ChatDBManager.INSTANCE.ChatMessageChanged += INSTANCE_ChatMessageChanged;
            }
            else
            {
                loadDummyContent();
            }

            // Enable the test button only on debug builds:
#if DEBUG
            test_mfo.Visibility = Visibility.Visible;
#endif
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (!IsDummy)
            {
                // Unsubscribe to chat and chat message changed events:
                ChatDBManager.INSTANCE.NewChatMessage -= INSTANCE_NewChatMessage;
                ChatDBManager.INSTANCE.ChatMessageChanged -= INSTANCE_ChatMessageChanged;

                if (Client != null)
                {
                    Client.NewChatState -= Client_NewChatState;
                }
            }
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
                if (Chat != null && Chat.chatType == ChatType.CHAT && string.Equals(chatId, Chat.id))
                {
                    storeChatState(args.STATE);
                    args.Cancel = true;
                }
            });
        }

        private void message_tbx_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                if (Settings.getSettingBoolean(SettingsConsts.ENTER_TO_SEND_MESSAGES))
                {
                    sendMessage();
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

        private void Test_mfo_Click(object sender, RoutedEventArgs e)
        {
            Client.PUB_SUB_COMMAND_HELPER.requestSubscriptions("pubsub.404.city", null, null);
        }

        private void message_tbx_GotFocus(object sender, RoutedEventArgs e)
        {
            if (shouldSendChatState())
            {
                Client.GENERAL_COMMAND_HELPER.sendChatStateAsync(Chat.chatJabberId, ChatState.COMPOSING).ConfigureAwait(false);
            }
        }

        private void message_tbx_LostFocus(object sender, RoutedEventArgs e)
        {
            if (shouldSendChatState())
            {
                Client.GENERAL_COMMAND_HELPER.sendChatStateAsync(Chat.chatJabberId, ChatState.ACTIVE).ConfigureAwait(false);
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
                        for (int i = 0; i < CHAT_MESSAGES.Count; i++)
                        {
                            if (CHAT_MESSAGES[i].message != null && Equals(CHAT_MESSAGES[i].message.id, args.MESSAGE.id))
                            {
                                // Only the main thread should update the list to prevent problems:
                                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => CHAT_MESSAGES[i].message = args.MESSAGE);
                            }
                        }
                    });
                }
            });
        }

        private void AccountImageWithPresenceControl_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (!IsDummy)
            {
                showProfile();
            }
        }

        private async void leave_mfo_Click(object sender, RoutedEventArgs e)
        {
            if (!IsDummy)
            {
                await leaveRoomAsync().ConfigureAwait(false);
            }
        }

        private async void join_mfo_Click(object sender, RoutedEventArgs e)
        {
            if (!IsDummy)
            {
                await joinRoomAsync().ConfigureAwait(false);
            }
        }

        private void info_mfo_Click(object sender, RoutedEventArgs e)
        {
            if (!IsDummy)
            {
                showProfile();
            }
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

        private void copyChatName_mfi_Click(object sender, RoutedEventArgs e)
        {
            if (Chat != null)
            {
                switch (Chat.chatType)
                {
                    case ChatType.MUC:
                        if (MUCInfo != null && !string.IsNullOrEmpty(MUCInfo.name))
                        {
                            UiUtils.addTextToClipboard(MUCInfo.name);
                            return;
                        }
                        break;
                }

                UiUtils.addTextToClipboard(Chat.chatJabberId);
            }
        }

        private void copyAccountName_mfi_Click(object sender, RoutedEventArgs e)
        {
            if (Chat != null)
            {
                UiUtils.addTextToClipboard(Chat.userAccountId);
            }
        }

        private void copyChatState_mfi_Click(object sender, RoutedEventArgs e)
        {
            if (Chat != null)
            {
                switch (Chat.chatType)
                {
                    case ChatType.MUC:
                        if (MUCInfo != null)
                        {
                            UiUtils.addTextToClipboard(MUCInfo.subject);
                            return;
                        }
                        break;
                }

                UiUtils.addTextToClipboard(Chat.status);
            }
        }

        private void chatDetails_grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (!IsDummy)
            {
                showProfile();
            }
        }

        private void chatDetails_grid_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            FlyoutBase flyoutBase = FlyoutBase.GetAttachedFlyout(senderElement);
            flyoutBase.ShowAt(senderElement);
        }

        private void omemo_tmfo_Click(object sender, RoutedEventArgs e)
        {
            Chat.omemoEnabled = omemo_tmfo.IsChecked;
            showChat(Chat);
            ChatTable cpy = Chat;
            Task.Run(() => ChatDBManager.INSTANCE.setChatTableValue(nameof(cpy.id), cpy.id, nameof(cpy.omemoEnabled), cpy.omemoEnabled));
        }

        private void scrollDown_btn_Click(object sender, RoutedEventArgs e)
        {
            if (CHAT_MESSAGES.Count >= 1)
            {
                invertedListView_lstv.ScrollIntoView(CHAT_MESSAGES[CHAT_MESSAGES.Count - 1]);
            }
        }

        private void clipImgLib_btn_Click(object sender, RoutedEventArgs e)
        {
            // ToDo implement
        }

        private void clipImgCam_btn_Click(object sender, RoutedEventArgs e)
        {
            // ToDo implement
        }

        private void clipDraw_btn_Click(object sender, RoutedEventArgs e)
        {
            // ToDo implement
        }

        private void clipFile_btn_Click(object sender, RoutedEventArgs e)
        {
            // ToDo implement
        }

        private void Value_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (ChatTemp == null)
            {
                return;
            }

            switch (e.PropertyName)
            {
                case nameof(ChatTemplate.chat):
                    showChat(ChatTemp.chat);
                    showMUCInfo(ChatTemp.mucInfo, ChatTemp.chat);
                    break;

                case nameof(ChatTemplate.client):
                    showClient(ChatTemp.client);
                    if (ChatTemp.client != null)
                    {
                        ChatTemp.client.NewChatState -= Client_NewChatState;
                        ChatTemp.client.NewChatState += Client_NewChatState;
                    }
                    break;

                case nameof(ChatTemplate.mucInfo):
                    showMUCInfo(ChatTemp.mucInfo, ChatTemp.chat);
                    break;

                default:
                    break;
            }
        }
        #endregion
    }
}
