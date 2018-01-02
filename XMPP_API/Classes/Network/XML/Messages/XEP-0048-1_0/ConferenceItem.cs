using System.Xml;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0048_1_0
{
    public class ConferenceItem
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly string NAME;
        public readonly string JID;
        public readonly bool MINIMIZE;
        public readonly bool AUTOJOIN;

        public readonly string NICK;
        public readonly string PASSWORD;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 02/01/2018 Created [Fabian Sauter]
        /// </history>
        public ConferenceItem(XmlNode node)
        {
            if(node != null)
            {
                NAME = node.Attributes["name"]?.Value;
                JID = node.Attributes["jid"]?.Value;
                MINIMIZE = XMLUtils.tryParseToBool(node.Attributes["minimize"]?.Value);
                AUTOJOIN = XMLUtils.tryParseToBool(node.Attributes["autojoin"]?.Value);

                XmlNode nNode = XMLUtils.getChildNode(node, "nick");
                if(nNode != null)
                {
                    NICK = nNode.InnerText;
                }

                XmlNode pNode = XMLUtils.getChildNode(node, "password");
                if (pNode != null)
                {
                    PASSWORD = pNode.InnerText;
                }
            }
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


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
