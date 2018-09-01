using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWP_XMPP_Client.Dialogs
{
    public sealed partial class DeleteAccountDialog : ContentDialog
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public bool keepChats { get; private set; }
        public bool keepChatMessages { get; private set; }
        public bool deleteAccount { get; private set; }

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
        private void yes_ibtn_Click(object sender, RoutedEventArgs args)
        {
            keepChats = (bool)keepChats_cbx.IsChecked;
            keepChatMessages = (bool)keepChatMessages_cbx.IsChecked;
            deleteAccount = true;
            Hide();
        }

        private void no_ibtn_Click(object sender, RoutedEventArgs args)
        {
            keepChats = (bool)keepChats_cbx.IsChecked;
            keepChatMessages = (bool)keepChatMessages_cbx.IsChecked;
            deleteAccount = false;
            Hide();
        }

        #endregion
    }
}
