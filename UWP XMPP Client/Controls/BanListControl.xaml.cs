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

namespace UWP_XMPP_Client.Controls
{
    public sealed partial class BanListControl : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public ChatTable Chat
        {
            get { return (ChatTable)GetValue(ChatProperty); }
            set { SetValue(ChatProperty, value); }
        }
        public static readonly DependencyProperty ChatProperty = DependencyProperty.Register("Chat", typeof(ChatTable), typeof(BanListControl), null);

        public MUCChatInfoTable MUCInfo
        {
            get { return (MUCChatInfoTable)GetValue(MUCInfoProperty); }
            set { SetValue(MUCInfoProperty, value); }
        }
        public static readonly DependencyProperty MUCInfoProperty = DependencyProperty.Register("MUCInfo", typeof(MUCChatInfoTable), typeof(BanListControl), null);

        public XMPPClient Client
        {
            get { return (XMPPClient)GetValue(ClientProperty); }
            set { SetValue(ClientProperty, value); }
        }
        public static readonly DependencyProperty ClientProperty = DependencyProperty.Register("Client", typeof(XMPPClient), typeof(BanListControl), null);

        private ObservableCollection<MUCBanedUserTemplate> banedUsers;

        MessageResponseHelper<IQMessage> requestBanListHelper;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 12/03/2018 Created [Fabian Sauter]
        /// </history>
        public BanListControl()
        {
            this.requestBanListHelper = null;
            this.banedUsers = new ObservableCollection<MUCBanedUserTemplate>();
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
        private void requestBanList()
        {
            if (MUCInfo != null && Chat != null && Client != null && MUCInfo.state == Data_Manager2.Classes.MUCState.ENTERD && Client.isConnected())
            {
                string chatId = MUCInfo.chatId;
                string nickname = MUCInfo.nickname;
                string roomJid = Chat.chatJabberId;
                Task.Run(async () =>
                {
                    if (isAllowedToRequestBanList(chatId, nickname))
                    {
                        await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => requestBanListHelper = Client.MUC_COMMAND_HELPER.requestBanList(roomJid, onMessage, onTimeout));
                    }
                    else
                    {
                        await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => Visibility = Visibility.Collapsed);
                    }
                });
            }
        }

        private void onTimeout()
        {

        }

        private bool onMessage(IQMessage iq)
        {
            if(iq is IQErrorMessage)
            {

                return true;
            }
            else if (iq is BanListMessage)
            {
                BanListMessage banListMessage = iq as BanListMessage;
                Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    banedUsers.Clear();
                    foreach (BanedUser user in banListMessage.banedUsers)
                    {
                        banedUsers.Add(new MUCBanedUserTemplate() { banedUser = user });
                    }
                    Visibility = Visibility.Visible;
                }).AsTask();
                return true;
            }

            return false;
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

        private void members_dgrid_SelectionChanged(object sender, Telerik.UI.Xaml.Controls.Grid.DataGridSelectionChangedEventArgs e)
        {

        }

        #endregion
    }
}
