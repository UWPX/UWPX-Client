using System.Xml.Linq;
using XMPP_API.Classes.Network.XML.Messages.XEP_0030;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0060
{
    public class PubSubDiscoverNodeItemsMessage : DiscoRequestMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly string NODE_NAME;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 17/07/2018 Created [Fabian Sauter]
        /// </history>
        public PubSubDiscoverNodeItemsMessage(string from, string to, string nodeName) : base(from, to, DiscoType.ITEMS)
        {
            NODE_NAME = nodeName;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        protected override XElement getQuery()
        {
            XElement query = base.getQuery();
            query.Add(new XAttribute("node", NODE_NAME));
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
