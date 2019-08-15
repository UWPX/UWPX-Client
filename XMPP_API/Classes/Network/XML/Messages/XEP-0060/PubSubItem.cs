using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0060
{
    public class PubSubItem : AbstractPubSubItem
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly IXElementable CONTENT;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 02/06/2018 Created [Fabian Sauter]
        /// </history>
        public PubSubItem(string id, IXElementable content)
        {
            this.id = id;
            CONTENT = content;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        protected override XElement getContent(XNamespace ns)
        {
            return CONTENT?.toXElement(ns);
        }

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
