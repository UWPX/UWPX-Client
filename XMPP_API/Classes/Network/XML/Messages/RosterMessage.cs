using System.Collections;
using System.Xml;
using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages
{
    public class RosterMessage : IQMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly ArrayList ITEMS;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 28/08/2017 Created [Fabian Sauter]
        /// </history>
        public RosterMessage(XmlNode n) : base(n)
        {
            XmlNode query = XMLUtils.getChildNode(n, "query", Consts.XML_XMLNS, "jabber:iq:roster");
            this.ITEMS = new ArrayList();
            if (query != null)
            {
                foreach (XmlNode n1 in query.ChildNodes)
                {
                    if (n1.Name.Equals("presence"))
                    {
                        this.ITEMS.Add(new PresenceMessage(n1));
                    }
                    else
                    {
                        this.ITEMS.Add(new RosterItem(n1));
                    }
                }
            }
        }

        public RosterMessage(string from, string to) : base(from, to, GET, getRandomId())
        {
            this.ITEMS = null;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        protected override XElement getQuery()
        {
            XNamespace ns = Consts.XML_ROSTER_NAMESPACE;
            return new XElement(ns + "query");
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
