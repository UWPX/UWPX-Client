using System;
using Storage.Classes.Models.Chat;
using Windows.UI.Xaml.Data;

namespace UWPX_UI_Context.Classes.ValueConverter
{
    public sealed class MessageStateStringValueConverter: IValueConverter
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
                        return "Sending...";

                    case MessageState.SEND:
                        return "Send";

                    case MessageState.DELIVERED:
                        return "Delivered 🎉";

                    case MessageState.UNREAD:
                        return "Unread";

                    case MessageState.READ:
                        return "Read";

                    case MessageState.TO_ENCRYPT:
                        return "Encrypting 🔓";

                    case MessageState.ENCRYPT_FAILED:
                        return "Encryption failed!";

                    default:
                        return state.ToString();
                }
            }
            return "Unknown!";
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
