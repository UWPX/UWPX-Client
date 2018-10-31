using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using XMPP_API.Classes.Network;

namespace UWP_XMPP_Client.Dialogs
{
    public sealed partial class MoreAccountOptionsDialog : ContentDialog
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly ConnectionConfiguration CONNECTION_CONFIG;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 15/05/2018 Created [Fabian Sauter]
        /// </history>
        public MoreAccountOptionsDialog(ConnectionConfiguration connectionConfiguration)
        {
            this.CONNECTION_CONFIG = connectionConfiguration;
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
        private void save()
        {
            CONNECTION_CONFIG.disableStreamManagement = disableStreamManagement_tggls.IsOn;
            CONNECTION_CONFIG.disableMessageCarbons = disableMessageCarbons_tggls.IsOn;
            Hide();
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void save_btn_Click(object sender, RoutedEventArgs e)
        {
            save();
        }

        private void cancel_btn_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        #endregion
    }
}
