using System.Collections.Generic;
using System.Xml;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0048_1_0
{
    // https://xmpp.org/extensions/attic/xep-0048-1.0.html
    public class BookmarksResultMessage : IQMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public List<ConferenceItem> CONFERENCE_ITEMS;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 02/01/2018 Created [Fabian Sauter]
        /// </history>
        public BookmarksResultMessage(XmlNode answer) : base(answer)
        {
            XmlNode qNode = XMLUtils.getChildNode(answer, "query", Consts.XML_XMLNS, "jabber:iq:private");
            if(qNode != null)
            {
                XmlNode bNode = XMLUtils.getChildNode(qNode, "storage", Consts.XML_XMLNS, "storage:bookmarks");
                if(bNode != null){
                    CONFERENCE_ITEMS = new List<ConferenceItem>();
                    foreach (XmlNode n in bNode)
                    {
                        if(Equals(n.Name, "conference"))
                        {
                            CONFERENCE_ITEMS.Add(new ConferenceItem(n));
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


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
