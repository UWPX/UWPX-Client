using XMPP_API.Classes.Network.XML.Messages.XEP_0004;
using XMPP_API.Classes.Network.XML.Messages.XEP_0060;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0402
{
    public class AddBookmarkMessage: AbstractPubSubPublishMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly ConferenceItem CONFERENCE_ITEM;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 15/07/2018 Created [Fabian Sauter]
        /// </history>
        public AddBookmarkMessage(string from, ConferenceItem conferenceItem) : base(from, null, Consts.XML_XEP_0402_NAMESPACE)
        {
            CONFERENCE_ITEM = conferenceItem;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        protected override PubSubPublishOptions getPublishOptions()
        {
            PubSubPublishOptions options = PubSubPublishOptions.getDefaultPublishOptions();
            options.OPTIONS.fields.Add(new Field()
            {
                var = "pubsub#persist_items",
                value = true,
                type = FieldType.BOOLEAN
            });
            options.OPTIONS.fields.Add(new Field()
            {
                var = "pubsub#access_model",
                value = "whitelist",
                type = FieldType.LIST_SINGLE
            });

            return options;
        }

        protected override AbstractPubSubItem getPubSubItem()
        {
            return CONFERENCE_ITEM;
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
