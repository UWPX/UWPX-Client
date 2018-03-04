using System;
using Data_Manager2.Classes.DBManager;
using Data_Manager2.Classes.DBTables;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using UWP_XMPP_Client.DataTemplates;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using XMPP_API.Classes;
using UWP_XMPP_Client.Dialogs;
using XMPP_API.Classes.Network.XML.Messages.XEP_0249;

namespace UWP_XMPP_Client.Controls
{
    public sealed partial class MUCMembersControl : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public ChatTable Chat
        {
            get { return (ChatTable)GetValue(ChatProperty); }
            set
            {
                SetValue(ChatProperty, value);
                loadMembers();
            }
        }
        public static readonly DependencyProperty ChatProperty = DependencyProperty.Register("Chat", typeof(ChatTable), typeof(MUCMembersControl), null);

        public XMPPClient Client
        {
            get { return (XMPPClient)GetValue(ClientProperty); }
            set { SetValue(ClientProperty, value); }
        }
        public static readonly DependencyProperty ClientProperty = DependencyProperty.Register("Client", typeof(XMPPClient), typeof(MUCMembersControl), null);

        public MUCChatInfoTable MUCInfo
        {
            get { return (MUCChatInfoTable)GetValue(MUCInfoProperty); }
            set { SetValue(MUCInfoProperty, value); }
        }
        public static readonly DependencyProperty MUCInfoProperty = DependencyProperty.Register("MUCInfo", typeof(MUCChatInfoTable), typeof(MUCMembersControl), null);

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(string), typeof(MUCMembersControl), null);

        private ObservableCollection<MUCMemberTemplate> members;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 06/02/2018 Created [Fabian Sauter]
        /// </history>
        public MUCMembersControl()
        {
            this.members = new ObservableCollection<MUCMemberTemplate>();
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
        private void loadMembers()
        {
            if (Chat != null)
            {
                string chatId = Chat.id;
                Task.Run(async () =>
                {
                    List<MUCMemberTable> list = MUCDBManager.INSTANCE.getAllMUCMembers(chatId);
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        members.Clear();
                        foreach (MUCMemberTable m in list)
                        {
                            members.Add(new MUCMemberTemplate() { member = m });
                        }
                    });
                });
            }
        }

        private async Task inviteUserAsync()
        {
            InviteUserMUCDialog dialog = new InviteUserMUCDialog();
            await dialog.ShowAsync();

            if (!dialog.canceled)
            {
                string reason = null;
                if (!string.IsNullOrWhiteSpace(dialog.Reason))
                {
                    reason = dialog.Reason;
                }
                DirectMUCInvitationMessage msg = new DirectMUCInvitationMessage(Client.getXMPPAccount().getIdAndDomain(), dialog.UserJid, Chat.chatJabberId, MUCInfo.password, reason);
                await Client.sendMessageAsync(msg, true);
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void members_dgrid_SelectionChanged(object sender, Telerik.UI.Xaml.Controls.Grid.DataGridSelectionChangedEventArgs e)
        {
            remove_btn.IsEnabled = members_dgrid.SelectedItems.Count > 0;
        }

        private async void invite_btn_Click(object sender, RoutedEventArgs e)
        {
            await inviteUserAsync();
        }

        private void remove_btn_Click(object sender, RoutedEventArgs e)
        {

        }

        #endregion
    }
}
