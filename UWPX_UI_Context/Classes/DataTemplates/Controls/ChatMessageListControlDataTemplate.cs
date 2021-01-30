using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Logging;
using Shared.Classes;
using Shared.Classes.Collections;
using Storage.Classes.Models.Chat;

namespace UWPX_UI_Context.Classes.DataTemplates.Controls
{
    public class ChatMessageListControlDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly CustomObservableCollection<ChatMessageDataTemplate> CHAT_MESSAGES;
        private ChatDataTemplate _Chat;
        public ChatDataTemplate Chat
        {
            get => _Chat;
            set => SetChatProperty(value);
        }

        private bool _IsDummy;
        public bool IsDummy
        {
            get => _IsDummy;
            set => SetProperty(ref _IsDummy, value);
        }

        private bool _IsLoading;
        public bool IsLoading
        {
            get => _IsLoading;
            set => SetProperty(ref _IsLoading, value);
        }
        private bool _HasMoreMessages;
        public bool HasMoreMessages
        {
            get => _HasMoreMessages;
            set => SetProperty(ref _HasMoreMessages, value);
        }
        private string _UnreadCount;
        public string UnreadCount
        {
            get => _UnreadCount;
            set => SetProperty(ref _UnreadCount, value);
        }

        private readonly SemaphoreSlim CHAT_MESSAGES_SEMA = new SemaphoreSlim(1);

        public event PropertyChangedEventHandler ChatChanged;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ChatMessageListControlDataTemplate()
        {
            CHAT_MESSAGES = new CustomObservableCollection<ChatMessageDataTemplate>(true);
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private void SetChatProperty(ChatDataTemplate value)
        {
            ChatDataTemplate oldChat = _Chat;
            if (SetProperty(ref _Chat, value, nameof(Chat)) && !(value is null))
            {
                HasMoreMessages = true;
                if (!(oldChat is null))
                {
                    oldChat.ChatMessageChanged -= OnChatMessageChanged;
                    oldChat.NewChatMessage -= OnNewChatMessage;
                }
                if (!(value is null))
                {
                    value.ChatMessageChanged += OnChatMessageChanged;
                    value.NewChatMessage += OnNewChatMessage;
                }
                UpdateUnreadCount();

                ChatChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Chat)));
            }
        }

        public string GetLastMessageId()
        {
            return CHAT_MESSAGES.Count > 0 ? CHAT_MESSAGES[0].Message.id : null;
        }
        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void UpdateView(ChatDataTemplate oldChat, ChatDataTemplate newChat)
        {
            Chat = newChat;
        }

        public async Task OnNewChatMessageAsync(ChatMessageModel msg, ChatModel chat, MucInfoModel muc)
        {
            await CHAT_MESSAGES_SEMA.WaitAsync();
            await SharedUtils.CallDispatcherAsync(() =>
            {
                CHAT_MESSAGES.Add(new ChatMessageDataTemplate
                {
                    Chat = chat,
                    Message = msg,
                    MUC = muc
                });
            });
            CHAT_MESSAGES_SEMA.Release();
        }

        public async Task OnChatMessageChangedAsync(ChatMessageModel msg, bool removed)
        {
            await CHAT_MESSAGES_SEMA.WaitAsync();
            for (int i = 0; i < CHAT_MESSAGES.Count; i++)
            {
                if (string.Equals(CHAT_MESSAGES[i].Message.id, msg.id))
                {
                    if (removed)
                    {
                        CHAT_MESSAGES.RemoveAt(i);
                    }
                    else
                    {
                        CHAT_MESSAGES[i].Message = msg;
                    }
                    CHAT_MESSAGES_SEMA.Release();
                    return;
                }
            }
            CHAT_MESSAGES_SEMA.Release();
            Logger.Warn("OnChatMessageChanged failed - no chat message with id: " + msg.id + " for chat: " + msg.chatId);
        }

        public async Task InsertChatMessagesAsync(List<ChatMessageDataTemplate> msgs)
        {
            await CHAT_MESSAGES_SEMA.WaitAsync();
            CHAT_MESSAGES.InsertRange(0, msgs);
            CHAT_MESSAGES_SEMA.Release();
        }
        #endregion

        #region --Misc Methods (Private)--
        private void UpdateUnreadCount()
        {
            Task.Run(() =>
            {
                int count = (Chat is null) || (Chat.Chat is null) ? 0 : ChatDBManager.INSTANCE.getUnreadCount(Chat.Chat.id);
                if (count <= 0)
                {
                    UnreadCount = "";
                }
                else if (count > 99)
                {
                    UnreadCount = "99+";
                }
                else
                {
                    UnreadCount = count.ToString();
                }
            });
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void OnNewChatMessage(ChatDataTemplate chat, NewChatMessageEventArgs args)
        {
            UpdateUnreadCount();
        }

        private void OnChatMessageChanged(ChatDataTemplate chat, ChatMessageChangedEventArgs args)
        {
            UpdateUnreadCount();
        }

        #endregion
    }
}
