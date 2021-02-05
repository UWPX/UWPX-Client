using Shared.Classes;
using Storage.Classes.Models.Chat;
using Windows.UI.Xaml.Media.Imaging;

namespace Manager.Classes.Chat
{
    public sealed class ChatDataTemplate: AbstractDataTemplate
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
            set => SetProperty(ref _Chat, value);
        }
        private ChatMessageModel _LastMsg;
        public ChatMessageModel LastMsg
        {
            get => _LastMsg;
            set => SetProperty(ref _LastMsg, value);
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
            _Chat = chat;
            _Client = client;
            _LastMsg = lastMsg;
            _Image = image;
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
