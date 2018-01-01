using Logging;
using System;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using XMPP_API.Classes.Network;
using XMPP_API.Classes.Network.Events;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.MUC;
using XMPP_API.Classes.Network.XML.Messages.XEP_0030;
using XMPP_API.Classes.Network.XML.Messages.XEP_0048;
using XMPP_API.Classes.Network.XML.Messages.XEP_0085;

namespace XMPP_API.Classes
{
    public class XMPPClient
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private XMPPConnection connection;

        public delegate void NewRoosterEventHandler(XMPPClient client, NewValidMessageEventArgs args);
        public delegate void ConnectionStateChangedEventHandler(XMPPClient client, ConnectionStateChangedEventArgs args);
        public delegate void NewChatMessageEventHandler(XMPPClient client, NewChatMessageEventArgs args);
        public delegate void NewPresenceEventHandler(XMPPClient client, Events.NewPresenceMessageEventArgs args);
        public delegate void NewChatStateEventHandler(XMPPClient client, NewChatStateEventArgs args);
        public delegate void NewDiscoResponseMessageEventHandler(XMPPClient client, NewDiscoResponseMessageEventArgs args);

        public event NewRoosterEventHandler NewRoosterMessage;
        public event ConnectionStateChangedEventHandler ConnectionStateChanged;
        public event NewChatMessageEventHandler NewChatMessage;
        public event NewPresenceEventHandler NewPresence;
        public event NewChatStateEventHandler NewChatState;
        public event NewDiscoResponseMessageEventHandler NewDiscoResponseMessage;

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
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public bool isConnected()
        {
            return connection != null && connection.state == ConnectionState.CONNECTED;
        }

        public async Task<string> setPreseceAsync(string show, string status)
        {
            return await setPreseceAsync(null, null, show, status);
        }

        public async Task<string> setPreseceAsync(string from, string to, string show, string status)
        {
            PresenceMessage presenceMessage = new PresenceMessage(from, to, show, status, connection.account.presencePriorety);
            await connection.sendAsync(presenceMessage, true);
            return presenceMessage.getId();
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
            try
            {
                await connection.connectAsync();
            }
            catch (Exception e)
            {
                Logger.Error("Error during connectAsync in XMPPClient!", e);
            }
        }

        public async Task disconnectAsync()
        {
            await connection.disconnectAsync();
        }

        public async Task<MessageMessage> sendAsync(string to, string msg)
        {
            XMPPAccount account = connection.account;
            MessageMessage sendMessgageMessage = new MessageMessage(account.getIdDomainAndResource(), to, msg);
            await connection.sendAsync(sendMessgageMessage, false);
            return sendMessgageMessage;
        }

        public XMPPAccount getXMPPAccount()
        {
            return connection.account;
        }

        public async Task requestRoosterAsync()
        {
            XMPPAccount account = connection.account;
            await connection.sendAsync(new RosterMessage(account.getIdDomainAndResource(), account.getIdAndDomain()), false);
        }

        public async Task addToRosterAsync(string jabberId)
        {
            XMPPAccount account = connection.account;
            await connection.sendAsync(new AddToRosterMessage(account.getIdDomainAndResource(), jabberId), false);
        }

        public async Task requestPresenceSubscriptionAsync(string jabberId)
        {
            XMPPAccount account = connection.account;
            await connection.sendAsync(new PresenceMessage(account.getIdAndDomain(), jabberId, "subscribe"), false);
        }

        public async Task mUCRequestJoinedRoomsAsync()
        {
            XMPPAccount account = connection.account;
            await connection.sendAsync(new MUCRequestJoinedRoomsMessage(account.getIdAndDomain()), false);
        }

        public async Task unsubscribeFromPresence(string jabberId)
        {
            XMPPAccount account = connection.account;
            await connection.sendAsync(new PresenceMessage(account.getIdAndDomain(), jabberId, "unsubscribe"), false);
        }

        public async Task requestVCardAsync(string jabberId)
        {
            XMPPAccount account = connection.account;
            await connection.sendAsync(new RequestvCardMessage(jabberId, account.getIdDomainAndResource()), false);
        }

        public async Task requestBookmarksAsync()
        {
            XMPPAccount account = connection.account;
            await connection.sendAsync(new RequestBookmarksMessage(account.getIdDomainAndResource()), false);
        }

        public async Task answerPresenceSubscriptionRequest(string jabberId, bool accept)
        {
            XMPPAccount account = connection.account;
            await connection.sendAsync(new PresenceMessage(account.getIdAndDomain(), jabberId, accept ? "subscribed" : "unsubscribed"), false);
        }

        public async Task removeFromRosterAsync(string jabberId)
        {
            XMPPAccount account = connection.account;
            await connection.sendAsync(new RemoveFromRosterMessage(account.getIdDomainAndResource(), jabberId), false);
        }

        public async Task<string> createDiscoAsync(string target, DiscoType type)
        {
            XMPPAccount account = connection.account;
            DiscoRequestMessage disco = new DiscoRequestMessage(account.getIdDomainAndResource(), target, type);
            await connection.sendAsync(disco, false);
            return disco.getId();
        }

        public async Task sendChatStateAsync(string target, ChatState state)
        {
            XMPPAccount account = connection.account;
            await connection.sendAsync(new ChatStateMessage(target, account.getIdDomainAndResource(), state), false);
        }

        public async Task sendAsync(AbstractMessage msg)
        {
            await connection.sendAsync(msg, false);
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
        }

        private void Connection_ConnectionNewPresenceMessage(XMPPConnection handler, NewValidMessageEventArgs args)
        {
            NewPresence?.Invoke(this, new Events.NewPresenceMessageEventArgs(args.getMessage() as PresenceMessage));
        }

        #endregion
    }
}
