using Logging;
using System;
using System.Threading.Tasks;
using XMPP_API.Classes.Network;
using XMPP_API.Classes.Network.Events;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.MUC;

namespace XMPP_API.Classes
{
    public class XMPPClient
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private XMPPConnectionHandler connection;

        public delegate void NewRoosterEventHandler(XMPPClient client, NewPresenceEventArgs args);
        public delegate void ConnectionStateChangedEventHandler(XMPPClient client, ConnectionState state);
        public delegate void NewChatMessageEventHandler(XMPPClient client, NewChatMessageEventArgs args);
        public delegate void NewPresenceEventHandler(XMPPClient client, Events.NewPresenceEventArgs args);

        public event NewRoosterEventHandler NewRoosterMessage;
        public event ConnectionStateChangedEventHandler ConnectionStateChanged;
        public event NewChatMessageEventHandler NewChatMessage;
        public event NewPresenceEventHandler NewPresence;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 17/08/2017 Created [Fabian Sauter]
        /// </history>
        public XMPPClient(XMPPAccount sCC)
        {
            connection = new XMPPConnectionHandler(sCC);
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
            return connection != null && connection.getState() == ConnectionState.CONNECTED;
        }

        public async Task<string> setPreseceAsync(string show, string status)
        {
            return await setPreseceAsync(null, null, show, status);
        }

        public async Task<string> setPreseceAsync(string from, string to, string show, string status)
        {
            PresenceMessage presenceMessage = new PresenceMessage(from, to, show, status, connection.getXMPPAccount().presencePriorety);
            await connection.sendMessageAsync(presenceMessage, true);
            return presenceMessage.getId();
        }

        public ConnectionState getConnetionState()
        {
            return connection.getState();
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public async Task transferSocketOwnershipAsync()
        {
            await connection?.transferSocketOwnershipAsync();
        }

        public async Task connectAsync()
        {
            try
            {
                await connection.connectToServerAsync();
            }
            catch (Exception e)
            {
                Logger.Error("Error during connectAsync in XMPPClient!", e);
            }
        }

        public async Task disconnectAsync()
        {
            await connection.disconnectFromServerAsync();
        }

        public async Task<MessageMessage> sendMessageAsync(string to, string msg)
        {
            XMPPAccount sCC = connection.getXMPPAccount();
            MessageMessage sendMessgageMessage = new MessageMessage(sCC.getIdDomainAndResource(), to, msg);
            await connection.sendMessageAsync(sendMessgageMessage, false);
            return sendMessgageMessage;
        }

        public XMPPAccount getXMPPAccount()
        {
            return connection.getXMPPAccount();
        }

        public async Task requestRoosterAsync()
        {
            XMPPAccount sCC = connection.getXMPPAccount();
            await connection.sendMessageAsync(new RosterMessage(sCC.getIdDomainAndResource(), sCC.getIdAndDomain()), false);
        }

        public async Task addToRosterAsync(string jabberId)
        {
            XMPPAccount sCC = connection.getXMPPAccount();
            await connection.sendMessageAsync(new AddToRosterMessage(sCC.getIdDomainAndResource(), jabberId), false);
        }

        public async Task requestPresenceSubscriptionAsync(string jabberId)
        {
            XMPPAccount sCC = connection.getXMPPAccount();
            await connection.sendMessageAsync(new PresenceMessage(sCC.getIdAndDomain(), jabberId, "subscribe"), false);
        }

        public async Task mUCRequestJoinedRoomsAsync()
        {
            XMPPAccount sCC = connection.getXMPPAccount();
            await connection.sendMessageAsync(new MUCRequestJoinedRoomsMessage(sCC.getIdAndDomain()), false);
        }

        public async Task unsubscribeFromPresence(string jabberId)
        {
            XMPPAccount sCC = connection.getXMPPAccount();
            await connection.sendMessageAsync(new PresenceMessage(sCC.getIdAndDomain(), jabberId, "unsubscribe"), false);
        }

        public async Task requestVCardAsync(string jabberId)
        {
            XMPPAccount sCC = connection.getXMPPAccount();
            await connection.sendMessageAsync(new RequestvCardMessage(jabberId, sCC.getIdDomainAndResource()), false);
        }

        public async Task answerPresenceSubscriptionRequest(string jabberId, bool accept)
        {
            XMPPAccount sCC = connection.getXMPPAccount();
            await connection.sendMessageAsync(new PresenceMessage(sCC.getIdAndDomain(), jabberId, accept ? "subscribed" : "unsubscribed"), false);
        }

        public async Task removeFromRosterAsync(string jabberId)
        {
            XMPPAccount sCC = connection.getXMPPAccount();
            await connection.sendMessageAsync(new RemoveFromRosterMessage(sCC.getIdDomainAndResource(), jabberId), false);
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void Connection_ConnectionNewRoosterMessage(XMPPConnectionHandler handler, NewPresenceEventArgs args)
        {
            NewRoosterMessage?.Invoke(this, args);
        }

        private async void Connection_ConnectionStateChanged(AbstractConnectionHandler handler, ConnectionState state)
        {
            ConnectionStateChanged?.Invoke(this, state);
        }

        private void Connection_ConnectionNewValidMessage(XMPPConnectionHandler handler, NewPresenceEventArgs args)
        {
            AbstractMessage msg = args.getMessage();
            if(msg is MessageMessage)
            {
                NewChatMessage?.Invoke(this, new NewChatMessageEventArgs(msg as MessageMessage));
            }
        }

        private void Connection_ConnectionNewPresenceMessage(XMPPConnectionHandler handler, NewPresenceEventArgs args)
        {
            NewPresence?.Invoke(this, new Events.NewPresenceEventArgs(args.getMessage() as PresenceMessage));
        }

        #endregion
    }
}
