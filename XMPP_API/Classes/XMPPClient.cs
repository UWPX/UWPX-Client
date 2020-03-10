using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Logging;
using Windows.System.Threading;
using XMPP_API.Classes.Events;
using XMPP_API.Classes.Network;
using XMPP_API.Classes.Network.Events;
using XMPP_API.Classes.Network.XML;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.XEP_0045;
using XMPP_API.Classes.Network.XML.Messages.XEP_0060;
using XMPP_API.Classes.Network.XML.Messages.XEP_0085;
using XMPP_API.Classes.Network.XML.Messages.XEP_0184;
using XMPP_API.Classes.Network.XML.Messages.XEP_0384;

namespace XMPP_API.Classes
{
    public class XMPPClient: IMessageSender, IDisposable
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public XmppConnection connection
        {
            get;
            private set;
        }

        private bool holdConnection = false;
        private const int RECONNECT_DELAY = 5000;
        private TimeSpan reconnectDelay = TimeSpan.FromMilliseconds(RECONNECT_DELAY);
        private ThreadPoolTimer reconnectTimer;

        public delegate void ConnectionStateChangedEventHandler(XMPPClient client, ConnectionStateChangedEventArgs args);
        public delegate void NewChatMessageEventHandler(XMPPClient client, NewChatMessageEventArgs args);
        public delegate void NewPresenceEventHandler(XMPPClient client, NewPresenceMessageEventArgs args);
        public delegate void NewChatStateEventHandler(XMPPClient client, NewChatStateEventArgs args);
        public delegate void MessageSendEventHandler(XMPPClient client, MessageSendEventArgs args);
        public delegate void NewBookmarksResultMessageEventHandler(XMPPClient client, NewBookmarksResultMessageEventArgs args);
        public delegate void NewMUCMemberPresenceMessageEventHandler(XMPPClient client, NewMUCMemberPresenceMessageEventArgs args);
        public delegate void NewDeliveryReceiptHandler(XMPPClient client, NewDeliveryReceiptEventArgs args);
        public delegate void NewPubSubEventHandler(XMPPClient client, NewPubSubEventEventArgs args);
        public delegate void OmemoSessionBuildErrorEventHandler(XMPPClient client, OmemoSessionBuildErrorEventArgs args);

        public event NewValidMessageEventHandler NewRoosterMessage;
        public event ConnectionStateChangedEventHandler ConnectionStateChanged;
        public event NewChatMessageEventHandler NewChatMessage;
        public event NewPresenceEventHandler NewPresence;
        public event NewChatStateEventHandler NewChatState;
        public event MessageSendEventHandler MessageSend;
        public event NewMUCMemberPresenceMessageEventHandler NewMUCMemberPresenceMessage;
        public event NewValidMessageEventHandler NewValidMessage;
        public event NewBookmarksResultMessageEventHandler NewBookmarksResultMessage;
        public event NewDeliveryReceiptHandler NewDeliveryReceipt;
        public event NewPubSubEventHandler NewPubSubEvent;
        public event OmemoSessionBuildErrorEventHandler OmemoSessionBuildError;

        public GeneralCommandHelper GENERAL_COMMAND_HELPER => connection.GENERAL_COMMAND_HELPER;
        public MUCCommandHelper MUC_COMMAND_HELPER => connection.MUC_COMMAND_HELPER;
        public PubSubCommandHelper PUB_SUB_COMMAND_HELPER => connection.PUB_SUB_COMMAND_HELPER;
        public OmemoCommandHelper OMEMO_COMMAND_HELPER => connection.OMEMO_COMMAND_HELPER;

        private TaskCompletionSource<bool> connectDisconnectTCS;

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
            init(account);
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public bool isConnected()
        {
            return connection != null && connection.state == ConnectionState.CONNECTED;
        }

        public ConnectionState getConnetionState()
        {
            return connection.state;
        }

        public MessageParserStats getMessageParserStats()
        {
            return connection?.GetMessageParserStats();
        }

        public ConnectionError getLastConnectionError()
        {
            return connection.lastConnectionError;
        }

        public OmemoHelper getOmemoHelper()
        {
            return connection.omemoHelper;
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
                connection.NewRoosterMessage -= Connection_ConnectionNewRoosterMessage;
                connection.ConnectionStateChanged -= Connection_ConnectionStateChanged;
                connection.NewValidMessage -= Connection_ConnectionNewValidMessage;
                connection.NewPresenceMessage -= Connection_ConnectionNewPresenceMessage;
                connection.MessageSend -= Connection_MessageSend;
                connection.NewBookmarksResultMessage -= Connection_NewBookmarksResultMessage;
                connection.OmemoSessionBuildError -= Connection_OmemoSessionBuildErrorEvent;
            }

            init(account);
        }

        public void Dispose()
        {
            connectDisconnectTCS?.TrySetResult(false);
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task transferSocketOwnershipAsync()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            //await connection?.transferSocketOwnershipAsync();
        }

        public async Task connectAsync(bool holdConnection = true)
        {
            if (!(connectDisconnectTCS is null))
            {
                connectDisconnectTCS?.TrySetResult(false);
                await connectDisconnectTCS.Task;
            }
            connectDisconnectTCS = new TaskCompletionSource<bool>();

            Logger.Info("Connecting account: " + getXMPPAccount().getBareJid());
            this.holdConnection = holdConnection;
            try
            {
                await connection.ConnectAsync();
                await connectDisconnectTCS.Task;
            }
            catch (Exception e)
            {
                Logger.Error("Error during connectAsync in XMPPClient!", e);
            }
        }

        public async Task reconnectAsync()
        {
            Logger.Info("Reconnecting account: " + getXMPPAccount().getBareJid());
            StopReconnectTimer();
            await connection.ReconnectAsync(true);
        }

        public async Task disconnectAsync()
        {
            Logger.Info("Disconnecting account: " + getXMPPAccount().getBareJid());
            holdConnection = false;
            StopReconnectTimer();
            await connection.DisconnectAsync();
        }

        public async Task sendOmemoMessageAsync(OmemoMessageMessage msg, string chatJid, string accountJid)
        {
            if (connection.omemoHelper is null)
            {
                OmemoSessionBuildError?.Invoke(this, new OmemoSessionBuildErrorEventArgs(chatJid, Network.XML.Messages.XEP_0384.Signal.Session.OmemoSessionBuildError.KEY_ERROR, new List<OmemoMessageMessage> { msg }));
                Logger.Error("Failed to send OMEMO message - OmemoHelper is null");
            }
            else if (!connection.account.checkOmemoKeys())
            {
                OmemoSessionBuildError?.Invoke(this, new OmemoSessionBuildErrorEventArgs(chatJid, Network.XML.Messages.XEP_0384.Signal.Session.OmemoSessionBuildError.KEY_ERROR, new List<OmemoMessageMessage> { msg }));
                Logger.Error("Failed to send OMEMO message - keys are corrupted");
            }
            else
            {
                await connection.omemoHelper.sendOmemoMessageAsync(msg, chatJid, accountJid);
            }
        }

        public async Task<bool> SendAsync(AbstractMessage msg)
        {
            return await connection.SendAsync(msg);
        }

        public XMPPAccount getXMPPAccount()
        {
            return connection.account;
        }

        /// <summary>
        /// Enables OMEMO encryption for messages for this connection.
        /// Has to be enabled before connecting.
        /// </summary>
        /// <param name="omemoStore">A persistent store for all the OMEMO related data (e.g. device ids and keys).</param>
        /// <returns>Returns true on success.</returns>
        public bool enableOmemo(IOmemoStore omemoStore)
        {
            return connection.EnableOmemo(omemoStore);
        }
        #endregion

        #region --Misc Methods (Private)--
        private void init(XMPPAccount account)
        {
            connection = new XmppConnection(account);
            connection.NewRoosterMessage += Connection_ConnectionNewRoosterMessage;
            connection.ConnectionStateChanged += Connection_ConnectionStateChanged;
            connection.NewValidMessage += Connection_ConnectionNewValidMessage;
            connection.NewPresenceMessage += Connection_ConnectionNewPresenceMessage;
            connection.MessageSend += Connection_MessageSend;
            connection.NewBookmarksResultMessage += Connection_NewBookmarksResultMessage;
            connection.OmemoSessionBuildError += Connection_OmemoSessionBuildErrorEvent;
        }

        private void StopReconnectTimer()
        {
            try
            {
                reconnectTimer?.Cancel();
            }
            catch (Exception) { }
            reconnectTimer = null;
        }

        private void StartReconnectTimer()
        {
            StopReconnectTimer();
            reconnectTimer = ThreadPoolTimer.CreateTimer(async (source) => await OnReconnectTimerTimeoutAsync(), reconnectDelay);
            Logger.Info("Started a reconnect timer for: " + connection.account.getBareJid());
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void Connection_ConnectionNewRoosterMessage(IMessageSender sender, NewValidMessageEventArgs args)
        {
            NewRoosterMessage?.Invoke(this, args);
        }

        private void Connection_ConnectionStateChanged(AbstractConnection sender, ConnectionStateChangedEventArgs args)
        {
            if (args.newState == ConnectionState.CONNECTED)
            {
                connectDisconnectTCS?.TrySetResult(true);
                Logger.Info("Connected to account: " + getXMPPAccount().getBareJid());
            }
            else if (args.newState == ConnectionState.DISCONNECTED)
            {
                connectDisconnectTCS?.TrySetResult(false);
                Logger.Info("Disconnected account: " + getXMPPAccount().getBareJid());

                if (holdConnection)
                {
                    StartReconnectTimer();
                }
            }
            else if (args.newState == ConnectionState.ERROR)
            {
                connectDisconnectTCS?.TrySetResult(false);

                if (holdConnection)
                {
                    StartReconnectTimer();
                }
            }

            ConnectionStateChanged?.Invoke(this, args);
        }

        private void Connection_ConnectionNewValidMessage(IMessageSender sender, NewValidMessageEventArgs args)
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
            else if (msg is DeliveryReceiptMessage dRMsg)
            {
                NewDeliveryReceipt?.Invoke(this, new NewDeliveryReceiptEventArgs(dRMsg));
            }
            else if (msg is AbstractPubSubEventMessage pubSubEventMsg)
            {
                NewPubSubEvent?.Invoke(this, new NewPubSubEventEventArgs(pubSubEventMsg));
            }

            NewValidMessage?.Invoke(this, args);
        }

        private void Connection_ConnectionNewPresenceMessage(IMessageSender connection, NewValidMessageEventArgs args)
        {
            // XEP-0045 (MUC member presence):
            if (args.MESSAGE is MUCMemberPresenceMessage)
            {
                NewMUCMemberPresenceMessage?.Invoke(this, new NewMUCMemberPresenceMessageEventArgs(args.MESSAGE as MUCMemberPresenceMessage));
            }
            else
            {
                NewPresence?.Invoke(this, new NewPresenceMessageEventArgs(args.MESSAGE as PresenceMessage));
            }
        }

        private void Connection_MessageSend(XmppConnection sender, MessageSendEventArgs args)
        {
            MessageSend?.Invoke(this, args);
        }

        private void Connection_NewBookmarksResultMessage(XmppConnection sender, NewBookmarksResultMessageEventArgs args)
        {
            NewBookmarksResultMessage?.Invoke(this, args);
        }

        private void Connection_OmemoSessionBuildErrorEvent(XmppConnection sender, OmemoSessionBuildErrorEventArgs args)
        {
            OmemoSessionBuildError?.Invoke(this, args);
        }

        private Task OnReconnectTimerTimeoutAsync()
        {
            if (!holdConnection)
            {
                return null;
            }
            Logger.Debug("Reconnect timer triggered for:" + connection.account.getBareJid());
            return connectAsync();
        }

        #endregion
    }
}
