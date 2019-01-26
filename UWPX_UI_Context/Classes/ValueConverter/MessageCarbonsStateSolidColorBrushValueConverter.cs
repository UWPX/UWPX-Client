using Logging;
using System;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using XMPP_API.Classes.Network;

namespace UWPX_UI_Context.Classes.ValueConverter
{
    public sealed class MessageCarbonsStateSolidColorBrushValueConverter : IValueConverter
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
                    case MessageCarbonsState.ERROR:
                    case MessageCarbonsState.UNAVAILABLE:
                        return new SolidColorBrush(Colors.Red);

                    case MessageCarbonsState.REQUESTED:
                        return new SolidColorBrush(Colors.Orange);

                    case MessageCarbonsState.ENABLED:
                        return new SolidColorBrush(Colors.Green);
                }
            }
            Logger.Warn("Invalid " + nameof(MessageCarbonsState) + ": " + value);
            return new SolidColorBrush(Colors.DimGray);
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
