using Logging;
using System;
using System.Threading.Tasks;
using XMPP_API.Classes.Network;
using XMPP_API.Classes.Network.Events;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.XEP_0030;
using XMPP_API.Classes.Network.XML.Messages.XEP_0045;
using XMPP_API.Classes.Network.XML.Messages.XEP_0048_1_0;
using XMPP_API.Classes.Network.XML.Messages.XEP_0085;

namespace XMPP_API.Classes
{
    public class XMPPClient
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private XMPPConnection connection;

        public delegate void ConnectionStateChangedEventHandler(XMPPClient client, ConnectionStateChangedEventArgs args);
        public delegate void NewChatMessageEventHandler(XMPPClient client, NewChatMessageEventArgs args);
        public delegate void NewPresenceEventHandler(XMPPClient client, Events.NewPresenceMessageEventArgs args);
        public delegate void NewChatStateEventHandler(XMPPClient client, NewChatStateEventArgs args);
        public delegate void NewDiscoResponseMessageEventHandler(XMPPClient client, NewDiscoResponseMessageEventArgs args);
        public delegate void MessageSendEventHandler(XMPPClient client, MessageSendEventArgs args);
        public delegate void NewBookmarksResultMessageEventHandler(XMPPClient client, NewBookmarksResultMessageEventArgs args);
        public delegate void NewMUCMemberPresenceMessageEventHandler(XMPPClient client, NewMUCMemberPresenceMessageEventArgs args);
        public delegate void NewValidMessageEventHandler(XMPPClient client, NewValidMessageEventArgs args);

        public event NewValidMessageEventHandler NewRoosterMessage;
        public event ConnectionStateChangedEventHandler ConnectionStateChanged;
        public event NewChatMessageEventHandler NewChatMessage;
        public event NewPresenceEventHandler NewPresence;
        public event NewChatStateEventHandler NewChatState;
        public event NewDiscoResponseMessageEventHandler NewDiscoResponseMessage;
        public event MessageSendEventHandler MessageSend;
        public event NewBookmarksResultMessageEventHandler NewBookmarksResultMessage;
        public event NewMUCMemberPresenceMessageEventHandler NewMUCMemberPresenceMessage;
        public event NewValidMessageEventHandler NewValidMessage;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 17/08/2017 Created [Fabian Sauter]
        /// </history>
        public XMPPClient(XMPPAccount account)
        {
            connection = new XMPPConnection(account);
            connection.ConnectionNewRoosterMessage += Connection_ConnectionNewRoosterMessage;
            connection.ConnectionStateChanged += Connection_ConnectionStateChanged;
            connection.ConnectionNewValidMessage += Connection_ConnectionNewValidMessage;
            connection.ConnectionNewPresenceMessage += Connection_ConnectionNewPresenceMessage;
            connection.MessageSend += Connection_MessageSend;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public bool isConnected()
        {
            return connection != null && connection.state == ConnectionState.CONNECTED;
        }

        public async Task<string> setPreseceAsync(Presence presence, string status)
        {
            return await setPreseceAsync(null, null, presence, status);
        }

        public async Task<string> setPreseceAsync(string from, string to, Presence presence, string status)
        {
            PresenceMessage presenceMessage = new PresenceMessage(from, to, presence, status, connection.account.presencePriorety);
            await connection.sendAsync(presenceMessage, false, false);
            return presenceMessage.getId();
        }

        public async Task setBookmarkAsync(ConferenceItem conference)
        {
            await connection.sendAsync(new SetBookmarksMessage(connection.account.getIdDomainAndResource(), conference), true, false);
        }

        public ConnectionState getConnetionState()
        {
            return connection.state;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public async Task transferSocketOwnershipAsync()
        {
            //await connection?.transferSocketOwnershipAsync();
        }

        public async Task connectAsync()
        {
            Logger.Info("Connecting account: " + getXMPPAccount().getIdAndDomain());
            try
            {
                await connection.connectAsync();
            }
            catch (Exception e)
            {
                Logger.Error("Error during connectAsync in XMPPClient!", e);
            }
        }

        public async Task reconnectAsync()
        {
            Logger.Info("Reconnecting account: " + getXMPPAccount().getIdAndDomain());
            await connection.reconnectAsync();
        }

        public async Task disconnectAsync()
        {
            Logger.Info("Disconnecting account: " + getXMPPAccount().getIdAndDomain());
            await connection.disconnectAsync();
        }

        public async Task<MessageMessage> sendAsync(string to, string msg, string chatType)
        {
            return await sendAsync(to, msg, chatType, null);
        }

        public async Task<MessageMessage> sendAsync(string to, string msg, string chatType, string nickname)
        {
            XMPPAccount account = connection.account;
            MessageMessage sendMessgageMessage = new MessageMessage(account.getIdDomainAndResource(), to, msg, chatType, nickname);
            await connection.sendAsync(sendMessgageMessage, true, false);
            return sendMessgageMessage;
        }

        public async Task sendMessageAsync(AbstractMessage msg, bool cacheIfNotConnected)
        {
            await connection.sendAsync(msg, cacheIfNotConnected, false);
        }

        public XMPPAccount getXMPPAccount()
        {
            return connection.account;
        }

        public async Task requestRoosterAsync()
        {
            XMPPAccount account = connection.account;
            await connection.sendAsync(new RosterMessage(account.getIdDomainAndResource(), account.getIdAndDomain()), false, false);
        }

        public async Task addToRosterAsync(string jabberId)
        {
            XMPPAccount account = connection.account;
            await connection.sendAsync(new AddToRosterMessage(account.getIdDomainAndResource(), jabberId), true, false);
        }

        public async Task requestPresenceSubscriptionAsync(string jabberId)
        {
            XMPPAccount account = connection.account;
            await connection.sendAsync(new PresenceMessage(account.getIdAndDomain(), jabberId, "subscribe"), true, false);
        }

        public async Task unsubscribeFromPresence(string jabberId)
        {
            XMPPAccount account = connection.account;
            await connection.sendAsync(new PresenceMessage(account.getIdAndDomain(), jabberId, "unsubscribe"), true, false);
        }

        public async Task requestVCardAsync(string jabberId)
        {
            XMPPAccount account = connection.account;
            await connection.sendAsync(new RequestvCardMessage(jabberId, account.getIdDomainAndResource()), false, false);
        }

        public async Task requestBookmarksAsync()
        {
            XMPPAccount account = connection.account;
            await connection.sendAsync(new RequestBookmarksMessage(account.getIdDomainAndResource()), false, false);
        }

        public async Task answerPresenceSubscriptionRequest(string jabberId, bool accept)
        {
            XMPPAccount account = connection.account;
            await connection.sendAsync(new PresenceMessage(account.getIdAndDomain(), jabberId, accept ? "subscribed" : "unsubscribed"), true, false);
        }

        public async Task removeFromRosterAsync(string jabberId)
        {
            XMPPAccount account = connection.account;
            await connection.sendAsync(new RemoveFromRosterMessage(account.getIdDomainAndResource(), jabberId), true, false);
        }

        public async Task<string> createDiscoAsync(string target, DiscoType type)
        {
            XMPPAccount account = connection.account;
            DiscoRequestMessage disco = new DiscoRequestMessage(account.getIdDomainAndResource(), target, type);
            await connection.sendAsync(disco, false, false);
            return disco.getId();
        }

        public async Task sendChatStateAsync(string target, ChatState state)
        {
            XMPPAccount account = connection.account;
            await connection.sendAsync(new ChatStateMessage(target, account.getIdDomainAndResource(), state), false, false);
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void Connection_ConnectionNewRoosterMessage(XMPPConnection handler, NewValidMessageEventArgs args)
        {
            NewRoosterMessage?.Invoke(this, args);
        }

        private void Connection_ConnectionStateChanged(AbstractConnection connection, ConnectionStateChangedEventArgs args)
        {
            if (args.newState == ConnectionState.CONNECTED)
            {
                Logger.Info("Connected to account: " + getXMPPAccount().getIdAndDomain());
            }
            else if (args.newState == ConnectionState.DISCONNECTED)
            {
                Logger.Info("Disconnected account: " + getXMPPAccount().getIdAndDomain());
            }

            ConnectionStateChanged?.Invoke(this, args);
        }

        private void Connection_ConnectionNewValidMessage(XMPPConnection handler, NewValidMessageEventArgs args)
        {
            AbstractMessage msg = args.getMessage();
            if (msg is MessageMessage)
            {
                NewChatMessage?.Invoke(this, new NewChatMessageEventArgs(msg as MessageMessage));
            }
            else if (msg is ChatStateMessage)
            {
                NewChatState?.Invoke(this, new NewChatStateEventArgs(msg as ChatStateMessage));
            }
            else if (msg is DiscoResponseMessage)
            {
                NewDiscoResponseMessage?.Invoke(this, new NewDiscoResponseMessageEventArgs(msg as DiscoResponseMessage));
            }
            // XEP-0048-1.0 (bookmarks result):
            else if (msg is BookmarksResultMessage)
            {
                NewBookmarksResultMessage?.Invoke(this, new NewBookmarksResultMessageEventArgs(msg as BookmarksResultMessage));
            }

            NewValidMessage?.Invoke(this, args);
        }

        private void Connection_ConnectionNewPresenceMessage(XMPPConnection handler, NewValidMessageEventArgs args)
        {
            // XEP-0045 (MUC member presence):
            if (args.getMessage() is MUCMemberPresenceMessage)
            {
                NewMUCMemberPresenceMessage?.Invoke(this, new NewMUCMemberPresenceMessageEventArgs(args.getMessage() as MUCMemberPresenceMessage));
            }
            else
            {
                NewPresence?.Invoke(this, new Events.NewPresenceMessageEventArgs(args.getMessage() as PresenceMessage));
            }
        }

        private void Connection_MessageSend(XMPPConnection handler, MessageSendEventArgs args)
        {
            MessageSend?.Invoke(this, args);
        }

        #endregion
    }
}
