using Shared.Classes;
using Storage.Classes.Models.Chat;
using Windows.UI.Xaml;

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
        private string _ChatName;
        public string ChatName
        {
            get => _ChatName;
            set => SetProperty(ref _ChatName, value);
        }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void UpdateView(ChatModel chat)
        {
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
            MuteGlyph = chat.muted ? "\uE74F" : "\uE767";
            MuteTooltip = chat.muted ? "Unmute" : "Mute";
            ChatName = string.IsNullOrEmpty(chat.contactInfo.name) ? chat.bareJid : chat.contactInfo.name;
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
