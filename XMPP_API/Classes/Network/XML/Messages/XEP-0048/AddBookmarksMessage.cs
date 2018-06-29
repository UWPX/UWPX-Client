using System.Collections.Generic;
using XMPP_API.Classes.Network.XML.Messages.XEP_0004;
using XMPP_API.Classes.Network.XML.Messages.XEP_0060;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0048
{
    public class AddBookmarksMessage : PubSubPublishMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly List<ConferenceItem> CONFERENCE_ITEMS;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 29/06/2018 Created [Fabian Sauter]
        /// </history>
        public AddBookmarksMessage(string from, ConferenceItem conferenceItem) : base(from, Consts.XML_XEP_0048_NAMESPACE)
        {
            this.CONFERENCE_ITEMS = new List<ConferenceItem>();
            this.CONFERENCE_ITEMS.Add(conferenceItem);
        }

        public AddBookmarksMessage(string from, List<ConferenceItem> conferenceItems) : base(from, Consts.XML_XEP_0048_NAMESPACE)
        {
            this.CONFERENCE_ITEMS = conferenceItems;
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

        protected override PubSubItem getPubSubItem()
        {
            return new PubSubItem("current", new StorageItem(CONFERENCE_ITEMS));
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
