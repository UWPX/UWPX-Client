using System;
using Windows.UI.Xaml.Data;
using XMPP_API.Classes;
using XMPP_API.Classes.Network;

namespace UWPX_UI_Context.Classes.ValueConverter
{
    public sealed class ConnectionStatePresenceValueConverter: IValueConverter
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
            if (value is ConnectionState state)
            {
                switch (state)
                {
                    case ConnectionState.DISCONNECTED:
                        return Presence.Unavailable;

                    case ConnectionState.DISCONNECTING:
                    case ConnectionState.CONNECTING:
                        return Presence.Chat;

                    case ConnectionState.CONNECTED:
                        return Presence.Online;

                    case ConnectionState.ERROR:
                        return Presence.Dnd;
                }
            }
            return Presence.Unavailable;
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
