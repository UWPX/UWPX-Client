using Data_Manager2.Classes.DBTables;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using XMPP_API.Classes;
using System;
using Data_Manager2.Classes.DBManager;

namespace UWP_XMPP_Client.Controls
{
    public sealed partial class AccountImageWithPresenceControl : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public Presence Presence
        {
            get { return (Presence)GetValue(PresenceProperty); }
            set
            {
                SetValue(PresenceProperty, value);
                onPresenceUpdated();
            }
        }
        public static readonly DependencyProperty PresenceProperty = DependencyProperty.Register("Presence", typeof(Presence), typeof(AccountImageWithPresenceControl), null);

        public ImageSource Image
        {
            get { return (BitmapImage)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }
        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register("Image", typeof(BitmapImage), typeof(AccountImageWithPresenceControl), null);

        #region --Presence-Automation-- // Subscribes to the presence changed event
        public XMPPClient Client
        {
            get { return (XMPPClient)GetValue(ClientProperty); }
            set
            {
                unsubscribeFromEvents();
                SetValue(ClientProperty, value);
                subscribeToEvents();
            }
        }
        public static readonly DependencyProperty ClientProperty = DependencyProperty.Register("Client", typeof(XMPPClient), typeof(ChatMasterControl), null);

        public ChatTable Chat
        {
            get { return (ChatTable)GetValue(ChatProperty); }
            set
            {
                // Unsubscribe events:
                if (Chat != null && Chat.chatType == Data_Manager2.Classes.ChatType.MUC)
                {
                    ChatManager.INSTANCE.MUCInfoChanged -= INSTANCE_MUCInfoChanged;
                }

                SetValue(ChatProperty, value);

                // Subscribe to MUC info changed event if chat type is MUC:
                if (value != null && value.chatType == Data_Manager2.Classes.ChatType.MUC)
                {
                    ChatManager.INSTANCE.MUCInfoChanged += INSTANCE_MUCInfoChanged;
                    mUCInfo = ChatManager.INSTANCE.getMUCInfo(Chat.id);
                }
                showCurrentPresence();
            }
        }

        public static readonly DependencyProperty ChatProperty = DependencyProperty.Register("Chat", typeof(ChatTable), typeof(ChatMasterControl), null);

        private MUCChatInfoTable mUCInfo;

        #endregion

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 05/09/2017 Created [Fabian Sauter]
        /// </history>
        public AccountImageWithPresenceControl()
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
        private void onPresenceUpdated()
        {
            switch (Presence)
            {
                case Presence.Online:
                    presence_brdr.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 84, 168, 27));
                    break;
                case Presence.Chat:
                    presence_brdr.BorderBrush = new SolidColorBrush(Colors.White);
                    break;
                case Presence.Away:
                    presence_brdr.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 235, 140, 16));
                    break;
                case Presence.Xa:
                    presence_brdr.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 235, 73, 16));
                    break;
                case Presence.Dnd:
                    presence_brdr.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 162, 16, 37));
                    break;
                default:
                    presence_brdr.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 76, 74, 75));
                    break;
            }
        }

        private void unsubscribeFromEvents()
        {
            if (Client != null)
            {
                Client.NewPresence -= Client_NewPresence;
            }
        }

        private void subscribeToEvents()
        {
            if (Client != null)
            {
                Client.NewPresence += Client_NewPresence;
            }
        }

        private void showCurrentPresence()
        {
            if (Chat != null)
            {
                switch (Chat.chatType)
                {
                    case Data_Manager2.Classes.ChatType.MUC:
                        placeholder_tbx.Text = "\uE125";
                        if (mUCInfo != null)
                        {
                            Presence = mUCInfo.getMUCPresence();
                        }
                        else
                        {
                            Presence = Presence.Unavailable;
                        }
                        break;

                    default:
                        Presence = Chat.presence;
                        placeholder_tbx.Text = "\uE77B";
                        break;
                }
            }
        }
        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private async void Client_NewPresence(XMPPClient client, XMPP_API.Classes.Events.NewPresenceMessageEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
             {
                 if (Chat != null && string.Equals(args.getFrom(), Chat.chatJabberId))
                 {
                     Presence = args.getPresence();
                 }
             });
        }

        private async void INSTANCE_MUCInfoChanged(ChatManager handler, Data_Manager.Classes.Events.MUCInfoChangedEventArgs args)
        {
            if (!args.REMOVED && mUCInfo != null && mUCInfo.chatId.Equals(args.MUC_INFO.chatId))
            {
                mUCInfo = args.MUC_INFO;
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => Presence = mUCInfo.getMUCPresence());
            }
        }

        #endregion
    }
}
