using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWP_XMPP_Client.Controls.Muc
{
    public sealed partial class SaveableTextBox : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(SaveableTextBox), null);
        public string PlaceholderText
        {
            get { return (string)GetValue(PlaceholderTextProperty); }
            set { SetValue(PlaceholderTextProperty, value); }
        }
        public static readonly DependencyProperty PlaceholderTextProperty = DependencyProperty.Register("PlaceholderText", typeof(string), typeof(SaveableTextBox), null);
        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(string), typeof(SaveableTextBox), null);
        public bool EnableSaving
        {
            get { return (bool)GetValue(EnableSavingProperty); }
            set { SetValue(EnableSavingProperty, value); }
        }
        public static readonly DependencyProperty EnableSavingProperty = DependencyProperty.Register("EnableSaving", typeof(bool), typeof(SaveableTextBox), null);

        private bool isSaving;

        public event RoutedEventHandler SaveClick;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 20/02/2018 Created [Fabian Sauter]
        /// </history>
        public SaveableTextBox()
        {
            isSaving = false;
            EnableSaving = true;
            IsEnabledChanged += SaveableTextBox_IsEnabledChanged;
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
            text_tbx.IsEnabled = IsEnabled && !isSaving;
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

        private void text_tbx_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter && IsEnabled)
            {
                Text = text_tbx.Text;
                SaveClick?.Invoke(sender, e);
            }
        }

        private void SaveableTextBox_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            updateControlsEnabled();
        }

        #endregion
    }
}
