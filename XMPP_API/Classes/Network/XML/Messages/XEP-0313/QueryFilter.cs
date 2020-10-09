using System.Xml.Linq;
using XMPP_API.Classes.Network.XML.Messages.XEP_0004;

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

        public void with(string jid)
        {
            form.fields.Add(new Field()
            {
                var = "with",
                type = FieldType.NONE,
                value = jid
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
