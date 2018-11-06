using Data_Manager2.Classes;
using Data_Manager2.Classes.DBManager;
using Data_Manager2.Classes.DBTables;
using Data_Manager2.Classes.Events;
using Data_Manager2.Classes.Toast;
using Logging;
using Microsoft.Toolkit.Uwp.UI;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UWP_XMPP_Client.Classes;
using UWP_XMPP_Client.Classes.Collections;
using UWP_XMPP_Client.DataTemplates;
using UWP_XMPP_Client.Dialogs;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using XMPP_API.Classes;
using XMPP_API.Classes.Network.XML.Messages.XEP_0085;

namespace UWP_XMPP_Client.Pages
{
    public sealed partial class ChatPage : Page
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly MyAdvancedCollectionView CHATS_ACV;
        private readonly ObservableChatDictionaryList CHATS;
        private readonly ChatFilter CHAT_FILTER;

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
            this.CHATS = new ObservableChatDictionaryList();
            this.CHATS_ACV = new MyAdvancedCollectionView(CHATS, true)
            {
                Filter = aCVFilter
            };

            this.CHATS_ACV.ObserveFilterProperty(nameof(ChatTemplate.chat));
            this.CHATS_ACV.SortDescriptions.Add(new SortDescription(nameof(ChatTemplate.chat), SortDirection.Descending));
            this.CHAT_FILTER = new ChatFilter(this.CHATS_ACV);
            this.InitializeComponent();
            SystemNavigationManager.GetForCurrentView().BackRequested += ChatPage2_BackRequested;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        /// <summary>
        /// Returns the current MasterDetailsView control.
        /// </summary>
        public MasterDetailsView getMasterDetailsView()
        {
            return masterDetail_pnl;
        }

        /// <summary>
        /// Returns true if the chat type of the given chat is CHAT and chat state messages aren't disabled.
        /// </summary>
        /// <param name="chat">The chat which </param>
        /// <returns></returns>
        private bool shouldSendChatState(ChatTable chat)
        {
            return !Settings.getSettingBoolean(SettingsConsts.DONT_SEND_CHAT_STATE) && chat != null && chat.chatType == ChatType.CHAT;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private bool aCVFilter(object o)
        {
            return CHAT_FILTER.filter(o);
        }

        /// <summary>
        /// Returns a list of ChatTemplates loaded from the DB, bases on the XMPPClients from the ConnectionHandler.
        /// </summary>
        private List<ChatTemplate> getChatsFromDB()
        {
            List<ChatTemplate> list = new List<ChatTemplate>();
            foreach (XMPPClient c in ConnectionHandler.INSTANCE.getClients())
            {
                foreach (ChatTable chat in ChatDBManager.INSTANCE.getAllChatsForClient(c.getXMPPAccount().getIdAndDomain()))
                {
                    if (chat.chatType == ChatType.MUC)
                    {
                        list.Add(new ChatTemplate(c, chat, MUCDBManager.INSTANCE.getMUCInfo(chat.id), null));
                    }
                    else
                    {
                        list.Add(new ChatTemplate(c, chat, null));
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Loads all chats and inserts them into the chatsList.
        /// </summary>
        /// <param name="selectedChatId">The id of the chat which should get selected.</param>
        private void loadChats(string selectedChatId)
        {
            // Load all chats:
            Task.Run(() =>
            {
                ChatTemplate selectedChat = null;
                List<ChatTemplate> chats = getChatsFromDB();
                for (int i = 0; i < chats.Count; i++)
                {
                    if (string.Equals(selectedChatId, chats[i].chat.id))
                    {
                        selectedChat = chats[i];
                    }
                }

                // Show selected chat:
                Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    // Clear list:
                    CHATS.Clear();

                    // Add chats:
                    using (CHATS_ACV.DeferRefresh())
                    {
                        CHATS.AddRange(chats, false);
                    }
                    if (masterDetail_pnl.SelectedItem == null && selectedChat != null)
                    {
                        masterDetail_pnl.SelectedItem = selectedChat;
                    }
                }).AsTask();
            });
        }

        /// <summary>
        /// Adds a new chat to the chatsList and the DB.
        /// </summary>
        /// <param name="client">Which account/client owns this chat?</param>
        /// <param name="jID">The JID if the new chat.</param>
        /// <param name="addToRoster">Should the chat get added to the users roster?</param>
        /// <param name="requestSubscription">Request a presence subscription?</param>
        private async Task addChatAsync(XMPPClient client, string jID, bool addToRoster, bool requestSubscription)
        {
            if (client == null || jID == null)
            {
                string errorMessage = "Unable to add chat! client ?= " + (client == null) + " jabberId ?=" + (jID == null);
                Logger.Error(errorMessage);
                TextDialog dialog = new TextDialog(errorMessage, "Error");
                await UiUtils.showDialogAsyncQueue(dialog);
            }
            else
            {
                if (addToRoster)
                {
                    await client.GENERAL_COMMAND_HELPER.addToRosterAsync(jID).ConfigureAwait(false);
                }
                if (requestSubscription)
                {
                    await client.GENERAL_COMMAND_HELPER.requestPresenceSubscriptionAsync(jID).ConfigureAwait(false);
                }
                ChatDBManager.INSTANCE.setChat(new ChatTable
                {
                    id = ChatTable.generateId(jID, client.getXMPPAccount().getIdAndDomain()),
                    chatJabberId = jID,
                    userAccountId = client.getXMPPAccount().getIdAndDomain(),
                    ask = null,
                    inRoster = addToRoster,
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
            if (!CHAT_FILTER.setChatQuery(s) && force)
            {
                CHATS_ACV.RefreshFilter();
            }
        }

        private void updateFilterUi()
        {
            filterPresenceNotUnavailable_tmfo.IsChecked = CHAT_FILTER.notUnavailable;
            filterPresenceNotOnline_tmfo.IsChecked = CHAT_FILTER.notOnline;

            filterPresenceOnline_tmfo.IsChecked = CHAT_FILTER.hasPresenceFilter(Presence.Online);
            filterPresenceChat_tmfo.IsChecked = CHAT_FILTER.hasPresenceFilter(Presence.Chat);
            filterPresenceAway_tmfo.IsChecked = CHAT_FILTER.hasPresenceFilter(Presence.Away);
            filterPresenceXa_tmfo.IsChecked = CHAT_FILTER.hasPresenceFilter(Presence.Xa);
            filterPresenceDnd_tmfo.IsChecked = CHAT_FILTER.hasPresenceFilter(Presence.Dnd);
            filterPresenceUnavailable_tmfo.IsChecked = CHAT_FILTER.hasPresenceFilter(Presence.Unavailable);

            filterChat_tmfo.IsChecked = CHAT_FILTER.chat;
            filterMUC_tmfo.IsChecked = CHAT_FILTER.muc;
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
            if (e.NavigationMode == NavigationMode.New && e.Parameter is string && ((e.Parameter as string).Equals("App.xaml.cs") || (e.Parameter as string).Equals("AddAccountPage.xaml.cs")))
            {
                await UiUtils.showInitialStartDialogAsync();
                await UiUtils.showWhatsNewDialog();
            }
            else if (e.Parameter is ChatToastActivation chatToastActivation)
            {
                toastActivationString = chatToastActivation.CHAT_ID;
            }
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
            if (!(Window.Current.Content is Frame rootFrame))
            {
                return;
            }
            if (rootFrame.CanGoBack && e.Handled == false)
            {
                e.Handled = true;
                rootFrame.GoBack();
            }
        }

        private async void INSTANCE_ChatChanged(ChatDBManager handler, ChatChangedEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                // Backup selected chat:
                ChatTemplate selectedChat = null;
                if (masterDetail_pnl.SelectedItem != null && masterDetail_pnl.SelectedItem is ChatTemplate)
                {
                    selectedChat = masterDetail_pnl.SelectedItem as ChatTemplate;
                }

                if (args.REMOVED)
                {
                    CHATS.RemoveId(args.CHAT.id);
                    args.Cancel = true;

                    // Restore selected chat:
                    if (selectedChat != null && !string.Equals(args.CHAT.id, selectedChat.chat.id))
                    {
                        masterDetail_pnl.SelectedItem = selectedChat;
                    }
                    return;
                }
                else
                {
                    if (CHATS.UpdateChat(args.CHAT))
                    {
                        args.Cancel = true;
                        // Restore selected chat:
                        if (selectedChat != null)
                        {
                            masterDetail_pnl.SelectedItem = selectedChat;
                        }
                        return;
                    }
                }

                Task.Run(async () =>
                {
                    // Add the new chat to the list of chats:
                    foreach (XMPPClient c in ConnectionHandler.INSTANCE.getClients())
                    {
                        if (Equals(args.CHAT.userAccountId, c.getXMPPAccount().getIdAndDomain()))
                        {
                            ChatTemplate chat;
                            if (args.CHAT.chatType == ChatType.MUC)
                            {
                                chat = new ChatTemplate(c, args.CHAT, MUCDBManager.INSTANCE.getMUCInfo(args.CHAT.id), null);
                            }
                            else
                            {
                                chat = new ChatTemplate(c, args.CHAT, null);
                            }

                            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                            {
                                CHATS.Add(chat);
                                // Restore selected chat:
                                if (selectedChat != null)
                                {
                                    masterDetail_pnl.SelectedItem = selectedChat;
                                }
                            });
                        }
                    }
                });
            });
        }

        private async void INSTANCE_MUCInfoChanged(MUCDBManager handler, MUCInfoChangedEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => CHATS.UpdateMUCInfo(args.MUC_INFO));
        }

        private async void addChat_mfoi_Click(object sender, RoutedEventArgs e)
        {
            AddChatDialog dialog = new AddChatDialog();
            await UiUtils.showDialogAsyncQueue(dialog);
            if (!dialog.cancled)
            {
                await addChatAsync(dialog.client, dialog.jabberId, dialog.addToRoster, dialog.requestSubscription).ConfigureAwait(false);
            }
        }

        private async void addMUC_mfoi_Click(object sender, RoutedEventArgs e)
        {
            AddMUCDialog dialog = new AddMUCDialog();
            await UiUtils.showDialogAsyncQueue(dialog);
        }

        private void addMIX_mfoi_Click(object sender, RoutedEventArgs e)
        {
            // ToDo Add MIX support.
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
                        await c.client.GENERAL_COMMAND_HELPER.sendChatStateAsync(c.chat.chatJabberId, ChatState.ACTIVE);
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
                        await c.client.GENERAL_COMMAND_HELPER.sendChatStateAsync(c.chat.chatJabberId, ChatState.INACTIVE);
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

            // Subscribe to toast events:
            ToastHelper.OnChatMessageToast -= ToastHelper_OnChatMessageToast;
            ToastHelper.OnChatMessageToast += ToastHelper_OnChatMessageToast;

            // Load chat filter:
            filterChats_asb.Text = CHAT_FILTER.chatQuery;
            filterQuery_abb.IsChecked = CHAT_FILTER.chatQueryEnabled;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            // Unsubscribe from chat and MUC info changed events:
            ChatDBManager.INSTANCE.ChatChanged -= INSTANCE_ChatChanged;
            MUCDBManager.INSTANCE.MUCInfoChanged -= INSTANCE_MUCInfoChanged;

            // Unsubscribe from toast events:
            ToastHelper.OnChatMessageToast -= ToastHelper_OnChatMessageToast;
        }

        private void ToastHelper_OnChatMessageToast(OnChatMessageToastEventArgs args)
        {
            if(args.toasterTypeOverride == ChatMessageToasterType.FULL)
            {
                args.toasterTypeOverride = ChatMessageToasterType.REDUCED;
            }
        }

        private void filterChats_asb_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            filterChats(filterChats_asb.Text, false);
        }

        private void filterChats_asb_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            filterChats((args.QueryText ?? filterChats_asb.Text), true);
        }

        private void master_cmdb_Opening(object sender, object e)
        {
            changePresence_abb.IsEnabled = ConnectionHandler.INSTANCE.getClients().Count > 0;
        }

        private async void changePresence_abb_Click(object sender, RoutedEventArgs e)
        {
            ChangeAccountPresenceDialog dialog = new ChangeAccountPresenceDialog();
            await UiUtils.showDialogAsyncQueue(dialog);
        }

        private void manageBookmarks_abb_Click(object sender, RoutedEventArgs e)
        {
            (Window.Current.Content as Frame).Navigate(typeof(ManageBookmarksPage));
        }

        private async void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            await UiUtils.onPageSizeChangedAsync(e);
        }

        protected async override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            await UiUtils.onPageNavigatedFromAsync();
        }

        private void filterChat_tmfo_Click(object sender, RoutedEventArgs e)
        {
            CHAT_FILTER.setChatOnly(filterChat_tmfo.IsChecked);
            updateFilterUi();
        }

        private void filterMUC_tmfo_Click(object sender, RoutedEventArgs e)
        {
            CHAT_FILTER.setMUCOnly(filterMUC_tmfo.IsChecked);
            updateFilterUi();
        }

        private void filterPresenceNotUnavailable_tmfo_Click(object sender, RoutedEventArgs e)
        {
            CHAT_FILTER.setNotUnavailable(filterPresenceNotUnavailable_tmfo.IsChecked);
            updateFilterUi();
        }

        private void filterPresenceNotOnline_tmfo_Click(object sender, RoutedEventArgs e)
        {
            CHAT_FILTER.setNotOnline(filterPresenceNotOnline_tmfo.IsChecked);
            updateFilterUi();
        }

        private void filterPresenceOnline_tmfo_Click(object sender, RoutedEventArgs e)
        {
            CHAT_FILTER.setPresenceFilter(Presence.Online, filterPresenceOnline_tmfo.IsChecked);
            updateFilterUi();
        }

        private void filterPresenceChat_tmfo_Click(object sender, RoutedEventArgs e)
        {
            CHAT_FILTER.setPresenceFilter(Presence.Chat, filterPresenceChat_tmfo.IsChecked);
            updateFilterUi();
        }

        private void filterPresenceAway_tmfo_Click(object sender, RoutedEventArgs e)
        {
            CHAT_FILTER.setPresenceFilter(Presence.Away, filterPresenceAway_tmfo.IsChecked);
            updateFilterUi();
        }

        private void filterPresenceXa_tmfo_Click(object sender, RoutedEventArgs e)
        {
            CHAT_FILTER.setPresenceFilter(Presence.Xa, filterPresenceXa_tmfo.IsChecked);
            updateFilterUi();
        }

        private void filterPresenceDnd_tmfo_Click(object sender, RoutedEventArgs e)
        {
            CHAT_FILTER.setPresenceFilter(Presence.Dnd, filterPresenceDnd_tmfo.IsChecked);
            updateFilterUi();
        }

        private void filterPresenceUnavailable_tmfo_Click(object sender, RoutedEventArgs e)
        {
            CHAT_FILTER.setPresenceFilter(Presence.Unavailable, filterPresenceUnavailable_tmfo.IsChecked);
            updateFilterUi();
        }

        private void filterClear_mfo_Click(object sender, RoutedEventArgs e)
        {
            CHAT_FILTER.clearPresenceFilter();
            updateFilterUi();
        }

        private void filterQuery_abb_Checked(object sender, RoutedEventArgs e)
        {
            CHAT_FILTER.setChatQueryEnabled(true);
            filter_query_stckp.Visibility = Visibility.Visible;
            filterChats(filterChats_asb.Text, false);
        }

        private void filterQuery_abb_Unchecked(object sender, RoutedEventArgs e)
        {
            CHAT_FILTER.setChatQueryEnabled(false);
            filter_query_stckp.Visibility = Visibility.Collapsed;
            filterChats(string.Empty, false);
        }
        #endregion
    }
}
