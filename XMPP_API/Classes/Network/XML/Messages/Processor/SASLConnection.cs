using Logging;
using System.Collections;
using System.Threading.Tasks;
using XMPP_API.Classes.Network.Events;
using XMPP_API.Classes.Network.TCP;
using XMPP_API.Classes.Network.XML.Messages.Features;
using XMPP_API.Classes.Network.XML.Messages.Features.SASL;
using XMPP_API.Classes.Network.XML.Messages.Features.SASL.Plain;
using XMPP_API.Classes.Network.XML.Messages.Features.SASL.SHA1;

namespace XMPP_API.Classes.Network.XML.Messages.Processor
{
    class SASLConnection : AbstractMessageProcessor
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private SASLState state;
        private AbstractSASLMechanism selectedMechanism;
        private static readonly ArrayList OFFERED_MECHANISMS = new ArrayList() { "scram-sha-1" };
        //private static readonly ArrayList OFFERED_MECHANISMS = new ArrayList() { "plain" };

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 22/08/2017 Created [Fabian Sauter]
        /// </history>
        public SASLConnection(TCPConnection2 tcpConnection, XMPPConnection2 xMPPConnection) : base(tcpConnection, xMPPConnection)
        {
            reset();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private ArrayList getMechanisms(StreamFeaturesMessage msg)
        {
            foreach (StreamFeature sF in msg.getFeatures())
            {
                if (sF is SASLStreamFeature)
                {
                    return (sF as SASLStreamFeature).MECHANISMS;
                }
            }
            return null;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public override void reset()
        {
            state = SASLState.DISCONNECTED;
            selectedMechanism = null;
        }

        #endregion

        #region --Misc Methods (Private)--
        private void selectMechanism(ArrayList mechanisms)
        {
            string selected = null;
            foreach (string s in OFFERED_MECHANISMS)
            {
                foreach (string m in mechanisms)
                {
                    if (m.ToLower().Equals(s))
                    {
                        selected = s;
                        break;
                    }
                }
                if (selected != null)
                {
                    break;
                }
            }
            XMPPAccount sCC = XMPP_CONNECTION.account;
            switch (selected)
            {
                case "scram-sha-1":
                    selectedMechanism = new ScramSHA1SASLMechanism(sCC.user.userId, sCC.user.userPassword);
                    break;
                case "plain":
                    selectedMechanism = new PlainSASLMechanism(sCC.user.userId, sCC.user.userPassword);
                    break;
            }
        }

        #endregion

        #region --Misc Methods (Protected)--
        protected async override Task processMessageAsync(NewValidMessageEventArgs args)
        {
            AbstractMessage msg = args.getMessage();
            if (state == SASLState.CONNECTED || msg.isProcessed())
            {
                return;
            }

            switch (state)
            {
                case SASLState.DISCONNECTED:
                    if (msg is StreamFeaturesMessage || msg is OpenStreamAnswerMessage)
                    {
                        StreamFeaturesMessage features = null;
                        if (msg is OpenStreamAnswerMessage)
                        {
                            features = (msg as OpenStreamAnswerMessage).getStreamFeaturesMessage();
                        }
                        else
                        {
                            features = msg as StreamFeaturesMessage;
                        }

                        if (features == null)
                        {
                            return;
                        }

                        ArrayList mechanisms = getMechanisms(features);
                        if (mechanisms == null)
                        {
                            return;
                        }
                        setMessageProcessed(args);
                        selectMechanism(mechanisms);
                        if (selectedMechanism == null)
                        {
                            state = SASLState.ERROR;
                            await XMPP_CONNECTION.onMessageProcessorFailedAsync(new ConnectionError(ConnectionErrorCode.SASL_FAILED, "selectedMechanism == null"), true);
                            return;
                        }
                        await XMPP_CONNECTION.sendAsync(selectedMechanism.getSelectSASLMechanismMessage(), false, true);
                        state = SASLState.REQUESTED;
                    }
                    break;

                case SASLState.REQUESTED:
                case SASLState.CHALLENGING:
                    if (msg is ScramSHA1ChallengeMessage)
                    {
                        state = SASLState.CHALLENGING;
                        setMessageProcessed(args);
                        await XMPP_CONNECTION.sendAsync(selectedMechanism.generateResponse(msg), false, true);
                    }
                    else if (msg is SASLSuccessMessage)
                    {
                        state = SASLState.CONNECTED;
                        msg.setRestartConnection(AbstractMessage.SOFT_RESTART);
                        setMessageProcessed(args);
                    }
                    else if (msg is SASLFailureMessage)
                    {
                        SASLFailureMessage saslFailureMessage = msg as SASLFailureMessage;
                        state = SASLState.ERROR;

                        Logger.Error("Error during SASL authentication: " + saslFailureMessage.ERROR_TYPE + "\n" + saslFailureMessage.ERROR_MESSAGE);
                        if (saslFailureMessage.ERROR_TYPE == SASLErrorType.UNKNOWN_ERROR)
                        {
                            await XMPP_CONNECTION.onMessageProcessorFailedAsync(new ConnectionError(ConnectionErrorCode.SASL_FAILED, "SASL: " + saslFailureMessage.ERROR_MESSAGE), true);
                        }
                        else
                        {
                            await XMPP_CONNECTION.onMessageProcessorFailedAsync(new ConnectionError(ConnectionErrorCode.SASL_FAILED, "SASL: " + saslFailureMessage.ERROR_TYPE), true);
                        }
                    }
                    break;

                case SASLState.CONNECTED:
                    break;
            }

        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
