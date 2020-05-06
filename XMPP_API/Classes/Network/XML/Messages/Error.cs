using System;
using System.Xml;
using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages
{
    public class Error: IXElementable
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
            ERROR_TYPE = ErrorType.UNKNOWN;
            ERROR_NAME = ErrorName.UNKNOWN;
            ERROR_MESSAGE = "\"No message found.\"";
        }

        public Error(XmlNode n)
        {
            ERROR_NAME = getErrorName(n);
            ERROR_TYPE = getErrorType(n);
            ERROR_MESSAGE = '"' + n.InnerText + '"';
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

        private static string errorTypeToString(ErrorType errorType)
        {
            switch (errorType)
            {
                case ErrorType.AUTH:
                    return "auth";

                case ErrorType.WAIT:
                    return "wait";

                case ErrorType.MODIFY:
                    return "modify";

                case ErrorType.CANCEL:
                    return "cancel";

                default: // Should not happen
                    throw new NotImplementedException("No string representation for ErrorType: " + errorType.ToString());
            }
        }

        private static ErrorName getErrorName(XmlNode n)
        {
            foreach (XmlNode n1 in n.ChildNodes)
            {
                if (string.Equals(n1.NamespaceURI, Consts.XML_ERROR_NAMESPACE))
                {
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
            }
            return ErrorName.UNKNOWN;
        }

        private static string errorNameToString(ErrorName errorName)
        {
            switch (errorName)
            {
                case ErrorName.BAD_REQUEST:
                    return "bad-request";

                case ErrorName.CONFLICT:
                    return "conflict";

                case ErrorName.FEATURE_NOT_IMPLEMENTED:
                    return "feature-not-implemented";

                case ErrorName.FORBIDDEN:
                    return "forbidden";

                case ErrorName.GONE:
                    return "gone";

                case ErrorName.INTERNAL_SERVER_ERROR:
                    return "internal-server-error";

                case ErrorName.ITEM_NOT_FOUND:
                    return "item-not-found";

                case ErrorName.JID_MALFORMED:
                    return "jid-malformed";

                case ErrorName.NOT_ACCEPTABLE:
                    return "not-acceptable";

                case ErrorName.NOT_ALLOWED:
                    return "not-allowed";

                case ErrorName.NOT_AUTHORIZED:
                    return "not-authorized";

                case ErrorName.PAYMENT_REQUIRED:
                    return "payment-required";

                case ErrorName.RECIPIENT_UNAVAILABLE:
                    return "recipient-unavailable";

                case ErrorName.REDIRECT:
                    return "redirect";

                case ErrorName.REGISTRATION_REQUIRED:
                    return "registration-required";

                case ErrorName.REMOTE_SERVER_NOT_FOUND:
                    return "remote-server-not-found";

                case ErrorName.REMOTE_SERVER_TIMEOUT:
                    return "remote-server-timeout";

                case ErrorName.RESOURCE_CONSTRAINT:
                    return "resource-constraint";

                case ErrorName.SERVICE_UNAVAILABLE:
                    return "service-unavailable";

                case ErrorName.SUBSCRIPTION_REQUIRED:
                    return "subscription-required";

                case ErrorName.UNDEFINED_CONDITION:
                    return "undefined-condition";

                case ErrorName.UNEXPECTED_REQUEST:
                    return "unexpected-request";

                default: // Should not happen
                    throw new NotImplementedException("No string representation for ErrorName: " + errorName.ToString());
            }
        }

        public XElement toXElement(XNamespace ns)
        {
            XElement error = new XElement(ns + "error");
            error.Add(new XAttribute("type", errorTypeToString(ERROR_TYPE)));
            XNamespace nsInner = Consts.XML_ERROR_NAMESPACE;
            error.Add(new XElement(nsInner + errorNameToString(ERROR_NAME)));
            return error;
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
