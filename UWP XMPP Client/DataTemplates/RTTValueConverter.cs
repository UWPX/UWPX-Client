using System;
using Windows.UI.Xaml.Data;

namespace UWP_XMPP_Client.DataTemplates
{
    class RTTValueConverter : IValueConverter
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--


        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 30/05/2018 Created [Fabian Sauter]
        /// </history>
        public RTTValueConverter()
        {
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is uint i)
            {
                double val = i / 1000.0;
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
