using Shared.Classes;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml.Media.Imaging;

namespace UWPX_UI_Context.Classes.DataTemplates.Controls.Chat
{
    public class EditImageControlDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private SoftwareBitmap _OriginalImage;
        public SoftwareBitmap OriginalImage
        {
            get => _OriginalImage;
            set => SetProperty(ref _OriginalImage, value);
        }

        private WriteableBitmap _WritableImage;
        public WriteableBitmap WritableImage
        {
            get => _WritableImage;
            set => SetProperty(ref _WritableImage, value);
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
