using System;
using Windows.UI.Xaml.Data;

namespace UWPX_UI_Context.Classes.ValueConverter
{
    public sealed class UlongDataRateValueConverter : IValueConverter
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
            if (value is ulong l)
            {
                if (l < 800)
                {
                    return l + " bit/s";
                }

                if (l < 8_000)
                {
                    return l / 8 + " B/s";
                }

                if (l < 8_000_000)
                {
                    return l / 8000 + " kB/s";
                }

                return l / 8_000_000 + " MB/s";
            }
            return "0 bit/s";
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
