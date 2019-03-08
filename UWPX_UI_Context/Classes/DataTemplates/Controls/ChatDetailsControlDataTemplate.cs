using Data_Manager2.Classes;
using Data_Manager2.Classes.DBManager;
using Data_Manager2.Classes.DBTables;
using Data_Manager2.Classes.Toast;
using Logging;
using Shared.Classes;
using Shared.Classes.Collections;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using XMPP_API.Classes;

namespace UWPX_UI_Context.Classes.DataTemplates.Controls
{
    public class ChatDetailsControlDataTemplate : AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private string _NameText;
        public string NameText
        {
            get { return _NameText; }
            set { SetProperty(ref _NameText, value); }
        }
        private string _AccountText;
        public string AccountText
        {
            get { return _AccountText; }
            set { SetProperty(ref _AccountText, value); }
        }
        private string _StatusText;
        public string StatusText
        {
            get { return _StatusText; }
            set { SetProperty(ref _StatusText, value); }
        }
        private Visibility _EnterMucVisability;
        public Visibility EnterMucVisability
        {
            get { return _EnterMucVisability; }
            set { SetProperty(ref _EnterMucVisability, value); }
        }
        private Visibility _LeaveMucVisability;
        public Visibility LeaveMucVisability
        {
            get { return _LeaveMucVisability; }
            set { SetProperty(ref _LeaveMucVisability, value); }
        }
        private bool _DebugSettingsEnabled;
        public bool DebugSettingsEnabled
        {
            get { return _DebugSettingsEnabled; }
            set { SetProperty(ref _DebugSettingsEnabled, value); }
        }
        private Visibility _OmemoVisability;
        public Visibility OmemoVisability
        {
            get { return _OmemoVisability; }
            set { SetProperty(ref _OmemoVisability, value); }
        }
        private bool _OmemoEnabled;
        public bool OmemoEnabled
        {
            get { return _OmemoEnabled; }
            set { SetOmemoEnabledProperty(value); }
        }
        private bool _IsLoadingChatMessages;
        public bool IsLoadingChatMessages
        {
            get { return _IsLoadingChatMessages; }
            set { SetProperty(ref _IsLoadingChatMessages, value); }
        }
        private string _MessageText;
        public string MessageText
        {
            get { return _MessageText; }
            set { SetProperty(ref _MessageText, value); }
        }
        private Presence _AccountPresence;
        public Presence AccountPresence
        {
            get { return _AccountPresence; }
            set { SetProperty(ref _AccountPresence, value); }
        }
        private string _AccountInitials;
        public string AccountInitials
        {
            get { return _AccountInitials; }
            set { SetProperty(ref _AccountInitials, value); }
        }

        public readonly CustomObservableCollection<ChatMessageDataTemplate> CHAT_MESSAGES = new CustomObservableCollection<ChatMessageDataTemplate>(true);
        private readonly SemaphoreSlim CHAT_MESSAGES_SEMA = new SemaphoreSlim(1);

        private CancellationTokenSource loadChatMessagesCancelToken = null;
        private Task loadChatMessagesTask = null;
        private ChatTable chat;
        internal bool isDummy = false;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ChatDetailsControlDataTemplate()
        {
            LoadSettings();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public void SetOmemoEnabledProperty(bool value)
        {
            if (SetProperty(ref _OmemoEnabled, value, nameof(OmemoEnabled)) && !(chat is null))
            {
                chat.omemoEnabled = value;
                if (!isDummy)
                {
                    Task.Run(() => ChatDBManager.INSTANCE.setChat(chat, false, true));
                }
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void UpdateViewClient(XMPPClient client)
        {
            if (!(client is null))
            {
                AccountText = client.getXMPPAccount().getIdAndDomain();
            }
        }

        public void UpdateViewChat(ChatTable chat, MUCChatInfoTable muc)
        {
            if (!(chat is null))
            {
                // Do not reload chat messages, if for example only the presence changes:
                if (!isDummy && (this.chat is null || !string.Equals(chat.id, this.chat.id)))
                {
                    LoadChatMessages(chat, muc);
                }
                this.chat = chat;
                if (isDummy)
                {
                    AccountText = chat.userAccountId;
                }
                else
                {
                    MarkChatMessagesAsRead(chat);
                }

                if (chat.chatType == ChatType.MUC)
                {
                    OmemoVisability = Visibility.Collapsed;

                    if (muc.state != MUCState.ENTERD && muc.state != MUCState.ENTERING)
                    {
                        EnterMucVisability = Visibility.Visible;
                        LeaveMucVisability = Visibility.Collapsed;
                    }
                    else
                    {
                        EnterMucVisability = Visibility.Collapsed;
                        LeaveMucVisability = Visibility.Visible;
                    }
                }
                else
                {
                    NameText = chat.chatJabberId ?? "";
                    StatusText = chat.chatState ?? chat.status ?? "";
                    EnterMucVisability = Visibility.Collapsed;
                    LeaveMucVisability = Visibility.Collapsed;
                    OmemoEnabled = chat.omemoEnabled;
                    OmemoVisability = Visibility.Visible;
                }

                // Account image:
                AccountPresence = chat.presence;
                AccountInitials = "\uE77B";
            }
            else
            {
                this.chat = null;
            }
        }

        public void UpdateViewMuc(ChatTable chat, MUCChatInfoTable muc)
        {
            if (!(muc is null) && !(chat is null) && string.Equals(chat.id, muc.chatId))
            {
                NameText = string.IsNullOrWhiteSpace(muc.name) ? chat.chatJabberId : muc.name;
                StatusText = muc.subject ?? "";

                // Account image:
                AccountPresence = muc.getMUCPresence();
                AccountInitials = "\uE125";
            }
        }

        public async Task OnNewChatMessageAsync(ChatMessageTable msg, ChatTable chat, MUCChatInfoTable muc)
        {
            await CHAT_MESSAGES_SEMA.WaitAsync();
            CHAT_MESSAGES.Add(new ChatMessageDataTemplate
            {
                Chat = chat,
                Message = msg,
                MUC = muc
            });
            CHAT_MESSAGES_SEMA.Release();
        }

        public async Task OnChatMessageChangedAsync(ChatMessageTable msg)
        {
            await CHAT_MESSAGES_SEMA.WaitAsync();
            foreach (ChatMessageDataTemplate chatMsg in CHAT_MESSAGES)
            {
                if (string.Equals(chatMsg.Message.id, msg.id))
                {
                    chatMsg.Message = msg;
                    CHAT_MESSAGES_SEMA.Release();
                    return;
                }
            }
            CHAT_MESSAGES_SEMA.Release();
            Logger.Warn("OnChatMessageChanged failed - no chat message with id: " + msg.id + " for chat: " + msg.chatId);
        }

        public void LoadSettings()
        {
            DebugSettingsEnabled = Settings.getSettingBoolean(SettingsConsts.DEBUG_SETTINGS_ENABLED);
        }

        #endregion

        #region --Misc Methods (Private)--
        /// <summary>
        /// Starts a new Task and loads all chat messages for the given chat.
        /// </summary>
        /// <param name="chat">The chat which all chat messages should get loaded for.</param>
        /// <param name="muc">If the chat is of type MUC, than non null value. Else null.</param>
        private void LoadChatMessages(ChatTable chat, MUCChatInfoTable muc)
        {
            Task.Run(async () =>
            {
                if (!(loadChatMessagesCancelToken is null))
                {
                    loadChatMessagesCancelToken.Cancel();
                }
                loadChatMessagesCancelToken = new CancellationTokenSource();

                if (!(loadChatMessagesTask is null))
                {
                    await loadChatMessagesTask;
                }

                IsLoadingChatMessages = true;

                await CHAT_MESSAGES_SEMA.WaitAsync();
                CHAT_MESSAGES.Clear();
                CHAT_MESSAGES_SEMA.Release();

                List<ChatMessageDataTemplate> msgs = new List<ChatMessageDataTemplate>();
                loadChatMessagesTask = Task.Run(() =>
                {
                    foreach (ChatMessageTable msg in ChatDBManager.INSTANCE.getAllChatMessagesForChat(chat.id))
                    {
                        msgs.Add(new ChatMessageDataTemplate
                        {
                            Message = msg,
                            Chat = chat,
                            MUC = muc
                        });
                    }
                }, loadChatMessagesCancelToken.Token);

                await loadChatMessagesTask;

                if (!loadChatMessagesCancelToken.IsCancellationRequested)
                {
                    await CHAT_MESSAGES_SEMA.WaitAsync();
                    CHAT_MESSAGES.AddRange(msgs);
                    CHAT_MESSAGES_SEMA.Release();
                }

                IsLoadingChatMessages = false;
            });
        }

        /// <summary>
        /// Marks all chat messages as read and removes all toasts for the given chat.
        /// </summary>
        private void MarkChatMessagesAsRead(ChatTable chat)
        {
            // Mark all unread messages as read for this chat:
            ChatDBManager.INSTANCE.markAllMessagesAsRead(chat.id);
            // Remove notification group:
            ToastHelper.removeToastGroup(chat.id);
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
