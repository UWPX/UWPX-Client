using System.Xml;

namespace XMPP_API.Classes.Network.XML.Messages
{
    public class Error
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly ErrorType ERROR_TYPE;
        public readonly ErrorName ERROR_NAME;
        public readonly string ERROR_MESSAGE;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 29/06/2018 Created [Fabian Sauter]
        /// </history>
        public Error()
        {
            this.ERROR_TYPE = ErrorType.UNKNOWN;
            this.ERROR_NAME = ErrorName.UNKNOWN;
            this.ERROR_MESSAGE = "\"No message found.\"";
        }

        public Error(XmlNode n)
        {
            this.ERROR_NAME = getErrorName(n);
            this.ERROR_TYPE = getErrorType(n);
            this.ERROR_MESSAGE = '"' + n.InnerText + '"';
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public override string ToString()
        {
            return "type: " + ERROR_TYPE + ", name: " + ERROR_NAME + ", message: " + ERROR_MESSAGE;
        }

        #endregion

        #region --Misc Methods (Private)--
        private static ErrorType getErrorType(XmlNode n)
        {
            switch (n.Attributes["type"]?.Value)
            {
                case "auth":
                    return ErrorType.AUTH;

                case "wait":
                    return ErrorType.WAIT;

                case "modify":
                    return ErrorType.MODIFY;

                case "cancel":
                    return ErrorType.CANCEL;

                default:
                    return ErrorType.UNKNOWN;
            }
        }

        private static ErrorName getErrorName(XmlNode n)
        {
            foreach (XmlNode n1 in n.ChildNodes)
            {
                if (string.Equals(n1.NamespaceURI, Consts.XML_ERROR_NAMESPACE))
                    switch (n1.Name)
                    {
                        case "bad-request":
                            return ErrorName.BAD_REQUEST;

                        case "conflict":
                            return ErrorName.CONFLICT;

                        case "feature-not-implemented":
                            return ErrorName.FEATURE_NOT_IMPLEMENTED;

                        case "forbidden":
                            return ErrorName.FORBIDDEN;

                        case "gone":
                            return ErrorName.GONE;

                        case "internal-server-error":
                            return ErrorName.INTERNAL_SERVER_ERROR;

                        case "item-not-found":
                            return ErrorName.ITEM_NOT_FOUND;

                        case "jid-malformed":
                            return ErrorName.JID_MALFORMED;

                        case "not-acceptable":
                            return ErrorName.NOT_ACCEPTABLE;

                        case "not-allowed":
                            return ErrorName.NOT_ALLOWED;

                        case "not-authorized":
                            return ErrorName.NOT_AUTHORIZED;

                        case "payment-required":
                            return ErrorName.PAYMENT_REQUIRED;

                        case "recipient-unavailable":
                            return ErrorName.RECIPIENT_UNAVAILABLE;

                        case "redirect":
                            return ErrorName.REDIRECT;

                        case "registration-required":
                            return ErrorName.REGISTRATION_REQUIRED;

                        case "remote-server-not-found":
                            return ErrorName.REMOTE_SERVER_NOT_FOUND;

                        case "remote-server-timeout":
                            return ErrorName.REMOTE_SERVER_TIMEOUT;

                        case "resource-constraint":
                            return ErrorName.RESOURCE_CONSTRAINT;

                        case "service-unavailable":
                            return ErrorName.SERVICE_UNAVAILABLE;

                        case "subscription-required":
                            return ErrorName.SUBSCRIPTION_REQUIRED;

                        case "undefined-condition":
                            return ErrorName.UNDEFINED_CONDITION;

                        case "unexpected-request":
                            return ErrorName.UNEXPECTED_REQUEST;

                        default:
                            break;
                    }
            }
            return ErrorName.UNKNOWN;
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
