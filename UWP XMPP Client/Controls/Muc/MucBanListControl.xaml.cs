using System;
using Data_Manager2.Classes.DBManager;
using Data_Manager2.Classes.DBTables;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes;
using XMPP_API.Classes.Network.XML.Messages.XEP_0045;
using System.Collections.ObjectModel;
using UWP_XMPP_Client.DataTemplates;
using System.Collections.Generic;

namespace UWP_XMPP_Client.Controls.Muc
{
    public sealed partial class MucBanListControl : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public ChatTable Chat
        {
            get { return (ChatTable)GetValue(ChatProperty); }
            set { SetValue(ChatProperty, value); }
        }
        public static readonly DependencyProperty ChatProperty = DependencyProperty.Register("Chat", typeof(ChatTable), typeof(MucBanListControl), null);

        public MUCChatInfoTable MUCInfo
        {
            get { return (MUCChatInfoTable)GetValue(MUCInfoProperty); }
            set { SetValue(MUCInfoProperty, value); }
        }
        public static readonly DependencyProperty MUCInfoProperty = DependencyProperty.Register("MUCInfo", typeof(MUCChatInfoTable), typeof(MucBanListControl), null);

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
                }
            }
        }

        public static readonly DependencyProperty ClientProperty = DependencyProperty.Register("Client", typeof(XMPPClient), typeof(MucBanListControl), null);

        private readonly ObservableCollection<MUCBanedUserTemplate> BANNED_USERS;

        MessageResponseHelper<IQMessage> responseHelper;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 12/03/2018 Created [Fabian Sauter]
        /// </history>
        public MucBanListControl()
        {
            this.responseHelper = null;
            this.BANNED_USERS = new ObservableCollection<MUCBanedUserTemplate>();
            this.InitializeComponent();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private bool isAllowedToRequestBanList(string chatId, string nickname)
        {
            MUCOccupantTable occupant = MUCDBManager.INSTANCE.getMUCOccupant(chatId, nickname);
            return occupant != null && occupant.affiliation >= MUCAffiliation.ADMIN;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void showErrorMessage(string msg)
        {
            error_itbx.Text = msg;
            error_itbx.Visibility = Visibility.Visible;
        }

        private void requestBanList()
        {
            if (MUCInfo != null && Chat != null && Client != null)
            {
                if (MUCInfo.state != Data_Manager2.Classes.MUCState.ENTERD || !Client.isConnected())
                {
                    showErrorMessage("Room not joined!");
                    return;
                }

                reload_btn.IsEnabled = false;
                reload_prgr.Visibility = Visibility.Visible;
                error_itbx.Visibility = Visibility.Collapsed;

                string chatId = MUCInfo.chatId;
                string nickname = MUCInfo.nickname;
                string roomJid = Chat.chatJabberId;
                Task.Run(async () =>
                {
                    if (isAllowedToRequestBanList(chatId, nickname))
                    {
                        await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => responseHelper = Client.MUC_COMMAND_HELPER.requestBanList(roomJid, onRequestBanListMessage, onRequestBanListTimeout));
                    }
                    else
                    {
                        await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                        {
                            showErrorMessage("Unable to request ban list - missing permissions!");
                            reload_btn.IsEnabled = true;
                            reload_prgr.Visibility = Visibility.Collapsed;
                        });
                    }
                });
            }
        }

        private void onRequestBanListTimeout()
        {
            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                showErrorMessage("Unable to request ban list - timeout!");
                reload_btn.IsEnabled = true;
                reload_prgr.Visibility = Visibility.Collapsed;
            }).AsTask();
        }

        private bool onRequestBanListMessage(IQMessage iq)
        {
            if (iq is IQErrorMessage)
            {
                IQErrorMessage errorMessage = iq as IQErrorMessage;
                Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    showErrorMessage("Type: " + errorMessage.ERROR_OBJ.ERROR_NAME + "\nMessage: " + errorMessage.ERROR_OBJ.ERROR_MESSAGE);
                    reload_btn.IsEnabled = true;
                    reload_prgr.Visibility = Visibility.Collapsed;
                }).AsTask();
                return true;
            }
            else if (iq is BanListMessage)
            {
                BanListMessage banListMessage = iq as BanListMessage;
                Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    BANNED_USERS.Clear();
                    foreach (BanedUser user in banListMessage.BANED_USERS)
                    {
                        BANNED_USERS.Add(new MUCBanedUserTemplate { banedUser = user });
                    }
                    reload_btn.IsEnabled = true;
                    reload_prgr.Visibility = Visibility.Collapsed;
                }).AsTask();
                return true;
            }

            return false;
        }

        private bool onUpdateBanListMessage(IQMessage iq)
        {
            if (iq is IQErrorMessage)
            {
                IQErrorMessage errorMessage = iq as IQErrorMessage;
                Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    showErrorMessage("Type: " + errorMessage.ERROR_OBJ.ERROR_NAME + "\nMessage: " + errorMessage.ERROR_OBJ.ERROR_MESSAGE);
                    unban_btn.IsEnabled = true;
                    reload_btn.IsEnabled = true;
                    unban_prgr.Visibility = Visibility.Collapsed;
                }).AsTask();
                return true;
            }
            else
            {
                Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    requestBanList();
                    unban_btn.IsEnabled = true;
                    reload_btn.IsEnabled = true;
                    unban_prgr.Visibility = Visibility.Collapsed;
                }).AsTask();
                return true;
            }
        }

        private void onUpdateBanListTimeout()
        {
            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                showErrorMessage("Unable to unban user - timeout!");
                unban_btn.IsEnabled = true;
                reload_btn.IsEnabled = true;
                unban_prgr.Visibility = Visibility.Collapsed;
            }).AsTask();
        }

        private void unbanUsers(List<BanedUser> changedUsers)
        {
            unban_btn.IsEnabled = false;
            reload_btn.IsEnabled = false;
            unban_prgr.Visibility = Visibility.Visible;
            error_itbx.Visibility = Visibility.Collapsed;
            responseHelper = Client.MUC_COMMAND_HELPER.updateBanList(Chat.chatJabberId, changedUsers, onUpdateBanListMessage, onUpdateBanListTimeout);
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            requestBanList();
        }

        private void banedUsers_dgrid_SelectionChanged(object sender, Telerik.UI.Xaml.Controls.Grid.DataGridSelectionChangedEventArgs e)
        {
            unban_btn.IsEnabled = banedUsers_dgrid.SelectedItems.Count > 0;
        }

        private void unban_btn_Click(object sender, RoutedEventArgs e)
        {
            List<BanedUser> list = new List<BanedUser>();
            if (banedUsers_dgrid.SelectedItems.Count > 0)
            {
                foreach (object o in banedUsers_dgrid.SelectedItems)
                {
                    if (o is MUCBanedUserTemplate)
                    {
                        BanedUser bU = (o as MUCBanedUserTemplate).banedUser;

                        // Set affiliation to none => unban user:
                        bU.affiliation = MUCAffiliation.NONE;

                        list.Add(bU);
                    }
                }
            }
            unbanUsers(list);
        }

        private void reload_btn_Click(object sender, RoutedEventArgs e)
        {
            requestBanList();
        }

        private void Client_ConnectionStateChanged(XMPPClient client, XMPP_API.Classes.Network.Events.ConnectionStateChangedEventArgs args)
        {
            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => requestBanList()).AsTask();
        }

        #endregion
    }
}
