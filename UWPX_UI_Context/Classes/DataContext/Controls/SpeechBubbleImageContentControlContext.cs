using Data_Manager2.Classes;
using Data_Manager2.Classes.DBManager;
using Data_Manager2.Classes.DBTables;
using Logging;
using Shared.Classes.Network;
using System;
using System.Text;
using System.Threading.Tasks;
using UWPX_UI_Context.Classes.DataTemplates.Controls;
using Windows.Storage;
using Windows.UI.Xaml;

namespace UWPX_UI_Context.Classes.DataContext.Controls
{
    public sealed class SpeechBubbleImageContentControlContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly SpeechBubbleImageContentControlDataTemplate MODEL = new SpeechBubbleImageContentControlDataTemplate();
        private SpeechBubbleContentControlContext SpeechBubbleViewModel = null;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public async Task UpdateViewAsync(DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue is SpeechBubbleContentControlContext newValue)
            {
                SpeechBubbleViewModel = newValue;
                await LoadImageAsync(SpeechBubbleViewModel);
            }
            else
            {
                SpeechBubbleViewModel = null;
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
            MODEL.State = DownloadState.ERROR;
            Logger.Error(sb.ToString(), e);
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
        public async Task<bool> OpenImageUrlWithDefaultBrowserAsync(SpeechBubbleContentControlContext speechBubbleContentViewModel)
        {
            return await UiUtils.LaunchUriAsync(new Uri(speechBubbleContentViewModel.ChatMessageModel.Message.message));
        }

        public async Task RedownloadImageAsync()
        {
            if (!(MODEL.Image is null))
            {
                await ConnectionHandler.INSTANCE.IMAGE_DOWNLOAD_HANDLER.RedownloadImageAsync(MODEL.Image);
            }
        }

        public void CancelImageDownload()
        {
            if (!(MODEL.Image is null))
            {
                ConnectionHandler.INSTANCE.IMAGE_DOWNLOAD_HANDLER.CancelDownload(MODEL.Image);
            }
        }

        public async Task StartImageDownloadAsync()
        {
            if (!(MODEL.Image is null))
            {
                await ConnectionHandler.INSTANCE.IMAGE_DOWNLOAD_HANDLER.DownloadImageAsync(MODEL.Image);
            }
            else
            {
                MODEL.Image = await ConnectionHandler.INSTANCE.IMAGE_DOWNLOAD_HANDLER.DownloadImageAsync(SpeechBubbleViewModel.ChatMessageModel.Message);
            }
        }

        #endregion

        #region --Misc Methods (Private)--
        private async Task LoadImageAsync(SpeechBubbleContentControlContext speechBubbleContentViewModel)
        {
            if (SpeechBubbleViewModel is null || SpeechBubbleViewModel.ChatMessageModel.Message is null)
            {
                return;
            }
            ImageTable image = await ImageDBManager.INSTANCE.getImageAsync(SpeechBubbleViewModel.ChatMessageModel.Message);
            MODEL.UpdateView(image);
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
