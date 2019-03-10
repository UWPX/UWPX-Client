using UWP_XMPP_Client.Classes.Events;
using UWP_XMPP_Client.Pages;
using UWPX_UI_Context.Classes;
using UWPX_UI_Context.Classes.DataContext.Controls;
using UWPX_UI_Context.Classes.DataTemplates;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPX_UI.Controls.Chat
{
    public sealed partial class ChatDetailsControl : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public ChatDataTemplate Chat
        {
            get { return (ChatDataTemplate)GetValue(ChatProperty); }
            set { SetValue(ChatProperty, value); }
        }
        public static readonly DependencyProperty ChatProperty = DependencyProperty.Register(nameof(ChatDataTemplate), typeof(ChatDataTemplate), typeof(ChatDetailsControl), new PropertyMetadata(null, ChatPropertyChanged));

        public bool IsDummy
        {
            get { return (bool)GetValue(IsDummyProperty); }
            set { SetValue(IsDummyProperty, value); }
        }
        public static readonly DependencyProperty IsDummyProperty = DependencyProperty.Register(nameof(IsDummy), typeof(bool), typeof(ChatDetailsControl), new PropertyMetadata(false, OnIsDummyChanged));

        public readonly ChatDetailsControlContext VIEW_MODEL = new ChatDetailsControlContext();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
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
        public void UpdateView(DependencyPropertyChangedEventArgs args)
        {
            VIEW_MODEL.UpdateView(args);
        }

        #endregion

        #region --Misc Methods (Private)--
        private void LoadDummyContent()
        {
            Chat = new ChatDataTemplate
            {
                Chat = new Data_Manager2.Classes.DBTables.ChatTable("dave@xmpp.uwpx.org", "alice@xmpp.uwpx.org")
                {
                    presence = XMPP_API.Classes.Presence.Away,
                    status = "ʕノ•ᴥ•ʔノ ︵ ┻━┻",
                    omemoEnabled = true
                }
            };
            VIEW_MODEL.LoadDummyContent(Chat.Chat);
        }

        private void UpdateIsDummy()
        {
            VIEW_MODEL.OnIsDummyChanged(IsDummy);
            if (IsDummy)
            {
                LoadDummyContent();
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private static void ChatPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ChatDetailsControl detailsControl)
            {
                detailsControl.UpdateView(e);
            }
        }

        private void HeaderInfo_grid_RightTapped(object sender, Windows.UI.Xaml.Input.RightTappedRoutedEventArgs e)
        {
            if (sender is FrameworkElement element)
            {
                headerInfo_grid.ContextFlyout.ShowAt(element);
            }
        }

        private void CopyNameText_mfi_Click(object sender, RoutedEventArgs e)
        {
            UiUtils.SetClipboardText(VIEW_MODEL.MODEL.NameText);
        }

        private void CopyChatStatus_mfi_Click(object sender, RoutedEventArgs e)
        {
            UiUtils.SetClipboardText(VIEW_MODEL.MODEL.StatusText);
        }

        private void CopyAccountText_mfi_Click(object sender, RoutedEventArgs e)
        {
            UiUtils.SetClipboardText(VIEW_MODEL.MODEL.AccountText);
        }

        private void Info_mfo_Click(object sender, RoutedEventArgs e)
        {
            if (!IsDummy)
            {
                if (Chat.Chat.chatType == Data_Manager2.Classes.ChatType.MUC)
                {
                    UiUtils.NavigateToPage(typeof(MUCInfoPage), new NavigatedToMUCInfoEventArgs(Chat.Chat, Chat.Client, Chat.MucInfo));
                }
                else
                {
                    UiUtils.NavigateToPage(typeof(UserProfilePage), new NavigatedToUserProfileEventArgs(Chat.Chat, Chat.Client));
                }
            }
        }

        private async void Enter_mfo_Click(object sender, RoutedEventArgs e)
        {
            await VIEW_MODEL.EnterMucAsync(Chat);
        }

        private async void Leave_mfo_Click(object sender, RoutedEventArgs e)
        {
            await VIEW_MODEL.LeaveMucAsync(Chat);
        }

        private void Test_mfo_Click(object sender, RoutedEventArgs e)
        {
            if (!IsDummy)
            {

            }
        }

        private void ClipImgLib_btn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ClipImgCam_btn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ClipDraw_btn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ClipFile_btn_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void Send_btn_Click(object sender, RoutedEventArgs e)
        {
            await VIEW_MODEL.SendChatMessageAsync(Chat);
        }

        private async void Message_tbx_KeyUp(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            await VIEW_MODEL.OnChatMessageKeyDown(e, Chat);
        }

        private async void ReadOnOmemo_link_Click(object sender, RoutedEventArgs e)
        {
            await VIEW_MODEL.OnReadOnOmemoClickedAsync();
        }

        private static void OnIsDummyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ChatDetailsControl chatDetailsControl)
            {
                chatDetailsControl.UpdateIsDummy();
            }
        }

        #endregion
    }
}
