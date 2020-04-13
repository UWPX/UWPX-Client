using System;
using Logging;
using Windows.UI.Xaml.Data;
using XMPP_API.Classes.Network;

namespace UWPX_UI_Context.Classes.ValueConverter
{
    public sealed class PushStateTextValueConverter: IValueConverter
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
            if (value is PushState state)
            {
                switch (state)
                {
                    case PushState.DISABLED:
                        return "Disabled";

                    case PushState.NOT_SUPPORTED:
                        return "Not supported by your server";

                    case PushState.REQUESTED:
                        return "Requested";

                    case PushState.ENABLED:
                        return "Enabled";

                    case PushState.ERROR:
                        return "Error";
                }
            }
            Logger.Warn("Invalid " + nameof(PushState) + ": " + value);
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
