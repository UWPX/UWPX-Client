using Data_Manager2.Classes.DBTables;
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


        public Presence PresenceP
        {
            get { return (Presence)GetValue(PresencePProperty); }
            set { SetValue(PresencePProperty, value); }
        }
        public static readonly DependencyProperty PresencePProperty = DependencyProperty.Register(nameof(PresenceP), typeof(Presence), typeof(AccountImageWithPresenceControl), new PropertyMetadata(Presence.Unavailable));



        public ImageSource Image
        {
            get { return (BitmapImage)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }
        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register(nameof(Image), typeof(BitmapImage), typeof(AccountImageWithPresenceControl), null);

        public ChatTable Chat
        {
            get { return (ChatTable)GetValue(ChatProperty); }
            set
            {
                SetValue(ChatProperty, value);
                showPresence();
            }
        }

        public static readonly DependencyProperty ChatProperty = DependencyProperty.Register(nameof(Chat), typeof(ChatTable), typeof(AccountImageWithPresenceControl), null);

        public MUCChatInfoTable MUCInfo
        {
            get { return (MUCChatInfoTable)GetValue(MUCInfoProperty); }
            set
            {
                SetValue(MUCInfoProperty, value);
                if (MUCInfo != null)
                {
                    PresenceP = value.getMUCPresence();
                }
            }
        }
        public static readonly DependencyProperty MUCInfoProperty = DependencyProperty.Register(nameof(MUCInfo), typeof(MUCChatInfoTable), typeof(AccountImageWithPresenceControl), null);

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
            InitializeComponent();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void showPresence()
        {
            if (Chat != null)
            {
                switch (Chat.chatType)
                {
                    case Data_Manager2.Classes.ChatType.MUC:
                        contact_pp.Initials = "\uE125";
                        if (MUCInfo != null)
                        {
                            PresenceP = MUCInfo.getMUCPresence();
                        }
                        else
                        {
                            PresenceP = Presence.Unavailable;
                        }
                        break;

                    default:
                        PresenceP = Chat.presence;
                        contact_pp.Initials = "\uE77B";
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
