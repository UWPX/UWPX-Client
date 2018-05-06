using System.Threading.Tasks;
using XMPP_API.Classes.Network.Events;
using XMPP_API.Classes.Network.TCP;

namespace XMPP_API.Classes.Network.XML.Messages.Processor
{
    abstract class AbstractMessageProcessor
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
        public AbstractMessageProcessor(TCPConnection2 tcpConnection, XMPPConnection2 xMPPConnection)
        {
            this.TCP_CONNECTION = tcpConnection;
            this.XMPP_CONNECTION = xMPPConnection;
            XMPP_CONNECTION.ConnectionNewValidMessage -= XMPP_CONNECTION_ConnectionNewValidMessage;
            XMPP_CONNECTION.ConnectionNewValidMessage += XMPP_CONNECTION_ConnectionNewValidMessage;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        protected void setMessageProcessed(NewValidMessageEventArgs args, bool cancelEvent)
        {
            args.getMessage().setProcessed();
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

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private async void XMPP_CONNECTION_ConnectionNewValidMessage(XMPPConnection2 connection, NewValidMessageEventArgs args)
        {
            await processMessageAsync(args);
        }

        #endregion
    }
}
