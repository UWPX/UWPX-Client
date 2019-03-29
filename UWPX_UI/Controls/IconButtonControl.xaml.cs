using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace UWPX_UI.Controls
{
    public sealed partial class IconButtonControl : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public string Glyph
        {
            get { return (string)GetValue(GlyphProperty); }
            set { SetValue(GlyphProperty, value); }
        }
        public static readonly DependencyProperty GlyphProperty = DependencyProperty.Register(nameof(Glyph), typeof(string), typeof(IconButtonControl), new PropertyMetadata(""));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(IconButtonControl), new PropertyMetadata(""));

        public Brush GlyphForeground
        {
            get { return (Brush)GetValue(GlyphForegroundProperty); }
            set { SetValue(GlyphForegroundProperty, value); }
        }
        public static readonly DependencyProperty GlyphForegroundProperty = DependencyProperty.Register(nameof(GlyphForeground), typeof(Brush), typeof(IconButtonControl), new PropertyMetadata(new SolidColorBrush((Color)Application.Current.Resources["SystemAccentColor"])));

        public Thickness GlyphMargin
        {
            get { return (Thickness)GetValue(GlyphMarginProperty); }
            set { SetValue(GlyphMarginProperty, value); }
        }
        public static readonly DependencyProperty GlyphMarginProperty = DependencyProperty.Register(nameof(GlyphMargin), typeof(Thickness), typeof(IconButtonControl), new PropertyMetadata(new Thickness(0)));

        public Thickness TextMargin
        {
            get { return (Thickness)GetValue(TextMarginProperty); }
            set { SetValue(TextMarginProperty, value); }
        }
        public static readonly DependencyProperty TextMarginProperty = DependencyProperty.Register(nameof(TextMargin), typeof(Thickness), typeof(IconButtonControl), new PropertyMetadata(new Thickness(10, 0, 0, 0)));

        public delegate void ClickHandler(IconButtonControl sender, RoutedEventArgs args);
        public event ClickHandler Click;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public IconButtonControl()
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
