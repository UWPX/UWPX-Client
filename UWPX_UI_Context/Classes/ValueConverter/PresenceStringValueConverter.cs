using System;
using Windows.UI.Xaml.Data;
using XMPP_API.Classes;

namespace UWPX_UI_Context.Classes.ValueConverter
{
    public sealed class PresenceStringValueConverter: IValueConverter
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
            if (value is Presence p)
            {
                return p.ToString();
            }
            return Presence.Unavailable.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value is string s ? Utils.parsePresence(s) : Presence.Unavailable;
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
