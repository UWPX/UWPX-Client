using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Core;
using UWP_XMPP_Client.Classes.Events;
using XMPP_API.Classes;
using UWP_XMPP_Client.Classes;
using Data_Manager2.Classes.DBTables;
using Data_Manager2.Classes.DBManager;

namespace UWP_XMPP_Client.Pages
{
    public sealed partial class UserProfilePage : Page
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private ChatTable chat;
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
        private void setChat(ChatTable chat)
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
            name_tblck.Text = chat.chatJabberId;
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
                ChatManager.INSTANCE.ChatChanged -= INSTANCE_ChatChanged;
                ChatManager.INSTANCE.ChatChanged += INSTANCE_ChatChanged;
            }
        }

        private async void INSTANCE_ChatChanged(ChatManager handler, Data_Manager.Classes.Events.ChatChangedEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if(chat != null && chat.id.Equals(args.CHAT.id))
                {
                    showProfile();
                }
            });
        }

        #endregion
    }
}
