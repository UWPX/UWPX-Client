using UWPX_UI_Context.Classes;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace UWPX_UI.Controls
{
    public sealed partial class LoadingContentControl: UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(LoadingContentControl), new PropertyMetadata("Loading..."));

        public SolidColorBrush ProgressRingForeground
        {
            get => (SolidColorBrush)GetValue(ProgressRingForegroundProperty);
            set => SetValue(ProgressRingForegroundProperty, value);
        }
        public static readonly DependencyProperty ProgressRingForegroundProperty = DependencyProperty.Register(nameof(ProgressRingForeground), typeof(SolidColorBrush), typeof(LoadingContentControl), new PropertyMetadata(new SolidColorBrush(ThemeUtils.GetThemeResource<Color>("SystemAccentColor"))));

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public LoadingContentControl()
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
