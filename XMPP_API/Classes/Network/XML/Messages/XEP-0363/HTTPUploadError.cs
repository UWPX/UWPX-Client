using System;
using System.Xml;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0363
{
    public class HTTPUploadError
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly string TEXT;
        public readonly string TYPE;
        public readonly string TYPE_LONG;

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
        public HTTPUploadError(XmlNode node)
        {
            this.RETRY = false;
            this.RETRY_STAMP = DateTime.MinValue;

            this.TYPE = node.Attributes["type"]?.Value;

            foreach (XmlNode n in node.ChildNodes)
            {
                switch (n.Name)
                {
                    case "text" when string.Equals(n.NamespaceURI, Consts.XML_XEP_0198_FAILED_NAMESPACE):
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
                        if (n.NamespaceURI.Equals(Consts.XML_XEP_0198_FAILED_NAMESPACE))
                        {
                            this.TYPE_LONG = n.Name;
                        }
                        break;
                }
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
