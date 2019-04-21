using System;
using UWPX_UI.Classes;
using UWPX_UI_Context.Classes.DataContext.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace UWPX_UI.Controls.Chat
{
    public sealed partial class ChatBackgroundControl: UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly ChatBackgroundControlContext VIEW_MODEL = new ChatBackgroundControlContext();
        private SplashScreenImageScale curImageScale = SplashScreenImageScale.TINY;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ChatBackgroundControl()
        {
            InitializeComponent();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void SetImageScale()
        {
            if (ActualWidth == 0 || ActualHeight == 0)
            {
                return;
            }

            if (ActualWidth >= 3000 || ActualHeight >= 3000)
            {
                if (curImageScale != SplashScreenImageScale.HUGE)
                {
                    fallbackBackground_img.Source = new BitmapImage(new Uri("ms-appx:///Assets/Images/SplashScreen/splash_screen_4000.png", UriKind.Absolute));
                    curImageScale = SplashScreenImageScale.HUGE;
                }
            }
            else if (ActualWidth >= 2000 || ActualHeight >= 2000)
            {
                if (curImageScale != SplashScreenImageScale.LARGE)
                {
                    fallbackBackground_img.Source = new BitmapImage(new Uri("ms-appx:///Assets/Images/SplashScreen/splash_screen_3000.png", UriKind.Absolute));
                    curImageScale = SplashScreenImageScale.LARGE;
                }
            }
            else if (ActualWidth >= 1000 || ActualHeight >= 1000)
            {
                if (curImageScale != SplashScreenImageScale.MEDIUM)
                {
                    fallbackBackground_img.Source = new BitmapImage(new Uri("ms-appx:///Assets/Images/SplashScreen/splash_screen_2000.png", UriKind.Absolute));
                    curImageScale = SplashScreenImageScale.MEDIUM;
                }
            }
            else if (ActualWidth >= 800 || ActualHeight >= 800)
            {
                if (curImageScale != SplashScreenImageScale.SMALL)
                {
                    fallbackBackground_img.Source = new BitmapImage(new Uri("ms-appx:///Assets/Images/SplashScreen/splash_screen_1000.png", UriKind.Absolute));
                    curImageScale = SplashScreenImageScale.SMALL;
                }
            }
            else if (curImageScale != SplashScreenImageScale.TINY)
            {
                fallbackBackground_img.Source = new BitmapImage(new Uri("ms-appx:///Assets/Images/SplashScreen/splash_screen_800.png", UriKind.Absolute));
                curImageScale = SplashScreenImageScale.TINY;
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            this.SizeChanged -= ChatBackgroundControl_SizeChanged;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            SetImageScale();

            this.SizeChanged -= ChatBackgroundControl_SizeChanged;
            this.SizeChanged += ChatBackgroundControl_SizeChanged;
        }

        private void ChatBackgroundControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetImageScale();
        }

        #endregion
    }
}
