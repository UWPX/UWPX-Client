using System;
using System.Xml;
using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages
{
    internal class CloseStreamMessage: AbstractMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly string REASON;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public CloseStreamMessage(XmlNode node)
        {
            if (!(node is null))
            {
                foreach (XmlNode item in node.ChildNodes)
                {
                    if (item.Name.Equals("stream:error"))
                    {
                        REASON = item.InnerText;
                        break;
                    }
                }
            }
        }

        public CloseStreamMessage(string reason)
        {
            REASON = reason ?? "";
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public string getReason()
        {
            return REASON;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public override string toXmlString()
        {
            return Consts.XML_STREAM_CLOSE;
        }

        /// <summary>
        /// Not used in CloseStreamMessage.
        /// Use toXmlString() instead.
        /// </summary>
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
