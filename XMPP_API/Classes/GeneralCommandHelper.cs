using Logging;
using System;
using System.Threading.Tasks;
using XMPP_API.Classes.Network;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.Helper;
using XMPP_API.Classes.Network.XML.Messages.XEP_0030;
using XMPP_API.Classes.Network.XML.Messages.XEP_0085;

namespace XMPP_API.Classes
{
    public class GeneralCommandHelper
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
        /// 02/11/2018 Created [Fabian Sauter]
        /// </history>
        public GeneralCommandHelper(XMPPConnection2 connection)
        {
            CONNECTION = connection;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        /// <summary>
        /// Sends a PresenceMessage to the server with the given presence and status.
        /// </summary>
        /// <param name="presence">The presence that should get send to the server.</param>
        /// <param name="status">The status message that should get send to the server.</param>
        /// <returns>The id of the send PresenceMessage.</returns>
        public async Task<string> setPreseceAsync(Presence presence, string status)
        {
            return await setPreseceAsync(null, null, presence, status);
        }

        /// <summary>
        /// Sends a PresenceMessage to the given target with the given presence and status.
        /// </summary>
        /// <param name="from">Who is sending this message? E.g. 'witches@conference.jabber.org'. Can be null.</param>
        /// <param name="to">Who is the target of this message? E.g. 'witches@conference.jabber.org'. Can be null.</param>
        /// <param name="presence">The presence that should get send to the server.</param>
        /// <param name="status">The status message that should get send to the server.</param>
        /// /// <returns>The id of the send PresenceMessage.</returns>
        public async Task<string> setPreseceAsync(string from, string to, Presence presence, string status)
        {
            PresenceMessage presenceMessage = new PresenceMessage(from, to, presence, status, int.MinValue);
            await CONNECTION.sendAsync(presenceMessage);
            return presenceMessage.ID;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        /// <summary>
        /// Sends a RosterRequestMessage to the server and requests the current roster.
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
        /// Sends a RosterRequestMessage to the server and requests the current roster.
        /// </summary>
        /// <returns>True if sending the message succeeded.</returns>
        public async Task<bool> sendRequestRosterMessageAsync()
        {
            RosterRequestMessage msg = new RosterRequestMessage(CONNECTION.account.getFullJid(), CONNECTION.account.getBareJid());
            return await CONNECTION.sendAsync(msg);
        }

        /// <summary>
        /// Sends a presence probe to the given target.
        /// </summary>
        /// <param name="fromFullJid">Who is sending the presence probe (full JID)?</param>
        /// <param name="toBareJid">Who is the target of the presence probe (bare JID)?</param>
        public async Task sendPresenceProbeAsync(string fromFullJid, string toBareJid)
        {
            PresenceProbeMessage msg = new PresenceProbeMessage(fromFullJid, toBareJid);
            await CONNECTION.sendAsync(msg);
            Logger.Info("Send presence probe from " + fromFullJid + " to " + toBareJid);
        }

        /// <summary>
        /// Sends an AddToRosterMessage for the given bareJid.
        /// The message will be cached if not connected.
        /// </summary>
        /// <param name="bareJid">The bare JID that should get added to the roster.</param>
        /// <returns>The id of the send AddToRosterMessage.</returns>
        public async Task<string> addToRosterAsync(string bareJid)
        {
            AddToRosterMessage msg = new AddToRosterMessage(CONNECTION.account.getFullJid(), bareJid);
            await CONNECTION.sendAsync(msg, true);
            return msg.ID;
        }

        /// <summary>
        /// Starts a new Task and sends an RemoveFromRosterMessage for the given bareJid.
        /// The message will be cached if not connected.
        /// </summary>
        /// <param name="bareJid">The bare JID that should get removed from the roster.</param>
        /// <returns>The id of the send RemoveFromRosterMessage.</returns>
        public async Task<string> removeFromRosterAsync(string bareJid)
        {
            RemoveFromRosterMessage msg = new RemoveFromRosterMessage(CONNECTION.account.getFullJid(), bareJid);
            await CONNECTION.sendAsync(msg, true);
            return msg.ID;
        }

        /// <summary>
        /// Sends a ChatStateMessage to the given target.
        /// </summary>
        /// <param name="target">Who is the target of this message? E.g. 'witches@conference.jabber.org'.</param>
        /// <param name="state">The chat state.</param>
        public async Task sendChatStateAsync(string target, ChatState state)
        {
            ChatStateMessage chatStateMessage = new ChatStateMessage(target, CONNECTION.account.getFullJid(), state);
            await CONNECTION.sendAsync(chatStateMessage);
        }

        /// <summary>
        /// Sends a DiscoRequestMessage to the given target.
        /// </summary>
        /// <param name="target">The target e.g. 'witches@conference.jabber.org'.</param>
        /// <param name="type">The disco type</param>
        /// <param name="onMessage">The method that should get executed once the helper receives a new valid message.</param>
        /// <param name="onTimeout">The method that should get executed once the helper timeout gets triggered.</param>
        /// <returns>Returns a MessageResponseHelper listening for DiscoRequestMessage answers.</returns>
        public MessageResponseHelper<IQMessage> createDisco(string target, DiscoType type, MessageResponseHelper<IQMessage>.OnMessageHandler onMessage, MessageResponseHelper<IQMessage>.OnTimeoutHandler onTimeout)
        {
            MessageResponseHelper<IQMessage> helper = new MessageResponseHelper<IQMessage>(CONNECTION, onMessage, onTimeout);
            DiscoRequestMessage disco = new DiscoRequestMessage(CONNECTION.account.getFullJid(), target, type);
            helper.start(disco);
            return helper;
        }

        /// <summary>
        /// Sends a PresenceMessage to request a presence subscription from the given bare JID.
        /// </summary>
        /// <param name="bareJid">The bare JID of the target e.g. 'witches@conference.jabber.org'.</param>
        public async Task requestPresenceSubscriptionAsync(string bareJid)
        {
            PresenceMessage msg = new PresenceMessage(CONNECTION.account.getBareJid(), bareJid, "subscribe");
            await CONNECTION.sendAsync(msg, true);
        }

        /// <summary>
        /// Sends a PresenceMessage to unsubscribe form the given bare Jid.
        /// </summary>
        /// <param name="bareJid">The bare JID of the target e.g. 'witches@conference.jabber.org'.</param>
        public async Task unsubscribeFromPresenceAsync(string bareJid)
        {
            PresenceMessage msg = new PresenceMessage(CONNECTION.account.getBareJid(), bareJid, "unsubscribe");
            await CONNECTION.sendAsync(msg, true);
        }

        /// <summary>
        /// Sends a PresenceMessage to answer an presence subscription request from the given bare JID.
        /// </summary>
        /// <param name="bareJid">The bare JID of the target e.g. 'witches@conference.jabber.org'.</param>
        /// <param name="accept">Whether the request was accepted or not.</param>
        public async Task answerPresenceSubscriptionRequestAsync(string bareJid, bool accept)
        {
            PresenceMessage msg = new PresenceMessage(CONNECTION.account.getBareJid(), bareJid, accept ? "subscribed" : "unsubscribed");
            await CONNECTION.sendAsync(msg, true);
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
