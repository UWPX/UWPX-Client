using UWPX_UI.Dialogs;
using UWPX_UI.Pages;
using UWPX_UI_Context.Classes;
using UWPX_UI_Context.Classes.DataContext.Controls;
using UWPX_UI_Context.Classes.DataTemplates;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPX_UI.Controls.Settings
{
    public sealed partial class AccountControl: UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public AccountDataTemplate Account
        {
            get => (AccountDataTemplate)GetValue(AccountProperty);
            set => SetValue(AccountProperty, value);
        }
        public static readonly DependencyProperty AccountProperty = DependencyProperty.Register(nameof(Account), typeof(AccountDataTemplate), typeof(AccountControl), new PropertyMetadata(null));

        public readonly AccountControlContext VIEW_MODEL = new AccountControlContext();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public AccountControl()
        {
            InitializeComponent();
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
        private async void Info_btn_Click(object sender, RoutedEventArgs e)
        {
            AccountInfoDialog dialog = new AccountInfoDialog(Account);
            await UiUtils.ShowDialogAsync(dialog);
        }

        private void Edit_btn_Click(object sender, RoutedEventArgs e)
        {
            UiUtils.NavigateToPage(typeof(AddAccountPage), Account.Client);
        }

        #endregion
    }
}
