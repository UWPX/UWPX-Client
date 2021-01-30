using System;
using Shared.Classes;
using Storage.Classes.Models.Chat;
using Windows.UI.Xaml.Media.Imaging;
using XMPP_API.Classes;

namespace UWPX_UI_Context.Classes.DataTemplates
{
    public sealed class ChatDataTemplate: AbstractDataTemplate, IDisposable
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
        private MucInfoModel _MucInfo;
        public MucInfoModel MucInfo
        {
            get => _MucInfo;
            set => SetProperty(ref _MucInfo, value);
        }
        private XMPPClient _Client;
        public XMPPClient Client
        {
            get => _Client;
            set => SetProperty(ref _Client, value);
        }

        /// <summary>
        /// The index in the chats list.
        /// </summary>
        public int Index { get; set; }

        public delegate void NewChatMessageHandler(ChatDataTemplate chat, NewChatMessageEventArgs args);
        public delegate void ChatMessageChangedHandler(ChatDataTemplate chat, ChatMessageChangedEventArgs args);

        public event ChatMessageChangedHandler ChatMessageChanged;
        public event NewChatMessageHandler NewChatMessage;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ChatDataTemplate()
        {
            ChatDBManager.INSTANCE.NewChatMessage += INSTANCE_NewChatMessage;
            ChatDBManager.INSTANCE.ChatMessageChanged += INSTANCE_ChatMessageChanged;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void Dispose()
        {
            ChatDBManager.INSTANCE.NewChatMessage -= INSTANCE_NewChatMessage;
            ChatDBManager.INSTANCE.ChatMessageChanged -= INSTANCE_ChatMessageChanged;
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void INSTANCE_ChatMessageChanged(ChatDBManager handler, ChatMessageChangedEventArgs args)
        {
            if (string.Equals(Chat?.id, args.MESSAGE.chatId))
            {
                ChatMessageChanged?.Invoke(this, args);
            }
        }

        private void INSTANCE_NewChatMessage(ChatDBManager handler, NewChatMessageEventArgs args)
        {
            if (string.Equals(Chat?.id, args.MESSAGE.chatId))
            {
                NewChatMessage?.Invoke(this, args);
            }
        }

        #endregion
    }
}
