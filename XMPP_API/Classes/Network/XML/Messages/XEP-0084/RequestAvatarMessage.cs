using System.Xml.Linq;
using XMPP_API.Classes.Network.XML.Messages.XEP_0060;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0084
{
    public class RequestAvatarMessage: PubSubRequestNodeMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly string HASH;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public RequestAvatarMessage(string from, string to, string hash) : base(from, to, Consts.XML_XEP_0084_DATA_NAMESPACE, 1) {
            HASH = hash;
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
        protected override XElement getContent(XNamespace ns)
        {
            XElement itemsNode = new XElement(ns + "items");
            itemsNode.Add(new XAttribute("node", NODE_NAME));
            itemsNode.Add(new XAttribute("max_items", MAX_ITEMS));

            XElement itemNode = new XElement(ns + "item");
            itemNode.Add(new XAttribute("id", HASH));
            itemsNode.Add(itemNode);
            return itemsNode;
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
