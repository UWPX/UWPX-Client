using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace UWP_XMPP_Client.Controls
{
    public sealed partial class IconButtonControl : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(IconButtonControl), null);

        public string Icon
        {
            get { return (string)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(nameof(Icon), typeof(string), typeof(IconButtonControl), null);

        public FontFamily IconFontFamily
        {
            get { return (FontFamily)GetValue(IconFontFamilyProperty); }
            set { SetValue(IconFontFamilyProperty, value); }
        }
        public static readonly DependencyProperty IconFontFamilyProperty = DependencyProperty.Register(nameof(IconFontFamily), typeof(FontFamily), typeof(IconButtonControl), new PropertyMetadata(new FontFamily("Segoe MDL2 Assets")));

        public Brush IconForeground
        {
            get { return (Brush)GetValue(IconForegroundProperty); }
            set { SetValue(IconForegroundProperty, value); }
        }
        public static readonly DependencyProperty IconForegroundProperty = DependencyProperty.Register(nameof(IconForeground), typeof(Brush), typeof(IconButtonControl), new PropertyMetadata(new SolidColorBrush((Color)Application.Current.Resources["SystemColorButtonFaceColor"])));

        public delegate void ClickHandler(IconButtonControl sender, RoutedEventArgs args);
        public event ClickHandler Click;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 17/03/2018 Created [Fabian Sauter]
        /// </history>
        public IconButtonControl()
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
