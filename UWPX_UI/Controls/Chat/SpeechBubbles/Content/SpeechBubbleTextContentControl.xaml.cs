using Data_Manager2.Classes;
using Microsoft.Toolkit.Uwp.UI.Extensions;
using UWPX_UI_Context.Classes.DataContext.Controls;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace UWPX_UI.Controls.Chat.SpeechBubbles.Content
{
    public sealed partial class SpeechBubbleTextContentControl : UserControl, IShowFlyoutSpeechBubbleContent
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public SpeechBubbleContentControlContext SpeechBubbleContentViewModel
        {
            get { return (SpeechBubbleContentControlContext)GetValue(SpeechBubbleContentViewModelProperty); }
            set { SetValue(SpeechBubbleContentViewModelProperty, value); }
        }
        public static readonly DependencyProperty SpeechBubbleContentViewModelProperty = DependencyProperty.Register(nameof(SpeechBubbleContentViewModel), typeof(SpeechBubbleContentControlContext), typeof(SpeechBubbleTextContentControl), new PropertyMetadata(null));

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public SpeechBubbleTextContentControl()
        {
            this.InitializeComponent();
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

                // Spam detection is currently under development:
                if (Data_Manager2.Classes.Settings.getSettingBoolean(SettingsConsts.DEBUG_SETTINGS_ENABLED))
                {
                    markAsSpam_mfi.Visibility = Visibility.Visible;
                    markAsSpam_mfs.Visibility = Visibility.Visible;
                }
                else
                {
                    markAsSpam_mfi.Visibility = Visibility.Collapsed;
                    markAsSpam_mfs.Visibility = Visibility.Collapsed;
                }
            }
        }

        #endregion

        #region --Misc Methods (Private)--
        private void UpdateView(DependencyPropertyChangedEventArgs e)
        {
            SpeechBubbleContentViewModel.UpdateView(e);
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private static void OnChatMessageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SpeechBubbleTextContentControl speechBubbleTextContent)
            {
                speechBubbleTextContent.UpdateView(e);
            }
        }

        private void CopyMessage_mfi_Click(object sender, RoutedEventArgs e)
        {
            SpeechBubbleContentViewModel.SetMessageAsClipboardText();
        }

        private void CopySender_mfi_Click(object sender, RoutedEventArgs e)
        {
            SpeechBubbleContentViewModel.SetFromUserAsClipboardText();
        }

        private void CopyDate_mfi_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.Resources["ChatDateTimeStringValueConverter"] is IValueConverter converter)
            {
                SpeechBubbleContentViewModel.SetDateAsClipboardText(converter);
            }
        }

        private void ResendMsg_mfi_Click(object sender, RoutedEventArgs e)
        {
            ChatDetailsControl chatDetails = VisualTree.FindAscendant<ChatDetailsControl>(this);
            SpeechBubbleContentViewModel.ResendMessage(chatDetails?.VIEW_MODEL);
        }

        private async void MarkAsSpam_mfi_Click(object sender, RoutedEventArgs e)
        {
            await SpeechBubbleContentViewModel.MarkAsSpamAsync();
        }

        private async void DeleteMsg_mfi_Click(object sender, RoutedEventArgs e)
        {
            await SpeechBubbleContentViewModel.DeleteMessageAsync();
        }

        #endregion
    }
}
