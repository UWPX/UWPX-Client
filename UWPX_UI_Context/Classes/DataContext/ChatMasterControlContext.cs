using Data_Manager2.Classes.DBTables;
using UWPX_UI_Context.Classes.DataTemplates;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using XMPP_API.Classes;

namespace UWPX_UI_Context.Classes.DataContext
{
    public sealed class ChatMasterControlContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private ChatDataTemplate chat = null;

        public SolidColorBrush AccountColorBrush { get; set; } = new SolidColorBrush(Colors.Transparent);

        public Visibility RequestPresenceSubscriptionVisability { get; set; } = Visibility.Collapsed;
        public Visibility CancelPresenceSubscriptionVisability { get; set; } = Visibility.Collapsed;
        public Visibility RejectPresenceSubscriptionVisability { get; set; } = Visibility.Collapsed;
        public Visibility ProbePresenceVisability { get; set; } = Visibility.Collapsed;
        public bool PresenceFlyoutEnabled { get; set; } = false;

        public Visibility AccountActionsVisability { get; set; } = Visibility.Collapsed;

        public string Name { get; set; } = "";

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void UpdateView(DependencyPropertyChangedEventArgs args)
        {
            if (args.OldValue is ChatDataTemplate oldChat)
            {
                oldChat.PropertyChanged -= OldChat_PropertyChanged;
            }

            if (args.NewValue is ChatDataTemplate newChat)
            {
                newChat.PropertyChanged += OldChat_PropertyChanged;
                chat = newChat;
            }
            else
            {
                chat = null;
            }

            UpdateView();
        }

        #endregion

        #region --Misc Methods (Private)--
        private void UpdateView()
        {
            if (chat is null)
            {

            }
            else
            {
                UpdateView(chat.Client);
                UpdateView(chat.Chat);
                UpdateView(chat.MucInfo);
            }
        }

        private void UpdateView(XMPPClient client)
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

        private void UpdateView(ChatTable chat)
        {
            if (!(chat is null))
            {
                if (chat.chatType != Data_Manager2.Classes.ChatType.MUC)
                {
                    // Chat jabber id:
                    Name = chat.chatJabberId;

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
                    /*mute_tmfo.Text = chat.muted ? "Unmute" : "Mute";
                    mute_tmfo.IsChecked = chat.muted;
                    removeFromRoster_mfo.Text = chat.inRoster ? "Remove from roster" : "Add to roster";*/

                    //Slide list item:
                    //slideListItem_sli.LeftLabel = removeFromRoster_mfo.Text;
                }
                else
                {

                }
            }
        }

        private void UpdateView(MUCChatInfoTable muc)
        {
            if (!(muc is null))
            {

            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void OldChat_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            UpdateView();
        }

        #endregion
    }
}
