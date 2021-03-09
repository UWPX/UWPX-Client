using Shared.Classes;
using Windows.UI.Xaml.Media.Imaging;

namespace UWPX_UI_Context.Classes.DataTemplates.Controls
{
    public sealed class SpeechBubbleImageContentControlDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private string _ImagePath;
        public string ImagePath
        {
            get => _ImagePath;
            set => SetProperty(ref _ImagePath, value);
        }
        private BitmapImage _ImageBitmap;
        public BitmapImage ImageBitmap
        {
            get => _ImageBitmap;
            set => SetProperty(ref _ImageBitmap, value);
        }
        private bool _IsLoadingImage;
        public bool IsLoadingImage
        {
            get => _IsLoadingImage;
            set => SetProperty(ref _IsLoadingImage, value);
        }
        private string _ErrorText;
        public string ErrorText
        {
            get => _ErrorText;
            set => SetProperty(ref _ErrorText, value);
        }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
