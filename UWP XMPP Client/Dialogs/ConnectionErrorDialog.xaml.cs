using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using XMPP_API.Classes.Network;

namespace UWP_XMPP_Client.Dialogs
{
    public sealed partial class ConnectionErrorDialog : ContentDialog
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public ConnectionError LastConnectionError
        {
            get { return (ConnectionError)GetValue(LastConnectionErrorProperty); }
            set
            {
                SetValue(LastConnectionErrorProperty, value);
                showLastConnectionError();
            }
        }
        public static readonly DependencyProperty LastConnectionErrorProperty = DependencyProperty.Register("LastConnectionError", typeof(ConnectionError), typeof(ConnectionErrorDialog), null);

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 30/04/2018 Created [Fabian Sauter]
        /// </history>
        public ConnectionErrorDialog(ConnectionError lastConnectionError)
        {
            this.InitializeComponent();
            this.LastConnectionError = lastConnectionError;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void showLastConnectionError()
        {
            if (LastConnectionError != null)
            {
                errorCode_run.Text = (int)LastConnectionError.ERROR_CODE + " [" + LastConnectionError.ERROR_CODE + "]";
                if (LastConnectionError.ERROR_CODE == ConnectionErrorCode.SOCKET_ERROR)
                {
                    socketErrorCode_run.Text = (int)LastConnectionError.SOCKET_ERROR + " [" + LastConnectionError.SOCKET_ERROR + "]";
                }
                else
                {
                    socketErrorCode_run.Text = "-";
                }
                errorMessage_run.Text = LastConnectionError.ERROR_MESSAGE ?? "-";
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Hide();
        }

        #endregion
    }
}
