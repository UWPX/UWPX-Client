using System;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using XMPP_API.Classes.Network.XML.Messages.XEP_0392;

namespace UWPX_UI_Context.Classes.ValueConverter
{
    public sealed class StringBrushValueConverter : IValueConverter
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--


        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is string s)
            {
                Color color = ConsistentColorGenerator.GenForegroundColor(s, false, false);
                return new SolidColorBrush(color);
            }
            return new SolidColorBrush();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
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
