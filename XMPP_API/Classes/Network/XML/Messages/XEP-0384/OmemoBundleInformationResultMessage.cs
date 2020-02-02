using System.Xml;
using XMPP_API.Classes.Network.XML.Messages.XEP_0060;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0384
{
    public class OmemoBundleInformationResultMessage: AbstractPubSubResultMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public OmemoBundleInformation BUNDLE_INFO { get; private set; }
        public uint DEVICE_ID { get; private set; }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 07/08/2018 Created [Fabian Sauter]
        /// </history>
        public OmemoBundleInformationResultMessage(XmlNode node) : base(node)
        {
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
        protected override void loadContent(XmlNodeList content)
        {
            BUNDLE_INFO = new OmemoBundleInformation();
            foreach (XmlNode n in content)
            {
                if (string.Equals(n.Name, "items") && n.Attributes["node"] != null && n.Attributes["node"].Value.StartsWith(Consts.XML_XEP_0384_BUNDLE_INFO_NODE))
                {
                    uint.TryParse(n.Attributes["node"].Value.Replace(Consts.XML_XEP_0384_BUNDLE_INFO_NODE, ""), out uint deviceId);
                    DEVICE_ID = deviceId;
                    BUNDLE_INFO.loadBundleInformation(n);
                    return;
                }
            }
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
