using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMPP_API.Classes.Network.Events;
using XMPP_API.Classes.Network.TCP;
using XMPP_API.Classes.Network.XML.Messages.Features;
using XMPP_API.Classes.Network.XML.Messages.XEP_0198;

namespace XMPP_API.Classes.Network.XML.Messages.Processor
{
    class SMConnection : AbstractMessageProcessor
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private SMState state;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 17/03/2018 Created [Fabian Sauter]
        /// </history>
        public SMConnection(TCPConnection2 tcpConnection, XMPPConnection2 xMPPConnection) : base(tcpConnection, xMPPConnection)
        {
            reset();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public SMState getState()
        {
            return state;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public override void reset()
        {
            state = SMState.DISABLED;
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--
        protected override async Task processMessageAsync(NewValidMessageEventArgs args)
        {
            return;
            AbstractMessage msg = args.getMessage();
            if (state == SMState.ENABLED || state == SMState.ERROR || msg.isProcessed())
            {
                return;
            }

            switch (state)
            {
                case SMState.DISABLED:
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
                        if (features.containsFeature("sm"))
                        {
                            setMessageProcessed(args);
                            SMEnableMessage enableMsg = new SMEnableMessage();
                            await XMPP_CONNECTION.sendAsync(enableMsg, false, true);
                            state = SMState.REQUESTED;
                        }
                    }
                    break;

                case SMState.REQUESTED:
                    if (msg is SMEnableMessage enableAnswMsg)
                    {
                        setMessageProcessed(args);
                        // Update connection information:
                        state = SMState.ENABLED;
                    }
                    else if (msg is SMFailedMessage failedMsg)
                    {
                        setMessageProcessed(args);

                        // Handle errors:
                        state = SMState.ERROR;
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
