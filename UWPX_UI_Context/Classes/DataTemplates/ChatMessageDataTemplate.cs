using Data_Manager2.Classes.DBTables;

namespace UWPX_UI_Context.Classes.DataTemplates
{
    public sealed class ChatMessageDataTemplate : AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private ChatTable _Chat;
        public ChatTable Chat
        {
            get { return _Chat; }
            set { SetProperty(ref _Chat, value); }
        }
        private ChatMessageTable _Message;
        public ChatMessageTable Message
        {
            get { return _Message; }
            set { SetProperty(ref _Message, value); }
        }
        private MUCChatInfoTable _MUC;
        /// <summary>
        /// Only populate with valid if data the chat is of type MUC.
        /// </summary>
        public MUCChatInfoTable MUC
        {
            get { return _MUC; }
            set { SetProperty(ref _MUC, value); }
        }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


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
