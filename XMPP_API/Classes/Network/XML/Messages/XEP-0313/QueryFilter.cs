using System;
using System.Xml.Linq;
using XMPP_API.Classes.Network.XML.Messages.XEP_0004;
using XMPP_API.Classes.Network.XML.Messages.XEP_0082;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0313
{
    public class QueryFilter
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private DataForm form = new DataForm(DataFormType.SUBMIT);

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public QueryFilter()
        {
            form.fields.Add(new Field()
            {
                var = "FORM_TYPE",
                type = FieldType.HIDDEN,
                value = Consts.XML_XEP_0313_NAMESPACE
            });
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void addToXElement(XElement node)
        {
            form.addToXElement(node);
        }

        /// <summary>
        /// https://xmpp.org/extensions/xep-0313.html#query-limit-id
        /// <para/>
        /// Namespace: urn:xmpp:mam:2#extended
        /// </summary>
        public void BeforeId(string id)
        {
            form.fields.Add(new Field()
            {
                var = "before-id",
                type = FieldType.NONE,
                value = id
            });
        }

        /// <summary>
        /// https://xmpp.org/extensions/xep-0313.html#query-limit-id
        /// <para/>
        /// Namespace: urn:xmpp:mam:2#extended
        /// </summary>
        public void AfterId(string id)
        {
            form.fields.Add(new Field()
            {
                var = "after-id",
                type = FieldType.NONE,
                value = id
            });
        }

        /// <summary>
        /// https://xmpp.org/extensions/xep-0313.html#filter-jid
        /// <para/>
        /// Namespace: urn:xmpp:mam:2
        /// </summary>
        /// <param name="jid"></param>
        public void With(string jid)
        {
            form.fields.Add(new Field()
            {
                var = "with",
                type = FieldType.NONE,
                value = jid
            });
        }

        /// <summary>
        /// https://xmpp.org/extensions/xep-0313.html#filter-time
        /// <para/>
        /// Namespace: urn:xmpp:mam:2
        /// </summary>
        /// <param name="date"></param>
        public void Start(DateTime date)
        {
            form.fields.Add(new Field()
            {
                var = "start",
                type = FieldType.NONE,
                value = DateTimeHelper.ToString(date)
            });
        }

        /// <summary>
        /// https://xmpp.org/extensions/xep-0313.html#filter-time
        /// <para/>
        /// Namespace: urn:xmpp:mam:2
        /// </summary>
        /// <param name="date"></param>
        public void End(DateTime date)
        {
            form.fields.Add(new Field()
            {
                var = "end",
                type = FieldType.NONE,
                value = DateTimeHelper.ToString(date)
            });
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
