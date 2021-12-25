using System.Xml.Linq;
using XMPP_API.Classes.Network.XML.Messages.XEP_0060;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0084
{
    public class AvatarMetadataDataPubSubItem: AbstractPubSubItem
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly AvatarInfo INFO;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public AvatarMetadataDataPubSubItem(string avatarBase16Hash, AvatarInfo info)
        {
            INFO = info;
            id = avatarBase16Hash;
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
        protected override XElement getContent(XNamespace ns)
        {
            XNamespace xepNs = Consts.XML_XEP_0084_METADATA_NAMESPACE;
            XElement metadataNode = new XElement(xepNs + "metadata");
            metadataNode.Add(INFO.toXElement(xepNs));
            return metadataNode;
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
