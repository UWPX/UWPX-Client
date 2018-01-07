using System.ComponentModel;
using XMPP_API.Classes.Network.XML.Messages.XEP_0085;

namespace XMPP_API.Classes.Network.Events
{
    public class NewChatStateEventArgs : CancelEventArgs
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly ChatState STATE;
        public readonly string FROM;
        public readonly string TO;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 21/08/2017 Created [Fabian Sauter]
        /// </history>
        public NewChatStateEventArgs(ChatStateMessage message)
        {
            this.STATE = message.getState();
            this.FROM = Utils.getBareJidFromFullJid(message.getFrom());
            this.TO = Utils.getBareJidFromFullJid(message.getTo());
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


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
