using System;
using UWP_XMPP_Client.DataTemplates;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace UWP_XMPP_Client.Classes
{
    class UiUtils
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--


        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public static void setBackgroundImage(Image imgControl)
        {
            BackgroundImage img = BackgroundImageCache.selectedImage;
            if (img == null || img.imagePath == null)
            {
                imgControl.Source = null;
                imgControl.Visibility = Visibility.Collapsed;
                return;
            }
            imgControl.Source = new BitmapImage(new Uri(img.imagePath));
            imgControl.Visibility = Visibility.Visible;
        }

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
