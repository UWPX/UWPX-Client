using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0060
{
    public class PubSubSubscribeMessage : PubSubMessage
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
        public PubSubSubscribeMessage(string from, string to, string nodeName) : base(from, to)
        {
            this.NODE_NAME = nodeName;
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
            subscribeNode.Add(new XAttribute("jid", FROM));
            node.Add(subscribeNode);
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
