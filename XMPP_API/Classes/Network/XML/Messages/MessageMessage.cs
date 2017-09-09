using System.Xml;

namespace XMPP_API.Classes.Network.XML.Messages
{
    public class MessageMessage : AbstractAddressableMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly string MESSAGE;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Construktoren--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 17/08/2017 Created [Fabian Sauter]
        /// </history>
        public MessageMessage(string from, string to, string message) : base(from, to)
        {
            this.MESSAGE = message;
        }

        public MessageMessage(XmlNode node) : base(node.Attributes["from"]?.Value, node.Attributes["to"]?.Value, node.Attributes["id"]?.Value)
        {
            if(!node.HasChildNodes)
            {
                return;
            }
            foreach (XmlNode n in node.ChildNodes)
            {
                if(n.Name.Equals("body"))
                {
                    MESSAGE = n.InnerText;
                }
            }
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public string getMessage()
        {
            return MESSAGE;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public override string toXmlString()
        {
            string s = "";
            s += Consts.XML_MESSAGE_START.Replace('>', ' ');
            s += "from='" + FROM + "' to='" + TO;
            s += "' id='" + ID + "' type='chat'>";
            s += Consts.XML_BODY_START + MESSAGE + Consts.XML_BODY_CLOSE;
            s += Consts.XML_MESSAGE_CLOSE;
            return s;
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
