using UWPX_UI_Context.Classes.DataContext.Controls;
using UWPX_UI_Context.Classes.DataTemplates;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace UWPX_UI.Controls.Chat.SpeechBubbles
{
    public sealed partial class SpeechBubbleErrorControl: UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public ChatMessageDataTemplate ChatMessage
        {
            get => (ChatMessageDataTemplate)GetValue(ChatMessageProperty);
            set => SetValue(ChatMessageProperty, value);
        }
        public static readonly DependencyProperty ChatMessageProperty = DependencyProperty.Register(nameof(ChatMessage), typeof(ChatMessageDataTemplate), typeof(SpeechBubbleErrorControl), new PropertyMetadata(null, OnChatMessageChanged));

        private readonly SpeechBubbleContentControlContext VIEW_MODEL = new SpeechBubbleContentControlContext();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public SpeechBubbleErrorControl()
        {
            InitializeComponent();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void ShowFlyout(FrameworkElement sender, Point point)
        {
            if (Resources["options_mfo"] is MenuFlyout flyout)
            {
                flyout.ShowAt(sender, point);
            }
        }

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
        private static void OnChatMessageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SpeechBubbleErrorControl speechBubble)
            {
                speechBubble.UpdateView(e);
            }
        }

        private void CopyMessage_mfi_Click(object sender, RoutedEventArgs e)
        {
            VIEW_MODEL.SetMessageAsClipboardText();
        }

        private void CopyDate_mfi_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.Resources["ChatDateTimeStringValueConverter"] is IValueConverter converter)
            {
                VIEW_MODEL.SetDateAsClipboardText(converter);
            }
        }

        private async void DeleteMsg_mfi_Click(object sender, RoutedEventArgs e)
        {
            await VIEW_MODEL.DeleteMessageAsync();
        }

        private void UserControl_RightTapped(object sender, Windows.UI.Xaml.Input.RightTappedRoutedEventArgs e)
        {
            ShowFlyout(this, e.GetPosition(this));
        }

        #endregion
    }
}
