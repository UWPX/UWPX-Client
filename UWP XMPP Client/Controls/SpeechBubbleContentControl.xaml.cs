using Data_Manager2.Classes;
using Data_Manager2.Classes.DBManager;
using Data_Manager2.Classes.DBTables;
using Data_Manager2.Classes.Events;
using System;
using System.Threading.Tasks;
using UWP_XMPP_Client.Classes;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWP_XMPP_Client.Controls
{
    public sealed partial class SpeechBubbleContentControl : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public ChatMessageTable ChatMessage
        {
            get { return (ChatMessageTable)GetValue(ChatMessageProperty); }
            set
            {
                SetValue(ChatMessageProperty, value);
                showChatMessage();
            }
        }
        public static readonly DependencyProperty ChatMessageProperty = DependencyProperty.Register("ChatMessage", typeof(ChatMessageTable), typeof(SpeechBubbleTopControl), null);

        private string imgPath;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 14/12/2017 Created [Fabian Sauter]
        /// </history>
        public SpeechBubbleContentControl()
        {
            this.InitializeComponent();
            this.imgPath = null;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        /// <summary>
        /// Updates all controls with the proper content.
        /// </summary>
        private void showChatMessage()
        {
            if (ChatMessage != null)
            {
                imageError_grid.Visibility = Visibility.Collapsed;
                imageLoading_grid.Visibility = Visibility.Collapsed;
                loading_prgrb.Value = 0;
                if (ChatMessage.isImage)
                {
                    message_tbx.Visibility = Visibility.Collapsed;
                    ImageTable img = ImageDBManager.INSTANCE.getImageForMessage(ChatMessage);
                    if (img != null)
                    {
                        switch (img.state)
                        {
                            case Data_Manager2.Classes.DownloadState.WAITING:
                                waitForImageDownloadToFinish(img);
                                break;
                            case Data_Manager2.Classes.DownloadState.DOWNLOADING:
                                waitForImageDownloadToFinish(img);
                                break;
                        }
                        Task t = showImageAsync(img);
                        image_img.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        imageLoading_grid.Visibility = Visibility.Collapsed;
                        image_img.Visibility = Visibility.Collapsed;
                        imageError_grid.Visibility = Visibility.Visible;
                        message_tbx.Text = "Unable to get local image path.\nPlease tap to redownload!";
                        message_tbx.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    image_img.Visibility = Visibility.Collapsed;
                    message_tbx.Text = ChatMessage.message ?? "";
                    message_tbx.Visibility = Visibility.Visible;
                }
                DateTime localDateTime = ChatMessage.date.ToLocalTime();
                if (localDateTime.Date.CompareTo(DateTime.Now.Date) == 0)
                {
                    date_tbx.Text = localDateTime.ToString("HH:mm");
                }
                else
                {
                    date_tbx.Text = localDateTime.ToString("dd.MM.yyyy HH:mm");
                }
                switch (ChatMessage.state)
                {
                    case MessageState.SENDING:
                        state_tbx.Text = "\uE724";
                        break;
                    case MessageState.SEND:
                        state_tbx.Text = "\uE725";
                        break;
                    case MessageState.UNREAD:
                        state_tbx.Text = "\uEA63";
                        break;
                    case MessageState.READ:
                        state_tbx.Text = "\uEA64";
                        break;
                    default:
                        state_tbx.Text = "";
                        break;
                }
            }
        }

        private async Task showImageAsync(ImageTable img)
        {
            message_tbx.Visibility = Visibility.Collapsed;
            imageError_grid.Visibility = Visibility.Collapsed;
            imageLoading_grid.Visibility = Visibility.Visible;
            openImage_mfo.IsEnabled = false;
            redownloadImage_mfo.IsEnabled = false;
            imgPath = img.path;

            switch (img.state)
            {
                case Data_Manager2.Classes.DownloadState.DOWNLOADING:
                    loading_prgrb.IsIndeterminate = false;
                    break;
                case Data_Manager2.Classes.DownloadState.WAITING:
                    loading_prgrb.IsIndeterminate = true;
                    break;
                case Data_Manager2.Classes.DownloadState.DONE:
                    img.DownloadStateChanged -= Img_DownloadStateChanged;
                    img.DownloadProgressChanged -= Img_DownloadProgressChanged;
                    image_img.Visibility = Visibility.Visible;
                    imageLoading_grid.Visibility = Visibility.Collapsed;
                    image_img.Source = await img.getBitmapImageAsync();
                    openImage_mfo.IsEnabled = true;
                    redownloadImage_mfo.IsEnabled = true;
                    break;
                case Data_Manager2.Classes.DownloadState.ERROR:
                    img.DownloadStateChanged -= Img_DownloadStateChanged;
                    img.DownloadProgressChanged -= Img_DownloadProgressChanged;
                    imageLoading_grid.Visibility = Visibility.Collapsed;
                    imageError_grid.Visibility = Visibility.Visible;
                    image_img.Source = null;
                    message_tbx.Text = string.IsNullOrWhiteSpace(img.errorMessage) ? "No error message given!" : img.errorMessage;
                    message_tbx.Visibility = Visibility.Visible;
                    redownloadImage_mfo.IsEnabled = true;
                    break;
            }
        }

        private void waitForImageDownloadToFinish(ImageTable img)
        {
            img.DownloadStateChanged -= Img_DownloadStateChanged;
            img.DownloadStateChanged += Img_DownloadStateChanged;
            img.DownloadProgressChanged -= Img_DownloadProgressChanged;
            img.DownloadProgressChanged += Img_DownloadProgressChanged;

            imageLoading_grid.Visibility = Visibility.Visible;
            loading_prgrb.IsIndeterminate = img.state == Data_Manager2.Classes.DownloadState.WAITING;
        }

        private async Task retryImageDownloadAsync()
        {
            ImageTable img = ImageDBManager.INSTANCE.retryImageDownload(ChatMessage);
            if (img != null)
            {
                waitForImageDownloadToFinish(img);
                await showImageAsync(img);
            }
        }

        /// <summary>
        /// Tries to open the current imgPath with the default image viewer.
        /// </summary>
        private async Task openImageAsync()
        {
            try
            {
                StorageFile imageFile = await StorageFile.GetFileFromPathAsync(imgPath);
                await Windows.System.Launcher.LaunchFileAsync(imageFile);
            }
            catch (Exception ex)
            {
                MessageDialog dialog = new MessageDialog(ex.Message, "Ups, something went wrong!");
                await dialog.ShowAsync();
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void image_img_ImageExFailed(object sender, Microsoft.Toolkit.Uwp.UI.Controls.ImageExFailedEventArgs e)
        {
            imageLoading_grid.Visibility = Visibility.Collapsed;
            image_img.Visibility = Visibility.Collapsed;
            imageError_grid.Visibility = Visibility.Visible;
            message_tbx.Text = e.ErrorMessage;
            message_tbx.Visibility = Visibility.Visible;
        }

        private async void Img_DownloadStateChanged(ImageTable img, DownloadStateChangedEventArgs args)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () => await showImageAsync(img));
        }

        private async void Img_DownloadProgressChanged(ImageTable img, DownloadProgressChangedEventArgs args)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => loading_prgrb.Value = 100 * img.progress);
        }

        private async void imageError_grid_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () => await retryImageDownloadAsync());
        }

        private async void openImage_mfo_Click(object sender, RoutedEventArgs e)
        {
            await openImageAsync();
        }

        private void copyLink_mfo_Click(object sender, RoutedEventArgs e)
        {
            DataPackage package = new DataPackage();
            package.SetText(ChatMessage.message);
            Clipboard.SetContent(package);
        }

        private async void openLink_mfo_Click(object sender, RoutedEventArgs e)
        {
            await UiUtils.launchBrowserAsync(new Uri(ChatMessage.message));
        }

        private async void redownloadImage_mfo_Click(object sender, RoutedEventArgs e)
        {
            await retryImageDownloadAsync();
        }

        private void StackPanel_RightTapped(object sender, Windows.UI.Xaml.Input.RightTappedRoutedEventArgs e)
        {
            if (ChatMessage.isImage)
            {
                StackPanel stackPanel = (StackPanel)sender;
                menuFlyout.ShowAt(stackPanel, e.GetPosition(stackPanel));
                var a = ((FrameworkElement)e.OriginalSource).DataContext;
            }
        }

        private async void image_img_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            await openImageAsync();
        }

        #endregion
    }
}
