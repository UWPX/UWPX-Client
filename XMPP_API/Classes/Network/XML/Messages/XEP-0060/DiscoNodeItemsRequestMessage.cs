using System.Xml.Linq;
using XMPP_API.Classes.Network.XML.Messages.XEP_0030;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0060
{
    public class DiscoNodeItemsRequestMessage: DiscoRequestMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly string NODE_NAME;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public DiscoNodeItemsRequestMessage(string from, string to, string nodeName) : base(from, to, DiscoType.ITEMS)
        {
            NODE_NAME = nodeName;
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
        protected override XElement getQuery()
        {
            XElement queryNode = base.getQuery();
            if (!(NODE_NAME is null))
            {
                queryNode.Add(new XAttribute("node", NODE_NAME));
            }
            return queryNode;
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
