using XMPP_API.Classes.Network.XML.Messages.XEP_0004;
using XMPP_API.Classes.Network.XML.Messages.XEP_0060;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0384
{
    public class OmemoSetDeviceListMessage: AbstractPubSubPublishMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly OmemoXmlDevices DEVICES;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public OmemoSetDeviceListMessage(string from, OmemoXmlDevices devices) : base(from, null, Consts.XML_XEP_0384_DEVICE_LIST_NODE)
        {
            DEVICES = devices;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        protected override PubSubPublishOptions getPublishOptions()
        {
            PubSubPublishOptions options = PubSubPublishOptions.getDefaultPublishOptions();
            options.OPTIONS.fields.Add(new Field()
            {
                var = "pubsub#access_model",
                value = "open",
                type = FieldType.NONE
            });
            return options;
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
