using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using XMPP_API.Classes.Network.XML.Messages.XEP_0060;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0048
{
    public class StorageItem: AbstractPubSubItem
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly IList<ConferenceItem> CONFERENCES;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 22/07/2018 Created [Fabian Sauter]
        /// </history>
        public StorageItem() : this(new List<ConferenceItem>())
        {
        }

        public StorageItem(IList<ConferenceItem> conferences)
        {
            id = "current";
            CONFERENCES = conferences;
        }

        public StorageItem(XmlNode n) : this()
        {
            XmlNode storageNode = XMLUtils.getChildNode(n, "storage", Consts.XML_XMLNS, Consts.XML_XEP_0048_NAMESPACE);
            if (storageNode != null)
            {
                foreach (XmlNode n1 in storageNode)
                {
                    if (string.Equals(n1.Name, "conference"))
                    {
                        CONFERENCES.Add(new ConferenceItem(n1));
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
        protected override XElement getContent(XNamespace ns)
        {
            XNamespace ns1 = Consts.XML_XEP_0048_NAMESPACE;
            XElement storageNode = new XElement(ns1 + "storage");
            foreach (ConferenceItem item in CONFERENCES)
            {
                storageNode.Add(item.toXElement(ns1));
            }
            return storageNode;
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
