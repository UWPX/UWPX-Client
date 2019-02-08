using System;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using XMPP_API.Classes.Network;

namespace UWPX_UI_Context.Classes.ValueConverter.OMEMO
{
    public sealed class OmemoHelperStateBrushValueConverter : IValueConverter
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
            if (value is OmemoHelperState state)
            {
                switch (state)
                {
                    case OmemoHelperState.REQUESTING_DEVICE_LIST:
                    case OmemoHelperState.UPDATING_DEVICE_LIST:
                    case OmemoHelperState.ANNOUNCING_BUNDLE_INFO:
                        return new SolidColorBrush(Colors.Orange);

                    case OmemoHelperState.ENABLED:
                        return new SolidColorBrush(Colors.Green);
                }
            }
            return new SolidColorBrush(Colors.Red);
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
