using XMPP_API.Classes.Network.XML.Messages.XEP_0060;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0384
{
    public class OmemoSetDeviceListMessage : AbstractPubSubPublishMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly OmemoDevices DEVICES;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 04/08/2018 Created [Fabian Sauter]
        /// </history>
        public OmemoSetDeviceListMessage(string from, OmemoDevices devices) : base(from, null, Consts.XML_XEP_0384_DEVICE_LIST_NODE)
        {
            DEVICES = devices;
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
            return DEVICES;
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
