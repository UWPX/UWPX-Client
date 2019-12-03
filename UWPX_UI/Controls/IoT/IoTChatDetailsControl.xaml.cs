using UWPX_UI_Context.Classes.DataContext.Controls.IoT;
using UWPX_UI_Context.Classes.DataTemplates;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPX_UI.Controls.IoT
{
    public sealed partial class IoTChatDetailsControl: UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public ChatDataTemplate Chat
        {
            get => (ChatDataTemplate)GetValue(ChatProperty);
            set => SetValue(ChatProperty, value);
        }
        public static readonly DependencyProperty ChatProperty = DependencyProperty.Register(nameof(ChatDataTemplate), typeof(ChatDataTemplate), typeof(IoTChatDetailsControl), new PropertyMetadata(null, ChatPropertyChanged));

        public readonly IoTChatDetailsControlContext VIEW_MODEL = new IoTChatDetailsControlContext();
        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public IoTChatDetailsControl()
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
        private void UpdateView()
        {
            VIEW_MODEL.UpdateView(Chat);
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private static void ChatPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is IoTChatDetailsControl control)
            {
                control.UpdateView();
            }
        }

        #endregion
    }
}
