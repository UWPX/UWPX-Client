using Manager.Classes.Chat;
using Shared.Classes;
using UWPX_UI_Context.Classes.DataContext.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using XMPP_API.Classes;

namespace UWPX_UI.Controls.Chat
{
    public sealed partial class ContactInfoControl: UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly ContactInfoControlContext VIEW_MODEL = new ContactInfoControlContext();

        public ChatDataTemplate Chat
        {
            get => (ChatDataTemplate)GetValue(ChatProperty);
            set => SetValue(ChatProperty, value);
        }
        public static readonly DependencyProperty ChatProperty = DependencyProperty.Register(nameof(Chat), typeof(ChatDataTemplate), typeof(ContactInfoControl), new PropertyMetadata(null, OnChatChanged));

        public XMPPClient Client
        {
            get => (XMPPClient)GetValue(ClientProperty);
            set => SetValue(ClientProperty, value);
        }
        public static readonly DependencyProperty ClientProperty = DependencyProperty.Register(nameof(Client), typeof(XMPPClient), typeof(ContactInfoControl), new PropertyMetadata(null, OnClientChanged));

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ContactInfoControl()
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
            if (d is ContactInfoControl contactInfoControl)
            {
                contactInfoControl.UpdateView(e);
            }
        }

        private static void OnClientChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ContactInfoControl contactInfoControl)
            {
                contactInfoControl.UpdateView(e);
            }
        }

        private async void ProbePresence_mfo_Click(object sender, RoutedEventArgs e)
        {
            await VIEW_MODEL.SendPresenceProbeAsync(Chat).ConfAwaitFalse();
        }

        private async void RejectPresenceSubscription_mfo_Click(object sender, RoutedEventArgs e)
        {
            await VIEW_MODEL.RejectPresenceSubscriptionAsync(Chat).ConfAwaitFalse();
        }

        private async void RemoveFromRoster_mfo_Click(object sender, RoutedEventArgs e)
        {
            await VIEW_MODEL.SwitchChatInRosterAsync(Chat).ConfAwaitFalse();
        }

        private async void CancelPresenceSubscription_mfo_Click(object sender, RoutedEventArgs e)
        {
            await VIEW_MODEL.CancelPresenceSubscriptionAsync(Chat).ConfAwaitFalse();
        }

        private async void RequestPresenceSubscription_mfo_Click(object sender, RoutedEventArgs e)
        {
            await VIEW_MODEL.RequestPresenceSubscriptionAsync(Chat).ConfAwaitFalse();
        }

        private async void Mute_btn_Click(object sender, RoutedEventArgs e)
        {
            await VIEW_MODEL.ToggleChatMutedAsync(Chat).ConfAwaitFalse();
        }

        #endregion
    }
}
