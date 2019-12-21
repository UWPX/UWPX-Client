using System.ComponentModel;
using UWPX_UI.Classes.Events;
using UWPX_UI.Pages.Settings;
using UWPX_UI_Context.Classes;
using UWPX_UI_Context.Classes.DataContext.Dialogs;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPX_UI.Controls
{
    public sealed partial class AccountSelectionControl: UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly AccountSelectionControlContext VIEW_MODEL = new AccountSelectionControlContext();

        public string Header
        {
            get => (string)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(nameof(Header), typeof(string), typeof(AccountSelectionControl), new PropertyMetadata(""));

        public delegate void AddAccountClickHandler(AccountSelectionControl sender, CancelEventArgs args);
        /// <summary>
        /// Triggered once the user clicked on the "+" button for adding a new account.
        /// Will be triggered before the actual navigation happens.
        /// </summary>
        public event AddAccountClickHandler AddAccountClick;
        /// <summary>
        /// Triggered once the user clicked on the "+" button for adding a new account.
        /// Will be triggered after the actual navigation happened.
        /// </summary>
        public event AddAccountClickHandler AddAccountClicked;

        public delegate void AccountSelectionChangedHandler(AccountSelectionControl sender, AccountSelectionChangedEventArgs args);
        public event AccountSelectionChangedHandler AccountSelectionChanged;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public AccountSelectionControl()
        {
            InitializeComponent();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void SelectAccount(string accountBareJid)
        {
            VIEW_MODEL.SelectAccount(accountBareJid);
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void IconButtonControl_Click(IconButtonControl sender, RoutedEventArgs args)
        {
            CancelEventArgs cancelArgs = new CancelEventArgs();
            AddAccountClick?.Invoke(this, cancelArgs);
            if (!cancelArgs.Cancel)
            {
                UiUtils.NavigateToPage(typeof(AccountsSettingsPage));
            }
            AddAccountClicked?.Invoke(this, cancelArgs);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AccountSelectionChangedEventArgs args = new AccountSelectionChangedEventArgs(VIEW_MODEL.MODEL.SelectedItem?.Client);
            AccountSelectionChanged?.Invoke(this, args);
        }

        #endregion
    }
}
