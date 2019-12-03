using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XMPP_API.Classes.Network;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.Helper;
using XMPP_API.Classes.Network.XML.Messages.XEP_0004;
using XMPP_API.Classes.Network.XML.Messages.XEP_0030;
using XMPP_API.Classes.Network.XML.Messages.XEP_0060;
using XMPP_API.Classes.Network.XML.Messages.XEP_0402;

namespace XMPP_API.Classes
{
    public class PubSubCommandHelper
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly XMPPConnection2 CONNECTION;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 16/06/2018 Created [Fabian Sauter]
        /// </history>
        public PubSubCommandHelper(XMPPConnection2 connection)
        {
            CONNECTION = connection;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public string getPubSubServer()
        {
            throw new NotImplementedException();
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        /// <summary>
        /// Sends a RequestBookmarksMessage to request all bookmarks from the server.
        /// https://xmpp.org/extensions/xep-0048.html#storage-pubsub-retrieve
        /// </summary>
        /// <param name="onMessage">The method that should get executed once the helper receives a new valid message (can be null).</param>
        /// <param name="onTimeout">The method that should get executed once the helper timeout gets triggered (can be null).</param>
        /// <returns>Returns a MessageResponseHelper listening for RequestBookmarksMessage answers.</returns>
        public MessageResponseHelper<IQMessage> requestBookmars_xep_0048(MessageResponseHelper<IQMessage>.OnMessageHandler onMessage, MessageResponseHelper<IQMessage>.OnTimeoutHandler onTimeout)
        {
            MessageResponseHelper<IQMessage> helper = new MessageResponseHelper<IQMessage>(CONNECTION, onMessage, onTimeout);
            Network.XML.Messages.XEP_0048.RequestBookmarksMessage msg = new Network.XML.Messages.XEP_0048.RequestBookmarksMessage(CONNECTION.account.getFullJid());
            helper.start(msg);
            return helper;
        }

        /// <summary>
        /// Sends a SetBookmarksMessage to request all bookmarks from the server.
        /// https://xmpp.org/extensions/xep-0048.html#storage-pubsub-upload
        /// </summary>
        /// <param name="onMessage">The method that should get executed once the helper receives a new valid message (can be null).</param>
        /// <param name="onTimeout">The method that should get executed once the helper timeout gets triggered (can be null).</param>
        /// <returns>Returns a MessageResponseHelper listening for SetBookmarksMessage answers.</returns>
        public MessageResponseHelper<IQMessage> setBookmars_xep_0048(IList<Network.XML.Messages.XEP_0048.ConferenceItem> conferences, MessageResponseHelper<IQMessage>.OnMessageHandler onMessage, MessageResponseHelper<IQMessage>.OnTimeoutHandler onTimeout)
        {
            MessageResponseHelper<IQMessage> helper = new MessageResponseHelper<IQMessage>(CONNECTION, onMessage, onTimeout);
            Network.XML.Messages.XEP_0048.SetBookmarksMessage msg = new Network.XML.Messages.XEP_0048.SetBookmarksMessage(CONNECTION.account.getFullJid(), conferences);
            helper.start(msg);
            return helper;
        }

        /// <summary>
        /// Sends a SetBookmarksMessage to set bookmarks on the server.
        /// https://xmpp.org/extensions/xep-0048.html#storage-pubsub-upload
        /// </summary>
        /// <param name="conferences">A list of XEP-0048 <seealso cref="Network.XML.Messages.XEP_0048.ConferenceItem"/> objects that should be set.</param>
        /// <returns>The XEP-0048 SetBookmarksMessage result.</returns>
        public async Task<MessageResponseHelperResult<IQMessage>> setBookmars_xep_0048Async(IList<Network.XML.Messages.XEP_0048.ConferenceItem> conferences)
        {
            AsyncMessageResponseHelper<IQMessage> helper = new AsyncMessageResponseHelper<IQMessage>(CONNECTION);
            Network.XML.Messages.XEP_0048.SetBookmarksMessage msg = new Network.XML.Messages.XEP_0048.SetBookmarksMessage(CONNECTION.account.getFullJid(), conferences);
            return await helper.startAsync(msg);
        }

        /// <summary>
        /// Sends a RequestBookmarksMessage to request all bookmarks from the server.
        /// https://xmpp.org/extensions/xep-0402.html#retrieving-bookmarks
        /// </summary>
        /// <param name="onMessage">The method that should get executed once the helper receives a new valid message (can be null).</param>
        /// <param name="onTimeout">The method that should get executed once the helper timeout gets triggered (can be null).</param>
        /// <returns>Returns a MessageResponseHelper listening for RequestBookmarksMessage answers.</returns>
        public MessageResponseHelper<IQMessage> requestBookmars_xep_0402(MessageResponseHelper<IQMessage>.OnMessageHandler onMessage, MessageResponseHelper<IQMessage>.OnTimeoutHandler onTimeout)
        {
            MessageResponseHelper<IQMessage> helper = new MessageResponseHelper<IQMessage>(CONNECTION, onMessage, onTimeout);
            RequestBookmarksMessage msg = new RequestBookmarksMessage(CONNECTION.account.getFullJid());
            helper.start(msg);
            return helper;
        }

        /// <summary>
        /// Sends a AddBookmarksMessage to store the given bookmark on the server.
        /// https://xmpp.org/extensions/xep-0402.html#adding-a-bookmark
        /// </summary>
        /// <param name="conferenceItem">The bookmark that should get stored on the server.</param>
        /// <param name="onMessage">The method that should get executed once the helper receives a new valid message (can be null).</param>
        /// <param name="onTimeout">The method that should get executed once the helper timeout gets triggered (can be null).</param>
        /// <returns>Returns a MessageResponseHelper listening for AddBookmarksMessage answers.</returns>
        public MessageResponseHelper<IQMessage> addBookmark_xep_0402(ConferenceItem conferenceItem, MessageResponseHelper<IQMessage>.OnMessageHandler onMessage, MessageResponseHelper<IQMessage>.OnTimeoutHandler onTimeout)
        {
            MessageResponseHelper<IQMessage> helper = new MessageResponseHelper<IQMessage>(CONNECTION, onMessage, onTimeout);
            AddBookmarkMessage msg = new AddBookmarkMessage(CONNECTION.account.getFullJid(), conferenceItem);
            helper.start(msg);
            return helper;
        }

        /// <summary>
        /// Sends a RemoveBookmarksMessage to remove the given bookmark from the server.
        /// https://xmpp.org/extensions/xep-0402.html#removing-a-bookmark
        /// </summary>
        /// <param name="conferenceItem">The bookmark that should get removed from the server.</param>
        /// <param name="onMessage">The method that should get executed once the helper receives a new valid message (can be null).</param>
        /// <param name="onTimeout">The method that should get executed once the helper timeout gets triggered (can be null).</param>
        /// <returns>Returns a MessageResponseHelper listening for RemoveBookmarksMessage answers.</returns>
        public MessageResponseHelper<IQMessage> removeBookmark_xep_0402(ConferenceItem conferenceItem, MessageResponseHelper<IQMessage>.OnMessageHandler onMessage, MessageResponseHelper<IQMessage>.OnTimeoutHandler onTimeout)
        {
            MessageResponseHelper<IQMessage> helper = new MessageResponseHelper<IQMessage>(CONNECTION, onMessage, onTimeout);
            RemoveBookmarkMessage msg = new RemoveBookmarkMessage(CONNECTION.account.getFullJid(), conferenceItem);
            helper.start(msg);
            return helper;
        }

        /// <summary>
        /// Sends a DiscoverNodeMetadataMessage to discover the node metadata.
        /// https://xmpp.org/extensions/xep-0060.html#entity-metadata
        /// </summary>
        /// <param name="to">The target pubsub server (can be null).</param>
        /// <param name="nodeName">The name of the node, the metadata should get queried for.</param>
        /// <param name="onMessage">The method that should get executed once the helper receives a new valid message (can be null).</param>
        /// <param name="onTimeout">The method that should get executed once the helper timeout gets triggered (can be null).</param>
        /// <returns>Returns a MessageResponseHelper listening for DiscoverNodeMetadataMessage answers.</returns>
        public MessageResponseHelper<IQMessage> requestNodeMetadata(string to, string nodeName, MessageResponseHelper<IQMessage>.OnMessageHandler onMessage, MessageResponseHelper<IQMessage>.OnTimeoutHandler onTimeout)
        {
            MessageResponseHelper<IQMessage> helper = new MessageResponseHelper<IQMessage>(CONNECTION, onMessage, onTimeout);
            PubSubDiscoverNodeMetadataMessage msg = new PubSubDiscoverNodeMetadataMessage(CONNECTION.account.getFullJid(), to, nodeName);
            helper.start(msg);
            return helper;
        }

        /// <summary>
        /// Sends a PubSubSubscribeMessage to subscribe to the given node.
        /// https://xmpp.org/extensions/xep-0060.html#subscriber-subscribe
        /// </summary>
        /// <param name="to">The target pubsub server (can be null).</param>
        /// <param name="nodeName">The name of the node, you want to subscribe to.</param>
        /// <param name="onMessage">The method that should get executed once the helper receives a new valid message (can be null).</param>
        /// <param name="onTimeout">The method that should get executed once the helper timeout gets triggered (can be null).</param>
        /// <returns>Returns a MessageResponseHelper listening for PubSubSubscribeMessage answers.</returns>
        public MessageResponseHelper<IQMessage> requestNodeSubscription(string to, string nodeName, MessageResponseHelper<IQMessage>.OnMessageHandler onMessage, MessageResponseHelper<IQMessage>.OnTimeoutHandler onTimeout)
        {
            MessageResponseHelper<IQMessage> helper = new MessageResponseHelper<IQMessage>(CONNECTION, onMessage, onTimeout);
            PubSubSubscribeMessage msg = new PubSubSubscribeMessage(CONNECTION.account.getFullJid(), CONNECTION.account.getBareJid(), to, nodeName);
            helper.start(msg);
            return helper;
        }

        /// <summary>
        /// Sends a PubSubSubscribeMessage to subscribe to the given node.
        /// https://xmpp.org/extensions/xep-0060.html#subscriber-subscribe
        /// </summary>
        /// <param name="to">The target e.g. 'witches@conference.jabber.org' or 'pubsub.example.org'.</param>
        /// <param name="nodeName">The name of the node, you want to subscribe to.</param>
        /// <returns>Returns a MessageResponseHelperResult listening for DiscoNodeItemsRequestMessage answers.</returns>
        public async Task<MessageResponseHelperResult<IQMessage>> requestNodeSubscriptionAsync(string to, string nodeName)
        {
            AsyncMessageResponseHelper<IQMessage> helper = new AsyncMessageResponseHelper<IQMessage>(CONNECTION);
            PubSubSubscribeMessage msg = new PubSubSubscribeMessage(CONNECTION.account.getFullJid(), CONNECTION.account.getBareJid(), to, nodeName);
            return await helper.startAsync(msg);
        }

        /// <summary>
        /// Sends a PubSubCreateNodeMessage to create a pubsub node.
        /// https://xmpp.org/extensions/xep-0060.html#owner-create
        /// </summary>
        /// <param name="to">The target pubsub server (can be null).</param>
        /// <param name="nodeName">The name of the node, you want to create.</param>
        /// <param name="onMessage">The method that should get executed once the helper receives a new valid message (can be null).</param>
        /// <param name="onTimeout">The method that should get executed once the helper timeout gets triggered (can be null).</param>
        /// <returns>Returns a MessageResponseHelper listening for PubSubCreateNodeMessage answers.</returns>
        public MessageResponseHelper<IQMessage> createNode(string to, string nodeName, MessageResponseHelper<IQMessage>.OnMessageHandler onMessage, MessageResponseHelper<IQMessage>.OnTimeoutHandler onTimeout)
        {
            MessageResponseHelper<IQMessage> helper = new MessageResponseHelper<IQMessage>(CONNECTION, onMessage, onTimeout);
            PubSubCreateNodeMessage msg = new PubSubCreateNodeMessage(CONNECTION.account.getFullJid(), to, nodeName);
            helper.start(msg);
            return helper;
        }

        /// <summary>
        /// Sends a PubSubCreateNodeMessage to create a pubsub node.
        /// https://xmpp.org/extensions/xep-0060.html#owner-create-and-configure
        /// </summary>
        /// <param name="to">The target pubsub server (can be null).</param>
        /// <param name="nodeName">The name of the node, you want to create.</param>
        /// <param name="config">The configuration for the node.</param>
        /// <param name="onMessage">The method that should get executed once the helper receives a new valid message (can be null).</param>
        /// <param name="onTimeout">The method that should get executed once the helper timeout gets triggered (can be null).</param>
        /// <returns>Returns a MessageResponseHelper listening for PubSubCreateNodeMessage answers.</returns>
        public MessageResponseHelper<IQMessage> createNode(string to, string nodeName, DataForm config, MessageResponseHelper<IQMessage>.OnMessageHandler onMessage, MessageResponseHelper<IQMessage>.OnTimeoutHandler onTimeout)
        {
            MessageResponseHelper<IQMessage> helper = new MessageResponseHelper<IQMessage>(CONNECTION, onMessage, onTimeout);
            PubSubCreateNodeMessage msg = new PubSubCreateNodeMessage(CONNECTION.account.getFullJid(), to, nodeName, config);
            helper.start(msg);
            return helper;
        }

        /// <summary>
        /// Sends a PubSubDeleteNodeMessage to delete a pubsub node.
        /// https://xmpp.org/extensions/xep-0060.html#owner-delete
        /// </summary>
        /// <param name="toBareJid">The target pubsub server (can be null).</param>
        /// <param name="nodeName">The name of the node, you want to delete.</param>
        /// <returns>The result of the PubSubDeleteNodeMessage.</returns>
        public async Task<MessageResponseHelperResult<IQMessage>> deleteNodeAsync(string toBareJid, string nodeName)
        {
            AsyncMessageResponseHelper<IQMessage> helper = new AsyncMessageResponseHelper<IQMessage>(CONNECTION);
            PubSubDeleteNodeMessage msg = new PubSubDeleteNodeMessage(CONNECTION.account.getFullJid(), toBareJid, nodeName);
            return await helper.startAsync(msg);
        }

        /// <summary>
        /// Sends a PubSubRequestSubscriptionsMessage to request all subscriptions from a given server.
        /// https://xmpp.org/extensions/xep-0060.html#entity-subscriptions
        /// </summary>
        /// <param name="to">The target pubsub server.</param>
        /// <param name="onMessage">The method that should get executed once the helper receives a new valid message (can be null).</param>
        /// <param name="onTimeout">The method that should get executed once the helper timeout gets triggered (can be null).</param>
        /// <returns>Returns a MessageResponseHelper listening for PubSubRequestSubscriptionsMessage answers.</returns>
        public MessageResponseHelper<IQMessage> requestSubscriptions(string to, MessageResponseHelper<IQMessage>.OnMessageHandler onMessage, MessageResponseHelper<IQMessage>.OnTimeoutHandler onTimeout)
        {
            MessageResponseHelper<IQMessage> helper = new MessageResponseHelper<IQMessage>(CONNECTION, onMessage, onTimeout);
            PubSubRequestSubscriptionsMessage msg = new PubSubRequestSubscriptionsMessage(CONNECTION.account.getFullJid(), to);
            helper.start(msg);
            return helper;
        }

        /// <summary>
        /// Sends a PubSubRequestAffiliationsMessage to request all node affiliations from the given target.
        /// https://xmpp.org/extensions/xep-0060.html#entity-affiliations
        /// </summary>
        /// <param name="to">The target pubsub server.</param>
        /// <param name="onMessage">The method that should get executed once the helper receives a new valid message (can be null).</param>
        /// <param name="onTimeout">The method that should get executed once the helper timeout gets triggered (can be null).</param>
        /// <returns>Returns a MessageResponseHelper listening for PubSubRequestAffiliationsMessage answers.</returns>
        public MessageResponseHelper<IQMessage> requestAffiliations(string to, MessageResponseHelper<IQMessage>.OnMessageHandler onMessage, MessageResponseHelper<IQMessage>.OnTimeoutHandler onTimeout)
        {
            MessageResponseHelper<IQMessage> helper = new MessageResponseHelper<IQMessage>(CONNECTION, onMessage, onTimeout);
            PubSubRequestAffiliationsMessage msg = new PubSubRequestAffiliationsMessage(CONNECTION.account.getFullJid(), to);
            helper.start(msg);
            return helper;
        }

        /// <summary>
        /// Sends a DiscoRequestMessage to the given target to query all PubSub nodes.
        /// https://xmpp.org/extensions/xep-0060.html#entity-nodes
        /// </summary>
        /// <param name="to">The target e.g. 'witches@conference.jabber.org' or 'pubsub.example.org'.</param>
        /// <returns>Returns a MessageResponseHelperResult listening for DiscoRequestMessage answers.</returns>
        public async Task<MessageResponseHelperResult<IQMessage>> discoNodesAsync(string to)
        {
            AsyncMessageResponseHelper<IQMessage> helper = new AsyncMessageResponseHelper<IQMessage>(CONNECTION);
            DiscoRequestMessage msg = new DiscoRequestMessage(CONNECTION.account.getFullJid(), to, DiscoType.ITEMS);
            return await helper.startAsync(msg);
        }

        /// <summary>
        /// Sends a DiscoRequestMessage to the given target to query all PubSub nodes.
        /// https://xmpp.org/extensions/xep-0060.html#entity-discoveritems
        /// </summary>
        /// <param name="to">The target e.g. 'witches@conference.jabber.org' or 'pubsub.example.org'.</param>
        /// <returns>Returns a MessageResponseHelperResult listening for DiscoNodeItemsRequestMessage answers.</returns>
        public async Task<MessageResponseHelperResult<IQMessage>> discoNodesItemsAsync(string to, string nodeName)
        {
            AsyncMessageResponseHelper<IQMessage> helper = new AsyncMessageResponseHelper<IQMessage>(CONNECTION);
            DiscoNodeItemsRequestMessage msg = new DiscoNodeItemsRequestMessage(CONNECTION.account.getFullJid(), to, nodeName);
            return await helper.startAsync(msg);
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
