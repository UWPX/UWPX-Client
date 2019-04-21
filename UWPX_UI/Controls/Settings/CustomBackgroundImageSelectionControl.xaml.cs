using UWPX_UI_Context.Classes.DataContext.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPX_UI.Controls.Settings
{
    public sealed partial class CustomBackgroundImageSelectionControl: UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(CustomBackgroundImageSelectionControl), new PropertyMetadata(false, OnIsSelectedChanged));

        public readonly CustomBackgroundImageSelectionControlContext VIEW_MODEL = new CustomBackgroundImageSelectionControlContext();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public CustomBackgroundImageSelectionControl()
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
        private void OnIsSelectedChanged(DependencyPropertyChangedEventArgs e)
        {
            VIEW_MODEL.OnIsSelectedChanged(e);
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void GridViewItem_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            IsSelected = true;
        }

        private async void Browse_btn_Click(object sender, RoutedEventArgs e)
        {
            await VIEW_MODEL.BrowseImageAsync();
        }

        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CustomBackgroundImageSelectionControl control)
            {
                control.OnIsSelectedChanged(e);
            }
        }

        #endregion
    }
}
