using UWPX_UI_Context.Classes.DataTemplates;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace UWPX_UI.Controls.Chat.SpeechBubbles
{
    public sealed partial class SpeechBubbleTopControl : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public ChatMessageDataTemplate ChatMessage
        {
            get { return (ChatMessageDataTemplate)GetValue(ChatMessageProperty); }
            set { SetValue(ChatMessageProperty, value); }
        }
        public static readonly DependencyProperty ChatMessageProperty = DependencyProperty.Register(nameof(ChatMessage), typeof(ChatMessageDataTemplate), typeof(SpeechBubbleTopControl), new PropertyMetadata(null));

        private readonly PointCollection SPEECH_BUBBLE_POINTS = new PointCollection();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public SpeechBubbleTopControl()
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
        private void UpdateSpeechBubbleSize(Size size)
        {
            double width = size.Width < MinWidth ? MinWidth : size.Width;
            double height = (size.Height < MinHeight ? MinHeight : size.Height) + 10;
            /*speechBubble_poly.Points = new PointCollection()
            {
                new Point(0, 10),
                new Point(10, 10),
                new Point(10, 0),
                new Point(20, 10),
                new Point(width, 10),
                new Point(width, height),
                new Point(0, height)
            };*/
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void SpeechBubbleContentControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
        }

        #endregion

        private void SpeechBubble_poly_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //UpdateSpeechBubbleSize(e.NewSize);
        }
    }
}
