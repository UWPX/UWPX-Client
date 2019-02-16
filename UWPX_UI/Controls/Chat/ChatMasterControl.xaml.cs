using Data_Manager2.Classes;
using System.Threading.Tasks;
using UWPX_UI.Controls.Toolkit.SlidableListItem;
using UWPX_UI.Dialogs;
using UWPX_UI_Context.Classes;
using UWPX_UI_Context.Classes.DataContext.Controls;
using UWPX_UI_Context.Classes.DataTemplates;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace UWPX_UI.Controls.Chat
{
    public sealed partial class ChatMasterControl : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public ChatDataTemplate Chat
        {
            get { return (ChatDataTemplate)GetValue(ChatProperty); }
            set { SetValue(ChatProperty, value); }
        }
        public static readonly DependencyProperty ChatProperty = DependencyProperty.Register(nameof(ChatDataTemplate), typeof(ChatDataTemplate), typeof(ChatMasterControl), new PropertyMetadata(null, ChatPropertyChanged));

        private readonly ChatMasterControlContext VIEW_MODEL;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ChatMasterControl()
        {
            this.InitializeComponent();
            this.VIEW_MODEL = new ChatMasterControlContext(Resources);
            this.VIEW_MODEL.OnError += VIEW_MODEL_OnError;
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
            DeleteChatConfirmDialog dialog = new DeleteChatConfirmDialog();
            await UiUtils.ShowDialogAsync(dialog);
            await VIEW_MODEL.DeleteChatAsync(dialog.VIEW_MODEL.MODEL, Chat);
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        #region --Presence--
        private void RequestPresenceSubscription_mfo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CancelPresenceSubscription_mfo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RejectPresenceSubscription_mfo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ProbePresence_mfo_Click(object sender, RoutedEventArgs e)
        {

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
                case ChatType.CHAT:
                    chat_mfo.ShowAt(this, e.GetPosition(this));
                    break;
                case ChatType.MUC:
                    muc_mfo.ShowAt(this, e.GetPosition(this));
                    break;
                default:
                    break;
            }
        }

        private async void Mute_tmfo_Click(object sender, RoutedEventArgs e)
        {
            await VIEW_MODEL.SetChatMutedAsync(Chat, mute_tmfo.IsChecked);
        }

        private async void RemoveFromRoster_mfo_Click(object sender, RoutedEventArgs e)
        {
            await VIEW_MODEL.SwitchChatInRosterAsync(Chat);
        }

        private async void DeleteChat_mfo_Click(object sender, RoutedEventArgs e)
        {
            await DeleteChatAsync();
        }

        private void ShowInfo_mfo_Click(object sender, RoutedEventArgs e)
        {

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
            VIEW_MODEL.SwitchChatBookmarked(Chat);
        }

        private async void MuteMUC_tmfo_Click(object sender, RoutedEventArgs e)
        {
            await VIEW_MODEL.SetChatMutedAsync(Chat, muteMUC_tmfo.IsChecked);
        }

        private async void DeleteMUC_mfo_Click(object sender, RoutedEventArgs e)
        {
            await DeleteChatAsync();
        }

        private void AccountActionAccept_btn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AccountActionRefuse_btn_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void SlideListItem_sli_SwipeStatusChanged(SlidableListItem sender, SwipeStatusChangedEventArgs args)
        {
            if (args.NewValue == SwipeStatus.Idle)
            {
                if (args.OldValue == SwipeStatus.SwipingPassedLeftThreshold)
                {
                    await DeleteChatAsync();
                }
                else if (args.OldValue == SwipeStatus.SwipingPassedRightThreshold)
                {
                    if (Chat.Chat.chatType == ChatType.MUC)
                    {
                        VIEW_MODEL.SwitchChatBookmarked(Chat);
                    }
                    else
                    {
                        await VIEW_MODEL.SwitchChatInRosterAsync(Chat);
                    }
                }
            }
        }

        private void Muc_mfo_Opening(object sender, object e)
        {

        }

        private void ShowProfile_mfo_Click(object sender, RoutedEventArgs e)
        {

        }

        private static void ChatPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ChatMasterControl masterControl)
            {
                masterControl.UpdateView(e);
            }
        }

        private void VIEW_MODEL_OnError(ChatMasterControlContext sender, UWPX_UI_Context.Classes.Events.OnErrorEventArgs args)
        {
            InfoDialog dialog = new InfoDialog(args.TITLE, args.MESSAGE)
            {
                Foreground = new SolidColorBrush(Colors.Red)
            };
        }

        #endregion
    }
}
