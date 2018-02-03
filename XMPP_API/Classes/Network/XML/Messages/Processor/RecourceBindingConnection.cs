using System;
using XMPP_API.Classes.Network.Events;
using XMPP_API.Classes.Network.TCP;
using XMPP_API.Classes.Network.XML.Messages.Features;

namespace XMPP_API.Classes.Network.XML.Messages.Processor
{
    class RecourceBindingConnection : AbstractMessageProcessor
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private RecourceBindingState state;
        private string id;

        public event EventHandler ResourceBound;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 24/08/2017 Created [Fabian Sauter]
        /// </history>
        public RecourceBindingConnection(TCPConnection tcpConnection, XMPPConnection xMPPConnection) : base(tcpConnection, xMPPConnection)
        {
            reset();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public RecourceBindingState getState()
        {
            return state;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public override void reset()
        {
            state = RecourceBindingState.UNBOUND;
            id = null;
        }

        protected async override void processMessage(NewValidMessageEventArgs args)
        {
            AbstractMessage msg = args.getMessage();
            if (state == RecourceBindingState.BOUND || state == RecourceBindingState.ERROR || msg.isProcessed())
            {
                return;
            }
            switch (state)
            {
                case RecourceBindingState.UNBOUND:
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
                        if (features.containsFeature("bind"))
                        {
                            setMessageProcessed(args);
                            BindResourceMessage bindMsg = new BindResourceMessage(XMPP_CONNECTION.account.user.resource);
                            id = bindMsg.getId();
                            await XMPP_CONNECTION.sendAsync(bindMsg, false, true);
                            state = RecourceBindingState.BINDING;
                        }
                    }
                    break;
                case RecourceBindingState.BINDING:
                    if (msg is IQMessage)
                    {
                        IQMessage iQM = msg as IQMessage;
                        if (iQM.getId().Equals(id))
                        {
                            setMessageProcessed(args);
                            XMPP_CONNECTION.sendAsync(new StartSessionMessage(), false, true).Wait();
                            state = RecourceBindingState.BOUND;
                            ResourceBound?.Invoke(this, new EventArgs());
                        }
                    }
                    break;
            }
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
