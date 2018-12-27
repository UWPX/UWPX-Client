using Data_Manager2.Classes.DBManager;
using Data_Manager2.Classes.DBTables;
using Logging;
using System;
using System.Text;
using System.Threading.Tasks;
using UWPX_UI_Context.Classes.DataTemplates;
using Windows.Storage;
using Windows.UI.Xaml;

namespace UWPX_UI_Context.Classes.DataContext
{
    public sealed class SpeechBubbleImageContentControlContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly SpeechBubbleImageContentControlDataTemplate MODEL = new SpeechBubbleImageContentControlDataTemplate();
        private SpeechBubbleContentContext SpeechBubbleViewModel = null;

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
            if (args.NewValue is SpeechBubbleContentContext newValue)
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
            StringBuilder sb = new StringBuilder("Failed to open image: ");
            sb.Append(MODEL.ImagePath);
            sb.Append('\n');
            sb.Append(errMsg);
            MODEL.ErrorText = sb.ToString();
            MODEL.State = Data_Manager2.Classes.DownloadState.ERROR;
            Logger.Error(sb.ToString(), e);
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
        public async Task<bool> OpenImageUrlWithDefaultBrowserAsync(SpeechBubbleContentContext speechBubbleContentViewModel)
        {
            return await UiUtils.LaunchUriAsync(new Uri(speechBubbleContentViewModel.ChatMessageModel.Message.message));
        }

        public async Task RedownloadImageAsync()
        {

        }

        public async Task CancelImageDownloadAsync()
        {

        }

        public async Task StartImageDownloadAsync()
        {

        }

        #endregion

        #region --Misc Methods (Private)--
        private async Task LoadImageAsync(SpeechBubbleContentContext speechBubbleContentViewModel)
        {
            if (SpeechBubbleViewModel is null || SpeechBubbleViewModel.ChatMessageModel.Message is null)
            {
                return;
            }
            ImageTable image = await ImageDBManager.INSTANCE.getImageForMessageAsync(SpeechBubbleViewModel.ChatMessageModel.Message);
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
