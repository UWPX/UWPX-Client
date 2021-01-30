using Shared.Classes;
using Storage.Classes.Models.Chat;

namespace UWPX_UI_Context.Classes.DataTemplates
{
    public sealed class ChatMessageDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private ChatModel _Chat;
        public ChatModel Chat
        {
            get => _Chat;
            set => SetProperty(ref _Chat, value);
        }
        private ChatMessageModel _Message;
        public ChatMessageModel Message
        {
            get => _Message;
            set => SetProperty(ref _Message, value);
        }
        private MucInfoModel _MUC;
        /// <summary>
        /// Only populate with valid if data the chat is of type MUC.
        /// </summary>
        public MucInfoModel MUC
        {
            get => _MUC;
            set => SetProperty(ref _MUC, value);
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
