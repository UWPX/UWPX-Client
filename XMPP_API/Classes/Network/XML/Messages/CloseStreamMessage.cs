using System;
using System.Xml;
using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages
{
    internal class CloseStreamMessage: AbstractMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly XmlNode CLOSE_NODE;
        private readonly string REASON;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 20/08/2017 Created [Fabian Sauter]
        /// </history>
        public CloseStreamMessage(XmlNode node)
        {
            CLOSE_NODE = node;
            if (node is null)
            {
                return;
            }
            foreach (XmlNode item in node.ChildNodes)
            {
                if (item.Name.Equals("stream:error"))
                {
                    REASON = item.Value;
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
