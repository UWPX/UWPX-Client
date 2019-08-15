using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWP_XMPP_Client.Controls.Muc
{
    public sealed partial class SaveablePasswordBox : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }
        public static readonly DependencyProperty PasswordProperty = DependencyProperty.Register("Password", typeof(string), typeof(SaveablePasswordBox), null);
        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(string), typeof(SaveablePasswordBox), null);
        public bool EnableSaving
        {
            get { return (bool)GetValue(EnableSavingProperty); }
            set { SetValue(EnableSavingProperty, value); }
        }
        public static readonly DependencyProperty EnableSavingProperty = DependencyProperty.Register("EnableSaving", typeof(bool), typeof(SaveablePasswordBox), null);

        private bool isSaving;

        public event RoutedEventHandler SaveClick;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 06/03/2018 Created [Fabian Sauter]
        /// </history>
        public SaveablePasswordBox()
        {
            EnableSaving = true;
            isSaving = false;
            IsEnabledChanged += SaveablePasswordBox_IsEnabledChanged;
            InitializeComponent();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void onSavingDone()
        {
            isSaving = false;
            updateControlsEnabled();
            save_prgr.Visibility = Visibility.Collapsed;
        }

        public void onStartSaving()
        {
            isSaving = true;
            updateControlsEnabled();
            save_prgr.Visibility = Visibility.Visible;
        }

        #endregion

        #region --Misc Methods (Private)--
        private void updateControlsEnabled()
        {
            save_btn.IsEnabled = EnableSaving && IsEnabled && !isSaving;
            password_pwbx.IsEnabled = IsEnabled && !isSaving;
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void save_btn_Click(object sender, RoutedEventArgs e)
        {
            SaveClick?.Invoke(sender, e);
        }

        private void password_pwbx_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter && !IsEnabled)
            {
                Password = password_pwbx.Password;
                SaveClick?.Invoke(sender, e);
            }
        }

        private void SaveablePasswordBox_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            updateControlsEnabled();
        }

        #endregion
    }
}
