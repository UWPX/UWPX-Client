using Data_Manager2.Classes;
using System;
using Windows.UI.Xaml.Data;

namespace UWPX_UI_Context.Classes.ValueConverter
{
    public sealed class MessageStateGlyphTextValueConverter : IValueConverter
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
            if (value is MessageState state)
            {
                switch (state)
                {
                    case MessageState.SENDING:
                        return "\uE724";

                    case MessageState.SEND:
                        return "\uE725";

                    case MessageState.DELIVERED:
                        return "\uE725";

                    case MessageState.UNREAD:
                        return "\uEA63";

                    case MessageState.READ:
                        return "\uEA64";

                    case MessageState.TO_ENCRYPT:
                        return "\uE724";

                    case MessageState.ENCRYPT_FAILED:
                        return "\uEA39";

                    default:
                        return "\uE9CE";
                }
            }
            return "\uE9CE";
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
