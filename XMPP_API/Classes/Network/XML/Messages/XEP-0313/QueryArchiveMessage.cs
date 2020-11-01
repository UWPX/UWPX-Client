using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0313
{
    public class QueryArchiveMessage: IQMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly string QUERY_ID;
        public readonly QueryFilter FILTER;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public QueryArchiveMessage(QueryFilter filter, string from, string to) : base(from, to, SET, getRandomId())
        {
            QUERY_ID = getRandomId();
            FILTER = filter;
        }

        public QueryArchiveMessage(QueryFilter filter) : this(filter, null, null) { }

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
            FILTER.addToXElement(query);
            return query;
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
