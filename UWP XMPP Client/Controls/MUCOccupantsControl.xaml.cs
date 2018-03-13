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
using XMPP_API.Classes.Network.XML.Messages.XEP_0045;

namespace UWP_XMPP_Client.Controls
{
    public sealed partial class MUCOccupantsControl : UserControl
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
        public static readonly DependencyProperty ChatProperty = DependencyProperty.Register("Chat", typeof(ChatTable), typeof(MUCOccupantsControl), null);

        public XMPPClient Client
        {
            get { return (XMPPClient)GetValue(ClientProperty); }
            set { SetValue(ClientProperty, value); }
        }
        public static readonly DependencyProperty ClientProperty = DependencyProperty.Register("Client", typeof(XMPPClient), typeof(MUCOccupantsControl), null);

        public MUCChatInfoTable MUCInfo
        {
            get { return (MUCChatInfoTable)GetValue(MUCInfoProperty); }
            set { SetValue(MUCInfoProperty, value); }
        }
        public static readonly DependencyProperty MUCInfoProperty = DependencyProperty.Register("MUCInfo", typeof(MUCChatInfoTable), typeof(MUCOccupantsControl), null);

        private ObservableCollection<MUCOccupantTemplate> occupants;

        private bool canBan;
        private bool canKick;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 06/02/2018 Created [Fabian Sauter]
        /// </history>
        public MUCOccupantsControl()
        {
            this.occupants = new ObservableCollection<MUCOccupantTemplate>();
            MUCDBManager.INSTANCE.MUCOccupantChanged += INSTANCE_MUCMemberChanged;
            this.canBan = false;
            this.canKick = false;
            this.InitializeComponent();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private void setCanKickBan()
        {
            if (MUCInfo != null)
            {
                string chatId = MUCInfo.chatId;
                string nickname = MUCInfo.nickname;
                Task.Run(() =>
                {
                    MUCOccupantTable occupant = MUCDBManager.INSTANCE.getMUCOccupant(chatId, nickname);
                    if (occupant == null)
                    {
                        canBan = false;
                        canKick = false;
                    }
                    else
                    {
                        canKick = occupant.role >= MUCRole.MODERATOR;
                        canBan = occupant.affiliation >= MUCAffiliation.ADMIN;
                    }
                    Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => setIfKickBanButtonEnabled()).AsTask();
                });
            }
            else
            {
                canBan = false;
                canKick = false;
                setIfKickBanButtonEnabled();
            }
        }

        private void setIfKickBanButtonEnabled()
        {
            kickBan_btn.IsEnabled = (canBan || canKick) && members_dgrid.SelectedItems.Count > 0;
        }

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
                    List<MUCOccupantTable> list = MUCDBManager.INSTANCE.getAllMUCMembers(chatId);
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        occupants.Clear();
                        foreach (MUCOccupantTable m in list)
                        {
                            occupants.Add(new MUCOccupantTemplate() { occupant = m });
                        }
                    });
                });
            }
        }

        private async Task inviteUserAsync()
        {
            List<string> membersJidList = new List<string>();
            foreach (MUCOccupantTemplate m in occupants)
            {
                if (m.jid != null)
                {
                    membersJidList.Add(m.jid);
                }
            }
            InviteUserMUCDialog dialog = new InviteUserMUCDialog(Client, membersJidList);
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
            setIfKickBanButtonEnabled();
        }

        private async void invite_btn_Click(object sender, RoutedEventArgs e)
        {
            await inviteUserAsync();
        }

        private async void INSTANCE_MUCMemberChanged(MUCDBManager handler, Data_Manager2.Classes.Events.MUCOccupantChangedEventArgs args)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (Chat == null || !Equals(args.MUC_OCCUPANT.chatId, Chat.id))
                {
                    return;
                }

                // Update kick ban button IsEnabled:
                if (MUCInfo != null && Equals(args.MUC_OCCUPANT.nickname, MUCInfo.nickname))
                {
                    setCanKickBan();
                    setIfKickBanButtonEnabled();
                }

                // Add occupant to collection:
                for (int i = 0; i < occupants.Count; i++)
                {
                    if (Equals(occupants[i].occupant.id, args.MUC_OCCUPANT.id))
                    {
                        if (args.REMOVED)
                        {
                            occupants.RemoveAt(i);
                        }
                        else
                        {
                            occupants[i] = new MUCOccupantTemplate()
                            {
                                occupant = args.MUC_OCCUPANT
                            };
                        }
                        return;
                    }
                }
                occupants.Add(new MUCOccupantTemplate() { occupant = args.MUC_OCCUPANT });
            });
        }

        private async void kickBan_btn_Click(object sender, RoutedEventArgs e)
        {
            if (members_dgrid.SelectedItems.Count > 0)
            {
                ObservableCollection<MUCOccupantTemplate> collection = new ObservableCollection<MUCOccupantTemplate>();
                foreach (object o in members_dgrid.SelectedItems)
                {
                    if (o is MUCOccupantTemplate)
                    {
                        collection.Add(o as MUCOccupantTemplate);
                    }
                }

                MUCKickBanOccupantDialog dialog = new MUCKickBanOccupantDialog(collection, Client, Chat, canKick, canBan);
                await dialog.ShowAsync();
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            setCanKickBan();
        }

        #endregion
    }
}
