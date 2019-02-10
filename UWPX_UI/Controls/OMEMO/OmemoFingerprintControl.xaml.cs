using UWPX_UI_Context.Classes.DataTemplates;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPX_UI.Controls.OMEMO
{
    public sealed partial class OmemoFingerprintControl : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public AccountDataTemplate Account
        {
            get { return (AccountDataTemplate)GetValue(AccountProperty); }
            set { SetValue(AccountProperty, value); }
        }
        public static readonly DependencyProperty AccountProperty = DependencyProperty.Register(nameof(Account), typeof(AccountDataTemplate), typeof(OmemoFingerprintControl), new PropertyMetadata(null, OnAccountChanged));

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public OmemoFingerprintControl()
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
        private static void OnAccountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is OmemoFingerprintControl omemoFingerprintControl)
            {
                omemoDeviceListControl.UpdateView(e);
            }
        }

        #endregion
    }
}
