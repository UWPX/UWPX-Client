using Data_Manager.Classes.DBEntries;
using Data_Manager.Classes.Managers;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using XMPP_API.Classes;
using System;
using System.Threading.Tasks;
using XMPP_API.Classes.Network.XML.Messages;

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

        public ChatEntry Chat
        {
            get { return (ChatEntry)GetValue(ChatProperty); }
            set
            {
                SetValue(ChatProperty, value);
                showChatDescription();
                showMessages();
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
        /// 29/08/2017 Created [Fabian Sauter]
        /// </history>
        public ChatDetailsControl()
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
            if (Chat != null)
            {
                if (Chat.name == null)
                {
                    chatName_tblck.Text = Chat.id;
                }
                else
                {
                    chatName_tblck.Text = Chat.name + " (" + Chat.id + ')';
                }
            }
        }

        private void showMessages()
        {
            if (Client != null && Chat != null)
            {
                accountName_tblck.Text = Client.getSeverConnectionConfiguration().getIdAndDomain();
                invertedListView_lstv.Items.Clear();
                foreach (ChatMessageEntry msg in ChatManager.INSTANCE.getAllChatMessagesForChat(Chat))
                {
                    if (Chat.userAccountId.Equals(msg.fromUser))
                    {
                        invertedListView_lstv.Items.Insert(0, new SpeechBubbleDownControl() { Text = msg.message, Date = msg.date.ToLocalTime()});
                    }
                    else
                    {
                        invertedListView_lstv.Items.Insert(0, new SpeechBubbleTopControl() { Text = msg.message, Date = msg.date.ToLocalTime() });
                    }
                }
            }
        }

        private void linkEvents()
        {
            if (Client != null)
            {
                Client.NewChatMessage -= Client_NewChatMessage;
                Client.NewChatMessage += Client_NewChatMessage;
            }
        }

        private async Task sendMessageAsync(string msg)
        {
            MessageMessage sendMessage = await Client.sendMessageAsync(Chat.id, msg);
            invertedListView_lstv.Items.Add(new SpeechBubbleDownControl() { Text = msg, Date = DateTime.Now });
            ChatManager.INSTANCE.setChatMessageEntry(new ChatMessageEntry(sendMessage, Chat));
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private async void Client_NewChatMessage(XMPPClient client, XMPP_API.Classes.Network.Events.NewChatMessageEventArgs args)
        {
            MessageMessage msg = args.getMessage();
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (Chat.id.Equals(Utils.removeResourceFromJabberid(msg.getFrom())))
                {
                    if (Chat.userAccountId.Equals(msg.getFrom()))
                    {
                        invertedListView_lstv.Items.Add(new SpeechBubbleDownControl() { Text = msg.getMessage(), Date = DateTime.Now });
                    }
                    else
                    {
                        invertedListView_lstv.Items.Add(new SpeechBubbleTopControl() { Text = msg.getMessage(), Date = DateTime.Now });
                    }
                }
            });
        }

        private async void send_btn_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(message_tbx.Text))
            {
                await sendMessageAsync(message_tbx.Text);
                message_tbx.Text = "";
            }
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
            if(String.IsNullOrWhiteSpace(message_tbx.Text))
            {
                send_btn.IsEnabled = false;
            }
            else
            {
                send_btn.IsEnabled = true;
            }
        }

        #endregion
    }
}
