using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0313
{
    public class QueryArchiveResultMessage: IQMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly List<AbstractMessage> CONTENT;
        public readonly string QUERY_ID;
        public readonly string RESULT_ID;
        public readonly DateTime DELAY;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public QueryArchiveResultMessage(XmlNode node, XmlNode resultNode, XmlNode forwardedNode, List<AbstractMessage> content) : base(node)
        {
            QUERY_ID = resultNode.Attributes["queryid"]?.Value;
            CONTENT = content;
            XmlNode delayNode = XMLUtils.getChildNode(forwardedNode, "delay", Consts.XML_XMLNS, Consts.XML_XEP_0203_NAMESPACE);
            DELAY = MessageMessage.parseDelay(delayNode);
            foreach (AbstractMessage message in CONTENT)
            {
                if (message is MessageMessage msg)
                {
                    msg.addDelay(DELAY);
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
        protected override XElement getQuery()
        {
            XNamespace ns = Consts.XML_XEP_0313_NAMESPACE;
            XElement query = new XElement(ns + "query");
            query.Add(new XAttribute("queryid", QUERY_ID));
            return query;
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
