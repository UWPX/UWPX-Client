using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using XMPP_API.Classes;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.XEP_0402;

namespace UWP_XMPP_Client.Dialogs
{
    public sealed partial class AddBookmarkDialog : ContentDialog
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public bool success { get; private set; }
        public readonly XMPPClient CLIENT;
        private MessageResponseHelper<IQMessage> messageResponseHelper;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 15/07/2018 Created [Fabian Sauter]
        /// </history>
        public AddBookmarkDialog(XMPPClient client)
        {
            this.CLIENT = client;
            this.success = false;
            this.messageResponseHelper = null;
            this.InitializeComponent();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private ConferenceItem getConferenceItem()
        {
            return new ConferenceItem()
            {
                id = jid_tbx.Text,
                password = password_pwbx.Password,
                name = name_tbx.Text,
                nick = nick_tbx.Text,
                autoJoin = (bool)autoJoin_chbx.IsChecked,
            };
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void showError(string text)
        {
            error_itbx.Text = text;
            error_itbx.Visibility = Visibility.Visible;
        }

        private void hideError()
        {
            error_itbx.Visibility = Visibility.Collapsed;
        }

        private void addBookmark()
        {
            hideError();
            if (CLIENT == null || !CLIENT.isConnected())
            {
                showError("Client not connected!");
            }
            else
            {
                if (messageResponseHelper != null)
                {
                    messageResponseHelper.Dispose();
                }
                messageResponseHelper = CLIENT.PUB_SUB_COMMAND_HELPER.addBookmark(onMessage, onTimeout, getConferenceItem());
            }
        }

        private bool onMessage(IQMessage msg)
        {
            if (msg is IQErrorMessage errorMsg)
            {
                Logging.Logger.Error("Failed to add bookmark! " + msg.ToString());
                Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => showError("Failed - see logs!")).AsTask();
            }
            else
            {
                success = true;
                Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => Hide()).AsTask();
            }
            return true;
        }

        private void onTimeout()
        {
            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => showError("Failed - timeout!")).AsTask();
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void jid_tbx_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Space)
            {
                e.Handled = true;
            }
        }

        private void jid_tbx_TextChanged(object sender, TextChangedEventArgs e)
        {
            int selectionStart = jid_tbx.SelectionStart;
            jid_tbx.Text = jid_tbx.Text.ToLower();
            jid_tbx.SelectionStart = selectionStart;
            jid_tbx.SelectionLength = 0;
            bool isBareJid = Utils.isBareJid(jid_tbx.Text);
            jid_tbx.BorderBrush = new SolidColorBrush(isBareJid ? Colors.Green : Colors.Red);
            add_bnt.IsEnabled = isBareJid && !string.IsNullOrWhiteSpace(nick_tbx.Text);
        }

        private void add_bnt_Click(object sender, RoutedEventArgs e)
        {
            addBookmark();
        }

        private void cancel_ibtn_Click(object sender, RoutedEventArgs args)
        {
            if (messageResponseHelper != null)
            {
                messageResponseHelper.Dispose();
            }
            success = false;
            Hide();
        }

        private void nick_tbx_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool isBareJid = Utils.isBareJid(jid_tbx.Text);
            add_bnt.IsEnabled = isBareJid && !string.IsNullOrWhiteSpace(nick_tbx.Text);
        }

        #endregion
    }
}
