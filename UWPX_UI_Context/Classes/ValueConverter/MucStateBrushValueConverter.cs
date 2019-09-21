using System;
using Data_Manager2.Classes;
using Windows.UI.Xaml.Data;
using XMPP_API.Classes;

namespace UWPX_UI_Context.Classes.ValueConverter
{
    public class MucStateBrushValueConverter: IValueConverter
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
            if (value is MUCState state)
            {
                switch (state)
                {
                    case MUCState.ENTERING:
                    case MUCState.DISCONNECTING:
                        return UiUtils.GetPresenceBrush(Presence.Chat);

                    case MUCState.ENTERD:
                        return UiUtils.GetPresenceBrush(Presence.Online);

                    case MUCState.ERROR:
                    case MUCState.KICKED:
                    case MUCState.BANED:
                        return UiUtils.GetPresenceBrush(Presence.Xa);

                    case MUCState.DISCONNECTED:
                    default:
                        return UiUtils.GetPresenceBrush(Presence.Unavailable);
                }
            }
            return UiUtils.GetPresenceBrush(Presence.Unavailable);
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
