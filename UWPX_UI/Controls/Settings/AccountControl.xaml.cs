using UWPX_UI_Context.Classes.DataContext;
using UWPX_UI_Context.Classes.DataTemplates;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPX_UI.Controls.Settings
{
    public sealed partial class AccountControl : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public AccountDataTemplate Account
        {
            get { return (AccountDataTemplate)GetValue(AccountProperty); }
            set { SetValue(AccountProperty, value); }
        }
        public static readonly DependencyProperty AccountProperty = DependencyProperty.Register(nameof(Account), typeof(AccountDataTemplate), typeof(AccountControl), new PropertyMetadata(null, OnAccountChanged));

        public readonly AccountControlContext VIEW_MODEL = new AccountControlContext();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public AccountControl()
        {
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
        private void UpdateView(DependencyPropertyChangedEventArgs e)
        {
            VIEW_MODEL.UpdateView((AccountDataTemplate)e.NewValue);
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private static void OnAccountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AccountControl accountSettingsControl)
            {
                accountSettingsControl.UpdateView(e);
            }
        }

        #endregion
    }
}
