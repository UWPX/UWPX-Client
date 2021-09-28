using UWPX_UI_Context.Classes.DataContext.Controls.OMEMO;
using UWPX_UI_Context.Classes.DataTemplates;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPX_UI.Controls.OMEMO
{
    public sealed partial class OmemoTroubleshooterControl: UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly OmemoTroubleshooterControlContext VIEW_MODEL = new OmemoTroubleshooterControlContext();

        public AccountDataTemplate Account
        {
            get => (AccountDataTemplate)GetValue(AccountProperty);
            set => SetValue(AccountProperty, value);
        }
        public static readonly DependencyProperty AccountProperty = DependencyProperty.Register(nameof(Account), typeof(AccountDataTemplate), typeof(OmemoTroubleshooterControl), new PropertyMetadata(null));

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public OmemoTroubleshooterControl()
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
        private void OnTroubleshootClicked(IconProgressButtonControl sender, RoutedEventArgs args)
        {
            VIEW_MODEL.Troubleshoote(Account);
        }

        private void OnFixClicked(IconProgressButtonControl sender, RoutedEventArgs args)
        {
            VIEW_MODEL.Fix(Account);
        }

        #endregion
    }
}
