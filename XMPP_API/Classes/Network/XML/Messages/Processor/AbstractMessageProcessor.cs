using System.Threading.Tasks;
using XMPP_API.Classes.Network.Events;
using XMPP_API.Classes.Network.TCP;

namespace XMPP_API.Classes.Network.XML.Messages.Processor
{
    public abstract class AbstractMessageProcessor
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        protected readonly TCPConnection2 TCP_CONNECTION;
        protected readonly XMPPConnection2 XMPP_CONNECTION;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 21/08/2017 Created [Fabian Sauter]
        /// </history>
        protected AbstractMessageProcessor(TCPConnection2 tcpConnection, XMPPConnection2 xMPPConnection)
        {
            TCP_CONNECTION = tcpConnection;
            XMPP_CONNECTION = xMPPConnection;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        protected void setMessageProcessed(NewValidMessageEventArgs args, bool cancelEvent)
        {
            args.MESSAGE.setProcessed();
            args.Cancel = cancelEvent;
        }

        protected void setMessageProcessed(NewValidMessageEventArgs args)
        {
            setMessageProcessed(args, true);
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public abstract void reset();

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--
        protected abstract Task processMessageAsync(NewValidMessageEventArgs args);

        protected void stopListeningForMessages()
        {
            XMPP_CONNECTION.NewValidMessage -= XMPP_CONNECTION_ConnectionNewValidMessage;
        }

        protected void startListeningForMessages()
        {
            stopListeningForMessages();
            XMPP_CONNECTION.NewValidMessage += XMPP_CONNECTION_ConnectionNewValidMessage;
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private async void XMPP_CONNECTION_ConnectionNewValidMessage(IMessageSender sender, NewValidMessageEventArgs args)
        {
            await processMessageAsync(args);
        }

        #endregion
    }
}
