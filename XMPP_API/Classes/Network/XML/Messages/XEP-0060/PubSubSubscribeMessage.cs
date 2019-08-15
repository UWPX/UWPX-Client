using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0060
{
    public class PubSubSubscribeMessage : AbstractPubSubMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly string NODE_NAME;
        public readonly string FROM_BARE_JID;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 17/07/2018 Created [Fabian Sauter]
        /// </history>
        public PubSubSubscribeMessage(string fromFullJid, string fromBareJid, string to, string nodeName) : base(fromFullJid, to)
        {
            NODE_NAME = nodeName;
            FROM_BARE_JID = fromBareJid;
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
        protected override void addContent(XElement node, XNamespace ns)
        {
            XElement subscribeNode = new XElement(ns + "subscribe");
            subscribeNode.Add(new XAttribute("node", NODE_NAME));
            subscribeNode.Add(new XAttribute("jid", FROM_BARE_JID));
            node.Add(subscribeNode);
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
