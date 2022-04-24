using Shared.Classes;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace UWPX_UI_Context.Classes.DataTemplates.Controls
{
    public sealed class AccountImagePresenceControlDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private string _Initials;
        public string Initials
        {
            get => _Initials;
            set => SetProperty(ref _Initials, value);
        }

        private SoftwareBitmapSource _Image;
        public SoftwareBitmapSource Image
        {
            get => _Image;
            set => SetProperty(ref _Image, value);
        }

        private LinearGradientBrush _Background;
        public LinearGradientBrush Background
        {
            get => _Background;
            set => SetProperty(ref _Background, value);
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
