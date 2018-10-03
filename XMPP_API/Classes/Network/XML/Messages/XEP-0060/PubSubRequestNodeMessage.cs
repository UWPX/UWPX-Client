using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0060
{
    public class PubSubRequestNodeMessage : AbstractPubSubRequestMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly string NODE_NAME;
        public readonly uint MAX_ITEMS;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 17/03/2018 Created [Fabian Sauter]
        /// </history>
        public PubSubRequestNodeMessage(string from, string to, string nodeName, uint maxItems) : base(from, to)
        {
            this.NODE_NAME = nodeName;
            this.MAX_ITEMS = maxItems;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        protected override XElement getContent(XNamespace ns)
        {
            XElement itemsNode = new XElement(ns + "items");
            if (NODE_NAME != null)
            {
                itemsNode.Add(new XAttribute("node", NODE_NAME));
            }
            if (MAX_ITEMS > 0)
            {
                itemsNode.Add(new XAttribute("max_items", MAX_ITEMS));
            }
            return itemsNode;
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
