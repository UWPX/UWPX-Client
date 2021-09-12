using System;
using System.ComponentModel;
using Manager.Classes.Chat;
using UWPX_UI_Context.Classes.DataContext.Controls.OMEMO;
using UWPX_UI_Context.Classes.DataTemplates.Controls.OMEMO;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPX_UI.Controls.OMEMO
{
    public sealed partial class OmemoCheckSupportsControl: UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly OmemoCheckSupportsControlContext VIEW_MODEL = new OmemoCheckSupportsControlContext();

        public ChatDataTemplate Chat
        {
            get => (ChatDataTemplate)GetValue(ChatProperty);
            set => SetValue(ChatProperty, value);
        }
        public static readonly DependencyProperty ChatProperty = DependencyProperty.Register(nameof(Chat), typeof(ChatDataTemplate), typeof(OmemoCheckSupportsControlContext), new PropertyMetadata(null, OnChatChanged));

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public OmemoCheckSupportsControl()
        {
            InitializeComponent();
            VIEW_MODEL.MODEL.PropertyChanged += OnProperyChanged;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void Refresh()
        {
            VIEW_MODEL.Refresh();
        }

        #endregion

        #region --Misc Methods (Private)--
        private void UpdateViewState(VisualState newState)
        {
            VisualStateManager.GoToState(this, newState.Name, true);
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void OnRefreshClicked(object sender, RoutedEventArgs e)
        {
            VIEW_MODEL.Refresh();
        }

        private static void OnChatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is OmemoCheckSupportsControl control)
            {
                control.VIEW_MODEL.MODEL.Chat = e.NewValue as ChatDataTemplate;
            }
        }

        private void OnProperyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (string.Equals(e.PropertyName, nameof(OmemoCheckSupportsControlDataTemplate.Status)))
            {
                switch (VIEW_MODEL.MODEL.Status)
                {
                    case OmemoSupportedStatus.UNKNOWN:
                        UpdateViewState(Unknown);
                        break;
                    case OmemoSupportedStatus.CHECKING:
                        UpdateViewState(Checking);
                        break;
                    case OmemoSupportedStatus.SUPPORTED:
                        UpdateViewState(Supported);
                        break;
                    case OmemoSupportedStatus.ERROR:
                        UpdateViewState(Error);
                        break;
                    case OmemoSupportedStatus.OLD_VERSION:
                        UpdateViewState(OldVersion);
                        break;
                    case OmemoSupportedStatus.UNSUPPORTED:
                        UpdateViewState(Unsupported);
                        break;
                    default: // Should not happen.
                        throw new InvalidOperationException($"Invalid {nameof(OmemoSupportedStatus)}: {VIEW_MODEL.MODEL.Status}");
                }
            }
        }

        #endregion
    }
}
