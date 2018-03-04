using System.Xml;
using XMPP_API.Classes.Network.XML.Messages.XEP_0030;
using XMPP_API.Classes.Network.XML.Messages.XEP_0045.Configuration;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0045
{
    public class ExtendedDiscoResponseMessage : DiscoResponseMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public RoomConfiguration roomConfig;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 05/01/2018 Created [Fabian Sauter]
        /// </history>
        public ExtendedDiscoResponseMessage(XmlNode n) : base(n)
        {
            this.roomConfig = null;

            XmlNode qNode = XMLUtils.getChildNode(n, "query", Consts.XML_XMLNS, "http://jabber.org/protocol/disco#info");
            if (qNode != null)
            {
                XmlNode x = XMLUtils.getChildNode(qNode, "x", Consts.XML_XMLNS, "jabber:x:data");
                if (x != null)
                {
                    this.roomConfig = new RoomConfiguration(x);
                    return;
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
