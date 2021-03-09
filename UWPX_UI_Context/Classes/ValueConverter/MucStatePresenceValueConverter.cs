using System;
using Storage.Classes.Models.Chat;
using Windows.UI.Xaml.Data;
using XMPP_API.Classes;

namespace UWPX_UI_Context.Classes.ValueConverter
{
    public class MucStatePresenceValueConverter: IValueConverter
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
            if (value is MucState state)
            {
                switch (state)
                {
                    case MucState.ENTERING:
                    case MucState.DISCONNECTING:
                        return Presence.Chat;

                    case MucState.ENTERD:
                        return Presence.Online;

                    case MucState.ERROR:
                    case MucState.KICKED:
                    case MucState.BANED:
                        return Presence.Xa;

                    case MucState.DISCONNECTED:
                    default:
                        break;
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
