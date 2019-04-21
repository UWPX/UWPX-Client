using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace UWPX_UI.Controls
{
    public sealed partial class IconProgressButtonControl: UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public string Glyph
        {
            get => (string)GetValue(GlyphProperty);
            set => SetValue(GlyphProperty, value);
        }
        public static readonly DependencyProperty GlyphProperty = DependencyProperty.Register(nameof(Glyph), typeof(string), typeof(IconProgressButtonControl), new PropertyMetadata(""));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(IconProgressButtonControl), new PropertyMetadata(""));

        public Brush GlyphForeground
        {
            get => (Brush)GetValue(GlyphForegroundProperty);
            set => SetValue(GlyphForegroundProperty, value);
        }
        public static readonly DependencyProperty GlyphForegroundProperty = DependencyProperty.Register(nameof(GlyphForeground), typeof(Brush), typeof(IconProgressButtonControl), new PropertyMetadata(new SolidColorBrush((Color)Application.Current.Resources["SystemAccentColor"])));

        public Thickness GlyphMargin
        {
            get => (Thickness)GetValue(GlyphMarginProperty);
            set => SetValue(GlyphMarginProperty, value);
        }
        public static readonly DependencyProperty GlyphMarginProperty = DependencyProperty.Register(nameof(GlyphMargin), typeof(Thickness), typeof(IconProgressButtonControl), new PropertyMetadata(new Thickness(0)));

        public Visibility ProgressRingVisibility
        {
            get => (Visibility)GetValue(ProgressRingVisibilityProperty);
            set => SetValue(ProgressRingVisibilityProperty, value);
        }
        public static readonly DependencyProperty ProgressRingVisibilityProperty = DependencyProperty.Register(nameof(ProgressRingVisibility), typeof(Visibility), typeof(IconProgressButtonControl), new PropertyMetadata(Visibility.Collapsed));

        public delegate void ClickHandler(IconProgressButtonControl sender, RoutedEventArgs args);
        public event ClickHandler Click;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public IconProgressButtonControl()
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
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Click?.Invoke(this, e);
        }

        #endregion
    }
}
