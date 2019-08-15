using System;

namespace XMPP_API.Classes.Network.Events
{
    public class MessageSendEventArgs : EventArgs
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        /// <summary>
        /// The unique ID of the XMPP message.
        /// </summary>
        public readonly string ID;
        /// <summary>
        /// A unique ID for chat messages to identify them in the DB.
        /// </summary>
        public readonly string CHAT_MESSAGE_ID;
        /// <summary>
        /// Did the message got send with a delay?
        /// </summary>
        public readonly bool DELAYED;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 01/01/2018 Created [Fabian Sauter]
        /// </history>
        public MessageSendEventArgs(string id, string chatMessageId, bool delayed)
        {
            ID = id;
            CHAT_MESSAGE_ID = chatMessageId;
            DELAYED = delayed;
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
