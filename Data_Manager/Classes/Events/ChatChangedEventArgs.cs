using Data_Manager.Classes.DBEntries;
using System.ComponentModel;

namespace Data_Manager.Classes.Events
{
    public class ChatChangedEventArgs : CancelEventArgs
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly ChatEntry CHAT;
        private readonly bool REMOVED;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 10/09/2017 Created [Fabian Sauter]
        /// </history>
        public ChatChangedEventArgs(ChatEntry chat, bool removed)
        {
            this.CHAT = chat;
            this.REMOVED = removed;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public ChatEntry getChat()
        {
            return CHAT;
        }

        public bool gotRemoved()
        {
            return REMOVED;
        }

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
