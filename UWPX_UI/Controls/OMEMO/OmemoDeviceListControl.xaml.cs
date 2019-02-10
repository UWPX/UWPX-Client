using UWPX_UI_Context.Classes.DataContext.Controls;
using UWPX_UI_Context.Classes.DataTemplates;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPX_UI.Controls.OMEMO
{
    public sealed partial class OmemoDeviceListControl : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public AccountDataTemplate Account
        {
            get { return (AccountDataTemplate)GetValue(AccountProperty); }
            set { SetValue(AccountProperty, value); }
        }
        public static readonly DependencyProperty AccountProperty = DependencyProperty.Register(nameof(Account), typeof(AccountDataTemplate), typeof(OmemoDeviceListControl), new PropertyMetadata(null, OnAccountChanged));

        public readonly OmemoDeviceListControlContext VIEW_MODEL = new OmemoDeviceListControlContext();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public OmemoDeviceListControl()
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
            VIEW_MODEL.UpdateView(e);
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void Reset_ibtn_Click(IconProgressButtonControl sender, RoutedEventArgs args)
        {
            VIEW_MODEL.ResetOmemoDevices(Account.Client);
        }

        private void Refresh_ibtn_Click(object sender, RoutedEventArgs args)
        {
            VIEW_MODEL.RefreshOmemoDevices(Account.Client);
        }

        private static void OnAccountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is OmemoDeviceListControl omemoDeviceListControl)
            {
                omemoDeviceListControl.UpdateView(e);
            }
        }

        #endregion
    }
}
