using XMPP_API.Classes.Network.Events;
using XMPP_API.Classes.Network.TCP;

namespace XMPP_API.Classes.Network.XML.Messages.Processor
{
    abstract class AbstractMessageProcessor
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        protected readonly TCPConnectionHandler TCP_CONNECTION;
        protected readonly XMPPConnectionHandler XMPP_CONNECTION;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Construktoren--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 21/08/2017 Created [Fabian Sauter]
        /// </history>
        public AbstractMessageProcessor(TCPConnectionHandler tcpConnection, XMPPConnectionHandler xMPPConnection)
        {
            this.TCP_CONNECTION = tcpConnection;
            this.XMPP_CONNECTION = xMPPConnection;
            XMPP_CONNECTION.ConnectionNewValidMessage += XMPP_CONNECTION_ConnectionNewValidMessage;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        protected void setMessageProcessed(NewPresenceEventArgs args, bool cancelEvent)
        {
            args.getMessage().setProcessed();
            args.Cancel = cancelEvent;
        }

        protected void setMessageProcessed(NewPresenceEventArgs args)
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
        protected abstract void processMessage(NewPresenceEventArgs args);

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        protected void XMPP_CONNECTION_ConnectionNewValidMessage(XMPPConnectionHandler handler, NewPresenceEventArgs args)
        {
            processMessage(args);
        }

        #endregion
    }
}
