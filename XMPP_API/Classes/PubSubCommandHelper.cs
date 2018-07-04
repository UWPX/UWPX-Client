using System;
using System.Collections.Generic;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.XEP_0048;

namespace XMPP_API.Classes
{
    public class PubSubCommandHelper
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly XMPPClient CLIENT;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 16/06/2018 Created [Fabian Sauter]
        /// </history>
        public PubSubCommandHelper(XMPPClient client)
        {
            this.CLIENT = client;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        /// <summary>
        /// Sends a RequestBookmarksMessage for requesting all bookmarks from the server.
        /// </summary>
        /// <param name="onMessage">The method that should get executed once the helper receives a new valid message.</param>
        /// <param name="onTimeout">The method that should get executed once the helper timeout gets triggered.</param>
        /// <returns>Returns a MessageResponseHelper listening for RequestBookmarksMessage answers.</returns>
        public MessageResponseHelper<IQMessage> requestBookmars(Func<IQMessage, bool> onMessage, Action onTimeout)
        {
            MessageResponseHelper<IQMessage> helper = new MessageResponseHelper<IQMessage>(CLIENT, onMessage, onTimeout);
            RequestBookmarksMessage msg = new RequestBookmarksMessage(CLIENT.getXMPPAccount().getIdDomainAndResource());
            helper.start(msg);
            return helper;
        }

        /// <summary>
        /// Sends a AddBookmarksMessage for storing the given bookmark on the server.
        /// </summary>
        /// <param name="onMessage">The method that should get executed once the helper receives a new valid message.</param>
        /// <param name="onTimeout">The method that should get executed once the helper timeout gets triggered.</param>
        /// <param name="conferenceItem">The bookmark that should get stored on the server.</param>
        /// <returns>Returns a MessageResponseHelper listening for AddBookmarksMessage answers.</returns>
        public MessageResponseHelper<IQMessage> addBookmark(Func<IQMessage, bool> onMessage, Action onTimeout, ConferenceItem conferenceItem)
        {
            MessageResponseHelper<IQMessage> helper = new MessageResponseHelper<IQMessage>(CLIENT, onMessage, onTimeout);
            AddBookmarksMessage msg = new AddBookmarksMessage(CLIENT.getXMPPAccount().getIdDomainAndResource(), conferenceItem);
            helper.start(msg);
            return helper;
        }

        /// <summary>
        /// Sends a AddBookmarksMessage for storing the given bookmarks on the server.
        /// </summary>
        /// <param name="onMessage">The method that should get executed once the helper receives a new valid message.</param>
        /// <param name="onTimeout">The method that should get executed once the helper timeout gets triggered.</param>
        /// <param name="conferenceItem">The bookmark that should get stored on the server.</param>
        /// <returns>Returns a MessageResponseHelper listening for AddBookmarksMessage answers.</returns>
        public MessageResponseHelper<IQMessage> addBookmarks(Func<IQMessage, bool> onMessage, Action onTimeout, List<ConferenceItem> conferenceItems)
        {
            MessageResponseHelper<IQMessage> helper = new MessageResponseHelper<IQMessage>(CLIENT, onMessage, onTimeout);
            AddBookmarksMessage msg = new AddBookmarksMessage(CLIENT.getXMPPAccount().getIdDomainAndResource(), conferenceItems);
            helper.start(msg);
            return helper;
        }

        /// <summary>
        /// Sends a RemoveBookmarksMessage for removing the given bookmark from the server.
        /// </summary>
        /// <param name="onMessage">The method that should get executed once the helper receives a new valid message.</param>
        /// <param name="onTimeout">The method that should get executed once the helper timeout gets triggered.</param>
        /// <param name="conferenceItem">The bookmark that should get removed from the server.</param>
        /// <returns>Returns a MessageResponseHelper listening for RemoveBookmarksMessage answers.</returns>
        public MessageResponseHelper<IQMessage> removeBookmark(Func<IQMessage, bool> onMessage, Action onTimeout, ConferenceItem conferenceItem)
        {
            MessageResponseHelper<IQMessage> helper = new MessageResponseHelper<IQMessage>(CLIENT, onMessage, onTimeout);
            RemoveBookmarksMessage msg = new RemoveBookmarksMessage(CLIENT.getXMPPAccount().getIdDomainAndResource(), conferenceItem);
            helper.start(msg);
            return helper;
        }

        /// <summary>
        /// Sends a RemoveBookmarksMessage for removing the given bookmarks from the server.
        /// </summary>
        /// <param name="onMessage">The method that should get executed once the helper receives a new valid message.</param>
        /// <param name="onTimeout">The method that should get executed once the helper timeout gets triggered.</param>
        /// <param name="conferenceItem">The bookmark that should get removed from the server.</param>
        /// <returns>Returns a RemoveBookmarksMessage listening for AddBookmarksMessage answers.</returns>
        public MessageResponseHelper<IQMessage> removeBookmarks(Func<IQMessage, bool> onMessage, Action onTimeout, List<ConferenceItem> conferenceItems)
        {
            MessageResponseHelper<IQMessage> helper = new MessageResponseHelper<IQMessage>(CLIENT, onMessage, onTimeout);
            RemoveBookmarksMessage msg = new RemoveBookmarksMessage(CLIENT.getXMPPAccount().getIdDomainAndResource(), conferenceItems);
            helper.start(msg);
            return helper;
        }

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
