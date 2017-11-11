using System;
using System.Text.RegularExpressions;
using UWP_XMPP_Client.DataTemplates;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
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

        public static string getRandomMaterialColor()
        {
            Random r = new Random();
            switch (r.Next(17))
            {
                case 0:
                    return "#4CAF50";
                case 1:
                    return "#8BC34A";
                case 2:
                    return "#CDDC39";
                case 3:
                    return "#F44336";
                case 4:
                    return "#E91E63";
                case 5:
                    return "#9C27B0";
                case 6:
                    return "#673AB7";
                case 7:
                    return "#3F51B5";
                case 8:
                    return "#2196F3";
                case 9:
                    return "#03A9F4";
                case 10:
                    return "#00BCD4";
                case 11:
                    return "#009688";
                case 12:
                    return "#FFEB3B";
                case 13:
                    return "#FFC107";
                case 14:
                    return "#FF9800";
                case 15:
                    return "#FF5722";
                default:
                    return "#607D8B";
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public static bool isHexColor(string color)
        {
            Regex reg = new Regex("#[0-9a-fA-F]{6}");
            return color != null && reg.Match(color).Success;
        }

        public static SolidColorBrush convertHexColorToBrush(string color)
        {
            color = color.Replace("#", string.Empty);
            var r = (byte)Convert.ToUInt32(color.Substring(0, 2), 16);
            var g = (byte)Convert.ToUInt32(color.Substring(2, 2), 16);
            var b = (byte)Convert.ToUInt32(color.Substring(4, 2), 16);
            return new SolidColorBrush(Color.FromArgb(255, r, g, b));
        }

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
