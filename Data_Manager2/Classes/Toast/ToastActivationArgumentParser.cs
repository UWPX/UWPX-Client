using System;
using System.Linq;
using Logging;
using Windows.Foundation;
using XMPP_API.Classes.XmppUri;

namespace Data_Manager2.Classes.Toast
{
    public static class ToastActivationArgumentParser
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
        public static AbstractToastActivation parseArguments(string activationString)
        {
            if (string.IsNullOrEmpty(activationString) || !activationString.Contains('='))
            {
                return null;
            }

            if (!Uri.TryCreate(activationString, UriKind.RelativeOrAbsolute, out Uri result))
            {
                Logger.Warn("Failed to parse activationString to Uri: " + activationString);
                return null;
            }

            WwwFormUrlDecoder query = UriUtils.parseUriQuery(result);
            if (query is null)
            {
                return null;
            }

            string type = query.Where(x => string.Equals(x.Name, AbstractToastActivation.TYPE_QUERY)).Select(x => x.Value).FirstOrDefault();
            if (string.IsNullOrEmpty(type))
            {
                Logger.Warn("Failed to parse activationString - no type query: " + activationString);
                return null;
            }

            switch (type)
            {
                case ChatToastActivation.TYPE:
                    return new ChatToastActivation(result);

                case MarkChatAsReadToastActivation.TYPE:
                    return new MarkChatAsReadToastActivation(result);

                case MarkMessageAsReadToastActivation.TYPE:
                    return new MarkMessageAsReadToastActivation(result);

                case SendReplyToastActivation.TYPE:
                    return new SendReplyToastActivation(result);

                default:
                    return null;
            }
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
