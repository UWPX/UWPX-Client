using Data_Manager2.Classes.DBTables;
using System.ComponentModel;

namespace Data_Manager2.Classes.Events
{
    public class ChatChangedEventArgs : CancelEventArgs
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly ChatTable CHAT;
        public readonly bool REMOVED;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 10/09/2017 Created [Fabian Sauter]
        /// </history>
        public ChatChangedEventArgs(ChatTable chat, bool removed)
        {
            this.CHAT = chat;
            this.REMOVED = removed;
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
