using System;
using Data_Manager2.Classes.DBTables;
using UWP_XMPP_Client.Dialogs;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using XMPP_API.Classes;
using XMPP_API.Classes.Network.XML.Messages;

namespace UWP_XMPP_Client.Controls
{
    public sealed partial class KickBanOccupantControl : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public MUCOccupantTable Occupant
        {
            get { return (MUCOccupantTable)GetValue(OccupantProperty); }
            set { SetValue(OccupantProperty, value); }
        }
        public static readonly DependencyProperty OccupantProperty = DependencyProperty.Register("Occupant", typeof(MUCOccupantTable), typeof(KickBanOccupantControl), null);

        private XMPPClient client;
        private MUCKickBanOccupantDialog dialog;
        private ChatTable chat;
        private MessageResponseHelper<IQMessage> kickMessageResponseHelper;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 02/03/2018 Created [Fabian Sauter]
        /// </history>
        public KickBanOccupantControl(MUCKickBanOccupantDialog dialog, XMPPClient client, ChatTable chat)
        {
            this.dialog = dialog;
            this.client = client;
            this.chat = chat;
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
        public void kick()
        {
            kickSingle_btn.IsEnabled = false;
            kickSingle_prgr.Visibility = Visibility.Visible;
            banSingle_btn.IsEnabled = false;
            error_itbx.Visibility = Visibility.Collapsed;

            kickMessageResponseHelper = client.MUC_COMMAND_HELPER.kickOccupant(chat.chatJabberId, Occupant.nickname, dialog.Reason, onKickMessage, onKickTimeout);
        }

        private bool onKickMessage(IQMessage msg)
        {
            if(msg is IQErrorMessage)
            {
                IQErrorMessage errorMessage = msg as IQErrorMessage;
                Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    error_itbx.Text = "Type: " + errorMessage.ERROR_TYPE + ", Message: " + errorMessage.ERROR_MESSAGE;
                    error_itbx.Visibility = Visibility.Visible;
                    enableButtons();
                }).AsTask();
                return true;
            }
            if(Equals(msg.getMessageType(), IQMessage.RESULT))
            {
                Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    enableButtons();
                    dialog.removeOccupant(Occupant);
                }).AsTask();
                return true;
            }
            return false;
        }

        private void onKickTimeout()
        {
            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                error_itbx.Text = "Failed to kick occupant - timeout!";
                error_itbx.Visibility = Visibility.Visible;
                enableButtons();
            }).AsTask();
        }

        private void enableButtons()
        {
            kickSingle_btn.IsEnabled = true;
            kickSingle_prgr.Visibility = Visibility.Collapsed;
            banSingle_btn.IsEnabled = true;
            banSingle_prgr.Visibility = Visibility.Collapsed;
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void banSingle_btn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void kickSingle_btn_Click(object sender, RoutedEventArgs e)
        {
            kick();
        }

        #endregion
    }
}
