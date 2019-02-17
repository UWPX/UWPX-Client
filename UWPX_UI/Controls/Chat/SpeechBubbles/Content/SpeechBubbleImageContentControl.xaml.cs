using Logging;
using Shared.Classes.Network;
using System;
using System.Threading.Tasks;
using UWPX_UI.Dialogs;
using UWPX_UI_Context.Classes;
using UWPX_UI_Context.Classes.DataContext.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace UWPX_UI.Controls.Chat.SpeechBubbles.Content
{
    public sealed partial class SpeechBubbleImageContentControl : UserControl, IShowFlyoutSpeechBubbleContent
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public SpeechBubbleContentControlContext SpeechBubbleContentViewModel
        {
            get { return (SpeechBubbleContentControlContext)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(SpeechBubbleContentViewModel), typeof(SpeechBubbleContentControlContext), typeof(SpeechBubbleImageContentControl), new PropertyMetadata(null, OnSpeechBubbleContentViewModelChanged));

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
        private void UpdateView(DependencyPropertyChangedEventArgs args)
        {
            VIEW_MODEL.UpdateView(args);
        }

        private async Task OpenImageWithDefaultImageViewerAsync()
        {
            try
            {
                if (!await VIEW_MODEL.OpenImageWithDefaultImageViewerAsync())
                {
                    Logger.Error("Failed to open image with default application!");
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

        private async Task OpenImageUrlWithDefaultBrowserAsync()
        {
            try
            {
                if (!await VIEW_MODEL.OpenImageUrlWithDefaultBrowserAsync(SpeechBubbleContentViewModel))
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
        private static void OnSpeechBubbleContentViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SpeechBubbleImageContentControl speechBubbleImageContent)
            {
                speechBubbleImageContent.UpdateView(e);
            }
        }

        private void Image_img_ImageExFailed(object sender, Microsoft.Toolkit.Uwp.UI.Controls.ImageExFailedEventArgs e)
        {
            VIEW_MODEL.OnImageExFailed(e.ErrorException, e.ErrorMessage);
        }

        private async void OpenImage_mfi_Click(object sender, RoutedEventArgs e)
        {
            await OpenImageWithDefaultImageViewerAsync();
        }

        private async void OpenImageBrowser_mfi_Click(object sender, RoutedEventArgs e)
        {
            await OpenImageUrlWithDefaultBrowserAsync();
        }

        private async void Image_img_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            await OpenImageWithDefaultImageViewerAsync();
        }

        private void CopyImageLink_mfi_Click(object sender, RoutedEventArgs e)
        {
            SpeechBubbleContentViewModel.SetMessageAsClipboardText();
        }

        private void CopyNickname_mfi_Click(object sender, RoutedEventArgs e)
        {
            SpeechBubbleContentViewModel.SetFromUserAsClipboardText();
        }

        private void Redownload_mfi_Click(object sender, RoutedEventArgs e)
        {
            VIEW_MODEL.RedownloadImage();
        }

        private async void StartDownload_mfi_Click(object sender, RoutedEventArgs e)
        {
            await VIEW_MODEL.StartImageDownloadAsync();
        }

        private void CancelDownload_mfi_Click(object sender, RoutedEventArgs e)
        {
            VIEW_MODEL.CancelImageDownload();
        }

        private void CopyDate_mfi_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.Resources["ChatDateTimeStringValueConverter"] is IValueConverter converter)
            {
                SpeechBubbleContentViewModel.SetDateAsClipboardText(converter);
            }
        }

        private void MenuFlyout_Opening(object sender, object e)
        {
            openImage_mfi.Visibility = Visibility.Collapsed;
            redownload_mfi.Visibility = Visibility.Collapsed;
            startDownload_mfi.Visibility = Visibility.Collapsed;
            cancelDownload_mfi.Visibility = Visibility.Collapsed;

            switch (VIEW_MODEL.MODEL.State)
            {
                case DownloadState.QUEUED:
                case DownloadState.DOWNLOADING:
                    cancelDownload_mfi.Visibility = Visibility.Visible;
                    break;

                case DownloadState.DONE:
                    redownload_mfi.Visibility = Visibility.Visible;
                    openImage_mfi.Visibility = Visibility.Visible;
                    break;

                case DownloadState.NOT_QUEUED:
                case DownloadState.ERROR:
                case DownloadState.CANCELED:
                    startDownload_mfi.Visibility = Visibility.Visible;
                    break;
            }
        }

        #endregion
    }
}
