using Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
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
        private TimedList<string> messageIdCache;

        public delegate void ConnectionNewValidMessageEventHandler(XMPPConnection handler, NewValidMessageEventArgs args);
        public delegate void MessageSendEventHandler(XMPPConnection handler, MessageSendEventArgs args);

        public event ConnectionNewValidMessageEventHandler ConnectionNewValidMessage;
        public event ConnectionNewValidMessageEventHandler ConnectionNewRoosterMessage;
        public event ConnectionNewValidMessageEventHandler ConnectionNewPresenceMessage;
        public event MessageSendEventHandler MessageSend;

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
            this.messageIdCache = new TimedList<string>();

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
        /// <param name="ignoreConnectionState">Don't message if there is no connection to the server.</param>
        public async Task sendAsync(AbstractMessage msg, bool ignoreConnectionState)
        {
            if (!ignoreConnectionState && state != ConnectionState.CONNECTED)
            {
                if (msg.shouldSaveUntilSend())
                {
                    MessageCache.INSTANCE.addMessage(account.getIdAndDomain(), msg);
                }
                else
                {
                    Logger.Warn("Did not send message, due to connection state is " + state + "\n" + msg.toXmlString());
                }
            }
            else
            {
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
            Task.Factory.StartNew(() => MessageSend?.Invoke(this, new MessageSendEventArgs(id, delayed)));
        }

        #endregion

        #region --Misc Methods (Protected)--
        protected async override Task cleanupAsync()
        {
            streamId = null;
            messageIdCache = new TimedList<string>();
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
            await sendAsync(openStreamMessage, true);
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
            List<AbstractMessage> messages;
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
                                    setState(ConnectionState.ERROR, lastErrorMessage);
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
            await sendAsync(new PresenceMessage(account.presencePriorety), true);
            setState(ConnectionState.CONNECTED);
            await sendAllOutstandingMessagesAsync();
        }

        #endregion
    }
}
