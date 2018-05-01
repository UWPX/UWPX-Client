using System;
using System.Xml;
using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages.Features.SASL
{
    public class SASLFailureMessage : AbstractMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly string ERROR_MESSAGE;
        public readonly SASLErrorType ERROR_TYPE;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 09/03/2018 Created [Fabian Sauter]
        /// </history>
        public SASLFailureMessage(XmlNode node)
        {
            this.ERROR_MESSAGE = null;
            if (node.HasChildNodes)
            {
                foreach (XmlNode n in node.ChildNodes)
                {
                    switch (n.Name)
                    {
                        case "text":
                            this.ERROR_MESSAGE = n.InnerText;
                            break;

                        case "aborted":
                            this.ERROR_TYPE = SASLErrorType.ABORTED;
                            break;

                        case "incorrect-encoding":
                            this.ERROR_TYPE = SASLErrorType.INCORRECT_ENCODING;
                            break;

                        case "invalid-authzid":
                            this.ERROR_TYPE = SASLErrorType.INVALID_AUTHZID;
                            break;

                        case "invalid-mechanism":
                            this.ERROR_TYPE = SASLErrorType.INVALID_MECHANISM;
                            break;

                        case "mechanism-too-weak":
                            this.ERROR_TYPE = SASLErrorType.MECHANISM_TOO_WAEK;
                            break;

                        case "not-authorized":
                            this.ERROR_TYPE = SASLErrorType.NOT_AUTHORIZED;
                            break;

                        case "temporary-auth-failure":
                            this.ERROR_TYPE = SASLErrorType.TEMPORARY_AUTH_FAILURE;
                            break;

                        case "account-disabled":
                            this.ERROR_TYPE = SASLErrorType.TEMPORARY_AUTH_FAILURE;
                            break;

                        case "credentials-expired":
                            this.ERROR_TYPE = SASLErrorType.TEMPORARY_AUTH_FAILURE;
                            break;

                        case "encryption-required":
                            this.ERROR_TYPE = SASLErrorType.TEMPORARY_AUTH_FAILURE;
                            break;

                        case "malformed-request":
                            this.ERROR_TYPE = SASLErrorType.TEMPORARY_AUTH_FAILURE;
                            break;

                        default:
                            this.ERROR_TYPE = SASLErrorType.UNKNOWN_ERROR;
                            break;
                    }
                }
            }

            if(ERROR_MESSAGE == null)
            {
                ERROR_MESSAGE = node.InnerXml;
            }
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public override XElement toXElement()
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
