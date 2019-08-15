using System.Xml;
using System.Xml.Linq;
using XMPP_API.Classes.Network.XML.Messages.XEP_0004;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0045.Configuration
{
    public class RoomInfoMessage : IQMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly bool IS_ROOM_CONFIGURATION_ALLOWED;
        public readonly DataForm ROOM_CONFIG;
        public readonly MUCAffiliation CONFIG_LEVEL;

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
                CONFIG_LEVEL = getRoomConfigLevel(qNode.Attributes[Consts.XML_XMLNS].Value);
                XmlNode x = XMLUtils.getChildNode(qNode, "x", Consts.XML_XMLNS, Consts.XML_XEP_0004_NAMESPACE);
                if (x != null)
                {
                    IS_ROOM_CONFIGURATION_ALLOWED = true;
                    ROOM_CONFIG = new DataForm(x);
                    return;
                }
                else
                {
                    IS_ROOM_CONFIGURATION_ALLOWED = false;
                    ROOM_CONFIG = null;
                }
            }
        }

        public RoomInfoMessage(string from, string to, DataForm roomConfig, MUCAffiliation configType) : base(from, to, SET, getRandomId())
        {
            ROOM_CONFIG = roomConfig;
            ROOM_CONFIG.FIELDS.Add(new Field()
            {
                type = FieldType.HIDDEN,
                value = Consts.XML_XEP_0045_NAMESPACE_ROOM_CONFIG,
                var = "FORM_TYPE"
            });
            CONFIG_LEVEL = configType;
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
        protected override XElement getQuery()
        {
            XNamespace qNs = Consts.XML_XEP_0045_NAMESPACE + "#" + Utils.mucAffiliationToString(CONFIG_LEVEL);
            XElement query = new XElement(qNs + "query");

            ROOM_CONFIG.addToXElement(query);
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
