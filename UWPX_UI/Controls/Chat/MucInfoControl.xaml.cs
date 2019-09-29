using Data_Manager2.Classes.DBTables;
using Shared.Classes;
using UWPX_UI.Dialogs;
using UWPX_UI_Context.Classes;
using UWPX_UI_Context.Classes.DataContext.Controls.Chat;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using XMPP_API.Classes;

namespace UWPX_UI.Controls.Chat
{
    public sealed partial class MucInfoControl: UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly MucInfoControlContext VIEW_MODEL = new MucInfoControlContext();

        public ChatTable Chat
        {
            get { return (ChatTable)GetValue(ChatProperty); }
            set { SetValue(ChatProperty, value); }
        }
        public static readonly DependencyProperty ChatProperty = DependencyProperty.Register(nameof(Chat), typeof(ChatTable), typeof(MucInfoControl), new PropertyMetadata(null, OnChatChanged));

        public MUCChatInfoTable MucInfo
        {
            get { return (MUCChatInfoTable)GetValue(MucInfoProperty); }
            set { SetValue(MucInfoProperty, value); }
        }
        public static readonly DependencyProperty MucInfoProperty = DependencyProperty.Register(nameof(MucInfo), typeof(MUCChatInfoTable), typeof(MucInfoControl), new PropertyMetadata(null, OnMucInfoChanged));

        public XMPPClient Client
        {
            get { return (XMPPClient)GetValue(ClientProperty); }
            set { SetValue(ClientProperty, value); }
        }
        public static readonly DependencyProperty ClientProperty = DependencyProperty.Register(nameof(Client), typeof(XMPPClient), typeof(MucInfoControl), new PropertyMetadata(null, OnClientChanged));

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public MucInfoControl()
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
        private void UpdateView(DependencyPropertyChangedEventArgs e)
        {
            VIEW_MODEL.UpdateView(e);
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private static void OnChatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MucInfoControl mucInfoControl)
            {
                mucInfoControl.UpdateView(e);
            }
        }

        private static void OnClientChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MucInfoControl mucInfoControl)
            {
                mucInfoControl.UpdateView(e);
            }
        }

        private static void OnMucInfoChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MucInfoControl mucInfoControl)
            {
                mucInfoControl.UpdateView(e);
            }
        }

        private async void Mute_btn_Click(object sender, RoutedEventArgs e)
        {
            await VIEW_MODEL.ToggleChatMutedAsync(Chat).ConfAwaitFalse();
        }

        private async void Enter_mfo_Click(object sender, RoutedEventArgs e)
        {
            await VIEW_MODEL.EnterMucAsync(Chat, MucInfo, Client).ConfAwaitFalse();
        }

        private async void Leave_mfo_Click(object sender, RoutedEventArgs e)
        {
            await VIEW_MODEL.LeaveMucAsync(Chat, MucInfo, Client).ConfAwaitFalse();
        }

        private void Bookmark_mfo_Click(object sender, RoutedEventArgs e)
        {
            VIEW_MODEL.ToggleChatBookmarked(Chat, Client);
        }

        private async void AutoJoin_tmfo_Click(object sender, RoutedEventArgs e)
        {
            await VIEW_MODEL.ToggleMucAutoJoinAsync(MucInfo).ConfAwaitFalse();
        }

        private async void ChangeNickname_mfo_Click(object sender, RoutedEventArgs e)
        {
            ChangeNicknameDialog dialog = new ChangeNicknameDialog(Chat, MucInfo, Client);
            await UiUtils.ShowDialogAsync(dialog);
        }

        #endregion
    }
}
