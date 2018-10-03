using System.Linq;

namespace Data_Manager2.Classes.ToastActivation
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

            string type = activationString.Substring(0, activationString.IndexOf('='));
            string args = activationString.Substring(activationString.IndexOf('=') + 1);

            switch (type)
            {
                case ChatToastActivation.TYPE:
                    return new ChatToastActivation(args, true);

                case MarkChatAsReadToastActivation.TYPE:
                    return new MarkChatAsReadToastActivation(args, true);

                case MarkMessageAsReadToastActivation.TYPE:
                    return new MarkMessageAsReadToastActivation(args, true);

                case SendReplyToastActivation.TYPE:
                    return new SendReplyToastActivation(args, true);

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
