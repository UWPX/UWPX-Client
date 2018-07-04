using System.Collections.Generic;
using XMPP_API.Classes.Network.XML.Messages.XEP_0060;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0048
{
    public class RemoveBookmarksMessage : PubSubRetractMessage
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
        /// 04/07/2018 Created [Fabian Sauter]
        /// </history>
        public RemoveBookmarksMessage(string from, ConferenceItem conferenceItem) : base(from, Consts.XML_XEP_0048_NAMESPACE)
        {
            this.CONFERENCE_ITEMS = new List<ConferenceItem>
            {
                conferenceItem
            };
        }

        public RemoveBookmarksMessage(string from, List<ConferenceItem> conferenceItems) : base(from, Consts.XML_XEP_0048_NAMESPACE)
        {
            this.CONFERENCE_ITEMS = conferenceItems;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
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
