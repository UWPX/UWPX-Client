using System;
using Logging;
using Storage.Classes.Models.Account;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace UWPX_UI_Context.Classes.ValueConverter
{
    public sealed class PushStateSolidColorBrushValueConverter: IValueConverter
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
                    case PushState.NOT_SUPPORTED:
                        return new SolidColorBrush(Colors.Red);

                    case PushState.DISABLING:
                    case PushState.ENABLING:
                        return new SolidColorBrush(Colors.Orange);

                    case PushState.ENABLED:
                        return new SolidColorBrush(Colors.Green);
                }
            }
            Logger.Warn("Invalid " + nameof(PushState) + ": " + value);
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
