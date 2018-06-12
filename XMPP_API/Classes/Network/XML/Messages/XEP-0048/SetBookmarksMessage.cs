using System.Collections.Generic;
using XMPP_API.Classes.Network.XML.Messages.XEP_0004;
using XMPP_API.Classes.Network.XML.Messages.XEP_0060;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0048
{
    public class SetBookmarksMessage : PubSubPublishMessage
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
        /// 12/06/2018 Created [Fabian Sauter]
        /// </history>
        public SetBookmarksMessage(string from, List<ConferenceItem> conferenceItems) : base(from, Consts.XML_XEP_0048_NAMESPACE, getPubSubItem(conferenceItems), getOptions())
        {
            this.CONFERENCE_ITEMS = conferenceItems;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private static PubSubPublishOptions getOptions()
        {
            PubSubPublishOptions options = PubSubPublishOptions.getDefaultPublishOptions();
            options.OPTIONS.FIELDS.Add(new Field()
            {
                var = "pubsub#persist_items",
                value = "true",
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

        private static PubSubItem getPubSubItem(List<ConferenceItem> conferenceItems)
        {
            return new PubSubItem("current", new StorageItem(conferenceItems));
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
