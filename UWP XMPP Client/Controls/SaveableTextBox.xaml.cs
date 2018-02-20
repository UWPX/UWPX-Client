using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWP_XMPP_Client.Controls
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
        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }
        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(SaveableTextBox), null);

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
            this.InitializeComponent();
            this.EnableSaving = true;
            this.IsReadOnly = false;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void onSavingDone()
        {
            save_btn.IsEnabled = false;
            save_prgr.Visibility = Visibility.Collapsed;
        }

        public void onStartSaving()
        {
            save_btn.IsEnabled = false;
            save_prgr.Visibility = Visibility.Visible;
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void save_btn_Click(object sender, RoutedEventArgs e)
        {
            SaveClick?.Invoke(sender, e);
        }

        private void text_tbx_TextChanged(object sender, TextChangedEventArgs e)
        {
            save_btn.IsEnabled = EnableSaving && !IsReadOnly && !Equals(Text, text_tbx.Text);
        }

        #endregion
    }
}
