using Data_Manager2.Classes.DBTables;
using Logging;
using Shared.Classes;
using Shared.Classes.Network;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

namespace UWPX_UI_Context.Classes.DataTemplates.Controls
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
        private BitmapImage _ImageBitmap;
        public BitmapImage ImageBitmap
        {
            get { return _ImageBitmap; }
            set { SetProperty(ref _ImageBitmap, value); }
        }
        private bool _IsLoadingImage;
        public bool IsLoadingImage
        {
            get { return _IsLoadingImage; }
            set { SetProperty(ref _IsLoadingImage, value); }
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
        private ImageTable _Image;
        public ImageTable Image
        {
            get { return _Image; }
            set { SetImage(value); }
        }

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

        private void SetImage(ImageTable value)
        {
            ImageTable oldImage = Image;
            if (SetProperty(ref _Image, value, nameof(Image)))
            {
                if (!(oldImage is null))
                {
                    oldImage.PropertyChanged -= Image_PropertyChanged;
                }
                if (!(Image is null))
                {
                    Image.PropertyChanged += Image_PropertyChanged;
                    State = Image.State;
                    ErrorText = Image.Error.ToString();
                    ImagePath = Path.Combine(Image.TargetFolderPath, Image.TargetFileName);
                }
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
                if (Image is null && Image.State != DownloadState.DONE)
                {
                    Image = null;
                }
                else
                {
                    ImageBitmap = await GetBitmapImageAsync(newValue);
                }
            }, loadImageCancellationSource.Token);
        }

        public void UpdateView(ImageTable image)
        {
            Image = image;
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
        private void Image_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!(sender is ImageTable image))
            {
                return;
            }

            if (string.Equals(e.PropertyName, nameof(ImageTable.State)))
            {
                State = image.State;
                if (image.State == DownloadState.DONE)
                {
                    OnImagePathChanged(ImagePath);
                }
            }
            else if (string.Equals(e.PropertyName, nameof(ImageTable.Error)))
            {
                ErrorText = image.Error.ToString();
            }
        }

        #endregion
    }
}
