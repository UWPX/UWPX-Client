using Logging;
using Microsoft.Toolkit.Uwp.Connectivity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Thread_Save_Components.Classes.Collections;
using Windows.System.Threading;
using XMPP_API.Classes.Network.Events;
using XMPP_API.Classes.Network.TCP;
using XMPP_API.Classes.Network.XML;
using XMPP_API.Classes.Network.XML.DBEntries;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.Processor;
using XMPP_API.Classes.Network.XML.Messages.XEP_0048;

namespace XMPP_API.Classes.Network
{
    public class XMPPConnection2 : AbstractConnection2
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly TCPConnection2 TCP_CONNECTION;

        public ConnectionError lastConnectionError;
        private int connectionErrorCount;

        private bool holdConnection;

        /// <summary>
        /// For parsing received messages.
        /// </summary>
        private MessageParser2 parser;
        private AbstractMessageProcessor[] messageProcessors;
        private string streamId;
        private TSTimedList<string> messageIdCache;

        public delegate void ConnectionNewValidMessageEventHandler(XMPPConnection2 connection, NewValidMessageEventArgs args);
        public delegate void MessageSendEventHandler(XMPPConnection2 connection, MessageSendEventArgs args);
        public delegate void NewBookmarksResultMessageEventHandler(XMPPConnection2 connection, NewBookmarksResultMessageEventArgs args);

        public event ConnectionNewValidMessageEventHandler ConnectionNewValidMessage;
        public event ConnectionNewValidMessageEventHandler ConnectionNewRoosterMessage;
        public event ConnectionNewValidMessageEventHandler ConnectionNewPresenceMessage;
        public event NewBookmarksResultMessageEventHandler NewBookmarksResultMessage;
        public event MessageSendEventHandler MessageSend;

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

            this.parser = new MessageParser2();
            this.messageProcessors = new AbstractMessageProcessor[4];
            this.streamId = null;
            this.messageIdCache = new TSTimedList<string>();

            this.connectionTimer = null;
            this.reconnectRequested = false;
            this.timeout = TimeSpan.FromMilliseconds(CONNECTION_TIMEOUT);

            // The order in which new messages should get processed (TLS -- SASL -- Stream Management -- Resource binding -- ...).
            // https://xmpp.org/extensions/xep-0170.html
            //-------------------------------------------------------------
            // TLS:
            this.messageProcessors[0] = new TLSConnection(TCP_CONNECTION, this);

            // SASL:
            this.messageProcessors[1] = new SASLConnection(TCP_CONNECTION, this);

            // XEP-0198 (Stream Management):
            this.messageProcessors[2] = new SMConnection(TCP_CONNECTION, this);

            // Resource binding:
            RecourceBindingConnection recourceBindingConnection = new RecourceBindingConnection(TCP_CONNECTION, this);
            recourceBindingConnection.ResourceBound += RecourceBindingConnection_ResourceBound;
            this.messageProcessors[3] = recourceBindingConnection;
            //-------------------------------------------------------------

            NetworkHelper.Instance.NetworkChanged += Instance_NetworkChanged;
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

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
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
        /// Sends the given message to the server or stores it until there is a connection to the server.
        /// </summary>
        /// <param name="msg">The message to send.</param>
        /// <param name="cacheIfNotConnected">Cache the message if the connection state does not equals 'CONNECTED', to ensure the message doesn't get lost.</param>
        /// <param name="sendIfNotConnected">Sends the message if the underlaying TCP connection is connected to the server and ignores the connection state of the XMPPConnection.</param>
        public async Task sendAsync(AbstractMessage msg, bool cacheIfNotConnected, bool sendIfNotConnected)
        {
            if (state == ConnectionState.CONNECTING)
            {
                resetConnectionTimer();
            }

            if (state != ConnectionState.CONNECTED && !sendIfNotConnected)
            {
                if (Logger.logLevel >= LogLevel.DEBUG)
                {
                    Logger.Warn("Did not send message, due to connection state is " + state + "\n" + msg.toXmlString());
                }
                else
                {
                    Logger.Warn("Did not send message, due to connection state is " + state);
                }

                if ((cacheIfNotConnected || msg.shouldSaveUntilSend()))
                {
                    MessageCache.INSTANCE.addMessage(account.getIdAndDomain(), msg);
                }
                if (!sendIfNotConnected)
                {
                    return;
                }
            }

            if (msg is IQMessage && msg.getId() != null)
            {
                messageIdCache.addTimed(msg.getId());
            }
            try
            {
                if (await TCP_CONNECTION.sendAsync(msg.toXmlString()))
                {
                    // Only trigger onMessageSend(...) for chat messages:
                    if (msg is MessageMessage m)
                    {
                        onMessageSend(msg.getId(), m.chatMessageId, false);
                        return;
                    }
                }
                else
                {
                    Logger.Error("Error during sending message for account: " + account.getIdAndDomain());
                }
            }
            catch (Exception e)
            {
                Logger.Error("Error during sending message for account: " + account.getIdAndDomain(), e);
            }

            if ((cacheIfNotConnected || msg.shouldSaveUntilSend()))
            {
                MessageCache.INSTANCE.addMessage(account.getIdAndDomain(), msg);
            }
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
        /// Sends all outstanding messages to the server.
        /// </summary>
        private async Task sendAllOutstandingMessagesAsync()
        {
            List<MessageTable> list = MessageCache.INSTANCE.getAllForAccount(account.getIdAndDomain());
            foreach (MessageTable entry in list)
            {
                if (state != ConnectionState.CONNECTED)
                {
                    return;
                }
                try
                {
                    if (entry.messageId != null)
                    {
                        messageIdCache.addTimed(entry.messageId);
                    }
                    if (await TCP_CONNECTION.sendAsync(entry.message))
                    {
                        MessageCache.INSTANCE.removeEntry(entry);

                        // Only trigger onMessageSend(...) for chat messages:
                        if (entry.isChatMessage)
                        {
                            onMessageSend(entry.messageId, entry.chatMessageId, true);
                        }
                    }
                }
                catch
                {
                }
            }
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
            string errorMessage = "Connection timeout got triggered for account: " + account.getIdAndDomain();
            Logger.Warn(errorMessage);
            lastConnectionError = new ConnectionError(ConnectionErrorCode.XMPP_CONNECTION_TIMEOUT, errorMessage);

            await onConnectionErrorAsync();
        }

        private async Task onConnectionErrorAsync()
        {
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
            if (TCP_CONNECTION.state == ConnectionState.CONNECTED)
            {
                await TCP_CONNECTION.sendAsync(Consts.XML_STREAM_CLOSE);
            }
        }

        private void resetMessageProcessors()
        {
            foreach (AbstractMessageProcessor mP in messageProcessors)
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
            OpenStreamMessage openStreamMessage = new OpenStreamMessage(account.getIdAndDomain(), account.user.domain);
            await sendAsync(openStreamMessage, false, true);
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
                messages = parser.parseMessages(data);
            }
            catch (Exception e)
            {
                Logger.Error("Error during message parsing." + e);
                return;
            }

            // Process messages:
            foreach (AbstractMessage msg in messages)
            {
                // Filter IQ messages which ids are not valid:
                if (msg is IQMessage)
                {
                    IQMessage iq = msg as IQMessage;
                    if (iq.GetType().Equals(IQMessage.RESULT) && messageIdCache.getTimed(iq.getId()) != null)
                    {
                        Logger.Warn("Invalid message id received!");
                        return;
                    }
                }

                // Invoke message processors:
                ConnectionNewValidMessage?.Invoke(this, new NewValidMessageEventArgs(msg));

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
                        throw new ArgumentException("Invalid restart type: " + msg.getRestartConnection());
                    }
                }

                // Filter already processed messages:
                if (msg.isProcessed())
                {
                    return;
                }

                // --------------------------------------------------------------------
                // Open stream:
                if (msg is OpenStreamAnswerMessage)
                {
                    OpenStreamAnswerMessage oA = msg as OpenStreamAnswerMessage;
                    if (oA.getId() == null)
                    {
                        // TODO Handle OpenStreamAnswerMessage id == null
                        //Error throw exception?!
                        return;
                    }
                    streamId = oA.getId();
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
                else if (msg is RosterMessage)
                {
                    ConnectionNewRoosterMessage?.Invoke(this, new NewValidMessageEventArgs(msg));
                }
                // Presence:
                else if (msg is PresenceMessage && (msg as PresenceMessage).getFrom() != null)
                {
                    ConnectionNewPresenceMessage?.Invoke(this, new NewValidMessageEventArgs(msg));
                }
                // Bookmarks:
                else if (msg is BookmarksResultMessage)
                {
                    NewBookmarksResultMessage?.Invoke(this, new NewBookmarksResultMessageEventArgs(msg as BookmarksResultMessage));
                }
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


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
            await sendAsync(new PresenceMessage(account.presencePriorety, account.presence, account.status), false, true);

            setState(ConnectionState.CONNECTED);

            await sendAllOutstandingMessagesAsync();
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
            }
        }

        #endregion
    }
}
