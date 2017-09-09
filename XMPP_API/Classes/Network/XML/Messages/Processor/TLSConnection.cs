using System;
using System.Diagnostics;
using System.Threading.Tasks;
using XMPP_API.Classes.Network.Events;
using XMPP_API.Classes.Network.TCP;
using XMPP_API.Classes.Network.XML.Messages.Features;
using XMPP_API.Classes.Network.XML.Messages.Features.TLS;

namespace XMPP_API.Classes.Network.XML.Messages.Processor
{
    class TLSConnection : AbstractMessageProcessor
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private TLSState state;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Construktoren--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 21/08/2017 Created [Fabian Sauter]
        /// </history>
        public TLSConnection(TCPConnectionHandler tcpConnection, XMPPConnectionHandler xMPPConnection) : base(tcpConnection, xMPPConnection)
        {
            reset();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public TLSState getState()
        {
            return state;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public override void reset()
        {
            state = TLSState.DISCONNECTED;
        }

        #endregion

        #region --Misc Methods (Private)--
        private async Task streamFeaturesMessageReceivedAsync(StreamFeaturesMessage features, NewPresenceEventArgs args)
        {
            if (tLSReqired(features))
            {
                // Starting the TSL process:
                setMessageProcessed(args);
                state = TLSState.CONNECTING;
                await XMPP_CONNECTION.sendMessageAsync(new RequesStartTLSMessage());
                state = TLSState.REQUESTED;
            }
        }

        private bool tLSReqired(StreamFeaturesMessage msg)
        {
            foreach (StreamFeature f in msg.getFeatures())
            {
                if (f is TLSStreamFeature)
                {
                    return f.isRequired();
                }
            }
            return false;
        }

        #endregion

        #region --Misc Methods (Protected)--
        protected async override void processMessage(NewPresenceEventArgs args)
        {
            AbstractMessage msg = args.getMessage();
            if(state == TLSState.CONNECTED || msg.isProcessed())
            {
                return;
            }
            if (msg is OpenStreamAnswerMessage)
            {
                StreamFeaturesMessage features = (msg as OpenStreamAnswerMessage).getStreamFeaturesMessage();
                if(features != null)
                {
                    await streamFeaturesMessageReceivedAsync(features, args);
                }
            }
            else if(msg is StreamFeaturesMessage)
            {
                await streamFeaturesMessageReceivedAsync(msg as StreamFeaturesMessage, args);
            }
            else if (msg is ProceedAnswerMessage)
            {
                setMessageProcessed(args);
                try
                {
                    // Has to be wait, because if it us await the main thread will continue ==> no soft restart!
                    TCP_CONNECTION.upgradeToTLS().Wait();
                    ServerConnectionConfiguration sCC = XMPP_CONNECTION.getSeverConnectionConfiguration();

                    // TLS established ==> resend stream header
                    msg.setRestartConnection(AbstractMessage.SOFT_RESTART);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Unable to establish TLS connection! " + e.Message + "\n" + e.StackTrace);
                    state = TLSState.ERROR;
                }
            }
            else if ((state == TLSState.CONNECTING || state == TLSState.REQUESTED) && msg is ErrorMessage)
            {
                setMessageProcessed(args);
                state = TLSState.ERROR;
                ErrorMessage error = msg as ErrorMessage;
                if(error.getType().Equals(Consts.XML_FAILURE))
                {
                    error.setRestartConnection(AbstractMessage.HARD_RESTART);
                }
            }
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
