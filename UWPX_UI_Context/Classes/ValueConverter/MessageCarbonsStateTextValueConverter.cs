using System;
using Logging;
using Windows.UI.Xaml.Data;
using XMPP_API.Classes.Network;

namespace UWPX_UI_Context.Classes.ValueConverter
{
    public sealed class MessageCarbonsStateTextValueConverter: IValueConverter
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
            if (value is MessageCarbonsState state)
            {
                switch (state)
                {
                    case MessageCarbonsState.DISABLED:
                        return "Disabled";

                    case MessageCarbonsState.NOT_SUPPORTED:
                        return "Not supported by your server";

                    case MessageCarbonsState.REQUESTED:
                        return "Requested";

                    case MessageCarbonsState.ENABLED:
                        return "Enabled";

                    case MessageCarbonsState.ERROR:
                        return "Error";
                }
            }
            Logger.Warn("Invalid " + nameof(MessageCarbonsState) + ": " + value);
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
