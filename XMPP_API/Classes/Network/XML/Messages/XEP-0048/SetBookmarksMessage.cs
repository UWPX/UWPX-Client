using System.Collections.Generic;
using XMPP_API.Classes.Network.XML.Messages.XEP_0004;
using XMPP_API.Classes.Network.XML.Messages.XEP_0060;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0048
{
    public class SetBookmarksMessage : PubSubPublishMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly StorageItem STORAGE;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 22/07/2018 Created [Fabian Sauter]
        /// </history>
        public SetBookmarksMessage(string from, List<ConferenceItem> conferences) : base(from, null, Consts.XML_XEP_0048_NAMESPACE)
        {
            STORAGE = new StorageItem(conferences);
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        protected override PubSubPublishOptions getPublishOptions()
        {
            PubSubPublishOptions options = PubSubPublishOptions.getDefaultPublishOptions();
            options.OPTIONS.FIELDS.Add(new Field()
            {
                var = "pubsub#persist_items",
                value = true,
                type = FieldType.BOOLEAN
            });
            options.OPTIONS.FIELDS.Add(new Field()
            {
                var = "pubsub#access_model",
                value = "whitelist",
                type = FieldType.NONE
            });

            return options;
        }

        protected override AbstractPubSubItem getPubSubItem()
        {
            return STORAGE;
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
