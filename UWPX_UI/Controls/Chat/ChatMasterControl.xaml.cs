using System.Threading.Tasks;
using Data_Manager2.Classes;
using UWPX_UI.Dialogs;
using UWPX_UI.Pages;
using UWPX_UI_Context.Classes;
using UWPX_UI_Context.Classes.DataContext.Controls;
using UWPX_UI_Context.Classes.DataTemplates;
using UWPX_UI_Context.Classes.Events;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace UWPX_UI.Controls.Chat
{
    public sealed partial class ChatMasterControl: UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public ChatDataTemplate Chat
        {
            get => (ChatDataTemplate)GetValue(ChatProperty);
            set => SetValue(ChatProperty, value);
        }
        public static readonly DependencyProperty ChatProperty = DependencyProperty.Register(nameof(ChatDataTemplate), typeof(ChatDataTemplate), typeof(ChatMasterControl), new PropertyMetadata(null, ChatPropertyChanged));

        private readonly ChatMasterControlContext VIEW_MODEL;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ChatMasterControl()
        {
            InitializeComponent();
            VIEW_MODEL = new ChatMasterControlContext(Resources);
            VIEW_MODEL.OnError += VIEW_MODEL_OnError;
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
        private async Task DeleteChatAsync()
        {
            DeleteChatConfirmDialog dialog = new DeleteChatConfirmDialog(Chat.Chat);
            await UiUtils.ShowDialogAsync(dialog);
            await VIEW_MODEL.DeleteChatAsync(dialog.VIEW_MODEL.MODEL, Chat);
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        #region --Presence--
        private async void RequestPresenceSubscription_mfo_Click(object sender, RoutedEventArgs e)
        {
            await VIEW_MODEL.RequestPresenceSubscriptionAsync(Chat);
        }

        private async void CancelPresenceSubscription_mfo_Click(object sender, RoutedEventArgs e)
        {
            await VIEW_MODEL.CancelPresenceSubscriptionAsync(Chat);
        }

        private async void RejectPresenceSubscription_mfo_Click(object sender, RoutedEventArgs e)
        {
            await VIEW_MODEL.RejectPresenceSubscriptionAsync(Chat);
        }

        private async void ProbePresence_mfo_Click(object sender, RoutedEventArgs e)
        {
            await VIEW_MODEL.SendPresenceProbeAsync(Chat);
        }

        #endregion
        private void UserControl_RightTapped(object sender, Windows.UI.Xaml.Input.RightTappedRoutedEventArgs e)
        {
            if (Chat is null || Chat.Chat is null)
            {
                return;
            }
            switch (Chat.Chat.chatType)
            {
                case ChatType.MUC:
                    muc_mfo.ShowAt(this, e.GetPosition(this));
                    break;

                default:
                    chat_mfo.ShowAt(this, e.GetPosition(this));
                    break;
            }
        }

        private async void Mute_tmfo_Click(object sender, RoutedEventArgs e)
        {
            await VIEW_MODEL.SetChatMutedAsync(Chat, mute_tmfo.IsChecked);
        }

        private async void RemoveFromRoster_mfo_Click(object sender, RoutedEventArgs e)
        {
            await VIEW_MODEL.ToggleChatInRosterAsync(Chat);
        }

        private async void DeleteChat_mfo_Click(object sender, RoutedEventArgs e)
        {
            await DeleteChatAsync();
        }

        private void ShowInfo_mfo_Click(object sender, RoutedEventArgs e)
        {
            UiUtils.NavigateToPage(typeof(MucInfoPage), new NavigatedToMucInfoPageEventArgs(Chat));
        }

        private async void Enter_mfo_Click(object sender, RoutedEventArgs e)
        {
            await VIEW_MODEL.EnterMucAsync(Chat);
        }

        private async void Leave_mfo_Click(object sender, RoutedEventArgs e)
        {
            await VIEW_MODEL.LeaveMucAsync(Chat);
        }

        private void Bookmark_tmfo_Click(object sender, RoutedEventArgs e)
        {
            VIEW_MODEL.ToggleChatBookmarked(Chat);
        }

        private async void MuteMUC_tmfo_Click(object sender, RoutedEventArgs e)
        {
            await VIEW_MODEL.SetChatMutedAsync(Chat, muteMUC_tmfo.IsChecked);
        }

        private async void DeleteMUC_mfo_Click(object sender, RoutedEventArgs e)
        {
            await DeleteChatAsync();
        }

        private void ShowProfile_mfo_Click(object sender, RoutedEventArgs e)
        {
            UiUtils.NavigateToPage(typeof(ContactInfoPage), new NavigatedToContactInfoPageEventArgs(Chat.Client, Chat.Chat));
        }

        private static void ChatPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ChatMasterControl masterControl)
            {
                masterControl.UpdateView(e);
            }
        }

        private async void VIEW_MODEL_OnError(ChatMasterControlContext sender, OnErrorEventArgs args)
        {
            InfoDialog dialog = new InfoDialog(args.TITLE, args.MESSAGE)
            {
                Foreground = new SolidColorBrush(Colors.Red)
            };
            await UiUtils.ShowDialogAsync(dialog);
        }

        private async void AccountActionAccept_ibtn_Click(IconButtonControl sender, RoutedEventArgs args)
        {
            await VIEW_MODEL.AnswerPresenceSubscriptionRequestAsync(Chat, true);
        }

        private async void AccountActionRefuse_ibtn_Click(IconButtonControl sender, RoutedEventArgs args)
        {
            await VIEW_MODEL.AnswerPresenceSubscriptionRequestAsync(Chat, false);
        }

        private void MarkAsRead_tmfo_Click(object sender, RoutedEventArgs e)
        {
            VIEW_MODEL.MarkAsRead(Chat);
        }

        private async void SwipeLeftItem_Invoked(SwipeItem sender, SwipeItemInvokedEventArgs args)
        {
            await DeleteChatAsync();
        }

        private async void SwipeRightItem_Invoked(SwipeItem sender, SwipeItemInvokedEventArgs args)
        {
            if (Chat.Chat.chatType == ChatType.MUC)
            {
                VIEW_MODEL.ToggleChatBookmarked(Chat);
            }
            else
            {
                await VIEW_MODEL.ToggleChatInRosterAsync(Chat);
            }
        }

        #endregion
    }
}
