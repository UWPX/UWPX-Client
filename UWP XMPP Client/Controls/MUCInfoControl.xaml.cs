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
                string chatJID = Chat.chatJabberId;
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

        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {

        }

        private void nickname_stbx_SaveClick(object sender, RoutedEventArgs e)
        {

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

        private void roomName_stbx_SaveClick(object sender, RoutedEventArgs e)
        {

        }

        #endregion
    }
}
