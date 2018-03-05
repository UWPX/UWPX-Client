using System.Collections.Generic;
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
        public List<string> roomFeatures;

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
            this.roomConfig = new RoomConfiguration();

            XmlNode qNode = XMLUtils.getChildNode(n, "query", Consts.XML_XMLNS, "http://jabber.org/protocol/disco#info");
            if (qNode != null)
            {
                foreach (XmlNode node in qNode)
                {
                    switch (node.Name)
                    {
                        case "x":
                            if (Equals(node.NamespaceURI, Consts.XML_XEP_0045_ROOM_INFO_DATA_NAMESPACE))
                            {
                                this.roomConfig.loadRoomConfig(node);
                            }
                            break;

                        case "feature":
                            string var = node.Attributes["var"]?.Value;
                            if (var != null && !Equals(var, Consts.XML_XEP_0045_NAMESPACE))
                            {
                                this.roomConfig.options.Add(new MUCInfoField()
                                {
                                    type = MUCInfoFieldType.BOOLEAN,
                                    var = var,
                                    value = true
                                });
                            }
                            break;
                    }
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
