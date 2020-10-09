using System.Xml;
using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0313
{
    public class QueryArchiveResultMessage: IQMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly MessageMessage MESSAGE;
        public readonly string QUERY_ID;
        public readonly string RESULT_ID;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public QueryArchiveResultMessage(XmlNode answer) : base(answer)
        {
            XmlNode resultNode = XMLUtils.getChildNode(answer, "result", Consts.XML_XMLNS, Consts.XML_XEP_0313_NAMESPACE);
            if (!(resultNode is null))
            {
                QUERY_ID = resultNode.Attributes["queryid"]?.Value;
                XmlNode forwardedNode = XMLUtils.getChildNode(resultNode, "forwarded", Consts.XML_XMLNS, Consts.XML_XEP_0297_NAMESPACE);
                if (!(forwardedNode is null))
                {
                    XmlNode messageNode = XMLUtils.getChildNode(forwardedNode, "message");
                    if (!(messageNode is null))
                    {
                        MESSAGE = new MessageMessage(messageNode, CarbonCopyType.NONE);

                        XmlNode delayNode = XMLUtils.getChildNode(forwardedNode, "delay", Consts.XML_XMLNS, Consts.XML_XEP_0203_NAMESPACE);
                        if (!(delayNode is null))
                        {
                            MESSAGE.parseDelay(delayNode);
                        }
                    }
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
