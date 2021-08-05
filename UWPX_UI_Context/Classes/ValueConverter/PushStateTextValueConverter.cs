using System;
using Logging;
using Storage.Classes.Models.Account;
using Windows.UI.Xaml.Data;

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
                        return "Not supported by your server.";

                    case PushState.ENABLING:
                        return "Enabling...";

                    case PushState.DISABLING:
                        return "Disabling...";

                    case PushState.ENABLED:
                        return "Enabled";
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
