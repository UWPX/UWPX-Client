using System;
using Shared.Classes;
using Storage.Classes.Models.Chat;

namespace Manager.Classes.Chat
{
    public sealed class ChatMessageDataTemplate: AbstractDataTemplate, IComparable
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private ChatMessageModel _Message;
        public ChatMessageModel Message
        {
            get => _Message;
            set => SetProperty(ref _Message, value);
        }
        private ChatModel _Chat;
        public ChatModel Chat
        {
            get => _Chat;
            set => SetProperty(ref _Chat, value);
        }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ChatMessageDataTemplate(ChatMessageModel message, ChatModel chat)
        {
            _Message = message;
            _Chat = chat;
        }

        public int CompareTo(object obj)
        {
            return obj is ChatMessageDataTemplate msg ? Message.date.CompareTo(msg.Message.date) : 1;
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
