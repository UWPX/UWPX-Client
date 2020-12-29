using XMPP_API.Classes.Network.XML.Messages.XEP_0004;
using XMPP_API.Classes.Network.XML.Messages.XEP_0060;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0384
{
    public class OmemoSetBundleInformationMessage: AbstractPubSubPublishMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly OmemoBundleInformation BUNDLE_INFO;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public OmemoSetBundleInformationMessage(string from, OmemoBundleInformation bundleInfo) : base(from, null, Consts.XML_XEP_0384_BUNDLE_INFO_NODE)
        {
            BUNDLE_INFO = bundleInfo;
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
            options.OPTIONS.fields.Add(new Field()
            {
                var = "pubsub#max_items",
                value = "max",
                type = FieldType.NONE
            });
            return options;
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
