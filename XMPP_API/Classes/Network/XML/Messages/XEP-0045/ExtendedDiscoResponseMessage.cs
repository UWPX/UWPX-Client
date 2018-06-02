using System.Collections.Generic;
using System.Xml;
using XMPP_API.Classes.Network.XML.Messages.XEP_0004;
using XMPP_API.Classes.Network.XML.Messages.XEP_0030;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0045
{
    public class ExtendedDiscoResponseMessage : DiscoResponseMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public DataForm roomConfig;
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
            this.roomConfig = new DataForm();

            XmlNode qNode = XMLUtils.getChildNode(n, "query", Consts.XML_XMLNS, "http://jabber.org/protocol/disco#info");
            if (qNode != null)
            {
                foreach (XmlNode node in qNode)
                {
                    switch (node.Name)
                    {
                        case "x":
                            if (Equals(node.NamespaceURI, Consts.XML_XEP_0004_NAMESPACE))
                            {
                                this.roomConfig.loadRoomConfig(node);
                            }
                            break;

                        case "feature":
                            string var = node.Attributes["var"]?.Value;
                            if (var != null && !Equals(var, Consts.XML_XEP_0045_NAMESPACE))
                            {
                                this.roomConfig.FIELDS.Add(new Field()
                                {
                                    type = FieldType.BOOLEAN,
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
