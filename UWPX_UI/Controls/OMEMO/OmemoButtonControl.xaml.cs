using System;
using Manager.Classes.Chat;
using UWPX_UI_Context.Classes.DataContext.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPX_UI.Controls.OMEMO
{
    public sealed partial class OmemoButtonControl: UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public bool OmemoEnabled
        {
            get => (bool)GetValue(OmemoEnabledProperty);
            set => SetValue(OmemoEnabledProperty, value);
        }
        public static readonly DependencyProperty OmemoEnabledProperty = DependencyProperty.Register(nameof(OmemoEnabled), typeof(bool), typeof(OmemoButtonControl), new PropertyMetadata(false, OnOmemoEnabledChanged));

        public ChatDataTemplate Chat
        {
            get => (ChatDataTemplate)GetValue(ChatProperty);
            set => SetValue(ChatProperty, value);
        }
        public static readonly DependencyProperty ChatProperty = DependencyProperty.Register(nameof(ChatDataTemplate), typeof(ChatDataTemplate), typeof(OmemoButtonControl), new PropertyMetadata(null));

        public delegate void OmemoEnabledChangedEventHandler(OmemoButtonControl sender, EventArgs e);
        public event OmemoEnabledChangedEventHandler OmemoEnabledChanged;

        public readonly OmemoButtonControlContext VIEW_MODEL = new OmemoButtonControlContext();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public OmemoButtonControl()
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
        private async void ReadOnOmemo_link_Click(object sender, RoutedEventArgs e)
        {
            await VIEW_MODEL.OnReadOnOmemoClickedAsync();
        }

        private static void OnOmemoEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is OmemoButtonControl control)
            {
                control.OmemoEnabledChanged?.Invoke(control, new EventArgs());
            }
        }

        private void OnFlyoutOpened(object sender, object e)
        {
            omemoSupportControl.Refresh();
        }

        #endregion        
    }
}
