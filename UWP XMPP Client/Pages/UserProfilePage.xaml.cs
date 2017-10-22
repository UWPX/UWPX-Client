using System;
using Data_Manager.Classes.DBEntries;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Core;
using UWP_XMPP_Client.Classes.Events;
using XMPP_API.Classes;
using UWP_XMPP_Client.Classes;

namespace UWP_XMPP_Client.Pages
{
    public sealed partial class UserProfilePage : Page
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private ChatEntry chat;
        private XMPPClient client;

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
            UiUtils.setBackgroundImage(backgroundImage_img);
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private void setChat(ChatEntry chat)
        {
            this.chat = chat;
            showProfile();
        }

        private void setClient(XMPPClient client)
        {
            this.client = client;
            showClient();
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void showClient()
        {
            if(client == null)
            {
                return;
            }
        }

        private void showProfile()
        {
            if (chat == null)
            {
                return;
            }
            if(chat.name == null)
            {
                name_tblck.Text = chat.id;
            }
            else
            {
                name_tblck.Text = chat.name + " (" + chat.id + ')';
            }
            status_tblck.Text = chat.status ?? "";
            account_tblck.Text = chat.userAccountId ?? "";
            showSubscriptionStatus(chat.subscription ?? "");
            imagePresence_aiwp.Presence = chat.presence;
            imagePresenceSmall_aiwp.Presence = chat.presence;
        }

        private void showSubscriptionStatus(string status)
        {
            subscriptionStatus_tblck.Text = status;
            switch (status)
            {
                case "both":
                    fromArrow_tblck.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    toArrow_tblck.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    break;

                case "from":
                    fromArrow_tblck.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    toArrow_tblck.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    break;

                case "to":
                    fromArrow_tblck.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    toArrow_tblck.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    break;

                case "none":
                default:
                    fromArrow_tblck.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    toArrow_tblck.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    break;
            }
        }

        private void bindEvents()
        {
            if(chat != null)
            {
                chat.ChatChanged -= Chat_ChatChanged;
                chat.ChatChanged += Chat_ChatChanged;
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if(e.Parameter is NavigatedToUserProfileEventArgs)
            {
                NavigatedToUserProfileEventArgs args = e.Parameter as NavigatedToUserProfileEventArgs;
                setChat(args.getChat());
                setClient(args.getClient());
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
