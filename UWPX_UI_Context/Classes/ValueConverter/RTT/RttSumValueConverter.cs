using System;
using Windows.Networking.Sockets;
using Windows.UI.Xaml.Data;

namespace UWPX_UI_Context.Classes.ValueConverter.RTT
{
    public sealed class RttSumValueConverter: IValueConverter
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
            if (value is StreamSocketInformation sockInfo)
            {
                RoundTripTimeStatistics roundTripTimeStatistics;
                try
                {
                    roundTripTimeStatistics = sockInfo.RoundTripTimeStatistics;
                }
                catch (Exception)
                {
                    return "0 ms";
                }

                double val = roundTripTimeStatistics.Sum / 1000.0;
                if (val > 1000)
                {
                    return Math.Round(val / 1000, 2) + " s";
                }

                return Math.Round(val, 2) + " ms";
            }
            return "0 ms";
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
