using Logging;
using System;
using System.Text;
using System.Threading.Tasks;
using UWPX_UI.Dialogs;
using UWPX_UI_Context.Classes;
using UWPX_UI_Context.Classes.DataContext;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPX_UI.Controls.Chat.SpeechBubbles.Content
{
    public sealed partial class SpeechBubbleImageContentControl : UserControl, IShowFlyoutSpeechBubbleContent
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public SpeechBubbleContentContext SpeechBubbleContentViewModel
        {
            get { return (SpeechBubbleContentContext)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(SpeechBubbleContentViewModel), typeof(SpeechBubbleContentContext), typeof(SpeechBubbleImageContentControl), new PropertyMetadata(null, OnSpeechBubbleContentViewModelChanged));

        private readonly SpeechBubbleImageContentControlContext VIEW_MODEL = new SpeechBubbleImageContentControlContext();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public SpeechBubbleImageContentControl()
        {
            this.InitializeComponent();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void ShowFlyout(FrameworkElement sender)
        {
            if (Resources["options_mfo"] is MenuFlyout flyout)
            {
                flyout.ShowAt(sender);
            }
        }

        #endregion

        #region --Misc Methods (Private)--
        private async Task UpdateViewAsync(DependencyPropertyChangedEventArgs args)
        {
            await VIEW_MODEL.UpdateViewAsync(args);
        }

        /// <summary>
        /// Tries to open the current imgPath with the default image viewer.
        /// </summary>
        private async Task OpenImageAsync()
        {
            try
            {
                StorageFile imageFile = await StorageFile.GetFileFromPathAsync(VIEW_MODEL.MODEL.ImagePath);
                await Windows.System.Launcher.LaunchFileAsync(imageFile);
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to open image with default application!", ex);
                InfoDialog dialog = new InfoDialog(ex.Message, "Ups, something went wrong!\nView the logs for more information.");
                await UiUtils.ShowDialogAsync(dialog);
            }
        }

        private async Task OpenImageUrlInBrowserAsync()
        {
            try
            {
                bool success = await UiUtils.LaunchUriAsync(new Uri(SpeechBubbleContentViewModel.ChatMessageModel.Message.message));
                if (!success)
                {
                    Logger.Error("Failed to open image URL with default application!");
                    InfoDialog dialog = new InfoDialog("Error", "Ups, something went wrong!\nView the logs for more information.");
                    await UiUtils.ShowDialogAsync(dialog);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to open image with default application!", ex);
                InfoDialog dialog = new InfoDialog(ex.Message, "Ups, something went wrong!\nView the logs for more information.");
                await UiUtils.ShowDialogAsync(dialog);
            }
        }
        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private static async void OnSpeechBubbleContentViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SpeechBubbleImageContentControl speechBubbleImageContent)
            {
                await speechBubbleImageContent.UpdateViewAsync(e);
            }
        }

        private void Image_img_ImageExFailed(object sender, Microsoft.Toolkit.Uwp.UI.Controls.ImageExFailedEventArgs e)
        {
            StringBuilder sb = new StringBuilder("Failed to open image: ");
            sb.Append(image_img.Source);
            sb.Append('\n');
            sb.Append(e.ErrorMessage);
            Logger.Warn(sb.ToString());
            VIEW_MODEL.MODEL.ErrorText = sb.ToString();
            VIEW_MODEL.MODEL.State = Data_Manager2.Classes.DownloadState.ERROR;
        }

        private async void OpenImage_mfi_Click(object sender, RoutedEventArgs e)
        {
            await OpenImageAsync();
        }

        private async void OpenImageBrowser_mfi_Click(object sender, RoutedEventArgs e)
        {
            await OpenImageUrlInBrowserAsync();
        }

        private async void Image_img_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            await OpenImageAsync();
        }

        private void CopyImageLink_mfi_Click(object sender, RoutedEventArgs e)
        {
            UiUtils.SetClipboardText(SpeechBubbleContentViewModel.ChatMessageModel.Message.message);
        }

        private void CopyNickname_mfi_Click(object sender, RoutedEventArgs e)
        {
            UiUtils.SetClipboardText(SpeechBubbleContentViewModel.ChatMessageModel.Message.fromUser);
        }

        #endregion

        private void CopyDate_mfi_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Redownload_mfi_Click(object sender, RoutedEventArgs e)
        {

        }

        private void StopDownload_mfi_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
