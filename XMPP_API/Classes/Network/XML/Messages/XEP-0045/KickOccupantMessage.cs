using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0045
{
    // https://xmpp.org/extensions/xep-0045.html#kick
    class KickOccupantMessage : IQMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly string REASON;
        public readonly string NICKNAME;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 08/03/2018 Created [Fabian Sauter]
        /// </history>
        public KickOccupantMessage(string from, string roomJid, string nickname, string reason) : base(from, roomJid, SET, getRandomId(), getKickQuery(reason, nickname))
        {
            this.REASON = reason;
            this.NICKNAME = nickname;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private static XElement getKickQuery(string reason, string nickname)
        {
            XNamespace ns = Consts.XML_XEP_0045_NAMESPACE_ADMIN;
            XElement query = new XElement(ns + "query");

            XElement item = new XElement(ns + "item");
            item.Add(new XAttribute("nick", nickname));
            item.Add(new XAttribute("role", "none"));
            if (reason != null)
            {
                item.Add(new XElement(ns + "reason")
                {
                    Value = reason
                });
            }
            query.Add(item);

            return query;
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
