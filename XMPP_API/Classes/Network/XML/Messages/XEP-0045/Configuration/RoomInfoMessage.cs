using System.Xml;
using System.Xml.Linq;
using XMPP_API.Classes.Network.XML.Messages.XEP_0004;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0045.Configuration
{
    public class RoomInfoMessage : IQMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public bool isRoomConfigrationAllowed;
        public DataForm roomConfig;
        public MUCAffiliation configLevel;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 07/07/2018 Created [Fabian Sauter]
        /// </history>
        public RoomInfoMessage(XmlNode node) : base(node)
        {
            XmlNode qNode = XMLUtils.getChildNode(node, "query", Consts.XML_XMLNS, Consts.MUC_ROOM_INFO_NAMESPACE_REGEX);
            if (qNode != null)
            {
                configLevel = getRoomConfigLevel(qNode.Attributes[Consts.XML_XMLNS].Value);
                XmlNode x = XMLUtils.getChildNode(qNode, "x", Consts.XML_XMLNS, Consts.XML_XEP_0004_NAMESPACE);
                if (x != null)
                {
                    this.isRoomConfigrationAllowed = true;
                    this.roomConfig = new DataForm(x);
                    return;
                }
                else
                {
                    isRoomConfigrationAllowed = false;
                    this.roomConfig = null;
                }
            }
        }

        public RoomInfoMessage(string from, string to, DataForm roomConfig, MUCAffiliation configType) : base(from, to, SET, getRandomId(), getQuery(roomConfig, configType))
        {
            this.roomConfig = roomConfig;
            this.configLevel = configType;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private MUCAffiliation getRoomConfigLevel(string ns)
        {
            return Utils.parseMUCAffiliation(ns.Substring(ns.LastIndexOf('#') + 1));
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private static XElement getQuery(DataForm roomConfig, MUCAffiliation configType)
        {
            XNamespace qNs = "http://jabber.org/protocol/muc#" + Utils.mucAffiliationToString(configType);
            XElement query = new XElement(qNs + "query");

            roomConfig.addToXElement(query);
            return query;
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
