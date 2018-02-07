using Data_Manager2.Classes.DBTables;
using System;
using System.Collections.Generic;
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
    public sealed partial class MUCInfoControl : UserControl
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
        public static readonly DependencyProperty chatProperty = DependencyProperty.Register("Chat", typeof(ChatTable), typeof(MUCInfoControl), null);

        public XMPPClient Client
        {
            get { return (XMPPClient)GetValue(clientProperty); }
            set
            {
                SetValue(clientProperty, value);
                requestRoomInfo();
            }
        }
        public static readonly DependencyProperty clientProperty = DependencyProperty.Register("Client", typeof(XMPPClient), typeof(MUCInfoControl), null);

        private MessageResponseHelper messageResponseHelper;

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
        public MUCInfoControl()
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
            if (messageResponseHelper != null || Client == null || Chat == null)
            {
                return;
            }

            loading_grid.Visibility = Visibility.Visible;
            info_srlv.Visibility = Visibility.Collapsed;
            timeout_stckpnl.Visibility = Visibility.Collapsed;

            messageResponseHelper = new MessageResponseHelper(Client, onNewMessage, onTimeout);
            RequestRoomInfoMessage msg = new RequestRoomInfoMessage(Chat.chatJabberId);
            messageResponseHelper.start(msg);
        }

        private bool onNewMessage(AbstractMessage msg)
        {
            if (msg is RoomInfoResponseMessage)
            {
                RoomInfoResponseMessage responseMessage = msg as RoomInfoResponseMessage;

                Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    options.Clear();
                    List<MUCInfoOptionTemplate> list = new List<MUCInfoOptionTemplate>();
                    foreach (AbstractConfigrurationOption o in responseMessage.roomConfig.options)
                    {
                        list.Add(new MUCInfoOptionTemplate() { option = o });
                    }
                    options.AddRange(list);
                    loading_grid.Visibility = Visibility.Collapsed;
                    info_srlv.Visibility = Visibility.Visible;
                }).AsTask();
                return true;
            }
            return false;
        }

        private void onTimeout()
        {
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                loading_grid.Visibility = Visibility.Collapsed;
                timeout_stckpnl.Visibility = Visibility.Visible;
            }).AsTask();
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void retry_btn_Click(object sender, RoutedEventArgs e)
        {
            messageResponseHelper?.Dispose();
            messageResponseHelper = null;
            requestRoomInfo();
        }

        #endregion
    }
}
