using UWPX_UI_Context.Classes.DataContext.Controls.Chat.MUC;
using UWPX_UI_Context.Classes.DataTemplates;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPX_UI.Controls.Chat.MUC
{
    public sealed partial class MucConfigurationControl: UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly MucConfigurationrControlContext VIEW_MODEL = new MucConfigurationrControlContext();

        public ChatDataTemplate Chat
        {
            get => (ChatDataTemplate)GetValue(ChatProperty);
            set => SetValue(ChatProperty, value);
        }
        public static readonly DependencyProperty ChatProperty = DependencyProperty.Register(nameof(Chat), typeof(ChatDataTemplate), typeof(MucConfigurationControl), new PropertyMetadata(null, OnChatChanged));

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public MucConfigurationControl()
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
        private void UpdateView(DependencyPropertyChangedEventArgs e)
        {
            VIEW_MODEL.UpdateView(e);
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private static void OnChatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MucConfigurationControl control)
            {
                control.UpdateView(e);
            }
        }

        private void Save_btn_Click(object sender, RoutedEventArgs e)
        {
            VIEW_MODEL.Save(Chat);
        }

        private void Reload_btn_Click(object sender, RoutedEventArgs e)
        {
            VIEW_MODEL.Reload(Chat);
        }

        private void Error_btn_Click(object sender, RoutedEventArgs e)
        {

        }

        #endregion
    }
}
