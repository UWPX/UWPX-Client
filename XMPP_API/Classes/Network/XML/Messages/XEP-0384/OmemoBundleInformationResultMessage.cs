using System.Xml;
using XMPP_API.Classes.Network.XML.Messages.XEP_0060;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0384
{
    public class OmemoBundleInformationResultMessage : AbstractPubSubResultMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly OmemoBundleInformation BUNDLE_INFO;

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
            this.BUNDLE_INFO = new OmemoBundleInformation();
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
            foreach (XmlNode n in content)
            {
                if (string.Equals(n.Name, "items") && n.Attributes["node"] != null && n.Attributes["node"].Value.StartsWith(Consts.XML_XEP_0384_BUNDLE_INFO_NODE))
                {
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
