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

namespace UWP_XMPP_Client.Controls
{
    public sealed partial class MUCInfoControl : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public ChatTable Chat
        {
            get { return (ChatTable)GetValue(ChatProperty); }
            set
            {
                SetValue(ChatProperty, value);
                showSubject();
            }
        }
        public static readonly DependencyProperty ChatProperty = DependencyProperty.Register("Chat", typeof(ChatTable), typeof(MUCInfoControl), null);

        public XMPPClient Client
        {
            get { return (XMPPClient)GetValue(ClientProperty); }
            set { SetValue(ClientProperty, value); }
        }
        public static readonly DependencyProperty ClientProperty = DependencyProperty.Register("Client", typeof(XMPPClient), typeof(MUCInfoControl), null);

        public MUCChatInfoTable MUCInfo
        {
            get { return (MUCChatInfoTable)GetValue(MUCInfoProperty); }
            set
            {
                SetValue(MUCInfoProperty, value);
                showMUCInfo();
                showSubject();
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


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void showMUCInfo()
        {
            if (MUCInfo != null)
            {
                Presence presence = MUCInfo.getMUCPresence();
                autoJoin_tgls.IsOn = MUCInfo.autoEnterRoom;
                enterState_tbx.Foreground = UiUtils.getPresenceBrush(presence);
                switch (presence)
                {
                    case Presence.Online:
                        enterState_tbx.Text = "joined";
                        join_btn.IsEnabled = false;
                        leave_btn.IsEnabled = true;
                        break;

                    case Presence.Chat:
                        enterState_tbx.Text = "joining/leaving...";
                        join_btn.IsEnabled = false;
                        leave_btn.IsEnabled = true;
                        break;

                    case Presence.Xa:
                        enterState_tbx.Text = "ERROR - view the log for more information";
                        leave_btn.IsEnabled = false;
                        join_btn.IsEnabled = true;
                        break;

                    default:
                        enterState_tbx.Text = "not joined yet";
                        leave_btn.IsEnabled = false;
                        join_btn.IsEnabled = true;
                        break;
                }
            }
        }

        private void showSubject()
        {
            if (Chat != null && MUCInfo != null)
            {
                string chatJID = Chat.id;
                string nick = MUCInfo.nickname;
                Task.Run(async () =>
                {
                    MUCMemberTable member = MUCDBManager.INSTANCE.getMUCMember(chatJID, nick);
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => subject_stbx.IsReadOnly = !(member != null && member.role == MUCRole.MODERATOR));
                });
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
            MUCChangeNicknameMessage msg = new MUCChangeNicknameMessage(Client.getXMPPAccount().getIdDomainAndResource(), Chat.chatJabberId, nickname_stbx.Text);
            changeNickHelper = new MessageResponseHelper<PresenceMessage>(Client, onChangeNickMessage, onChangeNickTimeout)
            {
                matchId = false
            };
            changeNickHelper.start(msg);
        }

        private bool onChangeNickMessage(PresenceMessage msg)
        {
            if (msg is MUCMemberPresenceMessage)
            {
                MUCMemberPresenceMessage mPMessage = msg as MUCMemberPresenceMessage;
                if (mPMessage.STATUS_CODES.Contains(MUCPresenceStatusCode.PRESENCE_SELFE_REFERENCE) && (mPMessage.STATUS_CODES.Contains(MUCPresenceStatusCode.MEMBER_NICK_CHANGED) || mPMessage.STATUS_CODES.Contains(MUCPresenceStatusCode.ROOM_NICK_CHANGED)))
                {
                    Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        nickname_stbx.Text = mPMessage.NICKNAME;
                        nickname_stbx.onSavingDone();
                        notificationBanner_ian.Show("Successfully changed nickname to: " + mPMessage.NICKNAME, 5000);
                    }).AsTask();
                    return true;
                }
            }
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
                nickname_stbx.Text = MUCInfo.nickname;
                nickname_stbx.onSavingDone();
                notificationBanner_ian.Show("Changing nickname failed (time out)!\nPlease retry.");
            }).AsTask();
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

        #endregion
    }
}
