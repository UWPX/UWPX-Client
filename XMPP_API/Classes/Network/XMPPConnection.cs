using Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Thread_Save_Components.Classes.Collections;
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
        private int connectionFaildCount;
        private string lastErrorMessage;

        /// <summary>
        /// For parsing received messages.
        /// </summary>
        private MessageParser2 parser;
        private ArrayList messageProcessors;
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
        private Timer connectionTimer;
        /// <summary>
        /// The connection timeout in ms.
        /// </summary>
        private static readonly int CONNECTION_TIMEOUT = 1000;

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
            this.connectionFaildCount = 0;
            this.lastErrorMessage = null;
            this.tCPConnection = new TCPConnection(account);
            this.tCPConnection.ConnectionStateChanged += TCPConnection_ConnectionStateChanged;
            this.tCPConnection.NewDataReceived += TCPConnection_NewDataReceived;

            this.parser = new MessageParser2();
            this.messageProcessors = new ArrayList(5);
            this.streamId = null;
            this.messageIdCache = new TSTimedList<string>();

            this.connectionTimer = null;

            // The order in which new messages should get processed (TLS -- SASL -- COMPRESSION -- ...).
            // https://xmpp.org/extensions/xep-0170.html
            //-------------------------------------------------------------
            this.messageProcessors.Add(new TLSConnection(tCPConnection, this));
            this.messageProcessors.Add(new SASLConnection(tCPConnection, this));
            RecourceBindingConnection recourceBindingConnection = new RecourceBindingConnection(tCPConnection, this);
            recourceBindingConnection.ResourceBound += RecourceBindingConnection_ResourceBound;
            this.messageProcessors.Add(recourceBindingConnection);
            //-------------------------------------------------------------
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public async override Task connectAsync()
        {
            switch (state)
            {
                case ConnectionState.DISCONNECTED:
                case ConnectionState.ERROR:
                    setState(ConnectionState.DISCONNECTING);
                    await tCPConnection.connectAsync();
                    break;
                default:
                    throw new InvalidOperationException("[XMPPConnection]: Unable to connect! state != Error or Disconnected! state = " + state);
            }
        }

        public async override Task disconnectAsync()
        {
            switch (state)
            {
                case ConnectionState.CONNECTING:
                case ConnectionState.CONNECTED:
                    setState(ConnectionState.DISCONNECTING);
                    // Disconnect the TCPConnection:
                    await tCPConnection.disconnectAsync();

                    // Cleanup:
                    await cleanupAsync();
                    setState(ConnectionState.DISCONNECTED);
                    break;
            }
        }

        /// <summary>
        /// Sends the given message to the server or stores it until there is a connection to the server.
        /// </summary>
        /// <param name="msg">The message to send.</param>
        /// <param name="cacheIfNotConnected">Cache the message if the connection state does not equals 'CONNECTED', to ensure the message doesn't get lost.</param>
        /// <param name="sendIfNotConnected">Sends the message if the underlaying TCP connection is connected to the server and ignores the connection state of the XMPPConnection.</param>
        public async Task sendAsync(AbstractMessage msg, bool cacheIfNotConnected, bool sendIfNotConnected)
        {
            if (state != ConnectionState.CONNECTED && !sendIfNotConnected)
            {
                Logger.Warn("Did not send message, due to connection state is " + state + "\n" + msg.toXmlString());
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
            await tCPConnection.sendAsync(msg.toXmlString());

            // Only trigger onMessageSend(...) for chat messages:
            if (msg is MessageMessage)
            {
                onMessageSend(msg.getId(), false);
            }
        }

        #endregion

        #region --Misc Methods (Private)--
        private async Task reconnectAsync()
        {
            await disconnectAsync();
            await connectAsync();
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
            connectionTimer = new Timer(async (o) => await onConnetionTimerTimeoutAsync(), null, CONNECTION_TIMEOUT, Timeout.Infinite);
        }

        /// <summary>
        /// Stops the connection timer by disposing it.
        /// </summary>
        private void stopConnectionTimer()
        {
            connectionTimer?.Dispose();
            connectionTimer = null;
        }

        /// <summary>
        /// Called once the connection timer got triggered.
        /// </summary>
        private async Task onConnetionTimerTimeoutAsync()
        {
            Logger.Warn("Connection timer got triggered for account: " + account.getIdAndDomain());

            if (++connectionFaildCount >= 3)
            {
                // Establishing the connection failed for the third time:
                setState(ConnectionState.ERROR, lastErrorMessage);
            }
            else
            {
                // Try to reconnect:
                connectionFaildCount++;
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
            connectionFaildCount = 0;
            lastErrorMessage = null;
        }

        protected void resetMessageProcessors()
        {
            for (int i = 0; i < messageProcessors.Count; i++)
            {
                (messageProcessors[i] as AbstractMessageProcessor).reset();
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
            resetConnectionTimer();
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
                if (Consts.ENABLE_DEBUG_OUTPUT)
                {
                    Debug.WriteLine("Error during message parsing: " + e.Message + "\n" + e.StackTrace + "\n" + data);
                }
                return;
            }

            // Process messages:
            foreach (AbstractMessage message in messages)
            {
                // Filter IQ messages which ids are not valid:
                if (message is IQMessage)
                {
                    IQMessage iq = message as IQMessage;
                    if (iq.GetType().Equals(IQMessage.RESULT) && messageIdCache.getTimed(iq.getId()) != null)
                    {
                        Logger.Info("Invalid message id received!");
                        return;
                    }
                }

                // Invoke message processors:
                ConnectionNewValidMessage?.Invoke(this, new NewValidMessageEventArgs(message));

                // Should restart connection?
                if (message.getRestartConnection() != AbstractMessage.NO_RESTART)
                {
                    if (message.getRestartConnection() == AbstractMessage.SOFT_RESTART)
                    {
                        await softRestartAsync();
                    }
                    else if (message.getRestartConnection() == AbstractMessage.HARD_RESTART)
                    {
                        await hardRestartAsync();
                    }
                    else
                    {
                        throw new ArgumentException("Invalid restart type: " + message.getRestartConnection());
                    }
                }

                // Filter already processed messages:
                if (message.isProcessed())
                {
                    return;
                }

                // --------------------------------------------------------------------
                // Open stream:
                if (message is OpenStreamAnswerMessage)
                {
                    OpenStreamAnswerMessage oA = message as OpenStreamAnswerMessage;
                    if (oA.getId() == null)
                    {
                        // TODO Handle OpenStreamAnswerMessage id == null
                        //Error throw exception?!
                        return;
                    }
                    streamId = oA.getId();
                }
                // Rooster:
                else if (message is RosterMessage)
                {
                    ConnectionNewRoosterMessage?.Invoke(this, new NewValidMessageEventArgs(message));
                }
                // Presence:
                else if (message is PresenceMessage && (message as PresenceMessage).getFrom() != null)
                {
                    ConnectionNewPresenceMessage?.Invoke(this, new NewValidMessageEventArgs(message));
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
                        connectionFaildCount++;
                        switch (state)
                        {
                            case ConnectionState.CONNECTING:
                            case ConnectionState.CONNECTED:
                                if (connectionFaildCount >= 3)
                                {
                                    // Establishing the connection failed for the third time:
                                    setState(ConnectionState.ERROR, "Server did not respond during connection for " + CONNECTION_TIMEOUT + "ms.");
                                }
                                else
                                {
                                    // Try to reconnect:
                                    connectionFaildCount++;
                                    await reconnectAsync();
                                }
                                break;

                            case ConnectionState.DISCONNECTING:
                                setState(ConnectionState.DISCONNECTED);
                                break;
                        }
                    }
                    else
                    {
                        // Unable to connect to server:
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
