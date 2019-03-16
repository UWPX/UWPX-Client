using System;
using System.Threading.Tasks;
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
        public RecourceBindingConnection(TCPConnection2 tcpConnection, XMPPConnection2 xMPPConnection) : base(tcpConnection, xMPPConnection)
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
            startListeningForMessages();
        }

        protected async override Task processMessageAsync(NewValidMessageEventArgs args)
        {
            AbstractMessage msg = args.MESSAGE;
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

                        if (features is null)
                        {
                            return;
                        }
                        if (features.containsFeature("bind"))
                        {
                            setMessageProcessed(args);
                            BindResourceMessage bindMsg = new BindResourceMessage(XMPP_CONNECTION.account.user.resourcePart);
                            id = bindMsg.ID;
                            await XMPP_CONNECTION.sendAsync(bindMsg, true);
                            state = RecourceBindingState.BINDING;
                        }
                    }
                    break;
                case RecourceBindingState.BINDING:
                    if (msg is IQMessage)
                    {
                        IQMessage iQM = msg as IQMessage;
                        if (string.Equals(iQM.ID, id))
                        {
                            stopListeningForMessages();
                            setMessageProcessed(args);
                            XMPP_CONNECTION.sendAsync(new StartSessionMessage(), true).Wait();
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
