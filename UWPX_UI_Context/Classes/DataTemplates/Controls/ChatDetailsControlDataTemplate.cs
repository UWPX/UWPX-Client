using Shared.Classes;
using Storage.Classes;
using Storage.Classes.Models.Account;
using Storage.Classes.Models.Chat;
using Windows.UI.Xaml;
using XMPP_API.Classes;
using XMPP_API.Classes.Network.XML.Messages.XEP_0085;

namespace UWPX_UI_Context.Classes.DataTemplates.Controls
{
    public class ChatDetailsControlDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private string _NameText;
        public string NameText
        {
            get => _NameText;
            set => SetProperty(ref _NameText, value);
        }
        private string _AccountText;
        public string AccountText
        {
            get => _AccountText;
            set => SetProperty(ref _AccountText, value);
        }
        private string _StatusText;
        public string StatusText
        {
            get => _StatusText;
            set => SetProperty(ref _StatusText, value);
        }
        private Visibility _EnterMucVisibility;
        public Visibility EnterMucVisibility
        {
            get => _EnterMucVisibility;
            set => SetProperty(ref _EnterMucVisibility, value);
        }
        private Visibility _LeaveMucVisibility;
        public Visibility LeaveMucVisibility
        {
            get => _LeaveMucVisibility;
            set => SetProperty(ref _LeaveMucVisibility, value);
        }
        private bool _DebugSettingsEnabled;
        public bool DebugSettingsEnabled
        {
            get => _DebugSettingsEnabled;
            set => SetProperty(ref _DebugSettingsEnabled, value);
        }
        private Visibility _OmemoVisibility;
        public Visibility OmemoVisibility
        {
            get => _OmemoVisibility;
            set => SetProperty(ref _OmemoVisibility, value);
        }
        private bool _IsLoadingChatMessages;
        public bool IsLoadingChatMessages
        {
            get => _IsLoadingChatMessages;
            set => SetProperty(ref _IsLoadingChatMessages, value);
        }
        private string _MessageText;
        public string MessageText
        {
            get => _MessageText;
            set => SetProperty(ref _MessageText, value);
        }
        private bool _IsEmojiFlyoutEnabled;
        public bool IsEmojiFlyoutEnabled
        {
            get => _IsEmojiFlyoutEnabled;
            set => SetProperty(ref _IsEmojiFlyoutEnabled, value);
        }
        private Presence _ChatPresence;
        public Presence ChatPresence
        {
            get => _ChatPresence;
            set => SetProperty(ref _ChatPresence, value);
        }
        private bool _EnterToSend;
        public bool EnterToSend
        {
            get => _EnterToSend;
            set => SetEnterToSendProperty(value);
        }
        private bool _Typing;
        public bool Typing
        {
            get => _Typing;
            set => SetProperty(ref _Typing, value);
        }

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
        private void SetEnterToSendProperty(bool value)
        {
            if (SetProperty(ref _EnterToSend, value, nameof(EnterToSend)))
            {
                Settings.SetSetting(SettingsConsts.ENTER_TO_SEND_MESSAGES, value);
            }
        }

        private void SetStatus(ChatModel chat)
        {
            bool typing = false;
            string status = "";
            switch (chat.chatState)
            {
                case ChatState.PAUSED:
                case ChatState.ACTIVE:
                    if (chat.chatType == ChatType.CHAT)
                    {
                        status = "online";
                    }
                    break;
                case ChatState.COMPOSING:
                    status = chat.status;
                    typing = true;
                    if (chat.chatType == ChatType.CHAT)
                    {
                        status = "typing...";
                    }
                    break;
                case ChatState.INACTIVE:
                case ChatState.GONE:
                case ChatState.UNKNOWN:
                default:
                    status = chat.status;
                    break;
            }
            Typing = typing;
            if (chat.chatType == ChatType.CHAT)
            {
                StatusText = status;
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void UpdateView(AccountModel account)
        {
            if (!(account is null))
            {
                AccountText = account.bareJid;
            }
        }

        public void UpdateView(ChatModel chat)
        {
            if (!(chat is null))
            {
                if (isDummy)
                {
                    AccountText = chat.accountBareJid;
                }

                if (chat.chatType != ChatType.MUC)
                {
                    NameText = string.IsNullOrEmpty(chat.customName) ? chat.bareJid : chat.customName;
                    EnterMucVisibility = Visibility.Collapsed;
                    LeaveMucVisibility = Visibility.Collapsed;
                    OmemoVisibility = Visibility.Visible;
                    ChatPresence = chat.presence;
                }
                SetStatus(chat);
            }
        }

        public void UpdateView(MucInfoModel muc)
        {
            if (!(muc is null))
            {
                NameText = string.IsNullOrWhiteSpace(muc.name) ? muc.chat.bareJid : muc.name;
                StatusText = muc.subject ?? "";
                ChatPresence = muc.GetMucPresence();

                OmemoVisibility = Visibility.Collapsed;
                if (muc.state != MucState.ENTERD && muc.state != MucState.ENTERING)
                {
                    EnterMucVisibility = Visibility.Visible;
                    LeaveMucVisibility = Visibility.Collapsed;
                }
                else
                {
                    EnterMucVisibility = Visibility.Collapsed;
                    LeaveMucVisibility = Visibility.Visible;
                }
            }
        }

        public void LoadSettings()
        {
            IsEmojiFlyoutEnabled = Settings.GetSettingBoolean(SettingsConsts.CHAT_ENABLE_EMOJI_BUTTON);
            DebugSettingsEnabled = Settings.GetSettingBoolean(SettingsConsts.DEBUG_SETTINGS_ENABLED);
            EnterToSend = Settings.GetSettingBoolean(SettingsConsts.ENTER_TO_SEND_MESSAGES);
        }

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
