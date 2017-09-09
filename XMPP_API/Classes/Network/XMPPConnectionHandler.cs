using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using XMPP_API.Classes.Exceptions;
using XMPP_API.Classes.Network.Events;
using XMPP_API.Classes.Network.TCP;
using XMPP_API.Classes.Network.XML;
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
        #region --Construktoren--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 17/08/2017 Created [Fabian Sauter]
        /// </history>
        public XMPPConnectionHandler(ServerConnectionConfiguration sCC) : base(sCC)
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


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
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
            await disconnectFromServerAsync(null);
        }

        public async Task disconnectFromServerAsync(string reason)
        {
            setState(ConnectionState.DISCONNECTING);
            await tcpConnection.sendMessageToServerAsync(new CloseStreamMessage(reason).toXmlString());
            await tcpConnection.disconnectFromServerAsync();
            cleanupConnection();
            setState(ConnectionState.DISCONNECTED);
        }

        public async Task sendMessageAsync(AbstractMessage msg)
        {
            if (msg is IQMessage)
            {
                iDCash.add((msg as IQMessage).getId());
            }
            await tcpConnection.sendMessageToServerAsync(msg.toXmlString());
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
            OpenStreamMessage openStreamMessage = new OpenStreamMessage(SCC.getIdAndDomain(), SCC.user.domain);
            await sendMessageAsync(openStreamMessage);
        }

        private async Task hardRestart()
        {
            await disconnectFromServerAsync();
            await connectToServerAsync();

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
            await sendMessageAsync(new PresenceMessage(SCC.presencePriorety));
            setState(ConnectionState.CONNECTED);
        }

        private void TcpConnection_ConnectionStateChanged(AbstractConnectionHandler handler, ConnectionState state)
        {
            Debug.WriteLine("New state: " + state);
        }

        private async void TcpConnection_ConnectionNewData(AbstractConnectionHandler handler, NewDataEventArgs args)
        {
            Debug.WriteLine("New Data recieved: " + args.getData());
            // Parse message:
            List<AbstractMessage> messages;
            try
            {
                messages = parser.parseMessages(args.getData());
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error during message parsing: " + e.Message + "\n" + e.StackTrace + "\n" + args.getData());
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
                        Debug.WriteLine("Invalid message id recived!");
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
                else if (message is RoosterMessage)
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
