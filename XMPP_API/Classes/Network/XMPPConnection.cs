using Logging;
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

namespace XMPP_API.Classes.Network
{
    class XMPPConnection : AbstractConnection
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        /// <summary>
        /// The TCP connection to the server.
        /// </summary>
        private TCPConnection tCPConnection;

        /// <summary>
        /// Handling connection exceptions.
        /// </summary>
        private int connectionErrorCount;
        private string lastErrorMessage;

        /// <summary>
        /// For parsing received messages.
        /// </summary>
        private MessageParser2 parser;
        private AbstractMessageProcessor[] messageProcessors;
        private string streamId;
        private TSTimedList<string> messageIdCache;

        public delegate void ConnectionNewValidMessageEventHandler(XMPPConnection handler, NewValidMessageEventArgs args);
        public delegate void MessageSendEventHandler(XMPPConnection handler, MessageSendEventArgs args);

        public event ConnectionNewValidMessageEventHandler ConnectionNewValidMessage;
        public event ConnectionNewValidMessageEventHandler ConnectionNewRoosterMessage;
        public event ConnectionNewValidMessageEventHandler ConnectionNewPresenceMessage;
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
        private const int CONNECTION_TIMEOUT = 2000;
        private bool reconnectRequested;
        private TimeSpan timeout;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 07/12/2017 Created [Fabian Sauter]
        /// </history>
        public XMPPConnection(XMPPAccount account) : base(account)
        {
            this.connectionErrorCount = 0;
            this.lastErrorMessage = null;
            this.tCPConnection = new TCPConnection(account);
            this.tCPConnection.ConnectionStateChanged += TCPConnection_ConnectionStateChanged;
            this.tCPConnection.NewDataReceived += TCPConnection_NewDataReceived;

            this.parser = new MessageParser2();
            this.messageProcessors = new AbstractMessageProcessor[3];
            this.streamId = null;
            this.messageIdCache = new TSTimedList<string>();

            this.connectionTimer = null;
            this.reconnectRequested = false;
            this.timeout = TimeSpan.FromMilliseconds(CONNECTION_TIMEOUT);

            // The order in which new messages should get processed (TLS -- SASL -- Resource binding -- ...).
            // https://xmpp.org/extensions/xep-0170.html
            //-------------------------------------------------------------
            // TLS:
            this.messageProcessors[0] = (new TLSConnection(tCPConnection, this));

            // SASL:
            this.messageProcessors[1] = (new SASLConnection(tCPConnection, this));

            // Resource binding:
            RecourceBindingConnection recourceBindingConnection = new RecourceBindingConnection(tCPConnection, this);
            recourceBindingConnection.ResourceBound += RecourceBindingConnection_ResourceBound;
            this.messageProcessors[2] = (recourceBindingConnection);
            //-------------------------------------------------------------
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public override void setState(ConnectionState newState, object param)
        {
            base.setState(newState, param);
            switch (newState)
            {
                case ConnectionState.DISCONNECTED:
                    if (reconnectRequested)
                    {
                        Task.Run(async () => await internalConnectAsync());
                    }
                    break;
                case ConnectionState.ERROR:
                    Task.Run(async () => await disconnectAsync());
                    break;
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        /// <summary>
        /// Connects to the server and resets the connectionErrorCount.
        /// Will reconnect, if already connected or connecting.
        /// </summary>
        public async override Task connectAsync()
        {
            // Reset connection error count:
            connectionErrorCount = 0;

            // Connect:
            await internalConnectAsync();
        }

        /// <summary>
        /// Disconnects from the server.
        /// </summary>
        /// <returns></returns>
        public async override Task disconnectAsync()
        {
            stopConnectionTimer();
            if (state == ConnectionState.DISCONNECTED)
            {
                return;
            }

            setState(ConnectionState.DISCONNECTING);
            // Disconnect the TCPConnection:
            await tCPConnection.disconnectAsync();

            // Cleanup:
            await cleanupAsync();
            setState(ConnectionState.DISCONNECTED);
        }

        /// <summary>
        /// Reconnects to the server.
        /// </summary>
        public async Task reconnectAsync()
        {
            reconnectRequested = true;
            await disconnectAsync();
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
                if (Logger.logLevel == LogLevel.DEBUG)
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
                await tCPConnection.sendAsync(msg.toXmlString());
            }
            catch (Exception e)
            {
                Logger.Error("Error during sending message for account: " + account.getIdAndDomain(), e);
                if ((cacheIfNotConnected || msg.shouldSaveUntilSend()))
                {
                    MessageCache.INSTANCE.addMessage(account.getIdAndDomain(), msg);
                    return;
                }
            }

            // Only trigger onMessageSend(...) for chat messages:
            if (msg is MessageMessage)
            {
                onMessageSend(msg.getId(), false);
            }
        }

        public async Task onMessageProcessorFailedAsync(string errorMessage, bool criticalError)
        {
            if (criticalError)
            {
                lastErrorMessage = errorMessage;
                await onConnectionErrorAsync();
            }
        }

        #endregion

        #region --Misc Methods (Private)--
        /// <summary>
        /// Connects to the server.
        /// </summary>
        private async Task internalConnectAsync()
        {
            switch (state)
            {
                case ConnectionState.DISCONNECTED:
                case ConnectionState.ERROR:
                    setState(ConnectionState.CONNECTING);
                    await tCPConnection.connectAsync();
                    break;

                default:
                    Logger.Warn("[XMPPConnection]: Trying to connect but state is not " + ConnectionState.DISCONNECTED + " or " + ConnectionState.ERROR + "! State = " + state);
                    await reconnectAsync();
                    break;
            }
        }

        /// <summary>
        /// Triggers the MessageSend event in a new task.
        /// </summary>
        /// <param name="id">The id of the message that got send.</param>
        /// <param name="delayed">If the message got send delayed (e.g. stored in message cache).</param>
        private void onMessageSend(string id, bool delayed)
        {
            Task.Run(() => MessageSend?.Invoke(this, new MessageSendEventArgs(id, delayed)));
        }

        /// <summary>
        /// Resets the connection timer.
        /// </summary>
        private void resetConnectionTimer()
        {
            stopConnectionTimer();

            connectionTimer = ThreadPoolTimer.CreateTimer(async (source) => await onConnetionTimerTimeoutAsync(), timeout);
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
        /// Called once the connection timer got triggered.
        /// </summary>
        private async Task onConnetionTimerTimeoutAsync()
        {
            Logger.Warn("Connection timeout got triggered for account: " + account.getIdAndDomain());

            await onConnectionErrorAsync();
        }

        private async Task onConnectionErrorAsync()
        {
            if (++connectionErrorCount >= 3)
            {
                // Establishing the connection failed for the third time:
                await disconnectAsync();
                setState(ConnectionState.ERROR, lastErrorMessage);
            }
            else
            {
                // Try to reconnect:
                await reconnectAsync();
            }
        }

        #endregion

        #region --Misc Methods (Protected)--
        protected async override Task cleanupAsync()
        {
            streamId = null;
            messageIdCache = new TSTimedList<string>();
            resetMessageProcessors();
            lastErrorMessage = null;
        }

        protected void resetMessageProcessors()
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
        protected async Task softRestartAsync()
        {
            OpenStreamMessage openStreamMessage = new OpenStreamMessage(account.getIdAndDomain(), account.user.domain);
            await sendAsync(openStreamMessage, false, true);
        }

        /// <summary>
        /// Performs a reconnect.
        /// </summary>
        /// <returns></returns>
        protected async Task hardRestartAsync()
        {
            await reconnectAsync();
        }

        /// <summary>
        /// Sends all outstanding messages to the server.
        /// </summary>
        protected async Task sendAllOutstandingMessagesAsync()
        {
            foreach (MessageTable entry in MessageCache.INSTANCE.getAllForAccount(account.getIdAndDomain()))
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
                    await tCPConnection.sendAsync(entry.message);
                    MessageCache.INSTANCE.removeEntry(entry);

                    // Only trigger onMessageSend(...) for chat messages:
                    if (entry.isChatMessage)
                    {
                        onMessageSend(entry.messageId, true);
                    }
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Parses the given messages and invokes the ConnectionNewValidMessage event for each message.
        /// </summary>
        /// <param name="data">The messages string to parse.</param>
        protected async Task parseMessageAsync(string data)
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
                            await disconnectAsync();
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
            }
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private async void TCPConnection_NewDataReceived(AbstractConnection handler, NewDataReceivedEventArgs args)
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

        private async void TCPConnection_ConnectionStateChanged(AbstractConnection connection, ConnectionStateChangedEventArgs arg)
        {
            switch (arg.newState)
            {
                case ConnectionState.CONNECTED:
                    // TCP connection established start reading:
                    tCPConnection.startReaderTask();

                    // Send initial stream header:
                    await softRestartAsync();
                    break;

                case ConnectionState.ERROR:
                    if (arg.oldState == ConnectionState.CONNECTED)
                    {
                        switch (state)
                        {
                            case ConnectionState.CONNECTING:
                            case ConnectionState.CONNECTED:
                                lastErrorMessage = "Server TCP connection failed!";
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
                        await disconnectAsync();
                        setState(ConnectionState.ERROR, arg.param);
                    }
                    break;
            }
        }

        private async void RecourceBindingConnection_ResourceBound(object sender, EventArgs e)
        {
            stopConnectionTimer();
            await sendAsync(new PresenceMessage(account.presencePriorety), false, true);
            setState(ConnectionState.CONNECTED);
            await sendAllOutstandingMessagesAsync();
        }

        #endregion
    }
}
