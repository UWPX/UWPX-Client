using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Logging;
using Microsoft.Toolkit.Uwp.Connectivity;
using Shared.Classes.Collections;
using Windows.System.Threading;
using XMPP_API.Classes.Events;
using XMPP_API.Classes.Network.Events;
using XMPP_API.Classes.Network.TCP;
using XMPP_API.Classes.Network.XML;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.Helper;
using XMPP_API.Classes.Network.XML.Messages.Processor;
using XMPP_API.Classes.Network.XML.Messages.XEP_0048;
using XMPP_API.Classes.Network.XML.Messages.XEP_0199;
using XMPP_API.Classes.Network.XML.Messages.XEP_0384;

namespace XMPP_API.Classes.Network
{
    public class XmppConnection: AbstractConnection, IDisposable, IMessageSender
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private const string LOGGER_TAG = "[XmppConnection]: ";

        public readonly TcpConnection TCP_CONNECTION;
        private readonly SemaphoreSlim CONNECT_DISCONNECT_SEMA = new SemaphoreSlim(1, 1);

        public OmemoHelper omemoHelper { get; private set; }

        /// <summary>
        /// For parsing received messages.
        /// </summary>
        private readonly MessageParser2 PARSER = new MessageParser2();
        private readonly AbstractMessageProcessor[] MESSAGE_PROCESSORS = new AbstractMessageProcessor[4];

        private TSTimedList<string> messageIdCache = new TSTimedList<string>();

        private int errorCount;
        private bool reconnectRequested;
        public ConnectionError lastConnectionError;

        /// <summary>
        /// A timer for connecting to an XMPP server.
        /// If the server does not respond for a set amount of time once the TCP connection has been established.
        /// A reconnect gets triggered.
        /// </summary>
        private ThreadPoolTimer connectionTimer;
        /// <summary>
        /// The connection timeout in ms.
        /// </summary>
        private const int CONNECTION_TIMEOUT = 5000;
        private TimeSpan timeout = TimeSpan.FromMilliseconds(CONNECTION_TIMEOUT);

        public delegate void MessageSendEventHandler(XmppConnection sender, MessageSendEventArgs args);
        public delegate void NewBookmarksResultMessageEventHandler(XmppConnection sender, NewBookmarksResultMessageEventArgs args);
        public delegate void OmemoSessionBuildErrorEventHandler(XmppConnection sender, OmemoSessionBuildErrorEventArgs args);

        public event NewValidMessageEventHandler NewValidMessage;
        public event NewValidMessageEventHandler NewRoosterMessage;
        public event NewValidMessageEventHandler NewPresenceMessage;
        public event NewBookmarksResultMessageEventHandler NewBookmarksResultMessage;
        public event MessageSendEventHandler MessageSend;
        public event OmemoSessionBuildErrorEventHandler OmemoSessionBuildError;

        public readonly GeneralCommandHelper GENERAL_COMMAND_HELPER;
        public readonly MUCCommandHelper MUC_COMMAND_HELPER;
        public readonly PubSubCommandHelper PUB_SUB_COMMAND_HELPER;
        public readonly OmemoCommandHelper OMEMO_COMMAND_HELPER;
        private readonly DiscoFeatureHelper DISCO_HELPER;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public XmppConnection(XMPPAccount account) : base(account)
        {
            TCP_CONNECTION = new TcpConnection(account);
            TCP_CONNECTION.ConnectionStateChanged += TcpConnection_ConnectionStateChanged;
            TCP_CONNECTION.NewDataReceived += TCP_CONNECTION_NewDataReceived;

            GENERAL_COMMAND_HELPER = new GeneralCommandHelper(this);
            MUC_COMMAND_HELPER = new MUCCommandHelper(this);
            PUB_SUB_COMMAND_HELPER = new PubSubCommandHelper(this);
            OMEMO_COMMAND_HELPER = new OmemoCommandHelper(this);
            DISCO_HELPER = new DiscoFeatureHelper(this);

            // The order in which new messages should get processed (TLS -- SASL -- Stream Management -- Resource binding -- ...).
            // https://xmpp.org/extensions/xep-0170.html
            //-------------------------------------------------------------
            // TLS:
            MESSAGE_PROCESSORS[0] = new TLSConnection(TCP_CONNECTION, this);

            // SASL:
            MESSAGE_PROCESSORS[1] = new SASLConnection(TCP_CONNECTION, this);

            // XEP-0198 (Stream Management):
            MESSAGE_PROCESSORS[2] = new SMConnection(TCP_CONNECTION, this);

            // Resource binding:
            RecourceBindingConnection recourceBindingConnection = new RecourceBindingConnection(TCP_CONNECTION, this);
            recourceBindingConnection.ResourceBound += RecourceBindingConnection_ResourceBound;
            MESSAGE_PROCESSORS[3] = recourceBindingConnection;
            //-------------------------------------------------------------

            NetworkHelper.Instance.NetworkChanged += Instance_NetworkChanged;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public MessageParserStats GetMessageParserStats()
        {
            return PARSER.STATS;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public async void Dispose()
        {
            await DisconnectAsync();
        }

        public Task<bool> SendAsync(AbstractMessage msg)
        {
            return SendAsync(msg, false);
        }

        /// <summary>
        /// Sends the given message to the server or stores it until there is a connection to the server.
        /// </summary>
        /// <param name="msg">The message to send.</param>
        /// <param name="sendIfNotConnected">Sends the message if the underlaying TCP connection is connected to the server and ignores the connection state of the XMPPConnection.</param>
        /// <returns>True if the message got send and didn't got cached.</returns>
        public async Task<bool> SendAsync(AbstractMessage msg, bool sendIfNotConnected)
        {
            if (state == ConnectionState.CONNECTING)
            {
                ResetConnectionTimer();
            }

            if (state != ConnectionState.CONNECTED && !sendIfNotConnected)
            {
                if (Logger.logLevel >= LogLevel.DEBUG)
                {
                    Logger.Warn("[XMPPConnection2]: Did not send message, due to connection state is " + state + "\n" + msg.toXmlString());
                }
                else
                {
                    Logger.Warn("[XMPPConnection2]: Did not send message, due to connection state is " + state);
                }

                if (!sendIfNotConnected)
                {
                    return false;
                }
            }

            if (msg is IQMessage && msg.ID != null)
            {
                messageIdCache.AddTimed(msg.ID);
            }
            try
            {
                if (await TCP_CONNECTION.SendAsync(msg.toXmlString()))
                {
                    // Only trigger onMessageSend(...) for chat messages:
                    if (msg is MessageMessage m)
                    {
                        OnMessageSend(msg.ID, m.chatMessageId, false);
                    }
                    return true;
                }
                else
                {
                    Logger.Error("[XMPPConnection2]: Error during sending message for account: " + account.getBareJid());
                }
            }
            catch (Exception e)
            {
                Logger.Error("[XMPPConnection2]: Error during sending message for account: " + account.getBareJid(), e);
            }
            return false;
        }

        public async Task ConnectAsync()
        {
            await CONNECT_DISCONNECT_SEMA.WaitAsync();
            errorCount = 0;

            await ConnectInternalAsync();
            CONNECT_DISCONNECT_SEMA.Release();
        }

        public async Task DisconnectAsync()
        {
            await CONNECT_DISCONNECT_SEMA.WaitAsync();
            reconnectRequested = false;
            await InternalDisconnectAsync();
            CONNECT_DISCONNECT_SEMA.Release();
        }

        public async Task OnMessageProcessorFailedAsync(ConnectionError connectionError, bool criticalError)
        {
            if (criticalError)
            {
                lastConnectionError = connectionError;
                await OnConnectionErrorAsync();
            }
        }

        /// <summary>
        /// Reconnects to the server.
        /// </summary>
        /// <param name="resetErrorCount">Whether the connectionErrorCount should get reset.</param>
        public async Task ReconnectAsync(bool resetErrorCount)
        {
            if (resetErrorCount)
            {
                errorCount = 0;
            }
            reconnectRequested = true;
            await InternalDisconnectAsync();
        }

        /// <summary>
        /// Enables OMEMO encryption for messages for this 
        /// Has to be enabled before connecting.
        /// </summary>
        /// <param name="omemoStore">A persistent store for all the OMEMO related data (e.g. device ids and keys).</param>
        /// <returns>Returns true on success.</returns>
        public bool EnableOmemo(IOmemoStore omemoStore)
        {
            if (state != ConnectionState.DISCONNECTED)
            {
                throw new InvalidOperationException(LOGGER_TAG + "Unable to enable OMEMO. state != " + ConnectionState.DISCONNECTED.ToString() + " - " + state.ToString());
            }

            // Load OMEMO keys for the current account:
            if (!account.omemoKeysGenerated)
            {
                Logger.Error(LOGGER_TAG + "Failed to enable OMEMO for account: " + account.getBareJid() + " - generate OMEMO keys first!");
                omemoHelper = null;
                return false;
            }
            else if (!account.loadOmemoKeys(omemoStore))
            {
                omemoHelper = null;
                return false;
            }
            omemoHelper = new OmemoHelper(this, omemoStore);
            return true;
        }

        public async Task EnablePushNotificationsAsync()
        {
            MessageResponseHelperResult<IQMessage> result = await GENERAL_COMMAND_HELPER.enablePushNotificationsAsync(account.pushServerBareJid, account.pushNode, account.pushNodeSecret);
            if (result.STATE == MessageResponseHelperResultState.SUCCESS)
            {
                if (result.RESULT is IQErrorMessage errorMessage)
                {
                    account.CONNECTION_INFO.pushState = PushState.ERROR;
                    Logger.Error("Failed to enable push notifications for '" + account.getBareJid() + "' - " + errorMessage.ERROR_OBJ.ToString());
                }
                else if (result.RESULT.TYPE != IQMessage.RESULT)
                {
                    account.CONNECTION_INFO.pushState = PushState.ERROR;
                    Logger.Error("Failed to enable push notifications for '" + account.getBareJid() + "' - server responded with: " + result.RESULT.TYPE);
                }
                else
                {
                    account.pushNodePublished = true;
                    account.CONNECTION_INFO.pushState = PushState.ENABLED;
                    Logger.Info("Successfully enabled push notifications for: '" + account.getBareJid() + '\'');
                }
            }
            else
            {
                account.CONNECTION_INFO.pushState = PushState.ERROR;
                Logger.Error("Failed to enable push notifications for '" + account.getBareJid() + "' - " + result.STATE);
            }
        }

        public async Task EnableMessageCarbonsAsync()
        {
            account.CONNECTION_INFO.msgCarbonsState = MessageCarbonsState.REQUESTED;
            MessageResponseHelperResult<IQMessage> result = await GENERAL_COMMAND_HELPER.enableMessageCarbonsAsync();
            if (result.STATE == MessageResponseHelperResultState.SUCCESS)
            {
                if (result.RESULT is IQErrorMessage errorMessage)
                {
                    account.CONNECTION_INFO.msgCarbonsState = MessageCarbonsState.ERROR;
                    Logger.Error("Failed to enable push message carbons for '" + account.getBareJid() + "' - " + errorMessage.ERROR_OBJ.ToString());
                }
                else if (result.RESULT.TYPE != IQMessage.RESULT)
                {
                    account.CONNECTION_INFO.msgCarbonsState = MessageCarbonsState.ERROR;
                    Logger.Error("Failed to enable push message carbons for '" + account.getBareJid() + "' - server responded with: " + result.RESULT.TYPE);
                }
                else
                {
                    account.CONNECTION_INFO.msgCarbonsState = MessageCarbonsState.ENABLED;
                    Logger.Info("Successfully enable push message carbons for: '" + account.getBareJid() + '\'');
                }
            }
            else
            {
                account.CONNECTION_INFO.msgCarbonsState = MessageCarbonsState.ERROR;
                Logger.Error("Failed to enable push message carbons for '" + account.getBareJid() + "' - " + result.STATE);
            }
        }

        #endregion

        #region --Misc Methods (Private)--
        private async Task ConnectInternalAsync()
        {
            if (state == ConnectionState.DISCONNECTED || state == ConnectionState.ERROR)
            {
                if (errorCount < 3)
                {
                    SetState(ConnectionState.CONNECTING);
                    messageIdCache.Clear();
                    ResetMessageProcessors();

                    await TCP_CONNECTION.ConnectAsync();
                }
            }
            else
            {
                Logger.Warn(LOGGER_TAG + "Trying to connect but state is not " + ConnectionState.DISCONNECTED + " or " + ConnectionState.ERROR + "! State = " + state);
                await ReconnectAsync(false);
            }
        }

        /// <summary>
        /// Disconnects from the server.
        /// </summary>
        /// <returns></returns>
        private async Task InternalDisconnectAsync()
        {
            StopConnectionTimer();
            if (state == ConnectionState.DISCONNECTED || state == ConnectionState.ERROR)
            {
                // Reset connection error count:
                errorCount = 0;
                await OnDisconnectedAsync();
            }
            else
            {
                SetState(ConnectionState.DISCONNECTING);

                // Send stream close message:
                await SendStreamCloseMessageAsync();

                // Disconnect the TCPConnection:
                await TCP_CONNECTION.DisconnectAsync();

                await OnDisconnectedAsync();

                SetState(ConnectionState.DISCONNECTED);
            }
        }

        /// <summary>
        /// Stops the connection timer by disposing it.
        /// </summary>
        private void StopConnectionTimer()
        {
            try
            {
                connectionTimer?.Cancel();
            }
            catch (Exception) { }

            connectionTimer = null;
        }

        private void ResetConnectionTimer()
        {
            StopConnectionTimer();

            connectionTimer = ThreadPoolTimer.CreateTimer(async (source) => await OnConnetionTimerTimeoutAsync(), timeout);
        }

        /// <summary>
        /// Called once the connection timer got triggered.
        /// </summary>
        private async Task OnConnetionTimerTimeoutAsync()
        {
            string errorMessage = "Connection timeout got triggered for account: " + account.getBareJid();
            Logger.Warn(LOGGER_TAG + errorMessage);
            lastConnectionError = new ConnectionError(ConnectionErrorCode.XMPP_CONNECTION_TIMEOUT, errorMessage);

            await OnConnectionErrorAsync();
        }

        /// <summary>
        /// Sends the stream close message if the TCP connection is connected.
        /// </summary>
        private async Task SendStreamCloseMessageAsync()
        {
            Logger.Debug(LOGGER_TAG + "Sending stream close...");
            if (TCP_CONNECTION.state == ConnectionState.CONNECTED)
            {
                await TCP_CONNECTION.SendAsync(Consts.XML_STREAM_CLOSE);
                Logger.Debug(LOGGER_TAG + "Stream close send.");
            }
            else
            {
                Logger.Debug(LOGGER_TAG + "Skipping sending stream close - TCP_CONNECTION.state != ConnectionState.CONNECTED: " + TCP_CONNECTION.state.ToString());
            }
        }

        private void ResetMessageProcessors()
        {
            foreach (AbstractMessageProcessor mP in MESSAGE_PROCESSORS)
            {
                mP.reset();
            }
        }

        /// <summary>
        /// Called on disconnected state entered.
        /// Initiates a reconnect if reconnectRequested is set.
        /// </summary>
        private async Task OnDisconnectedAsync()
        {
            if (reconnectRequested)
            {
                await ConnectInternalAsync();
            }
        }

        private async Task OnConnectionErrorAsync()
        {
            // Stop the connection timer:
            StopConnectionTimer();

            Logger.Debug(LOGGER_TAG + "OnConnectionErrorAsync() got triggered - connectionErrorCount: " + errorCount);
            if (++errorCount >= 3)
            {
                // Establishing the connection failed for the third time:
                await InternalDisconnectAsync();
                SetState(ConnectionState.ERROR, lastConnectionError);
            }
            else
            {
                // Try to reconnect:
                await ConnectInternalAsync();
            }
        }

        /// <summary>
        /// Resends the stream header.
        /// </summary>
        /// <returns></returns>
        private async Task SoftRestartAsync()
        {
            OpenStreamMessage openStreamMessage = new OpenStreamMessage(account.getBareJid(), account.user.domainPart);
            await SendAsync(openStreamMessage, true);
        }

        /// <summary>
        /// Performs a reconnect.
        /// </summary>
        /// <returns></returns>
        private async Task HardRestartAsync()
        {
            await ReconnectAsync(false);
        }

        /// <summary>
        /// Triggers the MessageSend event in a new task.
        /// </summary>
        /// <param name="id">The id of the message that got send.</param>
        /// <param name="delayed">If the message got send delayed (e.g. stored in message cache).</param>
        private void OnMessageSend(string id, string chatMessageId, bool delayed)
        {
            Task.Run(() => MessageSend?.Invoke(this, new MessageSendEventArgs(id, chatMessageId, delayed)));
        }

        /// <summary>
        /// Parses the given messages and invokes the ConnectionNewValidMessage event for each message.
        /// </summary>
        /// <param name="data">The messages string to parse.</param>
        private async Task ParseMessageAsync(string data)
        {
            // Parse message:
            List<AbstractMessage> messages = null;
            try
            {
                messages = PARSER.parseMessages(ref data);
            }
            catch (Exception e)
            {
                Logger.Error("[XMPPConnection2]: Error during message parsing." + e);
                return;
            }

            // Process messages:
            foreach (AbstractMessage msg in messages)
            {
                // Stream error messages:
                if (msg is StreamErrorMessage errorMessage)
                {
                    Logger.Warn("[XMPPConnection2]: Received stream error message: " + errorMessage.ToString());
                    lastConnectionError = new ConnectionError(ConnectionErrorCode.SERVER_ERROR, errorMessage.ToString());
                    await OnConnectionErrorAsync();
                    await SendStreamCloseMessageAsync();
                    return;
                }

                // Filter IQ messages which ids are not valid:
                if (msg is IQMessage iq)
                {
                    if (iq.GetType().Equals(IQMessage.RESULT) && messageIdCache.GetTimed(iq.ID) != null)
                    {
                        Logger.Warn("[XMPPConnection2]: Invalid message id received!");
                        return;
                    }
                }

                // Respond to XEP-0199 (XMPP Ping) messages:
                if (msg is PingMessage pingMsg)
                {
                    Logger.Debug("[XMPPConnection2]: XMPP ping received from " + pingMsg.getFrom());
                    await SendAsync(pingMsg.generateResponse(), true);
                }

                // Invoke message processors:
                NewValidMessage?.Invoke(this, new NewValidMessageEventArgs(msg));

                // Should restart connection?
                if (msg.getRestartConnection() != AbstractMessage.NO_RESTART)
                {
                    if (msg.getRestartConnection() == AbstractMessage.SOFT_RESTART)
                    {
                        await SoftRestartAsync();
                    }
                    else if (msg.getRestartConnection() == AbstractMessage.HARD_RESTART)
                    {
                        await HardRestartAsync();
                    }
                    else
                    {
                        throw new ArgumentException("[XMPPConnection2]: Invalid restart type: " + msg.getRestartConnection());
                    }
                }

                // Filter already processed messages:
                if (msg.isProcessed())
                {
                    return;
                }

                // --------------------------------------------------------------------
                // Open stream:
                if (msg is OpenStreamAnswerMessage oA)
                {
                    if (oA.ID is null)
                    {
                        // TODO Handle OpenStreamAnswerMessage id is null
                        //Error throw exception?!
                        return;
                    }
                }
                // Close stream message:
                else if (msg is CloseStreamMessage)
                {
                    switch (state)
                    {
                        case ConnectionState.CONNECTING:
                        case ConnectionState.CONNECTED:
                            await InternalDisconnectAsync();
                            break;
                    }
                }
                // Rooster:
                else if (msg is RosterResultMessage)
                {
                    NewRoosterMessage?.Invoke(this, new NewValidMessageEventArgs(msg));
                }
                // Presence:
                else if (msg is PresenceMessage presenceMessage && !(presenceMessage.getFrom() is null))
                {
                    NewPresenceMessage?.Invoke(this, new NewValidMessageEventArgs(msg));
                }
                // Bookmarks:
                else if (msg is BookmarksResultMessage bookmarksResultMessage)
                {
                    NewBookmarksResultMessage?.Invoke(this, new NewBookmarksResultMessageEventArgs(bookmarksResultMessage));
                }
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion

        #region --Misc Methods (Internal)--
        internal void OnOmemoSessionBuildError(OmemoSessionBuildErrorEventArgs args)
        {
            OmemoSessionBuildError?.Invoke(this, args);
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private async void TcpConnection_ConnectionStateChanged(AbstractConnection sender, ConnectionStateChangedEventArgs args)
        {
            switch (args.newState)
            {
                case ConnectionState.CONNECTED:
                    // Send initial stream header:
                    await SoftRestartAsync();
                    break;

                case ConnectionState.ERROR:
                    if (args.param is ConnectionError cError)
                    {
                        lastConnectionError = cError;
                    }
                    if (args.oldState == ConnectionState.CONNECTED)
                    {
                        switch (state)
                        {
                            case ConnectionState.CONNECTING:
                            case ConnectionState.CONNECTED:
                                await OnConnectionErrorAsync();
                                break;

                            case ConnectionState.DISCONNECTING:
                                SetState(ConnectionState.DISCONNECTED);
                                break;
                        }
                    }
                    else
                    {
                        // Unable to connect to server:
                        errorCount = 3;
                        SetState(ConnectionState.ERROR, args.param);
                    }
                    break;

                case ConnectionState.DISCONNECTED:
                    SetState(ConnectionState.DISCONNECTED);
                    break;
            }
        }

        private async void TCP_CONNECTION_NewDataReceived(TcpConnection sender, NewDataReceivedEventArgs args)
        {
            await ParseMessageAsync(args.data);

            // Stop or reset the connection timer.
            switch (state)
            {
                case ConnectionState.CONNECTING:
                    ResetConnectionTimer();
                    break;
                default:
                    StopConnectionTimer();
                    break;
            }
        }

        private async void RecourceBindingConnection_ResourceBound(object sender, EventArgs e)
        {
            StopConnectionTimer();

            // Send the initial presence message:
            await SendAsync(new PresenceMessage(account.presencePriorety, account.presence, account.status), true);

            SetState(ConnectionState.CONNECTED);
        }

        private async void Instance_NetworkChanged(object sender, EventArgs e)
        {
            if (NetworkHelper.Instance.ConnectionInformation.IsInternetAvailable)
            {
                if (!account.disabled && (state == ConnectionState.DISCONNECTED || state == ConnectionState.ERROR))
                {
                    await ReconnectAsync(true);
                }
            }
            else if (state == ConnectionState.CONNECTED)
            {
                await InternalDisconnectAsync();
            }
        }

        #endregion
    }
}
