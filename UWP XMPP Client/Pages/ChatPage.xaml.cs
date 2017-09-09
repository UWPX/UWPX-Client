using Data_Manager.Classes;
using Data_Manager.Classes.DBEntries;
using Data_Manager.Classes.Managers;
using System.Collections.ObjectModel;
using UWP_XMPP_Client.DataTemplates;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using XMPP_API.Classes;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace UWP_XMPP_Client.Pages
{
    public sealed partial class ChatPage : Page
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private ObservableCollection<Chat> chats { get; set; }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Construktoren--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 26/08/2017 Created [Fabian Sauter]
        /// </history>
        public ChatPage()
        {
            this.InitializeComponent();
            loadChats();
            SystemNavigationManager.GetForCurrentView().BackRequested += AbstractBackRequestPage_BackRequested;
            ConnectionHandler.INSTANCE.NewChat += INSTANCE_NewChat;
            ConnectionHandler.INSTANCE.connect();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void loadChats()
        {
            chats = new ObservableCollection<Chat>();

            foreach (XMPPClient c in ConnectionHandler.INSTANCE.getXMPPClients())
            {
                List<ChatEntry> list = ChatManager.INSTANCE.getAllChatsForClient(c);
                sortChats(list);
                foreach (ChatEntry chat in list)
                {
                    Chat chatElement = new Chat { chat = chat, client = c };
                    chats.Add(chatElement);
                    if(masterDetail_pnl.ViewState == Microsoft.Toolkit.Uwp.UI.Controls.MasterDetailsViewState.Both && masterDetail_pnl.SelectedItem == null)
                    {
                        masterDetail_pnl.SelectedItem = chatElement;
                    }
                }
            }
        }

        private void sortChats(List<ChatEntry> list)
        {
            list.Sort((ChatEntry a, ChatEntry b) =>
            {
                if(a == b && a == null)
                {
                    return 0;
                }
                if(a.lastActive == null)
                {
                    if(b.lastActive == null)
                    {
                        return 0;
                    }
                    return -1;
                }
                if(b.lastActive == null)
                {
                    return 1;
                }
                return a.lastActive.CompareTo(b.lastActive);
            });
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private async void INSTANCE_NewChat(ConnectionHandler handler, Data_Manager.Classes.Events.NewChatEventArgs args)
        {
            ChatEntry chat = args.getChat();
            foreach (XMPPClient c in ConnectionHandler.INSTANCE.getXMPPClients())
            {
                if (chat.userAccountId.Contains(c.getSeverConnectionConfiguration().getIdAndDomain()))
                {
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        Chat chatElement = new Chat { chat = args.getChat(), client = c };
                        chats.Add(chatElement);
                        if (masterDetail_pnl.ViewState == Microsoft.Toolkit.Uwp.UI.Controls.MasterDetailsViewState.Both && masterDetail_pnl.SelectedItem == null)
                        {
                            masterDetail_pnl.SelectedItem = chatElement;
                        }
                    });
                }
            }
        }

        private void AbstractBackRequestPage_BackRequested(object sender, BackRequestedEventArgs e)
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

        private void settings_abb_Click(object sender, RoutedEventArgs e)
        {
            (Window.Current.Content as Frame).Navigate(typeof(SettingsPage));
        }

        private void add_abb_Click(object sender, RoutedEventArgs e)
        {

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
        }

        #endregion
    }
}
