using Data_Manager2.Classes;
using Data_Manager2.Classes.DBManager;
using Data_Manager2.Classes.DBTables;
using Logging;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Threading.Tasks;
using UWP_XMPP_Client.Classes;
using UWP_XMPP_Client.Controls;
using UWP_XMPP_Client.DataTemplates;
using Windows.ApplicationModel.Activation;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using XMPP_API.Classes;

namespace UWP_XMPP_Client.Pages
{
    public sealed partial class ChatPage : Page
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private CustomObservableCollection<Chat> chatsList;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 26/11/2017 Created [Fabian Sauter]
        /// </history>
        public ChatPage()
        {
            this.chatsList = new CustomObservableCollection<Chat>();
            this.InitializeComponent();
            SystemNavigationManager.GetForCurrentView().BackRequested += ChatPage2_BackRequested;
            ChatManager.INSTANCE.ChatChanged += INSTANCE_ChatChanged;
            UiUtils.setBackgroundImage(backgroundImage_img);
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public MasterDetailsView getMasterDetailsView()
        {
            return masterDetail_pnl;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        /// <summary>
        /// Removes the given chat from the chatList and keeps the selected item in the masterDetail_pnl.
        /// </summary>
        /// <param name="c">The chat to remove.</param>
        private void removeChat(Chat c)
        {
            var selectedItem = masterDetail_pnl.SelectedItem;
            chatsList.Remove(c);
            if (c != selectedItem)
            {
                masterDetail_pnl.SelectedItem = selectedItem;
            }
        }

        private void addToChatsSorted(Chat c)
        {
            var selecetItem = masterDetail_pnl.SelectedItem;
            for (int i = 0; i < chatsList.Count; i++)
            {
                if (DateTime.Compare(chatsList[i].chat.lastActive, c.chat.lastActive) <= 0)
                {
                    chatsList.Insert(i, c);
                    masterDetail_pnl.SelectedItem = selecetItem;
                    return;
                }
            }
            chatsList.Add(c);
            masterDetail_pnl.SelectedItem = selecetItem;
        }

        private void loadChats(string selectedChatId)
        {
            // Clear list:
            chatsList.Clear();

            // Load all chats:
            Chat selectedChat = null;
            foreach (XMPPClient c in ConnectionHandler.INSTANCE.getClients())
            {
                foreach (ChatTable chat in ChatManager.INSTANCE.getAllChatsForClient(c.getXMPPAccount().getIdAndDomain()))
                {
                    Chat chatElement = new Chat { chat = chat, client = c };
                    addToChatsSorted(chatElement);
                    if (string.Equals(selectedChatId, chat.id))
                    {
                        selectedChat = chatElement;
                    }
                }
            }

            // Show selected chat:
            if (masterDetail_pnl.SelectedItem == null && selectedChat != null)
            {
                masterDetail_pnl.SelectedItem = selectedChat;
            }
        }

        private async Task addChatAsync(XMPPClient client, string jabberId, bool addToRooster, bool requestSubscription)
        {
            if (client == null || jabberId == null)
            {
                string errorMessage = "Unable to add chat! client ?= " + (client == null) + " jabberId ?=" + (jabberId == null);
                Logger.Error(errorMessage);
                MessageDialog messageDialog = new MessageDialog("Error")
                {
                    Content = errorMessage
                };
                await messageDialog.ShowAsync();
            }
            else
            {
                if (addToRooster)
                {
                    await client.addToRosterAsync(jabberId);
                }
                if (requestSubscription)
                {
                    await client.requestPresenceSubscriptionAsync(jabberId);
                }
                ChatManager.INSTANCE.setChat(new ChatTable()
                {
                    id = ChatTable.generateId(jabberId, client.getXMPPAccount().getIdAndDomain()),
                    chatJabberId = jabberId,
                    userAccountId = client.getXMPPAccount().getIdAndDomain(),
                    ask = null,
                    inRoster = false,
                    lastActive = DateTime.Now,
                    muted = false,
                    presence = Presence.Unavailable,
                    status = null,
                    subscription = requestSubscription ? "pending" : null
                }, false, true);
            }
        }
        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            loading_grid.Visibility = Visibility.Visible;
            main_grid.Visibility = Visibility.Collapsed;

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            string toastActivationString = null;
            if (e.Parameter is ToastNotificationActivatedEventArgs)
            {
                var toasActivationArgs = e.Parameter as ToastNotificationActivatedEventArgs;
                toastActivationString = toasActivationArgs.Argument;
                Logger.Info("ChatPage2 activated through toast with argument:" + toastActivationString);
            }
            ConnectionHandler.INSTANCE.connectAll();
            loadChats(toastActivationString);

            loading_grid.Visibility = Visibility.Collapsed;
            main_grid.Visibility = Visibility.Visible;
        }

        private void ChatPage2_BackRequested(object sender, BackRequestedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame == null)
            {
                return;
            }
            if (rootFrame.CanGoBack && e.Handled == false)
            {
                e.Handled = true;
                rootFrame.GoBack();
            }
        }

        private void INSTANCE_ChatChanged(ChatManager handler, Data_Manager.Classes.Events.ChatChangedEventArgs args)
        {
            ChatTable chatTable = args.CHAT;
            foreach (Chat c in chatsList)
            {
                if (c.chat != null && string.Equals(c.chat.id, chatTable.id))
                {
                    if (args.REMOVED)
                    {
                        removeChat(c);
                    }
                    else
                    {
                        c.chat = chatTable;
                    }
                    args.Cancel = true;
                    return;
                }
            }
            if (args.REMOVED)
            {
                args.Cancel = true;
                return;
            }

            foreach (XMPPClient c in ConnectionHandler.INSTANCE.getClients())
            {
                if (chatTable.userAccountId.Equals(c.getXMPPAccount().getIdAndDomain()))
                {
                    Chat chatElement = new Chat { chat = args.CHAT, client = c };
                    Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => addToChatsSorted(chatElement)).AsTask();
                }
            }
        }

        private async void addChat_mfoi_Click(object sender, RoutedEventArgs e)
        {
            AddChatContentDialog addChatContentDialog = new AddChatContentDialog();
            await addChatContentDialog.ShowAsync();
            if (!addChatContentDialog.cancled)
            {
                await addChatAsync(addChatContentDialog.client, addChatContentDialog.jabberId, addChatContentDialog.addToRooster, addChatContentDialog.requestSubscription);
            }
        }

        private void addMUC_mfoi_Click(object sender, RoutedEventArgs e)
        {

        }

        private void addMIX_mfoi_Click(object sender, RoutedEventArgs e)
        {

        }

        private void settings_abb_Click(object sender, RoutedEventArgs e)
        {
            (Window.Current.Content as Frame).Navigate(typeof(SettingsPage));
        }

        private async void masterDetail_pnl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!Settings.getSettingBoolean(SettingsConsts.DONT_SEND_CHAT_STATE))
            {
                // Send active chat state:
                foreach (var added in e.AddedItems)
                {
                    if (added is Chat)
                    {
                        Chat c = added as Chat;
                        await c.client.sendChatStateAsync(c.chat.chatJabberId, XMPP_API.Classes.Network.XML.Messages.XEP_0085.ChatState.ACTIVE);
                    }
                }
                // Send inactive chat state:
                foreach (var added in e.RemovedItems)
                {
                    if (added is Chat)
                    {
                        Chat c = added as Chat;
                        await c.client.sendChatStateAsync(c.chat.chatJabberId, XMPP_API.Classes.Network.XML.Messages.XEP_0085.ChatState.INACTIVE);
                    }
                }
            }
        }
        #endregion
    }
}
