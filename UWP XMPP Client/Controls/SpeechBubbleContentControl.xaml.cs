using Data_Manager2.Classes.DBManager;
using Data_Manager2.Classes.DBTables;
using System;
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

        public static readonly DependencyProperty ChatMessageProperty = DependencyProperty.Register("ChatMessage", typeof(ChatMessageTable), typeof(SpeechBubbleContentControl), null);

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
                    ImageTable img = ImageManager.INSTANCE.getImageForMessage(ChatMessage);
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
                            case Data_Manager2.Classes.DownloadState.DONE:
                            case Data_Manager2.Classes.DownloadState.ERROR:
                            default:
                                image_img.Source = img.path ?? "Error!";
                                break;
                        }
                    }
                    else
                    {
                        image_img.Source = "Error!";
                    }
                    image_img.Visibility = Visibility.Visible;
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
            }
        }

        private void showImage(ImageTable img)
        {
            switch (img.state)
            {
                case Data_Manager2.Classes.DownloadState.DOWNLOADING:
                    imageLoading_grid.Visibility = Visibility.Visible;
                    imageError_grid.Visibility = Visibility.Collapsed;
                    loading_prgrb.IsIndeterminate = false;
                    break;
                case Data_Manager2.Classes.DownloadState.WAITING:
                    imageLoading_grid.Visibility = Visibility.Visible;
                    imageError_grid.Visibility = Visibility.Collapsed;
                    loading_prgrb.IsIndeterminate = true;
                    break;
                case Data_Manager2.Classes.DownloadState.DONE:
                    img.DownloadStateChanged -= Img_DownloadStateChanged;
                    img.DownloadProgressChanged -= Img_DownloadProgressChanged;
                    image_img.Visibility = Visibility.Visible;
                    imageLoading_grid.Visibility = Visibility.Collapsed;
                    imageError_grid.Visibility = Visibility.Collapsed;
                    image_img.Source = img.path ?? "Error!";
                    break;
                case Data_Manager2.Classes.DownloadState.ERROR:
                    img.DownloadStateChanged -= Img_DownloadStateChanged;
                    img.DownloadProgressChanged -= Img_DownloadProgressChanged;
                    imageLoading_grid.Visibility = Visibility.Collapsed;
                    imageError_grid.Visibility = Visibility.Visible;
                    image_img.Source = img.path ?? "Error!";
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

        private void retryImageDownload()
        {
            ImageTable img = ImageManager.INSTANCE.retryImageDownload(ChatMessage);
            if(img != null)
            {
                waitForImageDownloadToFinish(img);
                showImage(img);
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
        }

        private async void Img_DownloadStateChanged(ImageTable img, Data_Manager.Classes.Events.DownloadStateChangedEventArgs args)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => showImage(img));
        }

        private async void Img_DownloadProgressChanged(ImageTable img, Data_Manager.Classes.Events.DownloadProgressChangedEventArgs args)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => loading_prgrb.Value = 100 * img.progress);
        }

        private async void imageError_grid_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => retryImageDownload());
        }

        #endregion
    }
}
