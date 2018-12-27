using Data_Manager2.Classes;
using Data_Manager2.Classes.DBTables;
using Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

namespace UWPX_UI_Context.Classes.DataTemplates
{
    public sealed class SpeechBubbleImageContentControlDataTemplate : AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private string _ImagePath;
        public string ImagePath
        {
            get { return _ImagePath; }
            set { SetImagePathProperty(value); }
        }
        private BitmapImage _Image;
        public BitmapImage Image
        {
            get { return _Image; }
            set { SetProperty(ref _Image, value); }
        }
        private string _ErrorText;
        public string ErrorText
        {
            get { return _ErrorText; }
            set { SetProperty(ref _ErrorText, value); }
        }
        private DownloadState _State;
        public DownloadState State
        {
            get { return _State; }
            set { SetProperty(ref _State, value); }
        }
        private double _DownloadProgress;
        public double DownloadProgress
        {
            get { return _DownloadProgress; }
            set { SetProperty(ref _DownloadProgress, value); }
        }
        private bool _IsLoadingImage;
        public bool IsLoadingImage
        {
            get { return _IsLoadingImage; }
            set { SetProperty(ref _IsLoadingImage, value); }
        }

        private ImageTable image = null;
        private CancellationTokenSource loadImageCancellationSource = null;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private void SetImagePathProperty(string value)
        {
            if (SetProperty(ref _ImagePath, value))
            {
                OnImagePathChanged(value);
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        private void OnImagePathChanged(string newValue)
        {
            if (!(loadImageCancellationSource is null))
            {
                loadImageCancellationSource.Cancel();
            }
            loadImageCancellationSource = new CancellationTokenSource();

            Task.Run(async () =>
            {
                Image = await GetBitmapImageAsync(newValue);
            }, loadImageCancellationSource.Token);
        }

        public void UpdateView(ImageTable image)
        {
            if (!(this.image is null))
            {
                this.image.DownloadProgressChanged -= ImageTable_DownloadProgressChanged;
                this.image.DownloadStateChanged -= Image_DownloadStateChanged;
            }

            this.image = image;
            if (!(this.image is null))
            {
                this.image.DownloadProgressChanged += ImageTable_DownloadProgressChanged;
                this.image.DownloadStateChanged += Image_DownloadStateChanged;

                ImagePath = image.path;
                ErrorText = image.errorMessage;
                State = image.state;
                DownloadProgress = image.progress;
            }
        }

        #endregion

        #region --Misc Methods (Private)--
        /// <summary>
        /// Converts the path to an BitmapImage.
        /// This is a workaround to open also images that are stored on a separate drive.
        /// </summary>
        /// <param name="path">The absolute path to the image.</param>
        /// <returns>The BitmapImage representation of the current path object.</returns>
        private async Task<BitmapImage> GetBitmapImageAsync(string path)
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
                await UiUtils.CallDispatcherAsync(async () =>
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
                if (!(ex is null))
                {
                    Logger.Error("Failed to load image: " + path, ex);
                    return null;
                }

                return img;
            }
            catch (Exception e)
            {
                Logger.Error("Failed to load image: " + path, e);
                return null;
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void Image_DownloadStateChanged(ImageTable img, Data_Manager2.Classes.Events.DownloadStateChangedEventArgs args)
        {
            State = args.STATE;
        }

        private void ImageTable_DownloadProgressChanged(ImageTable img, Data_Manager2.Classes.Events.DownloadProgressChangedEventArgs args)
        {
            DownloadProgress = args.PROGRESS;
        }

        #endregion
    }
}
