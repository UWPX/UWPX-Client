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
using Data_Manager2.Classes.Events;

namespace UWP_XMPP_Client.Pages
{
    public sealed partial class MUCInfoPage : Page
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public ChatTable Chat
        {
            get { return (ChatTable)GetValue(ChatProperty); }
            set { SetValue(ChatProperty, value); }
        }
        public static readonly DependencyProperty ChatProperty = DependencyProperty.Register("Chat", typeof(ChatTable), typeof(MUCInfoPage), null);

        public XMPPClient Client
        {
            get { return (XMPPClient)GetValue(ClientProperty); }
            set { SetValue(ClientProperty, value); }
        }
        public static readonly DependencyProperty ClientProperty = DependencyProperty.Register("Client", typeof(XMPPClient), typeof(MUCInfoPage), null);

        public MUCChatInfoTable MUCInfo
        {
            get { return (MUCChatInfoTable)GetValue(MUCInfoProperty); }
            set { SetValue(MUCInfoProperty, value); }
        }
        public static readonly DependencyProperty MUCInfoProperty = DependencyProperty.Register("MUCInfo", typeof(MUCChatInfoTable), typeof(MUCInfoPage), null);

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

                ChatDBManager.INSTANCE.ChatChanged -= INSTANCE_ChatChanged;
                ChatDBManager.INSTANCE.ChatChanged += INSTANCE_ChatChanged;
                MUCDBManager.INSTANCE.MUCInfoChanged -= INSTANCE_MUCInfoChanged;
                MUCDBManager.INSTANCE.MUCInfoChanged += INSTANCE_MUCInfoChanged;
            }
        }

        private async void INSTANCE_MUCInfoChanged(MUCDBManager handler, MUCInfoChangedEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (Chat != null && Chat.id.Equals(args.MUC_INFO.chatId))
                {
                    MUCInfo = args.MUC_INFO;
                }
            });
        }

        private async void INSTANCE_ChatChanged(ChatDBManager handler, ChatChangedEventArgs args)
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
            if (rootFrame is null)
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
