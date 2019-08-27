using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace UWPX_UI.Controls
{
    public sealed partial class IconTextBlockControl: UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public string Glyph
        {
            get => (string)GetValue(GlyphProperty);
            set => SetValue(GlyphProperty, value);
        }
        public static readonly DependencyProperty GlyphProperty = DependencyProperty.Register(nameof(Glyph), typeof(string), typeof(IconTextBlockControl), new PropertyMetadata(""));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(IconTextBlockControl), new PropertyMetadata(""));

        public Brush GlyphForeground
        {
            get => (Brush)GetValue(GlyphForegroundProperty);
            set => SetValue(GlyphForegroundProperty, value);
        }
        public static readonly DependencyProperty GlyphForegroundProperty = DependencyProperty.Register(nameof(GlyphForeground), typeof(Brush), typeof(IconTextBlockControl), new PropertyMetadata(new SolidColorBrush((Color)Application.Current.Resources["SystemAccentColor"])));

        public Style TextBlockStyle
        {
            get => (Style)GetValue(TextBlockStyleProperty);
            set => SetValue(TextBlockStyleProperty, value);
        }
        public static readonly DependencyProperty TextBlockStyleProperty = DependencyProperty.Register(nameof(TextBlockStyle), typeof(Style), typeof(IconTextBlockControl), new PropertyMetadata(null));

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public IconTextBlockControl()
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


        #endregion
    }
}
