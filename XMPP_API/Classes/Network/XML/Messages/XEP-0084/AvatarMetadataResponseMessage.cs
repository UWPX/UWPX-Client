using System.Collections.Generic;
using System.Xml;
using XMPP_API.Classes.Network.XML.Messages.XEP_0060;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0084
{
    public class AvatarMetadataResponseMessage: AbstractPubSubResultMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly List<AvatarInfo> INFOS = new List<AvatarInfo>();
        public string HASH { get; private set; }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public AvatarMetadataResponseMessage(XmlNode node) : base(node) { }

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
        protected override void loadContent(XmlNodeList content)
        {
            foreach (XmlNode itemsNode in content)
            {
                if (string.Equals(itemsNode.Name, "items") && string.Equals(itemsNode.Attributes["node"]?.Value, Consts.XML_XEP_0084_METADATA_NAMESPACE))
                {
                    foreach (XmlNode itemNode in itemsNode.ChildNodes)
                    {
                        if (string.Equals(itemNode.Name, "item"))
                        {
                            XmlAttribute idAttr = XMLUtils.getAttribute(itemNode, "id");
                            if (!string.IsNullOrEmpty(idAttr?.Value))
                            {
                                HASH = idAttr.Value;
                                XmlNode metadataNode = XMLUtils.getChildNode(itemNode, "metadata", Consts.XML_XMLNS, Consts.XML_XEP_0084_METADATA_NAMESPACE);
                                if (metadataNode is not null)
                                {
                                    INFOS.Clear();
                                    foreach (XmlNode infoNode in metadataNode.ChildNodes)
                                    {
                                        if (string.Equals(infoNode.Name, "info"))
                                        {
                                            INFOS.Add(new AvatarInfo(infoNode));
                                        }
                                    }
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
