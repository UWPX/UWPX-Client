using Logging;
using System;
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
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 21/08/2017 Created [Fabian Sauter]
        /// </history>
        public TLSConnection(TCPConnection tcpConnection, XMPPConnection xMPPConnection) : base(tcpConnection, xMPPConnection)
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
        private async Task streamFeaturesMessageReceivedAsync(StreamFeaturesMessage features, NewValidMessageEventArgs args)
        {
            if (tLSReqired(features))
            {
                // Starting the TSL process:
                setMessageProcessed(args);
                state = TLSState.CONNECTING;
                await XMPP_CONNECTION.sendAsync(new RequesStartTLSMessage(), false, true);
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
        protected async override Task processMessageAsync(NewValidMessageEventArgs args)
        {
            AbstractMessage msg = args.getMessage();
            if (msg.isProcessed())
            {
                return;
            }
            if ((state == TLSState.CONNECTING || state == TLSState.REQUESTED) && msg is ErrorMessage)
            {
                setMessageProcessed(args);
                state = TLSState.ERROR;
                ErrorMessage error = msg as ErrorMessage;
                if (error.getType().Equals(Consts.XML_FAILURE))
                {
                    error.setRestartConnection(AbstractMessage.HARD_RESTART);
                }
                return;
            }
            switch (state)
            {
                case TLSState.DISCONNECTED:
                case TLSState.CONNECTING:
                    if (msg is OpenStreamAnswerMessage)
                    {
                        StreamFeaturesMessage features = (msg as OpenStreamAnswerMessage).getStreamFeaturesMessage();
                        if (features != null)
                        {
                            await streamFeaturesMessageReceivedAsync(features, args);
                        }
                    }
                    else if (msg is StreamFeaturesMessage)
                    {
                        await streamFeaturesMessageReceivedAsync(msg as StreamFeaturesMessage, args);
                    }
                    break;

                case TLSState.REQUESTED:
                    if (msg is ProceedAnswerMessage)
                    {
                        setMessageProcessed(args);

                        XMPPAccount account = XMPP_CONNECTION.account;
                        Logger.Debug("Upgrading " + account.getIdAndDomain() + " connection to TLS...");
                        try
                        {
                            TCP_CONNECTION.upgradeToTLSAsync().Wait();
                        }
                        catch (AggregateException e)
                        {
                            if(e.InnerException is TaskCanceledException)
                            {
                                Logger.Error("Timeout during upgrading " + account.getIdAndDomain() + " to TLS!", e);
                                state = TLSState.ERROR;
                                await XMPP_CONNECTION.onMessageProcessorFailedAsync("TSL upgrading timeout!", true);
                                return;
                            }
                            else
                            {
                                Logger.Error("Error during upgrading " + account.getIdAndDomain() + " to TLS!", e.InnerException);
                                state = TLSState.ERROR;
                                await XMPP_CONNECTION.onMessageProcessorFailedAsync(e.InnerException?.Message, true);
                                return;
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.Error("Error during upgrading " + account.getIdAndDomain() + " to TLS!", e);
                            state = TLSState.ERROR;
                            await XMPP_CONNECTION.onMessageProcessorFailedAsync(e.Message, true);
                            return;
                        }
                        Logger.Debug("Success upgrading " + account.getIdAndDomain() + " to TLS.");

                        // TLS established ==> resend stream header
                        msg.setRestartConnection(AbstractMessage.SOFT_RESTART);
                    }
                    break;
            }
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
