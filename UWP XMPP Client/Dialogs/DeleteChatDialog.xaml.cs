using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWP_XMPP_Client.Dialogs
{
    public sealed partial class DeleteChatDialog : ContentDialog
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public bool keepChatLog;
        public bool deleteChat;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 26/11/2017 Created [Fabian Sauter]
        /// </history>
        public DeleteChatDialog()
        {
            this.deleteChat = false;
            this.keepChatLog = true;
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
            keepChatLog = (bool)keepChat_cbx.IsChecked;
            deleteChat = false;
            Hide();
        }

        private void yes_btn_Click(object sender, RoutedEventArgs e)
        {
            keepChatLog = (bool)keepChat_cbx.IsChecked;
            deleteChat = true;
            Hide();
        }

        #endregion
    }
}
