using System.Xml;
using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages
{
    public class AddToRosterMessage : IQMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly string BARE_JID;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 09/09/2017 Created [Fabian Sauter]
        /// </history>
        public AddToRosterMessage(XmlNode answer) : base(answer)
        {
        }

        public AddToRosterMessage(string fullJabberId, string bareJid) : base(fullJabberId, null, SET, getRandomId())
        {
            BARE_JID = bareJid;
            cacheUntilSend = true;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        protected override XElement getQuery()
        {
            XNamespace ns = Consts.XML_ROSTER_NAMESPACE;
            XElement node = new XElement(ns + "query");
            node.Add(new XElement(ns + "item", new XAttribute("jid", BARE_JID)));
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
