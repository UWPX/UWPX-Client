using Data_Manager2.Classes;
using Data_Manager2.Classes.DBManager;
using Data_Manager2.Classes.DBTables;
using Data_Manager2.Classes.Toast;
using System.Collections.Generic;
using System.Threading.Tasks;
using UWPX_UI_Context.Classes.Collections;
using Windows.UI.Xaml;
using XMPP_API.Classes;

namespace UWPX_UI_Context.Classes.DataTemplates
{
    public class ChatDetailsControlDataTemplate : AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private string _NameText;
        public string NameText
        {
            get { return _NameText; }
            set
            {
                _NameText = value;
                OnPropertyChanged();
            }
        }
        private string _AccountText;
        public string AccountText
        {
            get { return _AccountText; }
            set
            {
                _AccountText = value;
                OnPropertyChanged();
            }
        }
        private string _StatusText;
        public string StatusText
        {
            get { return _StatusText; }
            set
            {
                _StatusText = value;
                OnPropertyChanged();
            }
        }
        private Visibility _JoinMucVisability;
        public Visibility JoinMucVisability
        {
            get { return _JoinMucVisability; }
            set
            {
                _JoinMucVisability = value;
                OnPropertyChanged();
            }
        }
        private Visibility _LeaveMucVisability;
        public Visibility LeaveMucVisability
        {
            get { return _LeaveMucVisability; }
            set
            {
                _LeaveMucVisability = value;
                OnPropertyChanged();
            }
        }
        private Visibility _DebugVisability;
        public Visibility DebugVisability
        {
            get { return _DebugVisability; }
            set
            {
                _DebugVisability = value;
                OnPropertyChanged();
            }
        }
        private Visibility _OmemoVisability;
        public Visibility OmemoVisability
        {
            get { return _OmemoVisability; }
            set
            {
                _OmemoVisability = value;
                OnPropertyChanged();
            }
        }
        private bool _OmemoEnabled;
        public bool OmemoEnabled
        {
            get { return _OmemoEnabled; }
            set
            {
                _OmemoEnabled = value;
                OnPropertyChanged();
            }
        }
        private bool _IsLoadingChatMessages;
        public bool IsLoadingChatMessages
        {
            get { return _IsLoadingChatMessages; }
            set
            {
                _IsLoadingChatMessages = value;
                OnPropertyChanged();
            }
        }
        private string _MessageText;
        public string MessageText
        {
            get { return _MessageText; }
            set
            {
                _MessageText = value;
                OnPropertyChanged();
                Logging.Logger.Debug("MessageText: " + _MessageText);
            }
        }

        public readonly CustomObservableCollection<ChatMessageDataTemplate> CHAT_MESSAGES = new CustomObservableCollection<ChatMessageDataTemplate>();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


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

        public void UpdateViewChat(ChatTable chat)
        {
            if (!(chat is null))
            {
                LoadChatMessages(chat);
                MarkChatMessagesAsRead(chat);

                if (chat.chatType == ChatType.MUC)
                {
                    OmemoVisability = Visibility.Collapsed;
                }
                else
                {
                    NameText = chat.chatJabberId ?? "";
                    StatusText = chat.chatState ?? chat.status ?? "";
                    JoinMucVisability = Visibility.Collapsed;
                    LeaveMucVisability = Visibility.Collapsed;
                    OmemoEnabled = chat.omemoEnabled;
                    OmemoVisability = Visibility.Visible;
                }
            }
        }

        public void UpdateViewMuc(ChatTable chat, MUCChatInfoTable muc)
        {
            if (!(muc is null) && !(chat is null) && string.Equals(chat.id, muc.chatId))
            {
                NameText = string.IsNullOrWhiteSpace(muc.name) ? chat.chatJabberId : muc.name;
                StatusText = muc.subject ?? "";
            }
        }

        #endregion

        #region --Misc Methods (Private)--
        /// <summary>
        /// Starts a new Task and loads all chat messages for the given chat.
        /// </summary>
        /// <param name="chat">The chat which all chat messages should get loaded for.</param>
        private void LoadChatMessages(ChatTable chat)
        {
            Task.Run(() =>
            {
                IsLoadingChatMessages = true;
                CHAT_MESSAGES.Clear();
                List<ChatMessageDataTemplate> msgs = new List<ChatMessageDataTemplate>();
                foreach (ChatMessageTable msg in ChatDBManager.INSTANCE.getAllChatMessagesForChat(chat.id))
                {
                    msgs.Add(new ChatMessageDataTemplate
                    {
                        Message = msg,
                        Chat = chat
                    });
                }
                CHAT_MESSAGES.AddRange(msgs);
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

        private void OnMessageTextChanged(string oldValue, string newValue)
        {

        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
