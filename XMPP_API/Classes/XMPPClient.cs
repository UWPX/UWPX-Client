using Logging;
using System;
using System.Threading.Tasks;
using XMPP_API.Classes.Network;
using XMPP_API.Classes.Network.Events;
using XMPP_API.Classes.Network.XML;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.XEP_0030;
using XMPP_API.Classes.Network.XML.Messages.XEP_0045;
using XMPP_API.Classes.Network.XML.Messages.XEP_0048;
using XMPP_API.Classes.Network.XML.Messages.XEP_0054;
using XMPP_API.Classes.Network.XML.Messages.XEP_0085;
using XMPP_API.Classes.Network.XML.Messages.XEP_0184;
using XMPP_API.Classes.Network.XML.Messages.XEP_0280;

namespace XMPP_API.Classes
{
    public class XMPPClient
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private XMPPConnection2 connection;
        private MessageResponseHelper<IQMessage> discoMessageResponseHelper;
        private MessageResponseHelper<IQMessage> carbonsMessageResponseHelper;

        public delegate void ConnectionStateChangedEventHandler(XMPPClient client, ConnectionStateChangedEventArgs args);
        public delegate void NewChatMessageEventHandler(XMPPClient client, NewChatMessageEventArgs args);
        public delegate void NewPresenceEventHandler(XMPPClient client, Events.NewPresenceMessageEventArgs args);
        public delegate void NewChatStateEventHandler(XMPPClient client, NewChatStateEventArgs args);
        public delegate void NewDiscoResponseMessageEventHandler(XMPPClient client, NewDiscoResponseMessageEventArgs args);
        public delegate void MessageSendEventHandler(XMPPClient client, MessageSendEventArgs args);
        public delegate void NewBookmarksResultMessageEventHandler(XMPPClient client, NewBookmarksResultMessageEventArgs args);
        public delegate void NewMUCMemberPresenceMessageEventHandler(XMPPClient client, NewMUCMemberPresenceMessageEventArgs args);
        public delegate void NewValidMessageEventHandler(XMPPClient client, NewValidMessageEventArgs args);
        public delegate void NewDeliveryReceiptHandler(XMPPClient client, NewDeliveryReceiptEventArgs args);

        public event NewValidMessageEventHandler NewRoosterMessage;
        public event ConnectionStateChangedEventHandler ConnectionStateChanged;
        public event NewChatMessageEventHandler NewChatMessage;
        public event NewPresenceEventHandler NewPresence;
        public event NewChatStateEventHandler NewChatState;
        public event NewDiscoResponseMessageEventHandler NewDiscoResponseMessage;
        public event MessageSendEventHandler MessageSend;
        public event NewMUCMemberPresenceMessageEventHandler NewMUCMemberPresenceMessage;
        public event NewValidMessageEventHandler NewValidMessage;
        public event NewBookmarksResultMessageEventHandler NewBookmarksResultMessage;
        public event NewDeliveryReceiptHandler NewDeliveryReceipt;

        public readonly MUCCommandHelper MUC_COMMAND_HELPER;
        public readonly PubSubCommandHelper PUB_SUB_COMMAND_HELPER;
        public readonly OmemoCommandHelper OMEMO_COMMAND_HELPER;

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
            this.MUC_COMMAND_HELPER = new MUCCommandHelper(this);
            this.PUB_SUB_COMMAND_HELPER = new PubSubCommandHelper(this);
            this.OMEMO_COMMAND_HELPER = new OmemoCommandHelper(this);
            this.discoMessageResponseHelper = null;
            this.carbonsMessageResponseHelper = null;
            initConnection(account);
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
            PresenceMessage presenceMessage = new PresenceMessage(from, to, presence, status, int.MinValue);
            await connection.sendAsync(presenceMessage, false, false);
            return presenceMessage.ID;
        }

        public ConnectionState getConnetionState()
        {
            return connection.state;
        }

        public MessageParserStats getMessageParserStats()
        {
            return connection?.getMessageParserStats();
        }

        public ConnectionError getLastConnectionError()
        {
            return connection.lastConnectionError;
        }

        /// <summary>
        /// Sets the given XMPPAccount.
        /// Make sure you call disconnectAsyc() before to prevent memory leaks!
        /// </summary>
        /// <param name="account">The new XMPPAccount.</param>
        public void setAccount(XMPPAccount account)
        {
            // Cleanup old connection:
            if (connection != null)
            {
                switch (connection.state)
                {
                    case ConnectionState.CONNECTING:
                    case ConnectionState.CONNECTED:
                        throw new InvalidOperationException("Unable to set account, if the client is still connecting or connected! state = " + connection.state);
                }
                connection.ConnectionNewRoosterMessage -= Connection_ConnectionNewRoosterMessage;
                connection.ConnectionStateChanged -= Connection_ConnectionStateChanged;
                connection.ConnectionNewValidMessage -= Connection_ConnectionNewValidMessage;
                connection.ConnectionNewPresenceMessage -= Connection_ConnectionNewPresenceMessage;
                connection.MessageSend -= Connection_MessageSend;
                connection.NewBookmarksResultMessage -= Connection_NewBookmarksResultMessage;
            }

            initConnection(account);
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public async Task transferSocketOwnershipAsync()
        {
            //await connection?.transferSocketOwnershipAsync();
        }

        public void connect()
        {
            Logger.Info("Connecting account: " + getXMPPAccount().getIdAndDomain());
            try
            {
                connection.connectAndHold();
            }
            catch (Exception e)
            {
                Logger.Error("Error during connectAsync in XMPPClient!", e);
            }
        }

        public async Task reconnectAsync()
        {
            Logger.Info("Reconnecting account: " + getXMPPAccount().getIdAndDomain());
            await connection.reconnectAsync(true);
        }

        public async Task disconnectAsync()
        {
            Logger.Info("Disconnecting account: " + getXMPPAccount().getIdAndDomain());
            await connection.disconnectAsyncs();
        }

        public async Task sendAsync(MessageMessage msg)
        {
            await connection.sendAsync(msg, true, false);
        }

        public async Task sendOmemoEncrypted(MessageMessage msg)
        {
            await connection.sendOmemoEncrypted(msg);
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
            await connection.sendAsync(new RequestVCardMessage(jabberId, account.getIdDomainAndResource()), false, false);
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
            return disco.ID;
        }

        public async Task sendChatStateAsync(string target, ChatState state)
        {
            XMPPAccount account = connection.account;
            await connection.sendAsync(new ChatStateMessage(target, account.getIdDomainAndResource(), state), false, false);
        }

        #endregion

        #region --Misc Methods (Private)--
        private void initConnection(XMPPAccount account)
        {
            connection = new XMPPConnection2(account);
            connection.ConnectionNewRoosterMessage += Connection_ConnectionNewRoosterMessage;
            connection.ConnectionStateChanged += Connection_ConnectionStateChanged;
            connection.ConnectionNewValidMessage += Connection_ConnectionNewValidMessage;
            connection.ConnectionNewPresenceMessage += Connection_ConnectionNewPresenceMessage;
            connection.MessageSend += Connection_MessageSend;
            connection.NewBookmarksResultMessage += Connection_NewBookmarksResultMessage;
        }

        private void requestDisoInfo()
        {
            connection.account.CONNECTION_INFO.msgCarbonsState = MessageCarbonsState.DISABLED;
            if (discoMessageResponseHelper != null)
            {
                discoMessageResponseHelper.Dispose();
            }
            discoMessageResponseHelper = new MessageResponseHelper<IQMessage>(this, onDiscoMessage, onDiscoTimeout);
            discoMessageResponseHelper.start(new DiscoRequestMessage(connection.account.getIdDomainAndResource(), connection.account.user.domain, DiscoType.INFO));
        }

        private bool onDiscoMessage(IQMessage msg)
        {
            if (msg is DiscoResponseMessage disco)
            {
                switch (disco.DISCO_TYPE)
                {
                    case DiscoType.ITEMS:
                        break;

                    case DiscoType.INFO:
                        bool foundCarbons = false;
                        foreach (DiscoFeature f in disco.FEATURES)
                        {
                            if (string.Equals(f.VAR, Consts.XML_XEP_0280_NAMESPACE))
                            {
                                foundCarbons = true;
                                if (connection.account.connectionConfiguration.disableMessageCarbons)
                                {
                                    connection.account.CONNECTION_INFO.msgCarbonsState = MessageCarbonsState.DISABLED;
                                }
                                else if (connection.account.CONNECTION_INFO.msgCarbonsState != MessageCarbonsState.ENABLED)
                                {
                                    carbonsMessageResponseHelper = new MessageResponseHelper<IQMessage>(this, onCarbonsMessage, onCarbonsTimeout);
                                    carbonsMessageResponseHelper.start(new CarbonsEnableMessage(connection.account.getIdDomainAndResource()));
                                }
                                break;
                            }
                        }

                        if (!foundCarbons)
                        {
                            connection.account.CONNECTION_INFO.msgCarbonsState = MessageCarbonsState.UNAVAILABLE;
                        }
                        break;
                }
                return true;
            }
            else if (msg is IQErrorMessage errMsg)
            {
                connection.account.CONNECTION_INFO.msgCarbonsState = MessageCarbonsState.UNAVAILABLE;
                Logger.Error("Failed to request initial server disco: " + errMsg.ERROR_OBJ.ToString());
                return true;
            }
            return false;
        }

        private void onDiscoTimeout()
        {
            Logger.Error("Failed to request initial server disco - timeout!");
        }

        private bool onCarbonsMessage(IQMessage msg)
        {
            if (msg is IQErrorMessage errMsg)
            {
                connection.account.CONNECTION_INFO.msgCarbonsState = MessageCarbonsState.ERROR;
                Logger.Error("Failed to enable message carbons: " + errMsg.ERROR_OBJ.ToString());
                return true;
            }
            else if (string.Equals(msg.TYPE, IQMessage.RESULT))
            {
                connection.account.CONNECTION_INFO.msgCarbonsState = MessageCarbonsState.ENABLED;
                return true;
            }
            return false;
        }

        private void onCarbonsTimeout()
        {
            connection.account.CONNECTION_INFO.msgCarbonsState = MessageCarbonsState.ERROR;
            Logger.Error("Failed to enable message carbons - timeout!");
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void Connection_ConnectionNewRoosterMessage(XMPPConnection2 connection, NewValidMessageEventArgs args)
        {
            NewRoosterMessage?.Invoke(this, args);
        }

        private void Connection_ConnectionStateChanged(AbstractConnection2 connection, ConnectionStateChangedEventArgs args)
        {
            if (args.newState == ConnectionState.CONNECTED)
            {
                Logger.Info("Connected to account: " + getXMPPAccount().getIdAndDomain());
                requestDisoInfo();
            }
            else if (args.newState == ConnectionState.DISCONNECTED)
            {
                // Stop message processors:
                if (discoMessageResponseHelper != null)
                {
                    discoMessageResponseHelper.Dispose();
                    discoMessageResponseHelper = null;
                }
                if (carbonsMessageResponseHelper != null)
                {
                    carbonsMessageResponseHelper.Dispose();
                    carbonsMessageResponseHelper = null;
                }
                Logger.Info("Disconnected account: " + getXMPPAccount().getIdAndDomain());
            }

            ConnectionStateChanged?.Invoke(this, args);
        }

        private void Connection_ConnectionNewValidMessage(XMPPConnection2 connection, NewValidMessageEventArgs args)
        {
            AbstractMessage msg = args.MESSAGE;
            if (msg is MessageMessage mMsg)
            {
                NewChatMessage?.Invoke(this, new NewChatMessageEventArgs(mMsg));
            }
            else if (msg is ChatStateMessage sMsg)
            {
                NewChatState?.Invoke(this, new NewChatStateEventArgs(sMsg));
            }
            else if (msg is DiscoResponseMessage dMsg)
            {
                NewDiscoResponseMessage?.Invoke(this, new NewDiscoResponseMessageEventArgs(dMsg));
            }
            else if (msg is DeliveryReceiptMessage dRMsg)
            {
                NewDeliveryReceipt?.Invoke(this, new NewDeliveryReceiptEventArgs(dRMsg));
            }

            NewValidMessage?.Invoke(this, args);
        }

        private void Connection_ConnectionNewPresenceMessage(XMPPConnection2 connection, NewValidMessageEventArgs args)
        {
            // XEP-0045 (MUC member presence):
            if (args.MESSAGE is MUCMemberPresenceMessage)
            {
                NewMUCMemberPresenceMessage?.Invoke(this, new NewMUCMemberPresenceMessageEventArgs(args.MESSAGE as MUCMemberPresenceMessage));
            }
            else
            {
                NewPresence?.Invoke(this, new Events.NewPresenceMessageEventArgs(args.MESSAGE as PresenceMessage));
            }
        }

        private void Connection_MessageSend(XMPPConnection2 connection, MessageSendEventArgs args)
        {
            MessageSend?.Invoke(this, args);
        }

        private void Connection_NewBookmarksResultMessage(XMPPConnection2 connection, NewBookmarksResultMessageEventArgs args)
        {
            NewBookmarksResultMessage?.Invoke(this, args);
        }

        #endregion
    }
}
