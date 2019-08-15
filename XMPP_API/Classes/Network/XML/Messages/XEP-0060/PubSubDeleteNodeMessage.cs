using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0060
{
    public class PubSubDeleteNodeMessage : AbstractPubSubMessage
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
        /// 19/07/2018 Created [Fabian Sauter]
        /// </history>
        public PubSubDeleteNodeMessage(string from, string to, string nodeName) : base(from, to)
        {
            NODE_NAME = nodeName;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        protected override XNamespace getPubSubNamespace()
        {
            return Consts.XML_XEP_0060_NAMESPACE_OWNER;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--
        protected override void addContent(XElement node, XNamespace ns)
        {
            XElement delNode = new XElement(ns + "delete");
            delNode.Add(new XAttribute("node", NODE_NAME));
            node.Add(delNode);
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
