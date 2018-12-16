using Data_Manager2.Classes;
using Data_Manager2.Classes.DBManager;
using Data_Manager2.Classes.DBTables;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using XMPP_API.Classes;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.XEP_0249;

namespace UWPX_UI_Context.Classes.DataTemplates
{
    public class ChatMasterControlDataTemplate : AbstractNotifyPropertyChanged
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private Visibility _RequestPresenceSubscriptionVisability;
        public Visibility RequestPresenceSubscriptionVisability
        {
            get { return _RequestPresenceSubscriptionVisability; }
            set
            {
                _RequestPresenceSubscriptionVisability = value;
                OnPropertyChanged();
            }
        }
        private Visibility _CancelPresenceSubscriptionVisability;
        public Visibility CancelPresenceSubscriptionVisability
        {
            get { return _CancelPresenceSubscriptionVisability; }
            set
            {
                _CancelPresenceSubscriptionVisability = value;
                OnPropertyChanged();
            }
        }
        private Visibility _RejectPresenceSubscriptionVisability;
        public Visibility RejectPresenceSubscriptionVisability
        {
            get { return _RejectPresenceSubscriptionVisability; }
            set
            {
                _RejectPresenceSubscriptionVisability = value;
                OnPropertyChanged();
            }
        }
        private Visibility _ProbePresenceVisability;
        public Visibility ProbePresenceVisability
        {
            get { return _ProbePresenceVisability; }
            set
            {
                _ProbePresenceVisability = value;
                OnPropertyChanged();
            }
        }
        private Visibility _AccountActionsVisability;
        public Visibility AccountActionsVisability
        {
            get { return _AccountActionsVisability; }
            set
            {
                _AccountActionsVisability = value;
                OnPropertyChanged();
            }
        }
        private bool _PresenceFlyoutEnabled;
        public bool PresenceFlyoutEnabled
        {
            get { return _PresenceFlyoutEnabled; }
            set
            {
                _PresenceFlyoutEnabled = value;
                OnPropertyChanged();
            }
        }
        private SolidColorBrush _AccountColorBrush;
        public SolidColorBrush AccountColorBrush
        {
            get { return _AccountColorBrush; }
            set
            {
                _AccountColorBrush = value;
                OnPropertyChanged();
            }
        }
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
        private string _MuteText;
        public string MuteText
        {
            get { return _MuteText; }
            set
            {
                _MuteText = value;
                OnPropertyChanged();
            }
        }
        private string _RemoveFromRosterText;
        public string RemoveFromRosterText
        {
            get { return _RemoveFromRosterText; }
            set
            {
                _RemoveFromRosterText = value;
                OnPropertyChanged();
            }
        }
        private string _InfoText;
        public string InfoText
        {
            get { return _InfoText; }
            set
            {
                _InfoText = value;
                OnPropertyChanged();
            }
        }
        private Visibility _InfoTextVisability;
        public Visibility InfoTextVisability
        {
            get { return _InfoTextVisability; }
            set
            {
                _InfoTextVisability = value;
                OnPropertyChanged();
            }
        }
        private Visibility _InRosterVisability;
        public Visibility InRosterVisability
        {
            get { return _InRosterVisability; }
            set
            {
                _InRosterVisability = value;
                OnPropertyChanged();
            }
        }
        private string _LastActionText;
        public string LastActionText
        {
            get { return _LastActionText; }
            set
            {
                _LastActionText = value;
                OnPropertyChanged();
            }
        }
        private string _LastActionIconText;
        public string LastActionIconText
        {
            get { return _LastActionIconText; }
            set
            {
                _LastActionIconText = value;
                OnPropertyChanged();
            }
        }
        private Visibility _LastActionIconVisability;
        public Visibility LastActionIconVisability
        {
            get { return _LastActionIconVisability; }
            set
            {
                _LastActionIconVisability = value;
                OnPropertyChanged();
            }
        }
        private MessageState _LastActionState;
        public MessageState LastActionState
        {
            get { return _LastActionState; }
            set
            {
                _LastActionState = value;
                OnPropertyChanged();
            }
        }

        private readonly ResourceDictionary RESOURCES;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ChatMasterControlDataTemplate(ResourceDictionary resources)
        {
            this.RESOURCES = resources;
        }

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
                // Account color:
                if (UiUtils.IsHexColor(client.getXMPPAccount().color))
                {
                    AccountColorBrush = UiUtils.HexStringToBrush(client.getXMPPAccount().color);
                    AccountColorBrush.Opacity = 0.9;
                }
                else
                {
                    AccountColorBrush = new SolidColorBrush(Colors.Transparent);
                }
            }
        }

        public void UpdateViewChat(ChatTable chat)
        {
            if (!(chat is null))
            {
                if (chat.chatType != ChatType.MUC)
                {
                    // Chat jabber id:
                    NameText = chat.chatJabberId;

                    // Subscription state:
                    AccountActionsVisability = Visibility.Collapsed;
                    RequestPresenceSubscriptionVisability = Visibility.Collapsed;
                    CancelPresenceSubscriptionVisability = Visibility.Collapsed;
                    RejectPresenceSubscriptionVisability = Visibility.Collapsed;
                    ProbePresenceVisability = Visibility.Collapsed;
                    PresenceFlyoutEnabled = true;

                    switch (chat.subscription)
                    {
                        case "to":
                            CancelPresenceSubscriptionVisability = Visibility.Visible;
                            ProbePresenceVisability = Visibility.Visible;
                            break;
                        case "both":
                            CancelPresenceSubscriptionVisability = Visibility.Visible;
                            ProbePresenceVisability = Visibility.Visible;
                            RejectPresenceSubscriptionVisability = Visibility.Visible;
                            break;
                        case "subscribe":
                            PresenceFlyoutEnabled = false;
                            // showPresenceSubscriptionRequest();
                            break;
                        case "unsubscribe":
                            RequestPresenceSubscriptionVisability = Visibility.Visible;
                            // showRemovedChat();
                            break;
                        case "from":
                            RequestPresenceSubscriptionVisability = Visibility.Visible;
                            RejectPresenceSubscriptionVisability = Visibility.Visible;
                            break;
                        case "none":
                        default:
                            RequestPresenceSubscriptionVisability = Visibility.Visible;
                            break;
                    }

                    // Menu flyout:
                    MuteText = chat.muted ? "Unmute" : "Mute";
                    RemoveFromRosterText = chat.inRoster ? "Remove from roster" : "Add to roster";
                }

                // Subscription pending:
                if (chat.subscriptionRequested)
                {
                    InfoText = "Subscription pending...";
                    InfoTextVisability = Visibility.Visible;
                    CancelPresenceSubscriptionVisability = Visibility.Visible;
                    RequestPresenceSubscriptionVisability = Visibility.Collapsed;
                }
                else
                {
                    InfoTextVisability = Visibility.Collapsed;
                }

                // Last chat message:
                UpdateLastAction(chat);

                // Status icons:
                InRosterVisability = chat.inRoster ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public void UpdateViewMuc(ChatTable chat, MUCChatInfoTable muc)
        {
            if (!(muc is null) && !(chat is null))
            {
                NameText = string.IsNullOrWhiteSpace(muc.name) ? chat.chatJabberId : muc.name;
                RemoveFromRosterText = chat.inRoster ? "Remove bookmark" : "Bookmark";
            }
        }

        #endregion

        #region --Misc Methods (Private)--
        private void UpdateLastAction(ChatTable chat)
        {
            Task.Run(() =>
            {
                ChatMessageTable lastMsg = ChatDBManager.INSTANCE.getLastChatMessageForChat(chat.id);
                if (lastMsg is null)
                {
                    LastActionIconText = "";
                    LastActionIconVisability = Visibility.Collapsed;
                }
                else
                {
                    // Text and icon:
                    if (lastMsg.isImage)
                    {
                        LastActionIconText = "\uE722";
                        LastActionIconVisability = Visibility.Visible;
                        LastActionText = lastMsg.message ?? "You received an image";
                    }
                    else
                    {
                        switch (lastMsg.type)
                        {
                            case DirectMUCInvitationMessage.TYPE_MUC_DIRECT_INVITATION:
                                LastActionIconText = "\uE8F2";
                                LastActionIconVisability = Visibility.Visible;
                                LastActionText = "You have been invited to a MUC room";
                                break;

                            case MessageMessage.TYPE_ERROR:
                                LastActionIconText = "\xE7BA";
                                LastActionIconVisability = Visibility.Visible;
                                LastActionText = lastMsg.message ?? "You received an error message";
                                break;

                            case MUCHandler.TYPE_CHAT_INFO:
                                LastActionIconText = "\uE946";
                                LastActionIconVisability = Visibility.Visible;
                                LastActionText = (lastMsg.message ?? "-");
                                break;

                            default:
                                LastActionIconVisability = Visibility.Collapsed;
                                LastActionText = lastMsg.message ?? "";
                                break;
                        }
                    }
                }
            });
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
