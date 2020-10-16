using Data_Manager2.Classes;
using UWPX_UI.Pages;
using UWPX_UI.Pages.Settings;
using UWPX_UI_Context.Classes;
using UWPX_UI_Context.Classes.DataContext.Controls;
using UWPX_UI_Context.Classes.DataTemplates;
using UWPX_UI_Context.Classes.Events;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using XMPP_API.Classes.Network.XML.Messages.Helper;
using XMPP_API.Classes.Network.XML.Messages.XEP_0313;

namespace UWPX_UI.Controls.Chat
{
    public sealed partial class ChatDetailsControl: UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public ChatDataTemplate Chat
        {
            get => (ChatDataTemplate)GetValue(ChatProperty);
            set => SetValue(ChatProperty, value);
        }
        public static readonly DependencyProperty ChatProperty = DependencyProperty.Register(nameof(ChatDataTemplate), typeof(ChatDataTemplate), typeof(ChatDetailsControl), new PropertyMetadata(null, ChatPropertyChanged));

        public bool IsDummy
        {
            get => (bool)GetValue(IsDummyProperty);
            set => SetValue(IsDummyProperty, value);
        }
        public static readonly DependencyProperty IsDummyProperty = DependencyProperty.Register(nameof(IsDummy), typeof(bool), typeof(ChatDetailsControl), new PropertyMetadata(false, OnIsDummyChanged));

        public readonly ChatDetailsControlContext VIEW_MODEL = new ChatDetailsControlContext();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ChatDetailsControl()
        {
            InitializeComponent();
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

        public void OnPageNavigatedTo()
        {
            VIEW_MODEL.MODEL.LoadSettings();
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

        private void ShowEnterToSendTip()
        {
            if (VIEW_MODEL.MODEL.EnterToSend && !Data_Manager2.Classes.Settings.getSettingBoolean(SettingsConsts.CHAT_ENTER_TO_SEND_TIP_SHOWN))
            {
                enterToSend_tt.IsOpen = true;
                Data_Manager2.Classes.Settings.setSetting(SettingsConsts.CHAT_ENTER_TO_SEND_TIP_SHOWN, true);
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

        private void HeaderInfo_grid_RightTapped(object sender, RightTappedRoutedEventArgs e)
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
                if (Chat.Chat.chatType == ChatType.MUC)
                {
                    UiUtils.NavigateToPage(typeof(MucInfoPage), new NavigatedToMucInfoPageEventArgs(Chat));
                }
                else
                {
                    UiUtils.NavigateToPage(typeof(ContactInfoPage), new NavigatedToContactInfoPageEventArgs(Chat.Client, Chat.Chat));
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

        private async void Test_mfo_Click(object sender, RoutedEventArgs e)
        {
            if (!IsDummy)
            {
                QueryFilter filter = new QueryFilter();
                filter.with(Chat.Chat.chatJabberId);
                MessageResponseHelperResult<MamResult> result = await Chat.Client.GENERAL_COMMAND_HELPER.requestMamAsync(filter);
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

        private void Send_btn_Click(object sender, RoutedEventArgs e)
        {
            VIEW_MODEL.SendChatMessage(Chat);
            ShowEnterToSendTip();
        }

        private static void OnIsDummyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ChatDetailsControl chatDetailsControl)
            {
                chatDetailsControl.UpdateIsDummy();
            }
        }

        private void EmojiPickerControl_EmojiSelected(EmojiPickerControl sender, Classes.Events.EmojiSelectedEventArgs args)
        {
            string emoji = args.EMOJI.ToString();
            int i = message_tbx.SelectionStart;
            if (message_tbx.SelectionLength > 0)
            {
                message_tbx.SelectedText = emoji;
                message_tbx.SelectionLength = 0;
            }
            else
            {
                message_tbx.Text = message_tbx.Text.Insert(message_tbx.SelectionStart, emoji);
            }
            i += emoji.Length;
            message_tbx.SelectionStart = i;
        }

        private void MarkAsRead_tmfo_Click(object sender, RoutedEventArgs e)
        {
            VIEW_MODEL.MarkAsRead(Chat);
        }

        private void Message_tbx_EnterKeyDown(object sender, KeyRoutedEventArgs e)
        {
            VIEW_MODEL.OnEnterKeyDown(e, Chat);
        }

        private void Header_grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            chatMessages_cmg.ScrollHeaderMinSize = e.NewSize.Height;
        }

        private void ChatSettings_link_Click(object sender, RoutedEventArgs e)
        {
            UiUtils.NavigateToPage(typeof(ChatSettingsPage));
        }

        private void MarkasIotDevice_mfo_Click(object sender, RoutedEventArgs e)
        {
            VIEW_MODEL.MarkAsIotDevice(Chat.Chat);
        }

        #endregion
    }
}
