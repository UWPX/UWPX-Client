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
using Data_Manager.Classes;
using XMPP_API.Classes.Network.XML.Messages.XEP_0085;
using Data_Manager2.Classes.DBTables;
using Data_Manager2.Classes.DBManager;
using Data_Manager2.Classes;
using XMPP_API.Classes.Network.XML.Messages.XEP_0357;
using Push_App_Server.Classes;

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
                SetValue(ClientProperty, value);
                showMessages();
                linkEvents();
            }
        }
        public static readonly DependencyProperty ClientProperty = DependencyProperty.Register("Client", typeof(XMPPClient), typeof(ChatMasterControl), null);

        public ChatTable Chat
        {
            get { return (ChatTable)GetValue(ChatProperty); }
            set
            {
                SetValue(ChatProperty, value);
                showChatDescription();
                showMessages();
            }
        }
        public static readonly DependencyProperty ChatProperty = DependencyProperty.Register("Chat", typeof(ChatTable), typeof(ChatMasterControl), null);

        private int uuu = 0;

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
            this.InitializeComponent();
            UiUtils.setBackgroundImage(backgroundImage_img);
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
            if (Chat != null)
            {
                chatName_tblck.Text = Chat.chatJabberId ?? "";
                chatState_tblck.Text = Chat.chatState ?? "";
            }
        }

        private void showMessages()
        {
            if (Client != null && Chat != null)
            {
                accountName_tblck.Text = Client.getXMPPAccount().getIdAndDomain();
                invertedListView_lstv.Items.Clear();
                foreach (ChatMessageTable msg in ChatManager.INSTANCE.getAllChatMessagesForChat(Chat))
                {
                    showMessage(msg.type, msg.fromUser, msg.message, msg.date);
                }
                ChatManager.INSTANCE.markAllMessagesAsRead(Chat);
            }
        }

        private void showMessage(string type, string from, string msg, DateTime date)
        {
            switch (type)
            {
                case "error":
                    invertedListView_lstv.Items.Add(new SpeechBubbleErrorControl() { Text = msg, Date = date.ToLocalTime() });
                    break;
                default:
                    if (Chat.userAccountId.Equals(from))
                    {
                        invertedListView_lstv.Items.Add(new SpeechBubbleDownControl() { Text = msg, Date = date.ToLocalTime() });
                    }
                    else
                    {
                        invertedListView_lstv.Items.Add(new SpeechBubbleTopControl() { Text = msg, Date = date.ToLocalTime() });
                    }
                    break;
            }
        }

        private void linkEvents()
        {
            if (Client != null)
            {
                ChatManager.INSTANCE.NewChatMessage -= INSTANCE_NewChatMessage;
                ChatManager.INSTANCE.NewChatMessage += INSTANCE_NewChatMessage;
                Client.NewChatState -= Client_NewChatState;
                Client.NewChatState += Client_NewChatState;
            }
        }

        private async Task sendMessageAsync()
        {
            if (!String.IsNullOrWhiteSpace(message_tbx.Text))
            {
                MessageMessage sendMessage = await Client.sendMessageAsync(Chat.chatJabberId, message_tbx.Text);
                ChatManager.INSTANCE.setChatMessageEntry(new ChatMessageTable(sendMessage, Chat) { state = MessageState.SENDING}, true);
                Chat.lastActive = DateTime.Now;
                ChatManager.INSTANCE.setChat(Chat, false, false);

                message_tbx.Text = "";
                message_tbx.Focus(FocusState.Programmatic);
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
        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private async void INSTANCE_NewChatMessage(ChatManager handler, Data_Manager.Classes.Events.NewChatMessageEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                ChatMessageTable msg = args.MESSAGE;
                if (Chat.id.Equals(msg.chatId))
                {
                    msg.state = MessageState.READ;
                    ChatManager.INSTANCE.setChatMessageEntry(msg, false);
                    showMessage(msg.type, msg.fromUser, msg.message, msg.date);
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

        private void profile_btn_Click(object sender, RoutedEventArgs e)
        {
            (Window.Current.Content as Frame).Navigate(typeof(UserProfilePage), new NavigatedToUserProfileEventArgs(Chat, Client));
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            object o = (Window.Current.Content as Frame).Content;
            if(o is ChatPage)
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
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (string.Equals(args.getChatId(), Chat.id))
                {
                    storeChatState(args.getState());
                    args.Cancel = true;
                }
            });
        }

        private async void message_tbx_KeyUp(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter && Settings.getSettingBoolean(SettingsConsts.ENTER_TO_SEND_MESSAGES))
            {
                await sendMessageAsync();
            }
        }

        private async void test_bnt_Click(object sender, RoutedEventArgs e)
        {
            //await Client.createDiscoAsync(Chat.chatJabberId);
            //await Logging.Logger.openLogFolderAsync();
            /*switch (uuu)
            {
                case 0:
                    await Client.createDiscoAsync(Client.getXMPPAccount().getIdAndDomain());
                    break;
                case 1:
                    await Client.sendAsync(new RequestEnableNotificationsMessage("w215-5a-v4.eduroam.dynamic.rbg.tum.de"));
                    break;
                default:
                    break;
            }
            if(++uuu > 1)
            {
                uuu = 0;
            }*/
            DataWriter dW = new DataWriter(Client);
            await dW.connectAndSendAsync();
        }

        private async void message_tbx_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!Settings.getSettingBoolean(SettingsConsts.DONT_SEND_CHAT_STATE))
            {
                await Client.sendChatStateAsync(Chat.chatJabberId, ChatState.COMPOSING);
            }
        }

        private async void message_tbx_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!Settings.getSettingBoolean(SettingsConsts.DONT_SEND_CHAT_STATE))
            {
                await Client.sendChatStateAsync(Chat.chatJabberId, ChatState.ACTIVE);
            }
        }
        #endregion
    }
}
