using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0048_1_0
{
    // https://xmpp.org/extensions/attic/xep-0048-1.0.html
    public class SetBookmarksMessage : IQMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly ConferenceItem CONFERENCE;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 26/01/2018 Created [Fabian Sauter]
        /// </history>
        public SetBookmarksMessage(string from, ConferenceItem conference) : base(from, null, SET, getRandomId(), getSetBookmarksQuery(conference))
        {
            this.CONFERENCE = conference;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private static XElement getSetBookmarksQuery(ConferenceItem conference)
        {
            XNamespace ns = "jabber:iq:private";
            XElement node = new XElement(ns + "query");
            XNamespace nsS = "storage:bookmarks";
            XElement sNode = new XElement(nsS + "storage");
            node.Add(sNode);
            sNode.Add(conference.toXElement(nsS));
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
