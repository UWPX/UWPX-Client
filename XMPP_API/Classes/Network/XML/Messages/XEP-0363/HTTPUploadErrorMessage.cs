using System;
using System.Xml;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0363
{
    public class HTTPUploadErrorMessage : IQErrorMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly string TEXT;
        public readonly string TYPE_SHORT;
        public readonly string TYPE_LONG;
        public readonly HTTPUploadRequestSlotMessage REQUEST_MESSAGE;

        public readonly bool RETRY;
        public readonly DateTime RETRY_STAMP;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 02/06/2018 Created [Fabian Sauter]
        /// </history>
        public HTTPUploadErrorMessage(XmlNode node) : base(node)
        {
            this.RETRY = false;
            this.RETRY_STAMP = DateTime.MinValue;

            XmlNode errorNode = XMLUtils.getChildNode(node, "error");
            if (errorNode != null)
            {
                this.TYPE_SHORT = errorNode.Attributes["type"]?.Value;

                foreach (XmlNode n in errorNode.ChildNodes)
                {
                    switch (n.Name)
                    {
                        case "text" when string.Equals(n.NamespaceURI, Consts.XML_ERROR_NAMESPACE):
                            this.TEXT = n.InnerText;
                            break;

                        case "retry" when string.Equals(n.NamespaceURI, Consts.XML_XEP_0363_NAMESPACE):
                            this.RETRY = true;
                            XmlAttribute stamp = XMLUtils.getAttribute(n, "stamp");
                            if (stamp != null)
                            {
                                DateTimeParserHelper parserHelper = new DateTimeParserHelper();
                                this.RETRY_STAMP = parserHelper.parse(stamp.Value);
                            }
                            break;

                        default:
                            if (n.NamespaceURI.Equals(Consts.XML_ERROR_NAMESPACE))
                            {
                                this.TYPE_LONG = n.Name;
                            }
                            break;
                    }
                }
            }

            XmlNode requestNode = XMLUtils.getChildNode(node, "request", Consts.XML_XMLNS, Consts.XML_XEP_0363_NAMESPACE);
            if (requestNode != null)
            {
                this.REQUEST_MESSAGE = new HTTPUploadRequestSlotMessage(node);
            }
            else
            {
                this.REQUEST_MESSAGE = null;
            }
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


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
