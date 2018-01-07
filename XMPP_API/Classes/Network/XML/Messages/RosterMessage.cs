using System.Collections;
using System.Xml;
using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages
{
    public class RosterMessage : IQMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private ArrayList items;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 28/08/2017 Created [Fabian Sauter]
        /// </history>
        public RosterMessage(XmlNode answer) : base(answer)
        {
            loadItems();
        }

        public RosterMessage(string from, string to) : base(from, to, GET, getRandomId(), getRoosterQuery())
        {
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public ArrayList getItems()
        {
            return items;
        }

        private static XElement getRoosterQuery()
        {
            XNamespace ns = "jabber:iq:roster";
            return new XElement(ns + "query");
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void loadItems()
        {
            XmlNode query = XMLUtils.getChildNode(ANSWER, "query", "xmlns", "jabber:iq:roster");
            if(query == null)
            {
                return;
            }
            items = new ArrayList();
            foreach (XmlNode n in query.ChildNodes)
            {
                if(n.Name.Equals("presence"))
                {
                    items.Add(new PresenceMessage(n));
                }
                else
                {
                    items.Add(new RosterItem(n));
                }
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
