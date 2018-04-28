using Data_Manager2.Classes;
using Data_Manager2.Classes.DBManager;
using Data_Manager2.Classes.DBTables;
using System.Threading.Tasks;
using UWP_XMPP_Client.Classes;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using XMPP_API.Classes;
using XMPP_API.Classes.Network.XML.Messages.XEP_0045;
using System;
using XMPP_API.Classes.Network.XML.Messages;
using UWP_XMPP_Client.Pages.SettingsPages;

namespace UWP_XMPP_Client.Controls
{
    public sealed partial class MUCInfoControl : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public ChatTable Chat
        {
            get { return (ChatTable)GetValue(ChatProperty); }
            set { SetValue(ChatProperty, value); }
        }
        public static readonly DependencyProperty ChatProperty = DependencyProperty.Register("Chat", typeof(ChatTable), typeof(MUCInfoControl), null);

        public XMPPClient Client
        {
            get { return (XMPPClient)GetValue(ClientProperty); }
            set
            {
                if (Client != null)
                {
                    Client.ConnectionStateChanged -= Client_ConnectionStateChanged;
                }
                SetValue(ClientProperty, value);
                if (Client != null)
                {
                    Client.ConnectionStateChanged += Client_ConnectionStateChanged;
                    showMUCInfo();
                }
            }
        }
        public static readonly DependencyProperty ClientProperty = DependencyProperty.Register("Client", typeof(XMPPClient), typeof(MUCInfoControl), null);

        public MUCChatInfoTable MUCInfo
        {
            get { return (MUCChatInfoTable)GetValue(MUCInfoProperty); }
            set
            {
                SetValue(MUCInfoProperty, value);
                showMUCInfo();
            }
        }
        public static readonly DependencyProperty MUCInfoProperty = DependencyProperty.Register("MUCInfo", typeof(MUCChatInfoTable), typeof(MUCInfoControl), null);

        private MessageResponseHelper<PresenceMessage> changeNickHelper;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 12/02/2018 Created [Fabian Sauter]
        /// </history>
        public MUCInfoControl()
        {
            this.changeNickHelper = null;
            this.InitializeComponent();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private void setSubjectIsEnabled()
        {
            subject_stbx.IsEnabled = isClientConnected() && isMUCEntered();
        }

        private void setNotConnectedVisibility()
        {
            notConnected_itbx.Visibility = isClientConnected() ? Visibility.Collapsed : Visibility.Visible;
        }

        private bool isMUCEntered()
        {
            return MUCInfo != null && MUCInfo.state == MUCState.ENTERD;
        }

        private bool isClientConnected()
        {
            return Client != null && Client.getConnetionState() == XMPP_API.Classes.Network.ConnectionState.CONNECTED;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--


        private void showMUCInfo()
        {
            if (MUCInfo != null && Client != null)
            {
                Presence presence = MUCInfo.getMUCPresence();
                autoJoin_tgls.IsOn = MUCInfo.autoEnterRoom;
                enterState_tbx.Foreground = UiUtils.getPresenceBrush(presence);

                setSubjectIsEnabled();
                setNotConnectedVisibility();

                join_btn.IsEnabled = false;
                leave_btn.IsEnabled = false;

                switch (MUCInfo.state)
                {
                    case MUCState.DISCONNECTED:
                        enterState_tbx.Text = "not joined yet";
                        if (Client.getConnetionState() == XMPP_API.Classes.Network.ConnectionState.CONNECTED)
                        {
                            join_btn.IsEnabled = true;
                        }
                        break;

                    case MUCState.DISCONNECTING:
                        enterState_tbx.Text = "leaving...";
                        if (Client.getConnetionState() == XMPP_API.Classes.Network.ConnectionState.CONNECTED)
                        {
                            leave_btn.IsEnabled = true;
                        }
                        break;

                    case MUCState.ENTERING:
                        enterState_tbx.Text = "joining...";
                        if (Client.getConnetionState() == XMPP_API.Classes.Network.ConnectionState.CONNECTED)
                        {
                            leave_btn.IsEnabled = true;
                        }
                        break;

                    case MUCState.ENTERD:
                        enterState_tbx.Text = "joined";
                        if (Client.getConnetionState() == XMPP_API.Classes.Network.ConnectionState.CONNECTED)
                        {
                            leave_btn.IsEnabled = true;
                        }
                        break;

                    case MUCState.KICKED:
                        enterState_tbx.Text = "You have been kicked!";
                        if (Client.getConnetionState() == XMPP_API.Classes.Network.ConnectionState.CONNECTED)
                        {
                            join_btn.IsEnabled = true;
                        }
                        break;

                    case MUCState.BANED:
                        enterState_tbx.Text = "You have been baned!";
                        if (Client.getConnetionState() == XMPP_API.Classes.Network.ConnectionState.CONNECTED)
                        {
                            join_btn.IsEnabled = true;
                        }
                        break;

                    case MUCState.ERROR:
                    default:
                        enterState_tbx.Text = "ERROR - view the log for more information";
                        if (Client.getConnetionState() == XMPP_API.Classes.Network.ConnectionState.CONNECTED)
                        {
                            leave_btn.IsEnabled = true;
                        }
                        break;
                }
            }
        }

        private async Task leaveRoomAsync()
        {
            if (Client != null && MUCInfo != null && Chat != null)
            {
                await MUCHandler.INSTANCE.leaveRoomAsync(Client, Chat, MUCInfo);
            }
        }

        private async Task joinRoomAsync()
        {
            if (Client != null && MUCInfo != null && Chat != null)
            {
                await MUCHandler.INSTANCE.enterMUCAsync(Client, Chat, MUCInfo);
            }
        }

        private void saveSubject()
        {
            subject_stbx.onStartSaving();
            notificationBanner_ian.Dismiss();

            string from = Client.getXMPPAccount().getIdAndDomain() + '/' + MUCInfo.nickname;
            string to = Chat.chatJabberId;
            string id = Chat.id;
            MUCRoomSubjectMessage msg = new MUCRoomSubjectMessage(from, to, subject_stbx.Text);
            Task t = Client.sendMessageAsync(msg, true);
            Task.Run(async () =>
            {
                MUCDBManager.INSTANCE.setMUCSubject(id, msg.SUBJECT, false);

                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    notificationBanner_ian.Show("Successfully updated the room subject!", 5000);
                    subject_stbx.onSavingDone();
                });
            });
        }

        private void saveAutoJoin()
        {
            string chatId = MUCInfo.chatId;
            bool autoEnterRoom = autoJoin_tgls.IsOn;
            Task.Run(() => MUCDBManager.INSTANCE.setMUCAutoEnter(chatId, autoEnterRoom, true));
        }

        private void updateNick()
        {
            nickname_stbx.onStartSaving();

            if (string.IsNullOrEmpty(nickname_stbx.Text))
            {
                notificationBanner_ian.Show("Invalid nickname!");
                nickname_stbx.onSavingDone();
                return;
            }

            notificationBanner_ian.Dismiss();

            if (isMUCEntered())
            {
                changeNickHelper = Client.MUC_COMMAND_HELPER.changeNickname(Chat.chatJabberId, nickname_stbx.Text, onChangeNickMessage, onChangeNickTimeout);
            }
            else
            {
                MUCInfo.nickname = nickname_stbx.Text;
                MUCChatInfoTable info = MUCInfo;
                Task.Run(() => MUCDBManager.INSTANCE.setMUCChatInfo(info, false, false));
                nickname_stbx.onSavingDone();
            }
        }

        private bool onChangeNickMessage(PresenceMessage msg)
        {
            if (msg is MUCMemberPresenceMessage)
            {
                // Success:
                MUCMemberPresenceMessage mPMessage = msg as MUCMemberPresenceMessage;
                if (mPMessage.STATUS_CODES.Contains(MUCPresenceStatusCode.PRESENCE_SELFE_REFERENCE) && (mPMessage.STATUS_CODES.Contains(MUCPresenceStatusCode.MEMBER_NICK_CHANGED) || mPMessage.STATUS_CODES.Contains(MUCPresenceStatusCode.ROOM_NICK_CHANGED)))
                {
                    Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        MUCInfo.nickname = mPMessage.NICKNAME;
                        MUCChatInfoTable info = MUCInfo;
                        Task.Run(() => MUCDBManager.INSTANCE.setMUCChatInfo(info, false, false));

                        nickname_stbx.Text = mPMessage.NICKNAME;
                        nickname_stbx.onSavingDone();
                        notificationBanner_ian.Show("Successfully changed nickname to: " + mPMessage.NICKNAME, 5000);
                    }).AsTask();
                    return true;
                }
            }
            // Error:
            else if (msg is MUCErrorMessage && Equals(msg.getId(), changeNickHelper.sendId))
            {
                MUCErrorMessage errorMessage = msg as MUCErrorMessage;
                Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    nickname_stbx.Text = MUCInfo.nickname;
                    nickname_stbx.onSavingDone();
                    notificationBanner_ian.Show("Changing nickname failed:\nCode: " + errorMessage.ERROR_CODE + "\nType: " + errorMessage.ERROR_TYPE + "\nMessage:\n" + errorMessage.ERROR_MESSAGE);
                }).AsTask();
            }
            return false;
        }

        private void onChangeNickTimeout()
        {
            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                nickname_stbx.Text = MUCInfo.nickname ?? "";
                nickname_stbx.onSavingDone();
                notificationBanner_ian.Show("Changing nickname failed (time out)!\nPlease retry.");
            }).AsTask();
        }

        private void savePassword()
        {
            password_spwbx.onStartSaving();
            notificationBanner_ian.Dismiss();
            MUCInfo.password = password_spwbx.Password;
            MUCChatInfoTable info = MUCInfo;
            Task.Run(() => MUCDBManager.INSTANCE.setMUCChatInfo(info, false, true));
            password_spwbx.onSavingDone();
            notificationBanner_ian.Show("Successfully saved password!", 5000);
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private async void join_btn_Click(object sender, RoutedEventArgs e)
        {
            await joinRoomAsync();
        }

        private async void leave_btn_Click(object sender, RoutedEventArgs e)
        {
            await leaveRoomAsync();
        }

        private void nickname_stbx_SaveClick(object sender, RoutedEventArgs e)
        {
            updateNick();
        }

        private void subject_stbx_SaveClick(object sender, RoutedEventArgs e)
        {
            saveSubject();
        }

        private void autoJoin_tgls_Toggled(object sender, RoutedEventArgs e)
        {
            if (MUCInfo.autoEnterRoom != autoJoin_tgls.IsOn)
            {
                saveAutoJoin();
            }
        }

        private async void Client_ConnectionStateChanged(XMPPClient client, XMPP_API.Classes.Network.Events.ConnectionStateChangedEventArgs args)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => showMUCInfo());
        }

        private void notConnected_itbx_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            (Window.Current.Content as Frame).Navigate(typeof(AccountSettingsPage));
        }

        private void password_spwbx_SaveClick(object sender, RoutedEventArgs e)
        {
            savePassword();
        }

        #endregion
    }
}
