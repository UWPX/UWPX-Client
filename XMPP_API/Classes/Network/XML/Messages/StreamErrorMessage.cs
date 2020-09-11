using System;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages
{
    public class StreamErrorMessage: AbstractMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly StreamErrorType ERROR_TYPE;
        public readonly string ERROR_TEXT;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 10/11/2017 Created [Fabian Sauter]
        /// </history>
        public StreamErrorMessage(XmlNode n)
        {
            ERROR_TYPE = StreamErrorType.UNKNOWN;
            ERROR_TEXT = "";
            if (n.HasChildNodes)
            {
                foreach (XmlNode n1 in n.ChildNodes)
                {
                    if (string.Equals(n1.NamespaceURI, Consts.XML_STREAM_ERROR_NAMESPACE))
                    {
                        if (string.Equals(n1.Name, "text"))
                        {
                            ERROR_TEXT = n1.InnerXml;
                        }
                        else
                        {
                            ERROR_TYPE = parseStreamErrorType(n1.Name);
                        }
                    }
                }
            }
        }

        public override XElement toXElement()
        {
            throw new NotImplementedException();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public override string toXmlString()
        {
            StringBuilder sb = new StringBuilder(ERROR_TYPE.ToString());
            if (!string.IsNullOrEmpty(ERROR_TEXT))
            {
                sb.Append(": ");
                sb.Append(ERROR_TEXT);
            }
            return sb.ToString();
        }

        #endregion

        #region --Misc Methods (Private)--
        private StreamErrorType parseStreamErrorType(string s)
        {
            s = s.Replace('-', '_').ToUpper();
            StreamErrorType streamError = StreamErrorType.UNKNOWN;
            Enum.TryParse(s, out streamError);
            return streamError;
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
