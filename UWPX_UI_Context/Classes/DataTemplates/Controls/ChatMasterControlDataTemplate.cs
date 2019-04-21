using System.Threading.Tasks;
using Data_Manager2.Classes;
using Data_Manager2.Classes.DBManager;
using Data_Manager2.Classes.DBTables;
using Shared.Classes;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using XMPP_API.Classes;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.XEP_0249;

namespace UWPX_UI_Context.Classes.DataTemplates.Controls
{
    public class ChatMasterControlDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private Visibility _RequestPresenceSubscriptionVisability;
        public Visibility RequestPresenceSubscriptionVisability
        {
            get => _RequestPresenceSubscriptionVisability;
            set => SetProperty(ref _RequestPresenceSubscriptionVisability, value);
        }
        private Visibility _CancelPresenceSubscriptionVisability;
        public Visibility CancelPresenceSubscriptionVisability
        {
            get => _CancelPresenceSubscriptionVisability;
            set => SetProperty(ref _CancelPresenceSubscriptionVisability, value);
        }
        private Visibility _RejectPresenceSubscriptionVisability;
        public Visibility RejectPresenceSubscriptionVisability
        {
            get => _RejectPresenceSubscriptionVisability;
            set => SetProperty(ref _RejectPresenceSubscriptionVisability, value);
        }
        private Visibility _ProbePresenceVisability;
        public Visibility ProbePresenceVisability
        {
            get => _ProbePresenceVisability;
            set => SetProperty(ref _ProbePresenceVisability, value);
        }
        private Visibility _AccountActionsVisability;
        public Visibility AccountActionsVisability
        {
            get => _AccountActionsVisability;
            set => SetProperty(ref _AccountActionsVisability, value);
        }
        private bool _PresenceFlyoutEnabled;
        public bool PresenceFlyoutEnabled
        {
            get => _PresenceFlyoutEnabled;
            set => SetProperty(ref _PresenceFlyoutEnabled, value);
        }
        private SolidColorBrush _AccountColorBrush;
        public SolidColorBrush AccountColorBrush
        {
            get => _AccountColorBrush;
            set => SetProperty(ref _AccountColorBrush, value);
        }
        private string _NameText;
        public string NameText
        {
            get => _NameText;
            set => SetProperty(ref _NameText, value);
        }
        private string _MuteText;
        public string MuteText
        {
            get => _MuteText;
            set => SetProperty(ref _MuteText, value);
        }
        private string _RemoveFromRosterText;
        public string RemoveFromRosterText
        {
            get => _RemoveFromRosterText;
            set => SetProperty(ref _RemoveFromRosterText, value);
        }
        private string _InfoText;
        public string InfoText
        {
            get => _InfoText;
            set => SetProperty(ref _InfoText, value);
        }
        private Visibility _InfoTextVisability;
        public Visibility InfoTextVisability
        {
            get => _InfoTextVisability;
            set => SetProperty(ref _InfoTextVisability, value);
        }
        private Visibility _InRosterVisability;
        public Visibility InRosterVisability
        {
            get => _InRosterVisability;
            set => SetProperty(ref _InRosterVisability, value);
        }
        private string _LastActionText;
        public string LastActionText
        {
            get => _LastActionText;
            set => SetProperty(ref _LastActionText, value);
        }
        private string _LastActionIconText;
        public string LastActionIconText
        {
            get => _LastActionIconText;
            set => SetProperty(ref _LastActionIconText, value);
        }
        private MessageState _LastActionState;
        public MessageState LastActionState
        {
            get => _LastActionState;
            set => SetProperty(ref _LastActionState, value);
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
        private MUCState _MucState;
        public MUCState MucState
        {
            get => _MucState;
            set => SetProperty(ref _MucState, value);
        }
        private ChatType _ChatType;
        public ChatType ChatType
        {
            get => _ChatType;
            set => SetProperty(ref _ChatType, value);
        }
        private int _UnreadCount;
        public int UnreadCount
        {
            get => _UnreadCount;
            set => SetProperty(ref _UnreadCount, value);
        }

        private readonly ResourceDictionary RESOURCES;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ChatMasterControlDataTemplate(ResourceDictionary resources)
        {
            RESOURCES = resources;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void OnThemeChanged()
        {
            OnPropertyChanged(nameof(LastActionState));
        }

        public async void UpdateViewClient(XMPPClient client)
        {
            if (!(client is null))
            {
                // Account color:
                await SharedUtils.CallDispatcherAsync(() =>
                {
                    if (UiUtils.IsHexColor(client.getXMPPAccount().color))
                    {
                        AccountColorBrush = UiUtils.HexStringToBrush(client.getXMPPAccount().color);
                        AccountColorBrush.Opacity = 0.9;
                    }
                    else
                    {
                        AccountColorBrush = new SolidColorBrush(Colors.Transparent);
                    }
                });
            }
        }

        public void UpdateViewChat(ChatTable chat)
        {
            if (!(chat is null))
            {
                AccountActionsVisability = Visibility.Collapsed;
                if (chat.chatType != ChatType.MUC)
                {
                    // Chat jabber id:
                    NameText = chat.chatJabberId;

                    // Subscription state:
                    ProbePresenceVisability = Visibility.Collapsed;
                    RequestPresenceSubscriptionVisability = Visibility.Collapsed;
                    CancelPresenceSubscriptionVisability = Visibility.Collapsed;
                    RejectPresenceSubscriptionVisability = Visibility.Collapsed;
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
                            AccountActionsVisability = Visibility.Visible;
                            break;
                        case "unsubscribe":
                            RequestPresenceSubscriptionVisability = Visibility.Visible;
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
                    RemoveFromRosterText = chat.inRoster ? "Remove from roster" : "Add to roster";
                }

                // Menu flyout:
                MuteText = chat.muted ? "Unmute" : "Mute";

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
                UpdateUnreadCount(chat);

                // Status icons:
                InRosterVisability = chat.inRoster ? Visibility.Visible : Visibility.Collapsed;

                // Account image:
                AccountPresence = chat.presence;
                BareJid = chat.chatJabberId;
                ChatType = chat.chatType;
            }
        }

        public void UpdateViewMuc(ChatTable chat, MUCChatInfoTable muc)
        {
            if (!(muc is null) && !(chat is null))
            {
                NameText = string.IsNullOrWhiteSpace(muc.name) ? chat.chatJabberId : muc.name;
                RemoveFromRosterText = chat.inRoster ? "Remove bookmark" : "Bookmark";

                // Account image:
                AccountPresence = muc.getMUCPresence();

                MucState = muc.state;
            }
        }

        public void UpdateUnreadCount(ChatTable chat)
        {
            Task.Run(() =>
            {
                UnreadCount = ChatDBManager.INSTANCE.getUnreadCount(chat.id);
            });
        }

        public void UpdateLastAction(ChatTable chat)
        {
            Task.Run(() =>
            {
                ChatMessageTable lastMsg = ChatDBManager.INSTANCE.getLastChatMessageForChat(chat.id);
                if (lastMsg is null)
                {
                    LastActionIconText = "";
                    LastActionState = MessageState.READ;
                }
                else
                {
                    // Text and icon:
                    LastActionState = lastMsg.state;
                    if (lastMsg.isImage)
                    {
                        LastActionIconText = "\uE722";
                        LastActionText = lastMsg.message ?? "You received an image";
                    }
                    else
                    {
                        switch (lastMsg.type)
                        {
                            case DirectMUCInvitationMessage.TYPE_MUC_DIRECT_INVITATION:
                                LastActionIconText = "\uE8F2";
                                LastActionText = "You have been invited to a MUC room";
                                break;

                            case MessageMessage.TYPE_ERROR:
                                LastActionIconText = "\xE7BA";
                                LastActionText = lastMsg.message ?? "You received an error message";
                                break;

                            case MUCHandler.TYPE_CHAT_INFO:
                                LastActionIconText = "\uE946";
                                LastActionText = lastMsg.message ?? "-";
                                break;

                            default:
                                LastActionText = lastMsg.message ?? "";
                                LastActionIconText = "";
                                break;
                        }
                    }
                }
            });
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
