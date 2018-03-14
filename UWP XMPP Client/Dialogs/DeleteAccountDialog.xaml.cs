using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWP_XMPP_Client.Dialogs
{
    public sealed partial class DeleteAccountDialog : ContentDialog
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public bool keepChats;
        public bool keepChatMessages;
        public bool deleteAccount;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 14/03/2018 Created [Fabian Sauter]
        /// </history>
        public DeleteAccountDialog()
        {
            this.deleteAccount = false;
            this.keepChats = true;
            this.keepChatMessages = true;
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


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void no_btn_Click(object sender, RoutedEventArgs e)
        {
            keepChats = (bool)keepChats_cbx.IsChecked;
            keepChatMessages = (bool)keepChatMessages_cbx.IsChecked;
            deleteAccount = false;
            Hide();
        }

        private void yes_btn_Click(object sender, RoutedEventArgs e)
        {
            keepChats = (bool)keepChats_cbx.IsChecked;
            keepChatMessages = (bool)keepChatMessages_cbx.IsChecked;
            deleteAccount = true;
            Hide();
        }

        #endregion
    }
}
