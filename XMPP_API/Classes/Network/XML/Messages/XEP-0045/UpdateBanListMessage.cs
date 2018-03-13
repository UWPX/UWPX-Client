using System.Collections.Generic;
using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0045
{
    class UpdateBanListMessage : IQMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public List<BanedUser> changedUsers;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 13/03/2018 Created [Fabian Sauter]
        /// </history>
        public UpdateBanListMessage(string from, string roomJid, List<BanedUser> changedUsers) : base(from, roomJid, SET, getRandomId(), getQueryNode(changedUsers))
        {
            this.changedUsers = changedUsers;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private static XElement getQueryNode(List<BanedUser> changedUsers)
        {
            XNamespace ns = Consts.XML_XEP_0045_NAMESPACE_ADMIN;
            XElement queryNode = new XElement(ns + "query");

            foreach (BanedUser u in changedUsers)
            {
                u.addToNode(queryNode, ns);
            }

            return queryNode;
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
