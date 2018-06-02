using System.Collections.Generic;
using System.Xml;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0363
{
    public class HTTPUploadSlot
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly string URL_PUT;
        public readonly string URL_GET;
        public readonly Dictionary<string, string> HEADERS;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 17/03/2018 Created [Fabian Sauter]
        /// </history>
        public HTTPUploadSlot(XmlNode node)
        {
            XmlNode putNode = XMLUtils.getChildNode(node, "put");
            URL_PUT = putNode.Attributes["url"]?.Value;

            HEADERS = new Dictionary<string, string>();
            foreach (XmlAttribute att in putNode.Attributes)
            {
                HEADERS.Add(att.Name, att.Value);
            }

            XmlNode getNode = XMLUtils.getChildNode(node, "get");
            URL_GET = getNode.Attributes["url"]?.Value;
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
