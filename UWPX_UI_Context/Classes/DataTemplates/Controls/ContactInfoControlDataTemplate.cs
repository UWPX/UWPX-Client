using Manager.Classes;
using Shared.Classes;
using Storage.Classes.Models.Chat;
using Windows.UI.Xaml;
using XMPP_API.Classes;

namespace UWPX_UI_Context.Classes.DataTemplates.Controls
{
    public class ContactInfoControlDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private bool _PresenceFlyoutEnabled;
        public bool PresenceFlyoutEnabled
        {
            get => _PresenceFlyoutEnabled;
            set => SetProperty(ref _PresenceFlyoutEnabled, value);
        }
        private Visibility _RequestPresenceSubscriptionVisibility;
        public Visibility RequestPresenceSubscriptionVisibility
        {
            get => _RequestPresenceSubscriptionVisibility;
            set => SetProperty(ref _RequestPresenceSubscriptionVisibility, value);
        }
        private Visibility _CancelPresenceSubscriptionVisibility;
        public Visibility CancelPresenceSubscriptionVisibility
        {
            get => _CancelPresenceSubscriptionVisibility;
            set => SetProperty(ref _CancelPresenceSubscriptionVisibility, value);
        }
        private Visibility _RejectPresenceSubscriptionVisibility;
        public Visibility RejectPresenceSubscriptionVisibility
        {
            get => _RejectPresenceSubscriptionVisibility;
            set => SetProperty(ref _RejectPresenceSubscriptionVisibility, value);
        }
        private Visibility _ProbePresenceVisibility;
        public Visibility ProbePresenceVisibility
        {
            get => _ProbePresenceVisibility;
            set => SetProperty(ref _ProbePresenceVisibility, value);
        }
        private string _RemoveFromRosterText;
        public string RemoveFromRosterText
        {
            get => _RemoveFromRosterText;
            set => SetProperty(ref _RemoveFromRosterText, value);
        }
        private string _AccountBareJid;
        public string AccountBareJid
        {
            get => _AccountBareJid;
            set => SetProperty(ref _AccountBareJid, value);
        }
        private string _ChatBareJid;
        public string ChatBareJid
        {
            get => _ChatBareJid;
            set => SetChatBareJidProperty(value);
        }
        private string _ChatStatus;
        public string ChatStatus
        {
            get => _ChatStatus;
            set => SetProperty(ref _ChatStatus, value);
        }
        private string _ChatState;
        public string ChatState
        {
            get => _ChatState;
            set => SetProperty(ref _ChatState, value);
        }
        private string _MuteGlyph;
        public string MuteGlyph
        {
            get => _MuteGlyph;
            set => SetProperty(ref _MuteGlyph, value);
        }
        private string _MuteTooltip;
        public string MuteTooltip
        {
            get => _MuteTooltip;
            set => SetProperty(ref _MuteTooltip, value);
        }
        private Presence _Presence;
        public Presence Presence
        {
            get => _Presence;
            set => SetProperty(ref _Presence, value);
        }
        private string _Nickname;
        public string Nickname
        {
            get => _Nickname;
            set => SetNicknameProperty(value);
        }
        private bool _DifferentNickname;
        public bool DifferentNickname
        {
            get => _DifferentNickname;
            set => SetProperty(ref _DifferentNickname, value);
        }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private void SetNicknameProperty(string value)
        {
            if (SetProperty(ref _Nickname, value, nameof(Nickname)))
            {
                DifferentNickname = !string.Equals(Nickname, ChatBareJid);
            }
        }

        private void SetChatBareJidProperty(string value)
        {
            if (SetProperty(ref _ChatBareJid, value, nameof(ChatBareJid)))
            {
                DifferentNickname = !string.Equals(Nickname, ChatBareJid);
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void UpdateView(Client client)
        {
            AccountBareJid = client.dbAccount.bareJid;
        }

        public void UpdateView(ChatModel chat)
        {
            Presence = chat.presence;

            // Subscription state:
            ProbePresenceVisibility = Visibility.Collapsed;
            RequestPresenceSubscriptionVisibility = Visibility.Collapsed;
            CancelPresenceSubscriptionVisibility = Visibility.Collapsed;
            RejectPresenceSubscriptionVisibility = Visibility.Collapsed;
            PresenceFlyoutEnabled = true;

            switch (chat.subscription)
            {
                case "to":
                    CancelPresenceSubscriptionVisibility = Visibility.Visible;
                    ProbePresenceVisibility = Visibility.Visible;
                    break;
                case "both":
                    CancelPresenceSubscriptionVisibility = Visibility.Visible;
                    ProbePresenceVisibility = Visibility.Visible;
                    RejectPresenceSubscriptionVisibility = Visibility.Visible;
                    break;
                case "subscribe":
                    PresenceFlyoutEnabled = false;
                    break;
                case "unsubscribe":
                    RequestPresenceSubscriptionVisibility = Visibility.Visible;
                    break;
                case "from":
                    RequestPresenceSubscriptionVisibility = Visibility.Visible;
                    RejectPresenceSubscriptionVisibility = Visibility.Visible;
                    break;
                case "none":
                default:
                    RequestPresenceSubscriptionVisibility = Visibility.Visible;
                    break;
            }

            // Menu flyout:
            RemoveFromRosterText = chat.inRoster ? "Remove from roster" : "Add to roster";

            // Info:
            ChatBareJid = chat.bareJid;
            Nickname = chat.bareJid;
            ChatStatus = chat.status;
            ChatState = chat.chatState;
            MuteGlyph = chat.muted ? "\uE74F" : "\uE767";
            MuteTooltip = chat.muted ? "Unmute" : "Mute";
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
