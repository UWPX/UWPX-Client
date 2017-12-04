using Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using XMPP_API.Classes.Exceptions;
using XMPP_API.Classes.Network.Events;
using XMPP_API.Classes.Network.TCP;
using XMPP_API.Classes.Network.XML;
using XMPP_API.Classes.Network.XML.DBEntries;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.Processor;

namespace XMPP_API.Classes.Network
{
    public class XMPPConnectionHandler : AbstractConnectionHandler
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private TCPConnectionHandler tcpConnection;
        private MessageParser2 parser;
        private ArrayList messageProcessors;
        private string streamId;
        private MessageIdCash iDCash;

        public delegate void ConnectionNewValidMessageEventHandler(XMPPConnectionHandler handler, NewPresenceEventArgs args);

        public event ConnectionNewValidMessageEventHandler ConnectionNewValidMessage;
        public event ConnectionNewValidMessageEventHandler ConnectionNewRoosterMessage;
        public event ConnectionNewValidMessageEventHandler ConnectionNewPresenceMessage;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 17/08/2017 Created [Fabian Sauter]
        /// </history>
        public XMPPConnectionHandler(XMPPAccount sCC) : base(sCC)
        {
            this.tcpConnection = new TCPConnectionHandler(sCC);
            this.parser = new MessageParser2();
            this.messageProcessors = new ArrayList(5);
            this.streamId = null;
            this.iDCash = new MessageIdCash();

            // The order in which new message should get processed (TLS -- SASL -- COMPRESSION -- ...).
            // https://xmpp.org/extensions/xep-0170.html
            //-------------------------------------------------------------
            this.messageProcessors.Add(new TLSConnection(tcpConnection, this));
            this.messageProcessors.Add(new SASLConnection(tcpConnection, this));
            RecourceBindingConnection recourceBindingConnection = new RecourceBindingConnection(tcpConnection, this);
            recourceBindingConnection.ResourceBound += RecourceBindingConnection_ResourceBound;
            this.messageProcessors.Add(recourceBindingConnection);
            //-------------------------------------------------------------

            tcpConnection.ConnectionConnected += TcpConnection_ConnectionConnected;
            tcpConnection.ConnectionNewData += TcpConnection_ConnectionNewData;
            tcpConnection.ConnectionStateChanged += TcpConnection_ConnectionStateChanged;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public SocketErrorStatus getSocketErrorStatus()
        {
            return tcpConnection.getSocketErrorStatus();
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public async Task transferSocketOwnershipAsync()
        {
            await tcpConnection?.transferOwnershipAsync();
        }

        public override async Task connectToServerAsync()
        {
            if (getState() == ConnectionState.CONNECTED || getState() == ConnectionState.CONNECTING)
            {
                throw new XMPPGeneralException("Unable to connect! Already connecting or connected! - XMPPConnectionHandler");
            }
            setState(ConnectionState.CONNECTING);
            tcpConnection.connectAndHoldConnection();
        }

        public override async Task disconnectFromServerAsync()
        {
            await disconnectFromServerAsync("");
        }

        public async Task disconnectFromServerAsync(string reason)
        {
            setState(ConnectionState.DISCONNECTING);
            try
            {
                tcpConnection.sendMessageToServerAsync(new CloseStreamMessage(reason).toXmlString()).Wait(TimeSpan.FromSeconds(1));
            }
            catch (Exception e)
            {
                Logger.Warn("Unable to send close message! " + e.Message);
            }
            tcpConnection.disconnectFromServerAsync().Wait(TimeSpan.FromSeconds(1));
            cleanupConnection();
            setState(ConnectionState.DISCONNECTED);
        }

        public async Task sendMessageAsync(AbstractMessage msg, bool ignoreConnectionState)
        {
            if (!ignoreConnectionState && getState() != ConnectionState.CONNECTED)
            {
                if (msg.shouldSaveUntilSend())
                {
                    MessageCache.INSTANCE.addMessage(ACCOUNT.getIdAndDomain(), msg);
                }
                else
                {
                    Logger.Warn("Did not send message, due to connection state is " + getState() + "\n" + msg.toXmlString());
                }
            }
            else
            {
                if (msg is IQMessage)
                {
                    iDCash.add((msg as IQMessage).getId());
                }
                await tcpConnection.sendMessageToServerAsync(msg.toXmlString());
            }
        }

        #endregion

        #region --Misc Methods (Private)--
        private void resetMessageProcessors()
        {
            for (int i = 0; i < messageProcessors.Count; i++)
            {
                (messageProcessors[i] as AbstractMessageProcessor).reset();
            }
        }

        private async Task softRestart()
        {
            OpenStreamMessage openStreamMessage = new OpenStreamMessage(ACCOUNT.getIdAndDomain(), ACCOUNT.user.domain);
            await sendMessageAsync(openStreamMessage, true);
        }

        private async Task hardRestart()
        {
            await disconnectFromServerAsync();
            await connectToServerAsync();

        }

        private async Task sendAllOutstandingMessagesAsync()
        {
            foreach (MessageEntry entry in MessageCache.INSTANCE.getAllForAccount(ACCOUNT.getIdAndDomain()))
            {
                if (getState() != ConnectionState.CONNECTED)
                {
                    return;
                }
                try
                {
                    if (entry.iQMessageId != null)
                    {
                        iDCash.add(entry.iQMessageId);
                    }
                    await tcpConnection.sendMessageToServerAsync(entry.message);
                    MessageCache.INSTANCE.removeEntry(entry);
                }
                catch
                {
                }
            }
        }

        #endregion

        #region --Misc Methods (Protected)--
        protected override void cleanupConnection()
        {
            resetMessageProcessors();
            streamId = null;
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private async void RecourceBindingConnection_ResourceBound(object sender, EventArgs e)
        {
            await sendMessageAsync(new PresenceMessage(ACCOUNT.presencePriorety), true);
            setState(ConnectionState.CONNECTED);
            await sendAllOutstandingMessagesAsync();
        }

        private void TcpConnection_ConnectionStateChanged(AbstractConnectionHandler handler, ConnectionState state)
        {
            if (Consts.ENABLE_DEBUG_OUTPUT)
            {
                Debug.WriteLine("[XMPPConnectionHandler] New state: " + state);
            }
            if (state == ConnectionState.ERROR)
            {
                setState(ConnectionState.ERROR);
                cleanupConnection();
            }
        }

        private async void TcpConnection_ConnectionNewData(AbstractConnectionHandler handler, NewDataEventArgs args)
        {
            if (Consts.ENABLE_DEBUG_OUTPUT)
            {
                Logger.Info("Data received from (" + ACCOUNT.serverAddress + "):" + args.getData());
            }
            // Parse message:
            List<AbstractMessage> messages;
            try
            {
                messages = parser.parseMessages(args.getData());
            }
            catch (Exception e)
            {
                Logger.Error("Error during message parsing." + e);
                if (Consts.ENABLE_DEBUG_OUTPUT)
                {
                    Debug.WriteLine("Error during message parsing: " + e.Message + "\n" + e.StackTrace + "\n" + args.getData());
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
                    if (iq.GetType().Equals(IQMessage.RESULT) && !iDCash.contains(iq.getId()))
                    {
                        Logger.Info("Invalid message id received!");
                        return;
                    }
                }

                // Invoke message processors:
                ConnectionNewValidMessage?.Invoke(this, new NewPresenceEventArgs(message));

                // Should restart connection?
                if (message.getRestartConnection() != AbstractMessage.NO_RESTART)
                {
                    if (message.getRestartConnection() == AbstractMessage.SOFT_RESTART)
                    {
                        await softRestart();
                    }
                    else if (message.getRestartConnection() == AbstractMessage.HARD_RESTART)
                    {
                        await hardRestart();
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
                    ConnectionNewRoosterMessage?.Invoke(this, new NewPresenceEventArgs(message));
                }
                // Presence:
                else if (message is PresenceMessage && (message as PresenceMessage).getFrom() != null)
                {
                    ConnectionNewPresenceMessage?.Invoke(this, new NewPresenceEventArgs(message));
                }

            }
        }

        private async void TcpConnection_ConnectionConnected(AbstractConnectionHandler handler, EventArgs args)
        {
            await softRestart();
        }

        #endregion
    }
}
