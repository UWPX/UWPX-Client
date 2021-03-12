using System;
using System.ComponentModel;
using Shared.Classes;
using Storage.Classes.Models.Chat;
using Windows.UI.Xaml.Media.Imaging;

namespace Manager.Classes.Chat
{
    public sealed class ChatDataTemplate: AbstractDataTemplate, IComparable
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private BitmapImage _Image;
        public BitmapImage Image
        {
            get => _Image;
            set => SetProperty(ref _Image, value);
        }
        private ChatModel _Chat;
        public ChatModel Chat
        {
            get => _Chat;
            set => SetChatProperty(value);
        }
        private ChatMessageModel _LastMsg;
        public ChatMessageModel LastMsg
        {
            get => _LastMsg;
            set => SetLastMsgProperty(value);
        }
        private Client _Client;
        public Client Client
        {
            get => _Client;
            set => SetProperty(ref _Client, value);
        }

        /// <summary>
        /// The index in the chats list.
        /// </summary>
        public int Index { get; set; }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ChatDataTemplate(ChatModel chat, Client client, ChatMessageModel lastMsg, BitmapImage image)
        {
            Chat = chat;
            Client = client;
            LastMsg = lastMsg;
            Image = image;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private void SetChatProperty(ChatModel value)
        {
            ChatModel old = _Chat;
            if (SetProperty(ref _Chat, value, nameof(Chat)))
            {
                if (!(old is null))
                {
                    old.PropertyChanged -= OnChatPropertyChanged;
                }
                if (!(value is null))
                {
                    value.PropertyChanged += OnChatPropertyChanged;
                }
            }
        }

        private void SetLastMsgProperty(ChatMessageModel value)
        {
            ChatMessageModel old = _LastMsg;
            if (SetProperty(ref _LastMsg, value, nameof(LastMsg)))
            {
                if (!(old is null))
                {
                    old.PropertyChanged -= OnLastMsgPropertyChanged;
                }
                if (!(value is null))
                {
                    value.PropertyChanged += OnLastMsgPropertyChanged;
                }
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public int CompareTo(object obj)
        {
            return obj is ChatDataTemplate chat ? chat.Chat.lastActive.CompareTo(Chat.lastActive) : 1;
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void OnChatPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Chat) + '.' + e.PropertyName);
        }

        private void OnLastMsgPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(LastMsg) + '.' + e.PropertyName);
        }
        #endregion
    }
}
