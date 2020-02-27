using System.Threading.Tasks;
using XMPP_API.Classes.Network.Events;
using XMPP_API.Classes.Network.TCP;

namespace XMPP_API.Classes.Network.XML.Messages.Processor
{
    public abstract class AbstractMessageProcessor
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        protected readonly TcpConnection TCP_CONNECTION;
        protected readonly XmppConnection XMPP_CONNECTION;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 21/08/2017 Created [Fabian Sauter]
        /// </history>
        protected AbstractMessageProcessor(TcpConnection tcpConnection, XmppConnection xmppConnection)
        {
            TCP_CONNECTION = tcpConnection;
            XMPP_CONNECTION = xmppConnection;
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
