using System;
using Windows.UI.Xaml.Data;
using XMPP_API.Classes.Network;

namespace UWPX_UI_Context.Classes.ValueConverter.OMEMO
{
    public sealed class OmemoHelperStateStringValueConverter : IValueConverter
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
                    case OmemoHelperState.DISABLED:
                        return "Disabled";

                    case OmemoHelperState.REQUESTING_DEVICE_LIST:
                        return "Requesting device list";

                    case OmemoHelperState.UPDATING_DEVICE_LIST:
                        return "Updating device list";

                    case OmemoHelperState.ANNOUNCING_BUNDLE_INFO:
                        return "Announcing bundle info";

                    case OmemoHelperState.ENABLED:
                        return "Enabled";

                    case OmemoHelperState.ERROR:
                        return "Error - view logs";
                }
            }
            return "Unknown";
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
