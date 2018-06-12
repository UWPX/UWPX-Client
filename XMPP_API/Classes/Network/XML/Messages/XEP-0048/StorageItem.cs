using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0048
{
    class StorageItem : IXElementable
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly List<ConferenceItem> CONFERENCE_ITEMS;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 12/06/2018 Created [Fabian Sauter]
        /// </history>
        public StorageItem(List<ConferenceItem> conferenceItems)
        {
            this.CONFERENCE_ITEMS = conferenceItems;
        }

        public StorageItem(XmlNode node)
        {
            CONFERENCE_ITEMS = new List<ConferenceItem>();
            foreach (XmlNode n in node)
            {
                if (string.Equals(n.Name, "conference"))
                {
                    CONFERENCE_ITEMS.Add(new ConferenceItem(n));
                }
            }
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public XElement toXElement(XNamespace ns)
        {
            XNamespace sNs = Consts.XML_XEP_0048_NAMESPACE;
            XElement storage = new XElement(sNs + "storage");
            foreach (ConferenceItem c in CONFERENCE_ITEMS)
            {
                storage.Add(c.toXElement(sNs));
            }
            return storage;
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
