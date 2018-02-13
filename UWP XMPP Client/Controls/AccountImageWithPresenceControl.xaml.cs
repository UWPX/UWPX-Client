using Data_Manager2.Classes.DBTables;
using UWP_XMPP_Client.Classes;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using XMPP_API.Classes;

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
                showPresenceColor();
            }
        }
        public static readonly DependencyProperty PresenceProperty = DependencyProperty.Register("Presence", typeof(Presence), typeof(AccountImageWithPresenceControl), null);

        public ImageSource Image
        {
            get { return (BitmapImage)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }
        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register("Image", typeof(BitmapImage), typeof(AccountImageWithPresenceControl), null);

        public XMPPClient Client
        {
            get { return (XMPPClient)GetValue(ClientProperty); }
            set { SetValue(ClientProperty, value); }
        }
        public static readonly DependencyProperty ClientProperty = DependencyProperty.Register("Client", typeof(XMPPClient), typeof(AccountImageWithPresenceControl), null);

        public ChatTable Chat
        {
            get { return (ChatTable)GetValue(ChatProperty); }
            set
            {
                SetValue(ChatProperty, value);
                showPresence();
            }
        }

        public static readonly DependencyProperty ChatProperty = DependencyProperty.Register("Chat", typeof(ChatTable), typeof(AccountImageWithPresenceControl), null);

        public MUCChatInfoTable MUCInfo
        {
            get { return (MUCChatInfoTable)GetValue(MUCInfoProperty); }
            set
            {
                SetValue(MUCInfoProperty, value);
                if (MUCInfo != null)
                {
                    Presence = value.getMUCPresence();
                }
            }
        }
        public static readonly DependencyProperty MUCInfoProperty = DependencyProperty.Register("MUCInfo", typeof(MUCChatInfoTable), typeof(AccountImageWithPresenceControl), null);

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
        private void showPresenceColor()
        {
            presence_elipse.Fill = UiUtils.getPresenceBrush(Presence);
        }

        private void showPresence()
        {
            if (Chat != null)
            {
                switch (Chat.chatType)
                {
                    case Data_Manager2.Classes.ChatType.MUC:
                        placeholder_tbx.Text = "\uE125";
                        if (MUCInfo != null)
                        {
                            Presence = MUCInfo.getMUCPresence();
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


        #endregion
    }
}
