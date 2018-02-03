using System.Xml;
using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages
{
    class RemoveFromRosterMessage : IQMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--


        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 09/09/2017 Created [Fabian Sauter]
        /// </history>
        public RemoveFromRosterMessage(XmlNode answer) : base(answer)
        {
        }

        public RemoveFromRosterMessage(string fullJabberId, string target) : base(fullJabberId, null, SET, getRandomId(), getRemoveFromRoosterQuery(target))
        {
            this.cacheUntilSend = true;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private static XElement getRemoveFromRoosterQuery(string target)
        {
            XNamespace ns = "jabber:iq:roster";
            XElement node = new XElement(ns + "query");
            node.Add(new XElement(ns + "item", new XAttribute("jid", target), new XAttribute("subscription", "remove")));
            return node;
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
