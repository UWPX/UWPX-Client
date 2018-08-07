using XMPP_API.Classes.Network.XML.Messages.XEP_0060;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0384
{
    public class OmemoSetBundleInformationMessage : PubSubPublishMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly OmemoBundleInformation BUNDLE_INFO;
        public readonly uint DEVICE_ID;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 06/08/2018 Created [Fabian Sauter]
        /// </history>
        public OmemoSetBundleInformationMessage(string from, OmemoBundleInformation bundleInfo, uint deviceid) : base(from, null, Consts.XML_XEP_0384_BUNDLE_INFO_NODE + deviceid)
        {
            this.BUNDLE_INFO = bundleInfo;
            this.DEVICE_ID = deviceid;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        protected override PubSubPublishOptions getPublishOptions()
        {
            return null;
        }

        protected override AbstractPubSubItem getPubSubItem()
        {
            return BUNDLE_INFO;
        }

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
