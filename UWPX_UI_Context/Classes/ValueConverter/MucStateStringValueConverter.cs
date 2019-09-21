using System;
using Data_Manager2.Classes;
using Windows.UI.Xaml.Data;

namespace UWPX_UI_Context.Classes.ValueConverter
{
    public class MucStateStringValueConverter: IValueConverter
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
            return value is MUCState state ? state.ToString().ToLowerInvariant() : MUCState.DISCONNECTED.ToString().ToLowerInvariant();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            MUCState state = MUCState.DISCONNECTED;
            if (value is string s)
            {
                Enum.TryParse(s.ToUpperInvariant(), out state);
            }
            return state;
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
