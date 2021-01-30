using System.Threading.Tasks;
using Shared.Classes;
using Storage.Classes;
using Storage.Classes.Models.Chat;
using Windows.UI.Xaml;
using XMPP_API.Classes;

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
        private bool _OmemoEnabled;
        public bool OmemoEnabled
        {
            get => _OmemoEnabled;
            set => SetOmemoEnabledProperty(value);
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
        private Presence _AccountPresence;
        public Presence AccountPresence
        {
            get => _AccountPresence;
            set => SetProperty(ref _AccountPresence, value);
        }
        private string _BareJid;
        public string BareJid
        {
            get => _BareJid;
            set => SetProperty(ref _BareJid, value);
        }
        private bool _IsEmojiFlyoutEnabled;
        public bool IsEmojiFlyoutEnabled
        {
            get => _IsEmojiFlyoutEnabled;
            set => SetProperty(ref _IsEmojiFlyoutEnabled, value);
        }
        private ChatType _ChatType;
        public ChatType ChatType
        {
            get => _ChatType;
            set => SetProperty(ref _ChatType, value);
        }

        private bool _EnterToSend;
        public bool EnterToSend
        {
            get => _EnterToSend;
            set => SetEnterToSendProperty(value);
        }

        private ChatModel chat;
        private MucInfoModel muc;
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
                chat.omemo.enabled = value;
                if (!isDummy)
                {
                    Task.Run(() => ChatDBManager.INSTANCE.setChat(chat, false, true));
                }
            }
        }

        private void SetEnterToSendProperty(bool value)
        {
            if (SetProperty(ref _EnterToSend, value, nameof(EnterToSend)))
            {
                Settings.SetSetting(SettingsConsts.ENTER_TO_SEND_MESSAGES, value);
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void UpdateViewClient(XMPPClient client)
        {
            if (!(client is null))
            {
                AccountText = client.getXMPPAccount().getBareJid();
            }
        }

        public void UpdateViewChat(ChatModel chat, MucInfoModel muc)
        {
            if (!(chat is null))
            {
                this.chat = chat;
                this.muc = muc;
                if (isDummy)
                {
                    AccountText = chat.userAccountId;
                }

                if (chat.chatType == ChatType.MUC)
                {
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
                else
                {
                    NameText = chat.bareJid ?? "";
                    StatusText = chat.chatState ?? chat.status ?? "";
                    EnterMucVisibility = Visibility.Collapsed;
                    LeaveMucVisibility = Visibility.Collapsed;
                    OmemoEnabled = chat.omemoEnabled;
                    OmemoVisibility = Visibility.Visible;
                }

                // Account image:
                AccountPresence = chat.presence;
                BareJid = chat.bareJid;
                ChatType = chat.chatType;
            }
            else
            {
                this.chat = null;
            }
        }

        public void UpdateViewMuc(ChatModel chat, MucInfoModel muc)
        {
            if (!(muc is null) && !(chat is null) && string.Equals(chat.id, muc.chatId))
            {
                NameText = string.IsNullOrWhiteSpace(muc.name) ? chat.bareJid : muc.name;
                StatusText = muc.subject ?? "";

                // Account image:
                AccountPresence = muc.getMUCPresence();

                OmemoEnabled = false;
            }
        }

        public void LoadSettings()
        {
            IsEmojiFlyoutEnabled = Settings.getSettingBoolean(SettingsConsts.CHAT_ENABLE_EMOJI_BUTTON);
            DebugSettingsEnabled = Settings.getSettingBoolean(SettingsConsts.DEBUG_SETTINGS_ENABLED);
            EnterToSend = Settings.getSettingBoolean(SettingsConsts.ENTER_TO_SEND_MESSAGES);
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
