using Manager.Classes.Chat;
using Shared.Classes;
using UWPX_UI.Dialogs;
using UWPX_UI_Context.Classes;
using UWPX_UI_Context.Classes.DataContext.Controls.Chat.MUC;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPX_UI.Controls.Chat.MUC
{
    public sealed partial class MucInfoControl: UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly MucInfoControlContext VIEW_MODEL = new MucInfoControlContext();

        public ChatDataTemplate Chat
        {
            get => (ChatDataTemplate)GetValue(ChatProperty);
            set => SetValue(ChatProperty, value);
        }
        public static readonly DependencyProperty ChatProperty = DependencyProperty.Register(nameof(Chat), typeof(ChatDataTemplate), typeof(MucInfoControl), new PropertyMetadata(null, OnChatChanged));

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

        private void Mute_btn_Click(object sender, RoutedEventArgs e)
        {
            VIEW_MODEL.ToggleChatMuted(Chat);
        }

        private async void Enter_mfo_Click(object sender, RoutedEventArgs e)
        {
            await VIEW_MODEL.EnterMucAsync(Chat).ConfAwaitFalse();
        }

        private async void Leave_mfo_Click(object sender, RoutedEventArgs e)
        {
            await VIEW_MODEL.LeaveMucAsync(Chat).ConfAwaitFalse();
        }

        private async void Bookmark_mfo_Click(object sender, RoutedEventArgs e)
        {
            await VIEW_MODEL.ToggleChatBookmarkedAsync(Chat);
        }

        private void AutoJoin_tmfo_Click(object sender, RoutedEventArgs e)
        {
            VIEW_MODEL.ToggleMucAutoJoin(Chat);
        }

        private async void ChangeNickname_mfo_Click(object sender, RoutedEventArgs e)
        {
            ChangeNicknameDialog dialog = new ChangeNicknameDialog(Chat);
            await UiUtils.ShowDialogAsync(dialog);
        }

        #endregion
    }
}
