using Data_Manager2.Classes;
using Data_Manager2.Classes.DBTables;
using System.Threading.Tasks;
using UWP_XMPP_Client.Classes;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using XMPP_API.Classes;

namespace UWP_XMPP_Client.Controls
{
    public sealed partial class MUCInfoControl : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public ChatTable Chat
        {
            get { return (ChatTable)GetValue(ChatProperty); }
            set { SetValue(ChatProperty, value); }
        }
        public static readonly DependencyProperty ChatProperty = DependencyProperty.Register("Chat", typeof(ChatTable), typeof(MUCInfoControl), null);

        public XMPPClient Client
        {
            get { return (XMPPClient)GetValue(ClientProperty); }
            set { SetValue(ClientProperty, value); }
        }
        public static readonly DependencyProperty ClientProperty = DependencyProperty.Register("Client", typeof(XMPPClient), typeof(MUCInfoControl), null);

        public MUCChatInfoTable MUCInfo
        {
            get { return (MUCChatInfoTable)GetValue(MUCInfoProperty); }
            set
            {
                SetValue(MUCInfoProperty, value);
                showMUCInfo();
            }
        }
        public static readonly DependencyProperty MUCInfoProperty = DependencyProperty.Register("MUCInfo", typeof(MUCChatInfoTable), typeof(MUCInfoControl), null);

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 12/02/2018 Created [Fabian Sauter]
        /// </history>
        public MUCInfoControl()
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
        private void showMUCInfo()
        {
            if (MUCInfo != null)
            {
                Presence presence = MUCInfo.getMUCPresence();
                enterState_tbx.Foreground = UiUtils.getPresenceBrush(presence);
                switch (presence)
                {
                    case Presence.Online:
                        enterState_tbx.Text = "joined";
                        join_btn.IsEnabled = false;
                        leave_btn.IsEnabled = true;
                        break;
                    case Presence.Chat:
                        enterState_tbx.Text = "joining/leaving...";
                        join_btn.IsEnabled = false;
                        leave_btn.IsEnabled = false;
                        break;
                    default:
                        enterState_tbx.Text = "not joined yet";
                        leave_btn.IsEnabled = false;
                        join_btn.IsEnabled = true;
                        break;
                }
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
        private async void join_btn_Click(object sender, RoutedEventArgs e)
        {
            await joinRoomAsync();
        }

        private async void leave_btn_Click(object sender, RoutedEventArgs e)
        {
            await leaveRoomAsync();
        }

        #endregion
    }
}
