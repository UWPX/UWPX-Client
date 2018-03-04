using System.Xml;
using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0045.Configuration
{
    public class RoomInfoMessage : IQMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public bool isRoomConfigrationAllowed;
        public RoomConfiguration roomConfig;
        public MUCAffiliation configType;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 07/07/2018 Created [Fabian Sauter]
        /// </history>
        public RoomInfoMessage(XmlNode answer) : base(answer)
        {
            XmlNode qNode = XMLUtils.getChildNode(answer, "query", Consts.XML_XMLNS, Consts.MUC_ROOM_INFO_NAMESPACE_REGEX);
            if (qNode != null)
            {
                configType = getRoomConfigType(qNode.Attributes[Consts.XML_XMLNS].Value);
                XmlNode x = XMLUtils.getChildNode(qNode, "x", Consts.XML_XMLNS, Consts.XML_XEP_0045_ROOM_INFO_DATA_NAMESPACE);
                if (x != null)
                {
                    this.isRoomConfigrationAllowed = true;
                    this.roomConfig = new RoomConfiguration(x);
                    return;
                }
                else
                {
                    isRoomConfigrationAllowed = false;
                }
            }
        }

        public RoomInfoMessage(string from, string to, RoomConfiguration roomConfig, MUCAffiliation configType) : base(from, to, SET, getRandomId(), getQuery(roomConfig, configType))
        {
            this.roomConfig = roomConfig;
            this.configType = configType;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private MUCAffiliation getRoomConfigType(string ns)
        {
            return Utils.parseMUCAffiliation(ns.Substring(ns.LastIndexOf('#') + 1));
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private static XElement getQuery(RoomConfiguration roomConfig, MUCAffiliation configType)
        {
            XNamespace qNs = "http://jabber.org/protocol/muc#" + Utils.mucAffiliationToString(configType);
            XElement query = new XElement(qNs + "query");

            XNamespace xNs = Consts.XML_XEP_0045_ROOM_INFO_DATA_NAMESPACE;
            XElement x = new XElement(xNs + "x");
            x.Add(new XAttribute("type", "submit"));
            roomConfig.addToXElement(x, xNs);

            query.Add(x);
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
