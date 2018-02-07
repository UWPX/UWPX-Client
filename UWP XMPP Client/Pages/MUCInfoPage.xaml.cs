using Data_Manager2.Classes.DBManager;
using Data_Manager2.Classes.DBTables;
using UWP_XMPP_Client.Classes;
using UWP_XMPP_Client.Classes.Events;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using XMPP_API.Classes;
using System;

namespace UWP_XMPP_Client.Pages
{
    public sealed partial class MUCInfoPage : Page
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public ChatTable Chat
        {
            get { return (ChatTable)GetValue(chatProperty); }
            set
            {
                SetValue(chatProperty, value);
            }
        }
        public static readonly DependencyProperty chatProperty = DependencyProperty.Register("Chat", typeof(ChatTable), typeof(MUCInfoPage), null);

        public XMPPClient Client
        {
            get { return (XMPPClient)GetValue(clientProperty); }
            set { SetValue(clientProperty, value); }
        }
        public static readonly DependencyProperty clientProperty = DependencyProperty.Register("Client", typeof(XMPPClient), typeof(MUCInfoPage), null);

        public MUCChatInfoTable MUCInfo
        {
            get { return (MUCChatInfoTable)GetValue(mucInfoProperty); }
            set { SetValue(mucInfoProperty, value); }
        }
        public static readonly DependencyProperty mucInfoProperty = DependencyProperty.Register("MUCInfo", typeof(MUCChatInfoTable), typeof(UserProfilePage), null);

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 04/02/2018 Created [Fabian Sauter]
        /// </history>
        public MUCInfoPage()
        {
            this.InitializeComponent();
            SystemNavigationManager.GetForCurrentView().BackRequested += MUCInfoPage_BackRequested;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            UiUtils.setBackgroundImage(backgroundImage_img);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is NavigatedToMUCInfoEventArgs)
            {
                NavigatedToMUCInfoEventArgs args = e.Parameter as NavigatedToMUCInfoEventArgs;

                Client = args.CLIENT;
                Chat = args.CHAT;
                MUCInfo = args.MUC_INFO;

                ChatManager.INSTANCE.ChatChanged -= INSTANCE_ChatChanged;
                ChatManager.INSTANCE.ChatChanged += INSTANCE_ChatChanged;
                ChatManager.INSTANCE.MUCInfoChanged -= INSTANCE_MUCInfoChanged;
                ChatManager.INSTANCE.MUCInfoChanged += INSTANCE_MUCInfoChanged;
            }
        }

        private async void INSTANCE_MUCInfoChanged(ChatManager handler, Data_Manager.Classes.Events.MUCInfoChangedEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (Chat != null && Chat.id.Equals(args.MUC_INFO.chatId))
                {
                    MUCInfo = args.MUC_INFO;
                }
            });
        }

        private async void INSTANCE_ChatChanged(ChatManager handler, Data_Manager.Classes.Events.ChatChangedEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (Chat != null && Chat.id.Equals(args.CHAT.id))
                {
                    Chat = args.CHAT;
                }
            });
        }

        private void MUCInfoPage_BackRequested(object sender, BackRequestedEventArgs e)
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

        #endregion
    }
}
