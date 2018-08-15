using System;
using Data_Manager2.Classes.DBTables;
using UWP_XMPP_Client.Dialogs;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using XMPP_API.Classes;
using XMPP_API.Classes.Network.XML.Messages;

namespace UWP_XMPP_Client.Controls.Muc
{
    public sealed partial class MucKickBanOccupantControl : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public MUCOccupantTable Occupant
        {
            get { return (MUCOccupantTable)GetValue(OccupantProperty); }
            set { SetValue(OccupantProperty, value); }
        }
        public static readonly DependencyProperty OccupantProperty = DependencyProperty.Register("Occupant", typeof(MUCOccupantTable), typeof(MucKickBanOccupantControl), null);

        private XMPPClient client;
        private MUCKickBanOccupantDialog dialog;
        private ChatTable chat;
        private MessageResponseHelper<IQMessage> messageResponseHelper;

        private bool canBan;
        private bool canKick;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 02/03/2018 Created [Fabian Sauter]
        /// </history>
        public MucKickBanOccupantControl(MUCKickBanOccupantDialog dialog, XMPPClient client, ChatTable chat, bool canKick, bool canBan)
        {
            this.dialog = dialog;
            this.client = client;
            this.chat = chat;
            this.canKick = canKick;
            this.canBan = canBan;
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
            if (!kickSingle_btn.IsEnabled && canKick)
            {
                return;
            }

            kickSingle_btn.IsEnabled = false;
            kickSingle_prgr.Visibility = Visibility.Visible;
            banSingle_btn.IsEnabled = false;
            error_itbx.Visibility = Visibility.Collapsed;
            done_itbx.Visibility = Visibility.Collapsed;

            string reason = string.IsNullOrEmpty(dialog.Reason) ? null : dialog.Reason;
            messageResponseHelper = client.MUC_COMMAND_HELPER.kickOccupant(chat.chatJabberId, Occupant.nickname, reason, onKickMessage, onKickTimeout);
        }

        public void ban()
        {
            if (!banSingle_btn.IsEnabled && canBan)
            {
                return;
            }

            if(Occupant.jid == null)
            {
                error_itbx.Text = "Unable to occupant - JID is null!";
                error_itbx.Visibility = Visibility.Visible;
                return;
            }

            kickSingle_btn.IsEnabled = false;
            banSingle_prgr.Visibility = Visibility.Visible;
            banSingle_btn.IsEnabled = false;
            error_itbx.Visibility = Visibility.Collapsed;
            done_itbx.Visibility = Visibility.Collapsed;

            string reason = string.IsNullOrEmpty(dialog.Reason) ? null : dialog.Reason;
            messageResponseHelper = client.MUC_COMMAND_HELPER.banOccupant(chat.chatJabberId, Occupant.jid, reason, onBanMessage, onBanTimeout);
        }

        private bool onBanMessage(IQMessage msg)
        {
            if (msg is IQErrorMessage)
            {
                IQErrorMessage errorMessage = msg as IQErrorMessage;
                Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    error_itbx.Text = "Type: " + errorMessage.ERROR_OBJ.ERROR_NAME + "\nMessage: " + errorMessage.ERROR_OBJ.ERROR_MESSAGE;
                    error_itbx.Visibility = Visibility.Visible;
                    enableButtons();
                }).AsTask();
                return true;
            }
            if (Equals(msg.TYPE, IQMessage.RESULT))
            {
                Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    done_itbx.Text = "Success! Occupant got baned.";
                    done_itbx.Visibility = Visibility.Visible;
                    banSingle_prgr.Visibility = Visibility.Collapsed;
                }).AsTask();
                return true;
            }
            return false;
        }

        private bool onKickMessage(IQMessage msg)
        {
            if (msg is IQErrorMessage)
            {
                IQErrorMessage errorMessage = msg as IQErrorMessage;
                Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    error_itbx.Text = "Type: " + errorMessage.ERROR_OBJ.ERROR_NAME + "\nMessage: " + errorMessage.ERROR_OBJ.ERROR_MESSAGE;
                    error_itbx.Visibility = Visibility.Visible;
                    enableButtons();
                }).AsTask();
                return true;
            }
            if (Equals(msg.TYPE, IQMessage.RESULT))
            {
                Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    done_itbx.Text = "Success! Occupant got kicked.";
                    done_itbx.Visibility = Visibility.Visible;
                    kickSingle_prgr.Visibility = Visibility.Collapsed;
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

        private void onBanTimeout()
        {
            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                error_itbx.Text = "Failed to ban occupant - timeout!";
                error_itbx.Visibility = Visibility.Visible;
                enableButtons();
            }).AsTask();
        }

        private void enableButtons()
        {
            kickSingle_btn.IsEnabled = canKick;
            kickSingle_prgr.Visibility = Visibility.Collapsed;
            banSingle_btn.IsEnabled = canBan;
            banSingle_prgr.Visibility = Visibility.Collapsed;
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void banSingle_btn_Click(object sender, RoutedEventArgs e)
        {
            ban();
        }

        private void kickSingle_btn_Click(object sender, RoutedEventArgs e)
        {
            kick();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            enableButtons();
        }

        #endregion
    }
}
