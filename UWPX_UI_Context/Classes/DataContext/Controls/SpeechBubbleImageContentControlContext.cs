using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Logging;
using Manager.Classes;
using Shared.Classes;
using Shared.Classes.Network;
using Storage.Classes.Models.Chat;
using UWPX_UI_Context.Classes.DataTemplates.Controls;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

namespace UWPX_UI_Context.Classes.DataContext.Controls
{
    public sealed class SpeechBubbleImageContentControlContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly SpeechBubbleImageContentControlDataTemplate MODEL = new SpeechBubbleImageContentControlDataTemplate();
        private SpeechBubbleContentControlContext SpeechBubbleViewModel = null;

        private CancellationTokenSource loadImageCancellationSource = null;
        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public SpeechBubbleImageContentControlContext()
        {
            MODEL.PropertyChanged += OnModelPropertyChanged;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void UpdateView(DependencyPropertyChangedEventArgs args)
        {
            if (args.OldValue is SpeechBubbleContentControlContext oldValue)
            {
                SpeechBubbleViewModel.MODEL.Message.Message.imageReceived.PropertyChanged -= OnImagePropertyChanged;
            }

            if (args.NewValue is SpeechBubbleContentControlContext newValue)
            {
                Debug.Assert(newValue.MODEL.Message.Message.isImage);
                Debug.Assert(newValue.MODEL.Message.Message.imageReceived is not null);
                SpeechBubbleViewModel = newValue;
                SpeechBubbleViewModel.MODEL.Message.Message.imageReceived.PropertyChanged += OnImagePropertyChanged;
                LoadImageProperties(SpeechBubbleViewModel.MODEL.Message.Message.imageReceived);
            }
            else
            {
                SpeechBubbleViewModel = null;
                LoadImageProperties(null);
            }
        }

        public void OnImageExFailed(Exception e, string errMsg)
        {
            MODEL.IsLoadingImage = false;
            StringBuilder sb = new StringBuilder("Failed to open image: ");
            sb.Append(MODEL.ImagePath);
            sb.Append('\n');
            sb.Append(errMsg);
            MODEL.ErrorText = sb.ToString();
            Logger.Error(sb.ToString(), e);
            SpeechBubbleViewModel.MODEL.Message.Message.imageReceived.state = DownloadState.ERROR;
            SpeechBubbleViewModel.MODEL.Message.Message.imageReceived.Update();
        }

        public void OnImageExOpened()
        {
            MODEL.IsLoadingImage = false;
        }

        /// <summary>
        /// Tries to open the current ImagePath with the default image viewer application.
        /// </summary>
        /// <returns>Returns true on success.</returns>
        public async Task<bool> OpenImageWithDefaultImageViewerAsync()
        {
            StorageFile imageFile = await StorageFile.GetFileFromPathAsync(MODEL.ImagePath);
            return await Windows.System.Launcher.LaunchFileAsync(imageFile);
        }

        /// <summary>
        /// Tries to open the current image URL with the default web browser.
        /// </summary>
        /// <returns>Returns true on success.</returns>
        public IAsyncOperation<bool> OpenImageUrlWithDefaultBrowserAsync(SpeechBubbleContentControlContext speechBubbleContentViewModel)
        {
            return UiUtils.LaunchUriAsync(new Uri(speechBubbleContentViewModel.MODEL.Message.Message.message));
        }

        public Task RedownloadImageAsync()
        {
            return ConnectionHandler.INSTANCE.IMAGE_DOWNLOAD_HANDLER.RedownloadAsync(SpeechBubbleViewModel.MODEL.Message.Message.imageReceived);
        }

        public void CancelImageDownload()
        {
            ConnectionHandler.INSTANCE.IMAGE_DOWNLOAD_HANDLER.CancelDownload(SpeechBubbleViewModel.MODEL.Message.Message.imageReceived);
        }

        public Task StartImageDownloadAsync()
        {
            return ConnectionHandler.INSTANCE.IMAGE_DOWNLOAD_HANDLER.StartDownloadAsync(SpeechBubbleViewModel.MODEL.Message.Message.imageReceived);
        }

        #endregion

        #region --Misc Methods (Private)--
        private void LoadImageProperties(ChatMessageImageReceivedModel image)
        {
            if (image is not null)
            {
                MODEL.ErrorText = image.error.ToString();
                // Only set the image path in case the image was successfully downloaded to prevent exceptions:
                MODEL.ImagePath = image.state == DownloadState.DONE ? image.GetFullPath() : null;
            }
        }

        private void TryLoadingImageFromPath()
        {
            ChatMessageImageReceivedModel img = SpeechBubbleViewModel.MODEL.Message.Message.imageReceived;
            if (img.state == DownloadState.DONE)
            {
                if (loadImageCancellationSource is not null)
                {
                    loadImageCancellationSource.Cancel();
                }
                loadImageCancellationSource = new CancellationTokenSource();

                Task.Run(async () =>
                {
                    MODEL.IsLoadingImage = true;
                    MODEL.ImageBitmap = await GetBitmapImageAsync(img, MODEL.ImagePath);
                    if (MODEL.ImageBitmap is null)
                    {
                        MODEL.IsLoadingImage = false;
                    }
                }, loadImageCancellationSource.Token);
            }
        }

        /// <summary>
        /// Converts the path to an BitmapImage.
        /// This is a workaround to open also images that are stored on a separate drive.
        /// </summary>
        /// <param name="imgModel">The actual image the <paramref name="path"/> corresponds to.</param>
        /// <param name="path">The absolute path to the image.</param>
        /// <returns>The BitmapImage representation of the current path object.</returns>
        private async Task<BitmapImage> GetBitmapImageAsync(ChatMessageImageReceivedModel imgModel, string path)
        {
            if (path is null)
            {
                return null;
            }

            try
            {
                StorageFile file = await StorageFile.GetFileFromPathAsync(path);
                if (file is null)
                {
                    return null;
                }

                BitmapImage img = null;
                // Bitmap stuff has to be done in the UI thread,
                // so make sure we execute it there:
                Exception ex = null;
                await SharedUtils.CallDispatcherAsync(async () =>
                {
                    try
                    {
                        img = new BitmapImage();
                        img.SetSource(await file.OpenReadAsync());
                    }
                    catch (Exception e)
                    {
                        ex = e;
                    }
                });

                // If loading the image failed log the exception:
                if (ex is not null)
                {
                    Logger.Error("Failed to load image: " + path, ex);
                    MODEL.ErrorText = "Failed to load image. Try downloading it again.";
                    imgModel.state = DownloadState.ERROR;
                    imgModel.Update();
                    return null;
                }

                return img;
            }
            catch (Exception e)
            {
                Logger.Error("Failed to load image: " + path, e);
                MODEL.ErrorText = "Failed to load image. Try downloading it again.";
                imgModel.state = DownloadState.ERROR;
                imgModel.Update();
                return null;
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void OnImagePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ChatMessageImageReceivedModel img = SpeechBubbleViewModel.MODEL.Message.Message.imageReceived;
            if (img is null)
            {
                return;
            }
            switch (e.PropertyName)
            {
                case nameof(ChatMessageImageReceivedModel.state):
                case nameof(ChatMessageImageReceivedModel.targetFileName):
                case nameof(ChatMessageImageReceivedModel.targetFolderPath):
                    if (img.state == DownloadState.DONE)
                    {
                        MODEL.ImagePath = img.GetFullPath();
                    }
                    break;

                case nameof(ChatMessageImageReceivedModel.error):
                    MODEL.ErrorText = img.error.ToString();
                    break;

                default:
                    break;
            }
        }

        private void OnModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (string.Equals(e.PropertyName, nameof(SpeechBubbleImageContentControlDataTemplate.ImagePath)))
            {
                TryLoadingImageFromPath();
            }
        }

        #endregion
    }
}
