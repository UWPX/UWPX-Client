using Logging;
using Microsoft.Toolkit.Uwp.Connectivity;
using Shared.Classes.Collections;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.System.Threading;
using XMPP_API.Classes.Events;
using XMPP_API.Classes.Network.Events;
using XMPP_API.Classes.Network.TCP;
using XMPP_API.Classes.Network.XML;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.Processor;
using XMPP_API.Classes.Network.XML.Messages.XEP_0048;
using XMPP_API.Classes.Network.XML.Messages.XEP_0384;

namespace XMPP_API.Classes.Network
{
    public class XMPPConnection2 : AbstractConnection2, IMessageSender
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly TCPConnection2 TCP_CONNECTION;
        public OmemoHelper omemoHelper { get; private set; }
        private readonly DiscoFeatureHelper DISCO_FEATURE_HELPER;

        public ConnectionError lastConnectionError;
        private int connectionErrorCount;

        private bool holdConnection;

        /// <summary>
        /// For parsing received messages.
        /// </summary>
        private readonly MessageParser2 PARSER;
        private readonly AbstractMessageProcessor[] MESSAGE_PROCESSORS;
        private string streamId;
        private TSTimedList<string> messageIdCache;

        public delegate void MessageSendEventHandler(XMPPConnection2 connection, MessageSendEventArgs args);
        public delegate void NewBookmarksResultMessageEventHandler(XMPPConnection2 connection, NewBookmarksResultMessageEventArgs args);
        public delegate void OmemoSessionBuildErrorEventHandler(XMPPConnection2 connection, OmemoSessionBuildErrorEventArgs args);

        public event NewValidMessageEventHandler NewValidMessage;
        public event NewValidMessageEventHandler NewRoosterMessage;
        public event NewValidMessageEventHandler NewPresenceMessage;
        public event NewBookmarksResultMessageEventHandler NewBookmarksResultMessage;
        public event MessageSendEventHandler MessageSend;
        public event OmemoSessionBuildErrorEventHandler OmemoSessionBuildError;

        /// <summary>
        /// The timer for connecting to a server.
        /// If the server does not respond for a set amount of time during connection
        /// it triggers a reconnect or switches to the error state.
        /// It starts after the TCP connection got established.
        /// </summary>
        private ThreadPoolTimer connectionTimer;
        /// <summary>
        /// The connection timeout in ms.
        /// </summary>
        private const int CONNECTION_TIMEOUT = 5000;
        private bool reconnectRequested;
        private TimeSpan timeout;

        public readonly GeneralCommandHelper GENERAL_COMMAND_HELPER;
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
        /// 05/05/2018 Created [Fabian Sauter]
        /// </history>
        public XMPPConnection2(XMPPAccount account) : base(account)
        {
            this.holdConnection = false;
            this.connectionErrorCount = 0;
            this.lastConnectionError = null;
            this.TCP_CONNECTION = new TCPConnection2(account);
            this.TCP_CONNECTION.ConnectionStateChanged += TCPConnection_ConnectionStateChanged;
            this.TCP_CONNECTION.NewDataReceived += TCPConnection_NewDataReceived;

            this.PARSER = new MessageParser2();
            this.MESSAGE_PROCESSORS = new AbstractMessageProcessor[4];
            this.streamId = null;
            this.messageIdCache = new TSTimedList<string>();

            this.connectionTimer = null;
            this.reconnectRequested = false;
            this.timeout = TimeSpan.FromMilliseconds(CONNECTION_TIMEOUT);

            this.GENERAL_COMMAND_HELPER = new GeneralCommandHelper(this);
            this.MUC_COMMAND_HELPER = new MUCCommandHelper(this);
            this.PUB_SUB_COMMAND_HELPER = new PubSubCommandHelper(this);
            this.OMEMO_COMMAND_HELPER = new OmemoCommandHelper(this);

            // The order in which new messages should get processed (TLS -- SASL -- Stream Management -- Resource binding -- ...).
            // https://xmpp.org/extensions/xep-0170.html
            //-------------------------------------------------------------
            // TLS:
            this.MESSAGE_PROCESSORS[0] = new TLSConnection(TCP_CONNECTION, this);

            // SASL:
            this.MESSAGE_PROCESSORS[1] = new SASLConnection(TCP_CONNECTION, this);

            // XEP-0198 (Stream Management):
            this.MESSAGE_PROCESSORS[2] = new SMConnection(TCP_CONNECTION, this);

            // Resource binding:
            RecourceBindingConnection recourceBindingConnection = new RecourceBindingConnection(TCP_CONNECTION, this);
            recourceBindingConnection.ResourceBound += RecourceBindingConnection_ResourceBound;
            this.MESSAGE_PROCESSORS[3] = recourceBindingConnection;
            //-------------------------------------------------------------

            NetworkHelper.Instance.NetworkChanged += Instance_NetworkChanged;

            this.omemoHelper = null;
            this.DISCO_FEATURE_HELPER = new DiscoFeatureHelper(this);
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        protected override void setState(ConnectionState newState, object param)
        {
            base.setState(newState, param);
            switch (newState)
            {
                case ConnectionState.DISCONNECTED:
                    onDisconnected();
                    break;

                case ConnectionState.CONNECTED:
                    // Reset last error message:
                    lastConnectionError = null;
                    break;
            }
        }

        public MessageParserStats getMessageParserStats()
        {
            return PARSER.STATS;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        /// <summary>
        /// Enables OMEMO encryption for messages for this connection.
        /// Has to be enabled before connecting.
        /// </summary>
        /// <param name="omemoStore">A persistent store for all the OMEMO related data (e.g. device ids and keys).</param>
        /// <returns>Returns true on success.</returns>
        public bool enableOmemo(IOmemoStore omemoStore)
        {
            if (state != ConnectionState.DISCONNECTED)
            {
                throw new InvalidOperationException("[XMPPConnection2]: Unable to enable OMEMO. state != " + ConnectionState.DISCONNECTED.ToString() + " - " + state.ToString());
            }

            // Load OMEMO keys for the current account:
            if (!account.omemoKeysGenerated)
            {
                Logger.Error("[XMPPConnection2]: Failed to enable OMEMO for account: " + account.getBareJid() + " - generate OMEMO keys first!");
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

        public void connectAndHold()
        {
            holdConnection = true;
            connectionErrorCount = 0;
            Task t = connectAsync();
        }

        public async Task onMessageProcessorFailedAsync(ConnectionError connectionError, bool criticalError)
        {
            if (criticalError)
            {
                lastConnectionError = connectionError;
                await onConnectionErrorAsync();
            }
        }

        public async Task disconnectAsyncs()
        {
            holdConnection = false;
            await internalDisconnectAsync();
        }

        /// <summary>
        /// Reconnects to the server.
        /// </summary>
        /// <param name="resetErrorCount">Whether the connectionErrorCount should get reset.</param>
        public async Task reconnectAsync(bool resetErrorCount)
        {
            if (resetErrorCount)
            {
                connectionErrorCount = 0;
            }
            reconnectRequested = true;
            await internalDisconnectAsync();
        }

        /// <summary>
        /// Sends the given message to the server if connected. Won't cache the message if not connected!
        /// </summary>
        /// <param name="msg">The message to send.</param>
        /// <returns>True if the message got send and didn't got cached.</returns>
        public async Task<bool> sendAsync(AbstractMessage msg)
        {
            return await sendAsync(msg, false);
        }

        /// <summary>
        /// Sends the given message to the server or stores it until there is a connection to the server.
        /// </summary>
        /// <param name="msg">The message to send.</param>
        /// <param name="sendIfNotConnected">Sends the message if the underlaying TCP connection is connected to the server and ignores the connection state of the XMPPConnection.</param>
        /// <returns>True if the message got send and didn't got cached.</returns>
        public async Task<bool> sendAsync(AbstractMessage msg, bool sendIfNotConnected)
        {
            if (state == ConnectionState.CONNECTING)
            {
                resetConnectionTimer();
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
                messageIdCache.addTimed(msg.ID);
            }
            try
            {
                if (await TCP_CONNECTION.sendAsync(msg.toXmlString()))
                {
                    // Only trigger onMessageSend(...) for chat messages:
                    if (msg is MessageMessage m)
                    {
                        onMessageSend(msg.ID, m.chatMessageId, false);
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

        #endregion

        #region --Misc Methods (Private)--
        /// <summary>
        /// Called on disconnected state entered.
        /// Initiates a reconnect if reconnectRequested is set.
        /// </summary>
        private void onDisconnected()
        {
            if (reconnectRequested || holdConnection)
            {
                Task t = connectAsync();
            }
        }

        /// <summary>
        /// Disconnects from the server.
        /// </summary>
        /// <returns></returns>
        private async Task internalDisconnectAsync()
        {
            stopConnectionTimer();
            if (state == ConnectionState.DISCONNECTED)
            {
                // Reset connection error count:
                connectionErrorCount = 0;
                onDisconnected();
                return;
            }

            setState(ConnectionState.DISCONNECTING);

            // Send stream close message:
            await sendStreamCloseMessageAsync();

            // Disconnect the TCPConnection:
            TCP_CONNECTION.disconnect();

            setState(ConnectionState.DISCONNECTED);
        }

        /// <summary>
        /// Connects to the XMPP server.
        /// </summary>
        private async Task connectAsync()
        {
            if (!NetworkHelper.Instance.ConnectionInformation.IsInternetAvailable)
            {
                connectionErrorCount = 3;
                lastConnectionError = new ConnectionError(ConnectionErrorCode.NO_INTERNET);
                setState(ConnectionState.ERROR, lastConnectionError);
                reconnectRequested = false;
                holdConnection = false;
                Logger.Warn("[XMPPConnection2]: Unable to connect to " + account.serverAddress + " - no internet!");
                return;
            }

            switch (state)
            {
                case ConnectionState.DISCONNECTED:
                case ConnectionState.ERROR:
                    if (connectionErrorCount < 3)
                    {
                        setState(ConnectionState.CONNECTING);

                        // Reset stuff:
                        streamId = null;
                        messageIdCache = new TSTimedList<string>();
                        resetMessageProcessors();

                        await TCP_CONNECTION.connectAsync();
                    }
                    break;

                default:
                    Logger.Warn("[XMPPConnection2]: Trying to connect but state is not " + ConnectionState.DISCONNECTED + " or " + ConnectionState.ERROR + "! State = " + state);
                    await reconnectAsync(false);
                    break;
            }
        }

        /// <summary>
        /// Triggers the MessageSend event in a new task.
        /// </summary>
        /// <param name="id">The id of the message that got send.</param>
        /// <param name="delayed">If the message got send delayed (e.g. stored in message cache).</param>
        private void onMessageSend(string id, string chatMessageId, bool delayed)
        {
            Task.Run(() => MessageSend?.Invoke(this, new MessageSendEventArgs(id, chatMessageId, delayed)));
        }

        private void resetConnectionTimer()
        {
            stopConnectionTimer();

            connectionTimer = ThreadPoolTimer.CreateTimer(async (source) => await onConnetionTimerTimeoutAsync(), timeout);
        }

        /// <summary>
        /// Called once the connection timer got triggered.
        /// </summary>
        private async Task onConnetionTimerTimeoutAsync()
        {
            string errorMessage = "Connection timeout got triggered for account: " + account.getBareJid();
            Logger.Warn(errorMessage);
            lastConnectionError = new ConnectionError(ConnectionErrorCode.XMPP_CONNECTION_TIMEOUT, errorMessage);

            await onConnectionErrorAsync();
        }

        private async Task onConnectionErrorAsync()
        {
            // Stop the connection timer:
            stopConnectionTimer();

            Logger.Debug("[XMPPConnection2]: onConnectionErrorAsync() got triggered - connectionErrorCount: " + connectionErrorCount);
            if (++connectionErrorCount >= 3)
            {
                // Establishing the connection failed for the third time:
                holdConnection = false;
                await internalDisconnectAsync();
                setState(ConnectionState.ERROR, lastConnectionError);
            }
            else
            {
                // Try to reconnect:
                await reconnectAsync(false);
            }
        }

        /// <summary>
        /// Stops the connection timer by disposing it.
        /// </summary>
        private void stopConnectionTimer()
        {
            try
            {
                connectionTimer?.Cancel();
            }
            catch (Exception)
            {
            }

            connectionTimer = null;
        }

        /// <summary>
        /// Sends the stream close message if the TCP connection is connected.
        /// </summary>
        private async Task sendStreamCloseMessageAsync()
        {
            Logger.Debug("[XMPPConnection2]: Sending stream close...");
            if (TCP_CONNECTION.state == ConnectionState.CONNECTED)
            {
                await TCP_CONNECTION.sendAsync(Consts.XML_STREAM_CLOSE);
                Logger.Debug("[XMPPConnection2]: Stream close send.");
            }
            else
            {
                Logger.Debug("[XMPPConnection2]: Skipping sending stream close - TCP_CONNECTION.state != ConnectionState.CONNECTED: " + TCP_CONNECTION.state.ToString());
            }
        }

        private void resetMessageProcessors()
        {
            foreach (AbstractMessageProcessor mP in MESSAGE_PROCESSORS)
            {
                mP.reset();
            }
        }

        /// <summary>
        /// Resends the stream header.
        /// </summary>
        /// <returns></returns>
        private async Task softRestartAsync()
        {
            OpenStreamMessage openStreamMessage = new OpenStreamMessage(account.getBareJid(), account.user.domainPart);
            await sendAsync(openStreamMessage, true);
        }

        /// <summary>
        /// Performs a reconnect.
        /// </summary>
        /// <returns></returns>
        private async Task hardRestartAsync()
        {
            await reconnectAsync(false);
        }

        /// <summary>
        /// Parses the given messages and invokes the ConnectionNewValidMessage event for each message.
        /// </summary>
        /// <param name="data">The messages string to parse.</param>
        private async Task parseMessageAsync(string data)
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
                    await onConnectionErrorAsync();
                    await sendStreamCloseMessageAsync();
                    return;
                }

                // Filter IQ messages which ids are not valid:
                if (msg is IQMessage iq)
                {
                    if (iq.GetType().Equals(IQMessage.RESULT) && messageIdCache.getTimed(iq.ID) != null)
                    {
                        Logger.Warn("[XMPPConnection2]: Invalid message id received!");
                        return;
                    }
                }

                // Invoke message processors:
                NewValidMessage?.Invoke(this, new NewValidMessageEventArgs(msg));

                // Should restart connection?
                if (msg.getRestartConnection() != AbstractMessage.NO_RESTART)
                {
                    if (msg.getRestartConnection() == AbstractMessage.SOFT_RESTART)
                    {
                        await softRestartAsync();
                    }
                    else if (msg.getRestartConnection() == AbstractMessage.HARD_RESTART)
                    {
                        await hardRestartAsync();
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
                    streamId = oA.ID;
                }
                // Close stream message:
                else if (msg is CloseStreamMessage)
                {
                    switch (state)
                    {
                        case ConnectionState.CONNECTING:
                        case ConnectionState.CONNECTED:
                            await internalDisconnectAsync();
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
        internal void onOmemoSessionBuildError(OmemoSessionBuildErrorEventArgs args)
        {
            OmemoSessionBuildError?.Invoke(this, args);
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void Instance_NetworkChanged(object sender, EventArgs e)
        {
            if (NetworkHelper.Instance.ConnectionInformation.IsInternetAvailable)
            {
                if (!account.disabled && (state == ConnectionState.DISCONNECTED || state == ConnectionState.ERROR))
                {
                    Task t = reconnectAsync(true);
                }
            }
            else if (state == ConnectionState.CONNECTED)
            {
                Task t = internalDisconnectAsync();
            }
        }

        private async void RecourceBindingConnection_ResourceBound(object sender, EventArgs e)
        {
            stopConnectionTimer();

            // Send the initial presence message:
            await sendAsync(new PresenceMessage(account.presencePriorety, account.presence, account.status), true);

            setState(ConnectionState.CONNECTED);
        }

        private async void TCPConnection_NewDataReceived(TCPConnection2 connection, NewDataReceivedEventArgs args)
        {
            await parseMessageAsync(args.data);

            // Stop or reset the connection timer.
            switch (state)
            {
                case ConnectionState.CONNECTING:
                    resetConnectionTimer();
                    break;
                default:
                    stopConnectionTimer();
                    break;
            }
        }

        private async void TCPConnection_ConnectionStateChanged(AbstractConnection2 connection, ConnectionStateChangedEventArgs args)
        {
            switch (args.newState)
            {
                case ConnectionState.CONNECTED:
                    // TCP connection established start reading:
                    TCP_CONNECTION.startReaderTask();

                    // Send initial stream header:
                    await softRestartAsync();
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
                                await onConnectionErrorAsync();
                                break;

                            case ConnectionState.DISCONNECTING:
                                setState(ConnectionState.DISCONNECTED);
                                break;
                        }
                    }
                    else
                    {
                        // Unable to connect to server:
                        connectionErrorCount = 3;
                        await internalDisconnectAsync();
                        setState(ConnectionState.ERROR, args.param);
                    }
                    break;

                case ConnectionState.DISCONNECTED when holdConnection:
                    await reconnectAsync(false);
                    break;
            }
        }

        #endregion
    }
}
