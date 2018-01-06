using System.Xml;
using System.Xml.Linq;
using XMPP_API.Classes.Network.XML.Messages.Features;

namespace XMPP_API.Classes.Network.XML.Messages
{
    class OpenStreamAnswerMessage : AbstractMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly XmlNode STREAM_NODE;
        private readonly string FROM;
        private readonly string TO;
        private readonly StreamFeaturesMessage STREAM_FEATURES;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 29/01/2017 Created [Fabian Sauter]
        /// </history>
        public OpenStreamAnswerMessage(XmlNode streamNode) : base(streamNode.Attributes["id"]?.Value)
        {
            this.STREAM_NODE = streamNode;
            this.FROM = STREAM_NODE.Attributes["from"]?.Value;
            this.TO = STREAM_NODE.Attributes.GetNamedItem("to")?.Value;
            this.STREAM_FEATURES = getStreamFeaturesMessage(streamNode);
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public StreamFeaturesMessage getStreamFeaturesMessage()
        {
            return STREAM_FEATURES;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public override XElement toXElement()
        {
            throw new System.NotImplementedException();
        }

        public StreamFeaturesMessage getStreamFeaturesMessage(XmlNode node)
        {
            XmlNode n = XMLUtils.getChildNode(node, "stream:features");
            return n == null ? null : new StreamFeaturesMessage(n);
        }

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
