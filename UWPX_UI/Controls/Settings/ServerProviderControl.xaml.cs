using System;
using UWPX_UI_Context.Classes;
using UWPX_UI_Context.Classes.DataContext.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using XMPP_API.Classes;

namespace UWPX_UI.Controls.Settings
{
    public sealed partial class ServerProviderControl: UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly ServerProviderControlContext VIEW_MODEL = new ServerProviderControlContext();

        public Provider ServerProvider
        {
            get => (Provider)GetValue(ServerProviderProperty);
            set => SetValue(ServerProviderProperty, value);
        }
        public static readonly DependencyProperty ServerProviderProperty = DependencyProperty.Register(nameof(ServerProvider), typeof(Provider), typeof(ServerProviderControl), new PropertyMetadata(null, OnProviderChanged));

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ServerProviderControl()
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
        private void UpdateView(Provider provider)
        {
            VIEW_MODEL.UpdateView(provider);
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private static void OnProviderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is Provider provider && d is ServerProviderControl control)
            {
                control.UpdateView(provider);
            }
        }

        private async void OnLegalNoticeClicked(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            if (!string.IsNullOrEmpty(ServerProvider?.legalNotice))
            {
                await UiUtils.LaunchUriAsync(new Uri(ServerProvider.legalNotice));
            }
        }

        private async void OnRegisterClicked(IconButtonControl sender, RoutedEventArgs args)
        {
            if (sender.Tag is string s && !string.IsNullOrEmpty(s))
            {
                await UiUtils.LaunchUriAsync(new Uri(s));
            }
        }

        #endregion
    }
}
