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
        public TLSConnection(TCPConnection2 tcpConnection, XMPPConnection2 xMPPConnection) : base(tcpConnection, xMPPConnection)
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

        private void setState(TLSState state)
        {
            this.state = state;

            // Update account connection info:
            ConnectionInformation connectionInfo = TCP_CONNECTION.account.CONNECTION_INFO;
            connectionInfo.tlsConnected = state == TLSState.CONNECTED;
        }

        public TLSStreamFeature getTLSStreamFeature(StreamFeaturesMessage msg)
        {
            foreach (StreamFeature f in msg.getFeatures())
            {
                if (f is TLSStreamFeature)
                {
                    return f as TLSStreamFeature;
                }
            }
            return null;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public override void reset()
        {
            setState(TLSState.DISCONNECTED);
            startListeningForMessages();
        }

        #endregion

        #region --Misc Methods (Private)--
        private async Task streamFeaturesMessageReceivedAsync(StreamFeaturesMessage features, NewValidMessageEventArgs args)
        {
            TLSStreamFeature tlsFeature = getTLSStreamFeature(features);
            TLSConnectionMode connectionMode = TCP_CONNECTION.account.connectionConfiguration.tlsMode;

            if (tlsFeature != null)
            {
                if (connectionMode == TLSConnectionMode.PROHIBIT)
                {
                    if (tlsFeature.REQUIRED)
                    {
                        stopListeningForMessages();
                        string errorMsg = "TSL is required for server but TLS connection mode is set to prohibit!";
                        Logger.Error(errorMsg);
                        setState(TLSState.ERROR);
                        await XMPP_CONNECTION.onMessageProcessorFailedAsync(new ConnectionError(ConnectionErrorCode.TLS_CONNECTION_FAILED, errorMsg), true);
                    }
                    else
                    {
                        setState(TLSState.PROHIBITED);
                    }
                    return;
                }

                // Starting the TSL process:
                setMessageProcessed(args);
                setState(TLSState.CONNECTING);
                await XMPP_CONNECTION.sendAsync(new RequesStartTLSMessage(), false, true);
                setState(TLSState.REQUESTED);
            }
            else
            {
                if (connectionMode == TLSConnectionMode.FORCE)
                {
                    stopListeningForMessages();
                    string errorMsg = "TSL is not available for this server but TLS connection mode is set to force!";
                    Logger.Error(errorMsg);
                    setState(TLSState.ERROR);
                    await XMPP_CONNECTION.onMessageProcessorFailedAsync(new ConnectionError(ConnectionErrorCode.TLS_CONNECTION_FAILED, errorMsg), true);
                    return;
                }
            }
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
                setState(TLSState.ERROR);
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
                            if (e.InnerException is TaskCanceledException)
                            {
                                Logger.Error("Timeout during upgrading " + account.getIdAndDomain() + " to TLS!", e);
                                setState(TLSState.ERROR);
                                await XMPP_CONNECTION.onMessageProcessorFailedAsync(new ConnectionError(ConnectionErrorCode.TLS_CONNECTION_FAILED, "TSL upgrading timeout!"), true);
                                return;
                            }
                            else
                            {
                                Logger.Error("Error during upgrading " + account.getIdAndDomain() + " to TLS!", e.InnerException);
                                setState(TLSState.ERROR);
                                await XMPP_CONNECTION.onMessageProcessorFailedAsync(new ConnectionError(ConnectionErrorCode.TLS_CONNECTION_FAILED, e.InnerException?.Message), true);
                                return;
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.Error("Error during upgrading " + account.getIdAndDomain() + " to TLS!", e);
                            setState(TLSState.ERROR);
                            await XMPP_CONNECTION.onMessageProcessorFailedAsync(new ConnectionError(ConnectionErrorCode.TLS_CONNECTION_FAILED, e.Message), true);
                            return;
                        }

                        Logger.Debug("Success upgrading " + account.getIdAndDomain() + " to TLS.");

                        setState(TLSState.CONNECTED);
                        stopListeningForMessages();

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
