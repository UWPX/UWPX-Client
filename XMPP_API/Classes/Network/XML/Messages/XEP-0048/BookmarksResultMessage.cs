using System.Xml;
using System.Xml.Linq;
using XMPP_API.Classes.Network.XML.Messages.XEP_0060;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0048
{
    public class BookmarksResultMessage : PubSubResultMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public StorageItem storage;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 12/06/2018 Created [Fabian Sauter]
        /// </history>
        public BookmarksResultMessage(XmlNode node) : base(node)
        {
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        protected override XElement getQuery()
        {
            throw new System.NotImplementedException();
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--
        protected override void loadContent(XmlNode content)
        {
            if (string.Equals(content.Name, "items"))
            {
                XmlAttribute att = content.Attributes["node"];
                if (att != null && string.Equals(att.Value, Consts.XML_XEP_0048_NAMESPACE))
                {
                    foreach (XmlNode n in content.ChildNodes)
                    {
                        if (string.Equals(n.Name, "item"))
                        {
                            att = n.Attributes["id"];
                            if (att != null && string.Equals(att.Value, "current"))
                            {
                                XmlNode storageNode = XMLUtils.getChildNode(n, "storage", Consts.XML_XMLNS, Consts.XML_XEP_0048_NAMESPACE);
                                if (storageNode != null)
                                {
                                    storage = new StorageItem(storageNode);
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
