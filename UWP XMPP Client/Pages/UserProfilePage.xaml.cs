using System;
using Data_Manager.Classes.DBEntries;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Core;

namespace UWP_XMPP_Client.Pages
{
    public sealed partial class UserProfilePage : Page
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public ChatEntry Chat
        {
            get { return (ChatEntry)GetValue(ChatProperty); }
            set
            {
                SetValue(ChatProperty, value);
                showProfile();
            }
        }
        public static readonly DependencyProperty ChatProperty = DependencyProperty.Register("Chat", typeof(ChatEntry), typeof(UserProfilePage), null);

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 29/09/2017 Created [Fabian Sauter]
        /// </history>
        public UserProfilePage()
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
        private void showProfile()
        {
            if (Chat == null)
            {
                return;
            }
            if(Chat.name == null)
            {
                name_tblck.Text = Chat.id;
            }
            else
            {
                name_tblck.Text = Chat.name + " (" + Chat.id + ')';
            }
            status_tblck.Text = Chat.status ?? "";
            account_tblck.Text = Chat.userAccountId ?? "";
            subscriptionStatus_tblck.Text = Chat.subscription ?? "";
            imagePresence_aiwp.Presence = Chat.presence;
            imagePresenceSmall_aiwp.Presence = Chat.presence;
        }

        private void bindEvents()
        {
            if(Chat != null)
            {
                Chat.ChatChanged -= Chat_ChatChanged;
                Chat.ChatChanged += Chat_ChatChanged;
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if(e.Parameter is ChatEntry)
            {
                Chat = e.Parameter as ChatEntry;
            }
        }

        private async void Chat_ChatChanged(object sender, EventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                showProfile();
            });
        }

        #endregion
    }
}
