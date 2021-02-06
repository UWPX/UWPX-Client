using Manager.Classes.Chat;
using UWPX_UI_Context.Classes.DataContext.Pages;
using UWPX_UI_Context.Classes.Events;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using XMPP_API.Classes;

namespace UWPX_UI.Pages
{
    public sealed partial class ContactInfoPage: Page
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly ContactInfoPageContext VIEW_MODEL = new ContactInfoPageContext();

        public ChatDataTemplate Chat
        {
            get => (ChatDataTemplate)GetValue(ChatProperty);
            set => SetValue(ChatProperty, value);
        }
        public static readonly DependencyProperty ChatProperty = DependencyProperty.Register(nameof(Chat), typeof(ChatDataTemplate), typeof(ContactInfoPage), new PropertyMetadata(null));

        public XMPPClient Client
        {
            get => (XMPPClient)GetValue(ClientProperty);
            set => SetValue(ClientProperty, value);
        }
        public static readonly DependencyProperty ClientProperty = DependencyProperty.Register(nameof(Client), typeof(XMPPClient), typeof(ContactInfoPage), new PropertyMetadata(null));

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ContactInfoPage()
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
            if (e.Parameter is NavigatedToContactInfoPageEventArgs args)
            {
                Chat = args.CHAT;
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            titleBar.OnPageNavigatedFrom();
        }

        #endregion
    }
}
