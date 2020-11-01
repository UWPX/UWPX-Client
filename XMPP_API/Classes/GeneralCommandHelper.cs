using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Logging;
using XMPP_API.Classes.Network;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.Helper;
using XMPP_API.Classes.Network.XML.Messages.XEP_0030;
using XMPP_API.Classes.Network.XML.Messages.XEP_0085;
using XMPP_API.Classes.Network.XML.Messages.XEP_0280;
using XMPP_API.Classes.Network.XML.Messages.XEP_0313;
using XMPP_API.Classes.Network.XML.Messages.XEP_0357;

namespace XMPP_API.Classes
{
    public class GeneralCommandHelper
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly XmppConnection CONNECTION;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public GeneralCommandHelper(XmppConnection connection)
        {
            CONNECTION = connection;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        /// <summary>
        /// Sends a <seealso cref="PresenceMessage"/> to the server with the given presence and status.
        /// </summary>
        /// <param name="presence">The presence that should get send to the server.</param>
        /// <param name="status">The status message that should get send to the server.</param>
        /// <returns>The id of the send <seealso cref="PresenceMessage"/>.</returns>
        public async Task<string> setPreseceAsync(Presence presence, string status)
        {
            return await setPreseceAsync(null, null, presence, status);
        }

        /// <summary>
        /// Sends a <seealso cref="PresenceMessage"/> to the given target with the given presence and status.
        /// </summary>
        /// <param name="from">Who is sending this message? E.g. 'witches@conference.jabber.org'. Can be null.</param>
        /// <param name="to">Who is the target of this message? E.g. 'witches@conference.jabber.org'. Can be null.</param>
        /// <param name="presence">The presence that should get send to the server.</param>
        /// <param name="status">The status message that should get send to the server.</param>
        /// <returns>The id of the send <seealso cref="PresenceMessage"/>.</returns>
        public async Task<string> setPreseceAsync(string from, string to, Presence presence, string status)
        {
            PresenceMessage presenceMessage = new PresenceMessage(from, to, presence, status, int.MinValue);
            await CONNECTION.SendAsync(presenceMessage);
            return presenceMessage.ID;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        /// <summary>
        /// Sends a <seealso cref="RosterRequestMessage"/> to the server and requests the current roster.
        /// </summary>
        /// <returns>The result of the request.</returns>
        public async Task<MessageResponseHelperResult<IQMessage>> requestRosterAsync()
        {
            Predicate<IQMessage> predicate = (x) => { return true; };
            AsyncMessageResponseHelper<IQMessage> helper = new AsyncMessageResponseHelper<IQMessage>(CONNECTION, predicate);
            RosterRequestMessage msg = new RosterRequestMessage(CONNECTION.account.getFullJid(), CONNECTION.account.getBareJid());
            return await helper.startAsync(msg);
        }

        /// <summary>
        /// Sends a <seealso cref="RosterRequestMessage"/> to the server and requests the current roster.
        /// </summary>
        /// <returns>True if sending the message succeeded.</returns>
        public async Task<bool> sendRequestRosterMessageAsync()
        {
            RosterRequestMessage msg = new RosterRequestMessage(CONNECTION.account.getFullJid(), CONNECTION.account.getBareJid());
            return await CONNECTION.SendAsync(msg);
        }

        /// <summary>
        /// Sends a presence probe to the given target.
        /// </summary>
        /// <param name="fromFullJid">Who is sending the presence probe (full JID)?</param>
        /// <param name="toBareJid">Who is the target of the presence probe (bare JID)?</param>
        public async Task sendPresenceProbeAsync(string fromFullJid, string toBareJid)
        {
            PresenceProbeMessage msg = new PresenceProbeMessage(fromFullJid, toBareJid);
            await CONNECTION.SendAsync(msg);
            Logger.Info("Send presence probe from " + fromFullJid + " to " + toBareJid);
        }

        /// <summary>
        /// Sends an <seealso cref="AddToRosterMessage"/> for the given bareJid.
        /// The message will be cached if not connected.
        /// </summary>
        /// <param name="bareJid">The bare JID that should get added to the roster.</param>
        /// <returns>The id of the send <seealso cref="AddToRosterMessage"/>.</returns>
        public async Task<string> addToRosterAsync(string bareJid)
        {
            AddToRosterMessage msg = new AddToRosterMessage(CONNECTION.account.getFullJid(), bareJid);
            await CONNECTION.SendAsync(msg, true);
            return msg.ID;
        }

        /// <summary>
        /// Starts a new Task and sends an <seealso cref="RemoveFromRosterMessage"/> for the given bareJid.
        /// The message will be cached if not connected.
        /// </summary>
        /// <param name="bareJid">The bare JID that should get removed from the roster.</param>
        /// <returns>The id of the send <seealso cref="RemoveFromRosterMessage"/>.</returns>
        public async Task<string> removeFromRosterAsync(string bareJid)
        {
            RemoveFromRosterMessage msg = new RemoveFromRosterMessage(CONNECTION.account.getFullJid(), bareJid);
            await CONNECTION.SendAsync(msg, true);
            return msg.ID;
        }

        /// <summary>
        /// Sends a <seealso cref="ChatStateMessage"/> to the given target.
        /// </summary>
        /// <param name="target">Who is the target of this message? E.g. 'witches@conference.jabber.org'.</param>
        /// <param name="state">The chat state.</param>
        public async Task sendChatStateAsync(string target, ChatState state)
        {
            ChatStateMessage chatStateMessage = new ChatStateMessage(target, CONNECTION.account.getFullJid(), state);
            await CONNECTION.SendAsync(chatStateMessage);
        }

        /// <summary>
        /// Sends a <seealso cref="DiscoRequestMessage"/> to the given target.
        /// </summary>
        /// <param name="target">The target e.g. 'witches@conference.jabber.org'.</param>
        /// <param name="type">The disco type</param>
        /// <param name="onMessage">The method that should get executed once the helper receives a new valid message.</param>
        /// <param name="onTimeout">The method that should get executed once the helper timeout gets triggered.</param>
        /// <returns>Returns a <seealso cref="MessageResponseHelperResult"/> listening for <seealso cref="DiscoRequestMessage"/> answers.</returns>
        public MessageResponseHelper<IQMessage> createDisco(string target, DiscoType type, MessageResponseHelper<IQMessage>.OnMessageHandler onMessage, MessageResponseHelper<IQMessage>.OnTimeoutHandler onTimeout)
        {
            MessageResponseHelper<IQMessage> helper = new MessageResponseHelper<IQMessage>(CONNECTION, onMessage, onTimeout);
            DiscoRequestMessage disco = new DiscoRequestMessage(CONNECTION.account.getFullJid(), target, type);
            helper.start(disco);
            return helper;
        }

        /// <summary>
        /// Sends a <seealso cref="DiscoRequestMessage"/> to the given target.
        /// </summary>
        /// <param name="target">The target e.g. 'witches@conference.jabber.org'.</param>
        /// <param name="type">The disco type</param>
        /// <returns>Returns a <seealso cref="MessageResponseHelperResult"/> listening for <seealso cref="DiscoRequestMessage"/> answers.</returns>
        public async Task<MessageResponseHelperResult<IQMessage>> discoAsync(string target, DiscoType type)
        {
            Predicate<IQMessage> predicate = (x) => { return true; };
            AsyncMessageResponseHelper<IQMessage> helper = new AsyncMessageResponseHelper<IQMessage>(CONNECTION, predicate);
            DiscoRequestMessage msg = new DiscoRequestMessage(CONNECTION.account.getFullJid(), target, type);
            return await helper.startAsync(msg);
        }

        /// <summary>
        /// Sends a PresenceMessage to request a presence subscription from the given bare JID.
        /// </summary>
        /// <param name="bareJid">The bare JID of the target e.g. 'witches@conference.jabber.org'.</param>
        public async Task requestPresenceSubscriptionAsync(string bareJid)
        {
            PresenceMessage msg = new PresenceMessage(CONNECTION.account.getBareJid(), bareJid, "subscribe");
            await CONNECTION.SendAsync(msg, true);
        }

        /// <summary>
        /// Sends a <seealso cref="PresenceMessage"/> to unsubscribe form the given bare JID.
        /// </summary>
        /// <param name="bareJid">The bare JID of the target e.g. 'witches@conference.jabber.org'.</param>
        public async Task unsubscribeFromPresenceAsync(string bareJid)
        {
            PresenceMessage msg = new PresenceMessage(CONNECTION.account.getBareJid(), bareJid, "unsubscribe");
            await CONNECTION.SendAsync(msg, true);
        }

        /// <summary>
        /// Sends a <seealso cref="PresenceMessage"/> to answer an presence subscription request from the given bare JID.
        /// </summary>
        /// <param name="bareJid">The bare JID of the target e.g. 'witches@conference.jabber.org'.</param>
        /// <param name="accept">Whether the request was accepted or not.</param>
        public async Task answerPresenceSubscriptionRequestAsync(string bareJid, bool accept)
        {
            PresenceMessage msg = new PresenceMessage(CONNECTION.account.getBareJid(), bareJid, accept ? "subscribed" : "unsubscribed");
            await CONNECTION.SendAsync(msg, true);
        }

        /// <summary>
        /// Sends a "XEP-0357: Push Notifications" request to enable push notifications.
        /// </summary>
        /// <param name="pushServerBareJid">The bare JID of the push server e.g. 'push@xmpp.example.com'.</param>
        /// <param name="node">The "XEP-0060: Publish-Subscribe" node where the server should publish notifications to.</param>
        /// <param name="secret">The authentication secret for the "XEP-0060: Publish-Subscribe" node, where the server should publish notifications to.</param>
        /// <returns>Returns a <seealso cref="MessageResponseHelperResult"/> listening for <seealso cref="EnablePushNotificationsMessage"/> responses.</returns>
        public async Task<MessageResponseHelperResult<IQMessage>> enablePushNotificationsAsync(string pushServerBareJid, string node, string secret)
        {
            Predicate<IQMessage> predicate = (x) => { return true; };
            AsyncMessageResponseHelper<IQMessage> helper = new AsyncMessageResponseHelper<IQMessage>(CONNECTION, predicate);
            EnablePushNotificationsMessage msg = new EnablePushNotificationsMessage(pushServerBareJid, node, secret);
            return await helper.startAsync(msg);
        }

        /// <summary>
        /// Sends a "XEP-0357: Push Notifications" request to disable push notifications.
        /// </summary>
        /// <param name="pushServerBareJid">The bare JID of the push server.</param>
        /// <returns>Returns a <seealso cref="MessageResponseHelperResult"/> listening for <seealso cref="DisablePushNotificationsMessage"/> responses.</returns>
        public async Task<MessageResponseHelperResult<IQMessage>> disablePushNotificationsAsync(string pushServerBareJid)
        {
            Predicate<IQMessage> predicate = (x) => { return true; };
            AsyncMessageResponseHelper<IQMessage> helper = new AsyncMessageResponseHelper<IQMessage>(CONNECTION, predicate);
            DisablePushNotificationsMessage msg = new DisablePushNotificationsMessage(pushServerBareJid);
            return await helper.startAsync(msg);
        }

        /// <summary>
        /// Sends a "XEP-0280: Message Carbons" request to enable message carbons.
        /// </summary>
        /// <returns>Returns a <seealso cref="MessageResponseHelperResult"/> listening for <seealso cref="CarbonsEnableMessage"/> responses.</returns>
        public async Task<MessageResponseHelperResult<IQMessage>> enableMessageCarbonsAsync()
        {
            Predicate<IQMessage> predicate = (x) => { return true; };
            AsyncMessageResponseHelper<IQMessage> helper = new AsyncMessageResponseHelper<IQMessage>(CONNECTION, predicate);
            CarbonsEnableMessage msg = new CarbonsEnableMessage(CONNECTION.account.getFullJid());
            return await helper.startAsync(msg);
        }

        /// <summary>
        /// Sends a "XEP-0280: Message Carbons" request to enable message carbons.
        /// </summary>
        /// <returns>Returns a <seealso cref="MessageResponseHelperResult"/> listening for <seealso cref="CarbonsDisableMessage"/> responses.</returns>
        public async Task<MessageResponseHelperResult<IQMessage>> disableMessageCarbonsAsync()
        {
            Predicate<IQMessage> predicate = (x) => { return true; };
            AsyncMessageResponseHelper<IQMessage> helper = new AsyncMessageResponseHelper<IQMessage>(CONNECTION, predicate);
            CarbonsDisableMessage msg = new CarbonsDisableMessage(CONNECTION.account.getFullJid());
            return await helper.startAsync(msg);
        }

        /// <summary>
        /// Sends a <seealso cref="QueryArchiveMessage"/> to the server and requests the MAM archive.
        /// </summary>
        /// <param name="filter">A filter for filtering the MAM results like filtering by JID.</param>
        /// <returns>The result of the request.</returns>
        public async Task<MessageResponseHelperResult<MamResult>> requestMamAsync(QueryFilter filter) { return await requestMamAsync(filter, null); }

        /// <summary>
        /// Sends a <seealso cref="QueryArchiveMessage"/> to the server and requests the MAM archive.
        /// </summary>
        /// <param name="filter">A filter for filtering the MAM results like filtering by JID.</param>
        /// <param name="to">The target of the request. null for request to your own server. Used for requesting MUC-MAMs.</param>
        /// <returns>The result of the request.</returns>
        public async Task<MessageResponseHelperResult<MamResult>> requestMamAsync(QueryFilter filter, string to)
        {
            QueryArchiveMessage msg = new QueryArchiveMessage(filter, CONNECTION.account.getFullJid(), to);
            List<QueryArchiveResultMessage> results = new List<QueryArchiveResultMessage>();
            Predicate<AbstractAddressableMessage> predicate = (x) =>
            {
                if (x is QueryArchiveResultMessage result && string.Equals(result.QUERY_ID, msg.QUERY_ID))
                {
                    results.Insert(0, result);
                    return false;
                }
                return x is QueryArchiveFinishMessage fin && string.Equals(fin.ID, msg.ID) && string.Equals(fin.QUERY_ID, msg.QUERY_ID);
            };
            AsyncMessageResponseHelper<AbstractAddressableMessage> helper = new AsyncMessageResponseHelper<AbstractAddressableMessage>(CONNECTION, predicate)
            {
                matchId = false
            };
            MessageResponseHelperResult<AbstractAddressableMessage> finResult = await helper.startAsync(msg);
            MamResult mamResult = null;
            if (finResult.STATE == MessageResponseHelperResultState.SUCCESS)
            {
                mamResult = new MamResult(finResult.RESULT as QueryArchiveFinishMessage, results);
            }
            return new MessageResponseHelperResult<MamResult>(finResult.STATE, mamResult);
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
