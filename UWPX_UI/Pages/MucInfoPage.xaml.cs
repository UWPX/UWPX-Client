using Data_Manager2.Classes.DBManager;
using Data_Manager2.Classes.DBTables;
using Data_Manager2.Classes.Events;
using Shared.Classes;
using UWPX_UI_Context.Classes.DataContext.Pages;
using UWPX_UI_Context.Classes.Events;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using XMPP_API.Classes;

namespace UWPX_UI.Pages
{
    public sealed partial class MucInfoPage: Page
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly MucInfoPageContext VIEW_MODEL = new MucInfoPageContext();

        public ChatTable Chat
        {
            get { return (ChatTable)GetValue(ChatProperty); }
            set { SetValue(ChatProperty, value); }
        }
        public static readonly DependencyProperty ChatProperty = DependencyProperty.Register(nameof(Chat), typeof(ChatTable), typeof(MucInfoPage), new PropertyMetadata(null, OnChatChanged));

        public MUCChatInfoTable MucInfo
        {
            get { return (MUCChatInfoTable)GetValue(MucInfoProperty); }
            set { SetValue(MucInfoProperty, value); }
        }
        public static readonly DependencyProperty MucInfoProperty = DependencyProperty.Register(nameof(MucInfo), typeof(MUCChatInfoTable), typeof(MucInfoPage), new PropertyMetadata(null));

        public XMPPClient Client
        {
            get { return (XMPPClient)GetValue(ClientProperty); }
            set { SetValue(ClientProperty, value); }
        }
        public static readonly DependencyProperty ClientProperty = DependencyProperty.Register(nameof(Client), typeof(XMPPClient), typeof(MucInfoPage), new PropertyMetadata(null));

        // So we don't have to always interrupt the main task when a chat changed:
        private string chatId = null;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public MucInfoPage()
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


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            titleBar.OnPageNavigatedTo();

            if (e.Parameter is NavigatedToMucInfoPageEventArgs args)
            {
                Client = args.CLIENT;
                Chat = args.CHAT;
                MucInfo = args.MUC_INFO;
            }

            ChatDBManager.INSTANCE.ChatChanged -= INSTANCE_ChatChanged;
            ChatDBManager.INSTANCE.ChatChanged += INSTANCE_ChatChanged;
            MUCDBManager.INSTANCE.MUCInfoChanged -= INSTANCE_MUCInfoChanged;
            MUCDBManager.INSTANCE.MUCInfoChanged += INSTANCE_MUCInfoChanged;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            titleBar.OnPageNavigatedFrom();
            ChatDBManager.INSTANCE.ChatChanged -= INSTANCE_ChatChanged;
            MUCDBManager.INSTANCE.MUCInfoChanged -= INSTANCE_MUCInfoChanged;
        }

        private static void OnChatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MucInfoPage mucInfoPage)
            {
                mucInfoPage.chatId = e.NewValue is ChatTable chat ? chat.id : null;
            }
        }

        private async void INSTANCE_ChatChanged(ChatDBManager handler, ChatChangedEventArgs args)
        {
            if (!(args.CHAT is null) && string.Equals(args.CHAT.id, chatId))
            {
                await SharedUtils.CallDispatcherAsync(() => Chat = args.CHAT).ConfAwaitFalse();
            }
        }

        private async void INSTANCE_MUCInfoChanged(MUCDBManager handler, MUCInfoChangedEventArgs args)
        {
            if (!(args.MUC_INFO is null) && string.Equals(args.MUC_INFO.chatId, chatId))
            {
                await SharedUtils.CallDispatcherAsync(() => MucInfo = args.MUC_INFO).ConfAwaitFalse();
            }
        }

        #endregion
    }
}
