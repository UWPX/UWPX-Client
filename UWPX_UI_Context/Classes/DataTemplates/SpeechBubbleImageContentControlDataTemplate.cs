using Data_Manager2.Classes;
using Data_Manager2.Classes.DBTables;

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
            set { SetProperty(ref _ImagePath, value); }
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

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
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
