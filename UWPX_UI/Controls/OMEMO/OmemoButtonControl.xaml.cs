using UWPX_UI_Context.Classes.DataContext.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPX_UI.Controls.OMEMO
{
    public sealed partial class OmemoButtonControl: UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public bool OmemoEnabled
        {
            get => (bool)GetValue(OmemoEnabledProperty);
            set => SetValue(OmemoEnabledProperty, value);
        }
        public static readonly DependencyProperty OmemoEnabledProperty = DependencyProperty.Register(nameof(OmemoEnabled), typeof(bool), typeof(OmemoButtonControl), new PropertyMetadata(false));

        public readonly OmemoButtonControlContext VIEW_MODEL = new OmemoButtonControlContext();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public OmemoButtonControl()
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
        private async void ReadOnOmemo_link_Click(object sender, RoutedEventArgs e)
        {
            await VIEW_MODEL.OnReadOnOmemoClickedAsync();
        }

        #endregion
    }
}
