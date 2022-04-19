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
        private bool _Minimize;
        public bool Minimize
        {
            get => _Minimize;
            private set => SetProperty(ref _Minimize, value);
        }
        private bool _SameAuthor;
        public bool SameAuthor
        {
            get => _SameAuthor;
            private set => SetProperty(ref _SameAuthor, value);
        }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ChatMessageDataTemplate(ChatMessageModel message, ChatModel chat)
        {
            _Message = message;
            _Chat = chat;
            _Minimize = false;
            _SameAuthor = false;
        }

        public int CompareTo(object obj)
        {
            return obj is ChatMessageDataTemplate msg ? Message.date.CompareTo(msg.Message.date) : 1;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public void SetMinimizeAndSameAuthor(ChatMessageDataTemplate other)
        {
            if (other is null)
            {
                SameAuthor = string.IsNullOrEmpty(Message.fromNickname);
                Minimize = false;
                return;
            }
            SameAuthor = string.IsNullOrEmpty(Message.fromNickname) || string.Equals(Message.fromNickname, other.Message.fromNickname);
            Minimize = SameAuthor && (other.Message.date - Message.date).TotalMinutes <= 1;
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
