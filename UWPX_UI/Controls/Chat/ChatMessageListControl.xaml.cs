using UWPX_UI_Context.Classes.DataContext.Controls;
using UWPX_UI_Context.Classes.DataTemplates;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPX_UI.Controls.Chat
{
    public sealed partial class ChatMessageListControl: UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public ChatDataTemplate Chat
        {
            get => (ChatDataTemplate)GetValue(ChatProperty);
            set => SetValue(ChatProperty, value);
        }
        public static readonly DependencyProperty ChatProperty = DependencyProperty.Register(nameof(ChatDataTemplate), typeof(ChatDataTemplate), typeof(ChatMessageListControl), new PropertyMetadata(null, ChatPropertyChanged));

        public bool IsDummy
        {
            get => (bool)GetValue(IsDummyProperty);
            set => SetValue(IsDummyProperty, value);
        }
        public static readonly DependencyProperty IsDummyProperty = DependencyProperty.Register(nameof(IsDummy), typeof(bool), typeof(ChatMessageListControl), new PropertyMetadata(false, OnIsDummyChanged));

        public double ScrollHeaderMinSize
        {
            get => (double)GetValue(ScrollHeaderMinSizeProperty);
            set => SetValue(ScrollHeaderMinSizeProperty, value);
        }
        public static readonly DependencyProperty ScrollHeaderMinSizeProperty = DependencyProperty.Register(nameof(ScrollHeaderMinSize), typeof(double), typeof(ChatMessageListControl), new PropertyMetadata(0d));

        public readonly ChatMessageListControlContext VIEW_MODEL = new ChatMessageListControlContext();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ChatMessageListControl()
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
        private void UpdateView(DependencyPropertyChangedEventArgs args)
        {
            ChatDataTemplate oldChat = args.OldValue is ChatDataTemplate ? args.OldValue as ChatDataTemplate : null;
            ChatDataTemplate newChat = args.NewValue is ChatDataTemplate ? args.NewValue as ChatDataTemplate : null;
            VIEW_MODEL.UpdateView(oldChat, newChat);
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private static void ChatPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (d is ChatMessageListControl control)
            {
                control.UpdateView(args);
            }
        }

        private static void OnIsDummyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ChatMessageListControl control)
            {
                control.VIEW_MODEL.MODEL.IsDummy = e.NewValue is bool b && b;
            }
        }

        #endregion
    }
}
