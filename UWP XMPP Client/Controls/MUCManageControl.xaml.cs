using Data_Manager2.Classes.DBManager;
using Data_Manager2.Classes.DBTables;
using System;
using UWP_XMPP_Client.Classes;
using UWP_XMPP_Client.DataTemplates;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using XMPP_API.Classes;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.XEP_0045.Configuration;

namespace UWP_XMPP_Client.Controls
{
    public sealed partial class MUCManageControl : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public ChatTable Chat
        {
            get { return (ChatTable)GetValue(chatProperty); }
            set
            {
                SetValue(chatProperty, value);
                requestRoomInfo();
            }
        }
        public static readonly DependencyProperty chatProperty = DependencyProperty.Register("Chat", typeof(ChatTable), typeof(MUCManageControl), null);

        public XMPPClient Client
        {
            get { return (XMPPClient)GetValue(clientProperty); }
            set
            {
                SetValue(clientProperty, value);
                requestRoomInfo();
            }
        }
        public static readonly DependencyProperty clientProperty = DependencyProperty.Register("Client", typeof(XMPPClient), typeof(MUCManageControl), null);

        public MUCChatInfoTable MUCInfo
        {
            get { return (MUCChatInfoTable)GetValue(mucInfoProperty); }
            set
            {
                SetValue(mucInfoProperty, value);
                requestRoomInfo();
            }
        }
        public static readonly DependencyProperty mucInfoProperty = DependencyProperty.Register("MUCInfo", typeof(MUCChatInfoTable), typeof(MUCManageControl), null);

        private MessageResponseHelper<RoomInfoResponseMessage> messageResponseHelper;

        private CustomObservableCollection<MUCInfoOptionTemplate> options;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 07/02/2018 Created [Fabian Sauter]
        /// </history>
        public MUCManageControl()
        {
            this.options = new CustomObservableCollection<MUCInfoOptionTemplate>();
            this.messageResponseHelper = null;
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
        private void requestRoomInfo()
        {
            if (messageResponseHelper != null || Client == null || Chat == null || MUCInfo == null)
            {
                return;
            }

            info_grid.Visibility = Visibility.Collapsed;

            MUCMemberTable member = MUCManager.INSTANCE.getMUCMember(Chat.id, MUCInfo.nickname);
            if (member != null)
            {
                loading_grid.Visibility = Visibility.Visible;
                timeout_stckpnl.Visibility = Visibility.Collapsed;

                messageResponseHelper = new MessageResponseHelper<RoomInfoResponseMessage>(Client, onNewMessage, onTimeout);
                RequestRoomInfoMessage msg = new RequestRoomInfoMessage(Chat.chatJabberId, member.affiliation);
                messageResponseHelper.start(msg);
            }
            else
            {
                timeout_stckpnl.Visibility = Visibility.Visible;
                loading_grid.Visibility = Visibility.Collapsed;
                reload_btn.IsEnabled = true;

                faildToRequest_ian.Show("Failed to request information! It seems like you are no member of this room. Please reconnect or retry.");
            }
        }

        private bool onNewMessage(RoomInfoResponseMessage responseMessage)
        {
            // Add controls and update viability:
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                options.Clear();
                foreach (MUCInfoField o in responseMessage.roomConfig.options)
                {
                    if (o.type != MUCInfoFieldType.HIDDEN)
                    {
                        options.Add(new MUCInfoOptionTemplate() { option = o });
                    }
                }
                reload_btn.IsEnabled = true;
                timeout_stckpnl.Visibility = Visibility.Collapsed;
                loading_grid.Visibility = Visibility.Collapsed;
                info_grid.Visibility = Visibility.Visible;
            }).AsTask();
            return true;
        }

        private void onTimeout()
        {
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                retry_btn.IsEnabled = true;
                loading_grid.Visibility = Visibility.Collapsed;
                timeout_stckpnl.Visibility = Visibility.Visible;
            }).AsTask();
        }

        private void save()
        {
            save_prgr.Visibility = Visibility.Visible;
            save_prgr.IsActive = true;
            save_btn.IsEnabled = false;
        }

        private void reload()
        {
            reload_btn.IsEnabled = false;
            retry_btn.IsEnabled = false;
            messageResponseHelper?.Dispose();
            messageResponseHelper = null;
            requestRoomInfo();
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void retry_btn_Click(object sender, RoutedEventArgs e)
        {
            reload();
        }

        private void reload_btn_Click(object sender, RoutedEventArgs e)
        {
            reload();
        }

        private void save_btn_Click(object sender, RoutedEventArgs e)
        {
            save();
        }

        #endregion
    }
}
