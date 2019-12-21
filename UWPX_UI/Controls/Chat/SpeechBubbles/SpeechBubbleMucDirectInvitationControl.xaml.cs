using UWPX_UI.Dialogs;
using UWPX_UI_Context.Classes;
using UWPX_UI_Context.Classes.DataContext.Controls;
using UWPX_UI_Context.Classes.DataContext.Controls.Chat.SpeechBubbles;
using UWPX_UI_Context.Classes.DataTemplates;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPX_UI.Controls.Chat.SpeechBubbles
{
    public sealed partial class SpeechBubbleMucDirectInvitationControl: UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly SpeechBubbleContentControlContext VIEW_MODEL = new SpeechBubbleContentControlContext();
        /// <summary>
        /// Extends the current VIEW_MODEL with additional properties and methods.
        /// </summary>
        public readonly SpeechBubbleMucDirectInvitationControlContext INVITE_VIEW_MODEL = new SpeechBubbleMucDirectInvitationControlContext();

        public ChatMessageDataTemplate ChatMessage
        {
            get => (ChatMessageDataTemplate)GetValue(ChatMessageProperty);
            set => SetValue(ChatMessageProperty, value);
        }
        public static readonly DependencyProperty ChatMessageProperty = DependencyProperty.Register(nameof(ChatMessage), typeof(ChatMessageDataTemplate), typeof(SpeechBubbleErrorControl), new PropertyMetadata(null, OnChatMessageChanged));

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public SpeechBubbleMucDirectInvitationControl()
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
            INVITE_VIEW_MODEL.UpdateView(e);
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private static void OnChatMessageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SpeechBubbleMucDirectInvitationControl speechBubble)
            {
                speechBubble.UpdateView(e);
            }
        }

        private async void Accept_btn_Click(object sender, RoutedEventArgs e)
        {
            AddMucDialog dialog = new AddMucDialog(INVITE_VIEW_MODEL.MODEL.Invite, ChatMessage.Chat.userAccountId);
            await UiUtils.ShowDialogAsync(dialog);
            if (dialog.VIEW_MODEL.MODEL.Confirmed)
            {
                await INVITE_VIEW_MODEL.AcceptAsync(ChatMessage);
            }
        }

        private async void Decline_btn_Click(object sender, RoutedEventArgs e)
        {
            await INVITE_VIEW_MODEL.DeclineAsync(ChatMessage);
        }

        #endregion
    }
}
