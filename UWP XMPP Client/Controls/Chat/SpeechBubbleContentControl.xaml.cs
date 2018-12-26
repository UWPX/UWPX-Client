using Data_Manager2.Classes;
using Data_Manager2.Classes.DBManager;
using Data_Manager2.Classes.DBTables;
using Data_Manager2.Classes.Events;
using System;
using System.Threading.Tasks;
using UWP_XMPP_Client.Classes;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWP_XMPP_Client.Controls.Chat
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
        public static readonly DependencyProperty ChatMessageProperty = DependencyProperty.Register(nameof(ChatMessage), typeof(ChatMessageTable), typeof(SpeechBubbleContentControl), null);

        public ChatTable Chat
        {
            get { return (ChatTable)GetValue(ChatProperty); }
            set
            {
                SetValue(ChatProperty, value);
                setSenderNicknameVisability();
            }
        }
        public static readonly DependencyProperty ChatProperty = DependencyProperty.Register(nameof(Chat), typeof(ChatTable), typeof(SpeechBubbleContentControl), null);

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }
        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(nameof(Message), typeof(string), typeof(SpeechBubbleContentControl), new PropertyMetadata(""));

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
        private void setSenderNicknameVisability()
        {
            nick_stckp.Visibility = (Chat != null && Chat.chatType == ChatType.MUC) ? Visibility.Visible : Visibility.Collapsed;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        /// <summary>
        /// Updates all controls with the proper content.
        /// </summary>
        private async void showChatMessage()
        {
            if (ChatMessage != null)
            {
                imageError_grid.Visibility = Visibility.Collapsed;
                imageLoading_grid.Visibility = Visibility.Collapsed;
                loading_prgrb.Value = 0;
                if (ChatMessage.isImage)
                {
                    message_tbx.Visibility = Visibility.Collapsed;
                    if (ChatMessage.isDummyMessage)
                    {
                        Task.Run(async () => await retryImageDownloadAsync());
                    }
                    else
                    {
                        ImageTable img = await ImageDBManager.INSTANCE.getImageForMessageAsync(ChatMessage);

                        if (img != null)
                        {
                            switch (img.state)
                            {
                                case DownloadState.WAITING:
                                case DownloadState.DOWNLOADING:
                                    waitForImageDownloadToFinish(img);
                                    break;
                            }
                            await showImageAsync(img);
                            image_img.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            imageLoading_grid.Visibility = Visibility.Collapsed;
                            image_img.Visibility = Visibility.Collapsed;
                            imageError_grid.Visibility = Visibility.Visible;
                            Message = "Unable to get local image path.\nPlease tap to redownload!";
                            message_tbx.Visibility = Visibility.Visible;
                        }
                    }
                }
                else
                {
                    image_img.Visibility = Visibility.Collapsed;
                    Message = ChatMessage.message ?? "";
                    message_tbx.Visibility = Visibility.Visible;
                }
                switch (ChatMessage.state)
                {
                    case MessageState.SENDING:
                        state_tbx.Text = "\uE724";
                        stateCheck_tbx.Visibility = Visibility.Collapsed;
                        toEncryp_pgb.Visibility = Visibility.Collapsed;
                        break;

                    case MessageState.SEND:
                        state_tbx.Text = "\uE725";
                        stateCheck_tbx.Visibility = Visibility.Collapsed;
                        toEncryp_pgb.Visibility = Visibility.Collapsed;
                        break;

                    case MessageState.DELIVERED:
                        state_tbx.Text = "\uE725";
                        stateCheck_tbx.Visibility = Visibility.Visible;
                        toEncryp_pgb.Visibility = Visibility.Collapsed;
                        break;

                    case MessageState.UNREAD:
                        state_tbx.Text = "\uEA63";
                        stateCheck_tbx.Visibility = Visibility.Collapsed;
                        toEncryp_pgb.Visibility = Visibility.Collapsed;
                        break;

                    case MessageState.READ:
                        state_tbx.Text = "\uEA64";
                        stateCheck_tbx.Visibility = Visibility.Collapsed;
                        toEncryp_pgb.Visibility = Visibility.Collapsed;
                        break;

                    case MessageState.TO_ENCRYPT:
                        state_tbx.Text = "\uE724";
                        stateCheck_tbx.Visibility = Visibility.Collapsed;
                        toEncryp_pgb.Visibility = Visibility.Visible;
                        break;

                    case MessageState.ENCRYPT_FAILED:
                        state_tbx.Text = "\uEA39";
                        stateCheck_tbx.Visibility = Visibility.Collapsed;
                        toEncryp_pgb.Visibility = Visibility.Collapsed;
                        break;

                    default:
                        state_tbx.Text = "";
                        stateCheck_tbx.Visibility = Visibility.Collapsed;
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

            if (img.state == DownloadState.DONE)
            {
                image_img.Source = await img.getBitmapImageAsync();
                if (image_img.Source is null)
                {
                    img.DownloadStateChanged -= Img_DownloadStateChanged;
                    img.DownloadProgressChanged -= Img_DownloadProgressChanged;
                    imageLoading_grid.Visibility = Visibility.Collapsed;
                    imageError_grid.Visibility = Visibility.Visible;
                    Message = "Image not found!";
                    message_tbx.Visibility = Visibility.Visible;
                    redownloadImage_mfo.IsEnabled = true;
                    return;
                }
            }

            switch (img.state)
            {
                case DownloadState.DOWNLOADING:
                    loading_prgrb.IsIndeterminate = false;
                    break;

                case DownloadState.WAITING:
                    loading_prgrb.IsIndeterminate = true;
                    break;

                case DownloadState.DONE:
                    img.DownloadStateChanged -= Img_DownloadStateChanged;
                    img.DownloadProgressChanged -= Img_DownloadProgressChanged;
                    image_img.Visibility = Visibility.Visible;
                    imageLoading_grid.Visibility = Visibility.Collapsed;
                    openImage_mfo.IsEnabled = true;
                    redownloadImage_mfo.IsEnabled = true;
                    break;

                case DownloadState.ERROR:
                    img.DownloadStateChanged -= Img_DownloadStateChanged;
                    img.DownloadProgressChanged -= Img_DownloadProgressChanged;
                    imageLoading_grid.Visibility = Visibility.Collapsed;
                    imageError_grid.Visibility = Visibility.Visible;
                    image_img.Source = null;
                    Message = string.IsNullOrWhiteSpace(img.errorMessage) ? "No error message given!" : img.errorMessage;
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
            loading_prgrb.IsIndeterminate = img.state == DownloadState.WAITING;
        }

        private async Task retryImageDownloadAsync()
        {
            ImageTable img = await ImageDBManager.INSTANCE.retryImageDownloadAsync(ChatMessage);
            if (img != null)
            {
                waitForImageDownloadToFinish(img);
                await showImageAsync(img).ConfigureAwait(false);
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
            Message = e.ErrorMessage;
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
            UiUtils.addTextToClipboard(ChatMessage.message);
        }

        private async void openLink_mfo_Click(object sender, RoutedEventArgs e)
        {
            await UiUtils.launchUriAsync(new Uri(ChatMessage.message));
        }

        private async void redownloadImage_mfo_Click(object sender, RoutedEventArgs e)
        {
            await retryImageDownloadAsync().ConfigureAwait(false);
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
