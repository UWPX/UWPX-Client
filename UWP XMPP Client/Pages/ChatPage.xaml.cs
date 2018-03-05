using Data_Manager2.Classes;
using Data_Manager2.Classes.DBManager;
using Data_Manager2.Classes.DBTables;
using Logging;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using UWP_XMPP_Client.Classes;
using UWP_XMPP_Client.DataTemplates;
using UWP_XMPP_Client.Dialogs;
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
        private CustomObservableCollection<ChatTemplate> chatsList;
        private string currentChatQuery;

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
            this.chatsList = new CustomObservableCollection<ChatTemplate>();
            this.currentChatQuery = null;
            this.InitializeComponent();
            SystemNavigationManager.GetForCurrentView().BackRequested += ChatPage2_BackRequested;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public MasterDetailsView getMasterDetailsView()
        {
            return masterDetail_pnl;
        }

        private bool shouldSendChatState(ChatTable chat)
        {
            return !Settings.getSettingBoolean(SettingsConsts.DONT_SEND_CHAT_STATE) && chat != null && chat.chatType == ChatType.CHAT;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        /// <summary>
        /// Removes the given chat from the chatList and allChats list.
        /// </summary>
        /// <param name="c">The chat to remove.</param>
        private void removeChat(ChatTemplate c)
        {
            chatsList.Remove(c);
        }

        private void sortObservableCollection(ObservableCollection<ChatTemplate> collection)
        {
            List<ChatTemplate> sorted = collection.OrderBy(c => c.chat.lastActive).ToList();
            for (int i = 0; i < sorted.Count; i++)
            {
                collection.Move(collection.IndexOf(sorted[i]), i);
            }
        }

        private void sortList(List<ChatTemplate> list)
        {
            list.Sort((a, b) => { return DateTime.Compare(b.chat.lastActive, a.chat.lastActive); });
        }

        private List<ChatTemplate> getFilterdChats(string filter)
        {
            List<ChatTemplate> list = new List<ChatTemplate>();
            foreach (XMPPClient c in ConnectionHandler.INSTANCE.getClients())
            {
                foreach (ChatTable chat in ChatDBManager.INSTANCE.getAllChatsForClient(c.getXMPPAccount().getIdAndDomain(), filter))
                {
                    ChatTemplate chatElement = new ChatTemplate { chat = chat, client = c };
                    if (chat.chatType == ChatType.MUC)
                    {
                        chatElement.mucInfo = MUCDBManager.INSTANCE.getMUCInfo(chat.id);
                    }
                    list.Add(chatElement);
                }
            }
            return list;
        }

        private void loadChats(string selectedChatId)
        {
            // Clear list:
            chatsList.Clear();

            // Load all chats:
            Task.Run(() =>
            {
                ChatTemplate selectedChat = null;
                List<ChatTemplate> chats = getFilterdChats(null);
                for (int i = 0; i < chats.Count; i++)
                {
                    if (string.Equals(selectedChatId, chats[i].chat.chatJabberId))
                    {
                        selectedChat = chats[i];
                    }
                }

                // Sort chats:
                sortList(chats);

                // Show selected chat:
                Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    this.chatsList.AddRange(chats);
                    if (masterDetail_pnl.SelectedItem == null && selectedChat != null)
                    {
                        masterDetail_pnl.SelectedItem = selectedChat;
                    }
                }).AsTask();
            });
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
                ChatDBManager.INSTANCE.setChat(new ChatTable()
                {
                    id = ChatTable.generateId(jabberId, client.getXMPPAccount().getIdAndDomain()),
                    chatJabberId = jabberId,
                    userAccountId = client.getXMPPAccount().getIdAndDomain(),
                    ask = null,
                    inRoster = addToRooster,
                    lastActive = DateTime.Now,
                    muted = false,
                    presence = Presence.Unavailable,
                    status = null,
                    subscription = requestSubscription ? "pending" : null
                }, false, true);
            }
        }

        /// <summary>
        /// Filters all chats and only shows those that contain the given string.
        /// </summary>
        /// <param name="s">The string for filtering chats.</param>
        /// <param name="force">Force filtering.</param>
        private void filterChats(string s, bool force)
        {
            if (!force && Equals(s, currentChatQuery))
            {
                return;
            }

            currentChatQuery = s;
            Task.Run(async () =>
            {
                List<ChatTemplate> chats = getFilterdChats(s);
                sortList(chats);

                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    chatsList.Clear();
                    chatsList.AddRange(chats);
                });
            });
        }

        private bool doesChatTemplateMatchesFilter(ChatTemplate template, string filter)
        {
            return template.chat.chatJabberId.ToLower().Contains(filter) || (template.mucInfo != null && template.mucInfo.name != null && template.mucInfo.name.ToLower().Contains(filter));
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            loading_grid.Visibility = Visibility.Visible;
            main_grid.Visibility = Visibility.Collapsed;

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            string toastActivationString = null;
            if (e.NavigationMode == NavigationMode.New && e.Parameter is string && (e.Parameter as string).Equals("App.xaml.cs"))
            {
                await UiUtils.showInitialStartDialogAsync();
                await UiUtils.showWhatsNewDialog();
            }
            else if (e.Parameter is ToastNotificationActivatedEventArgs)
            {
                var toasActivationArgs = e.Parameter as ToastNotificationActivatedEventArgs;
                toastActivationString = toasActivationArgs.Argument;
                Logger.Info("ChatPage2 activated through toast with argument:" + toastActivationString);
            }
            ConnectionHandler.INSTANCE.connectAll();
            loadChats(toastActivationString);

            loading_grid.Visibility = Visibility.Collapsed;
            main_grid.Visibility = Visibility.Visible;

            if (e.Parameter is ShowAddMUCNavigationParameter)
            {
                ShowAddMUCNavigationParameter parameter = e.Parameter as ShowAddMUCNavigationParameter;
                AddMUCDialog dialog = new AddMUCDialog(parameter.ROOM_JID);
                await dialog.ShowAsync();
            }
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

        private void INSTANCE_ChatChanged(ChatDBManager handler, Data_Manager.Classes.Events.ChatChangedEventArgs args)
        {
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                // Find chat in chatsList:
                foreach (ChatTemplate chatTemplate in chatsList.Where((x) => x.chat != null && Equals(x.chat.id, args.CHAT.id)))
                {
                    if (args.REMOVED)
                    {
                        removeChat(chatTemplate);
                    }
                    else
                    {
                        chatTemplate.chat = args.CHAT;
                    }
                    args.Cancel = true;
                    return;
                }

                // If not found and should remove chat -> return:
                if (args.REMOVED)
                {
                    args.Cancel = true;
                    return;
                }

                Task.Run(() =>
                {
                    // Add the new chat to the list of chats:
                    foreach (XMPPClient c in ConnectionHandler.INSTANCE.getClients())
                    {
                        if (Equals(args.CHAT.userAccountId, c.getXMPPAccount().getIdAndDomain()))
                        {
                            ChatTemplate chatElement = new ChatTemplate { chat = args.CHAT, client = c };
                            if (args.CHAT.chatType == ChatType.MUC)
                            {
                                chatElement.mucInfo = MUCDBManager.INSTANCE.getMUCInfo(args.CHAT.id);
                            }

                            if (currentChatQuery == null || doesChatTemplateMatchesFilter(chatElement, currentChatQuery))
                            {
                                Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => chatsList.Insert(0, chatElement)).AsTask();
                            }
                        }
                    }
                });
            }).AsTask();
        }

        private void INSTANCE_MUCInfoChanged(MUCDBManager handler, Data_Manager.Classes.Events.MUCInfoChangedEventArgs args)
        {
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                foreach (ChatTemplate chatTemplate in chatsList.Where((x) => x.chat != null && x.chat.chatType == ChatType.MUC && Equals(x.chat.id, args.MUC_INFO.chatId)))
                {
                    chatTemplate.mucInfo = args.MUC_INFO;
                }
            }).AsTask();
        }

        private async void addChat_mfoi_Click(object sender, RoutedEventArgs e)
        {
            AddChatDialog addChatContentDialog = new AddChatDialog();
            await addChatContentDialog.ShowAsync();
            if (!addChatContentDialog.cancled)
            {
                await addChatAsync(addChatContentDialog.client, addChatContentDialog.jabberId, addChatContentDialog.addToRooster, addChatContentDialog.requestSubscription);
            }
        }

        private async void addMUC_mfoi_Click(object sender, RoutedEventArgs e)
        {
            AddMUCDialog dialog = new AddMUCDialog();
            await dialog.ShowAsync();

            if (!dialog.cancled)
            {

            }
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
            // Send active chat state:
            foreach (var added in e.AddedItems)
            {
                if (added is ChatTemplate)
                {
                    ChatTemplate c = added as ChatTemplate;
                    if (shouldSendChatState(c.chat))
                    {
                        await c.client.sendChatStateAsync(c.chat.chatJabberId, XMPP_API.Classes.Network.XML.Messages.XEP_0085.ChatState.ACTIVE);
                    }
                }
            }
            // Send inactive chat state:
            foreach (var added in e.RemovedItems)
            {
                if (added is ChatTemplate)
                {
                    ChatTemplate c = added as ChatTemplate;
                    if (shouldSendChatState(c.chat))
                    {
                        await c.client.sendChatStateAsync(c.chat.chatJabberId, XMPP_API.Classes.Network.XML.Messages.XEP_0085.ChatState.INACTIVE);
                    }
                }
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            UiUtils.setBackgroundImage(backgroundImage_img);

            // Subscribe to chat and MUC info changed events:
            ChatDBManager.INSTANCE.ChatChanged -= INSTANCE_ChatChanged;
            ChatDBManager.INSTANCE.ChatChanged += INSTANCE_ChatChanged;
            MUCDBManager.INSTANCE.MUCInfoChanged -= INSTANCE_MUCInfoChanged;
            MUCDBManager.INSTANCE.MUCInfoChanged += INSTANCE_MUCInfoChanged;
        }

        private void searchChats_asb_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            filterChats(searchChats_asb.Text.ToLower(), false);
        }

        private void searchChats_asb_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            filterChats((args.QueryText ?? searchChats_asb.Text).ToLower(), true);
        }

        #endregion
    }
}
