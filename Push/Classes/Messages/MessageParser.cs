using System;
using Logging;
using Newtonsoft.Json.Linq;

namespace Push.Classes.Messages
{
    public static class MessageParser
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
        public static AbstractMessage Parse(string msg)
        {
            JObject json;
            try
            {
                json = JObject.Parse(msg);
            }
            catch (Exception e)
            {
                Logger.Error("Failed to parse JSON push. Invalid JSON.", e);
                return null;
            }
            return ParseJsonSave(json);
        }

        #endregion

        #region --Misc Methods (Private)--
        private static AbstractMessage ParseJsonSave(JObject json)
        {
            try
            {
                return ParseJson(json);
            }
            catch (Exception e)
            {
                Logger.Error("Failed to parse JSON push message: " + json.ToString(), e);
            }
            return null;
        }

        private static AbstractMessage ParseJson(JObject json)
        {
            switch (json.Value<string>("action"))
            {
                case AbstractResponseMessage.ACTION_CONST:
                    switch (json.Value<uint>("status"))
                    {
                        case SuccessResultMessage.STATUS_CONST:
                            if (json.ContainsKey("accounts"))
                            {
                                return new SuccessSetPushAccountsMessage(json);
                            }
                            return new SuccessResultMessage(json);

                        case ErrorResultMessage.STATUS_CONST:
                            return new ErrorResultMessage(json);

                        default:
                            throw new Exception("Unknown message status.");
                    }

                case TestPushMessage.ACTION_CONST:
                    return new TestPushMessage(json);

                case PushMessage.ACTION_CONST:
                    return new PushMessage(json);

                default:
                    throw new Exception("Unknown message action.");
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
