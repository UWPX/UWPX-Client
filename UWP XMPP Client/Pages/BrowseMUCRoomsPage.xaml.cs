using System;
using System.Collections.ObjectModel;
using UWP_XMPP_Client.Classes;
using UWP_XMPP_Client.DataTemplates;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using XMPP_API.Classes;

namespace UWP_XMPP_Client.Pages
{
    public sealed partial class BrowseMUCRoomsPage : Page
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private ObservableCollection<MUCRoomTemplate> rooms;

        public XMPPClient Client
        {
            get { return (XMPPClient)GetValue(ClientProperty); }
            set { SetValue(ClientProperty, value); }
        }
        public static readonly DependencyProperty ClientProperty = DependencyProperty.Register("Client", typeof(XMPPClient), typeof(BrowseMUCRoomsPage), null);

        public string Server
        {
            get { return (string)GetValue(ServerProperty); }
            set { SetValue(ServerProperty, value); }
        }
        public static readonly DependencyProperty ServerProperty = DependencyProperty.Register("Server", typeof(string), typeof(BrowseMUCRoomsPage), null);

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 04/01/2018 Created [Fabian Sauter]
        /// </history>
        public BrowseMUCRoomsPage()
        {
            SystemNavigationManager.GetForCurrentView().BackRequested += BrowseMUCRoomsPage_BackRequested;
            this.Client = null;
            this.Server = null;
            this.rooms = new ObservableCollection<MUCRoomTemplate>();
            this.InitializeComponent();
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

        private void BrowseMUCRoomsPage_BackRequested(object sender, BackRequestedEventArgs e)
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if(e.Parameter is BrowseMUCNavigationParameter)
            {
                BrowseMUCNavigationParameter parameter = e.Parameter as BrowseMUCNavigationParameter;
                Client = parameter.client;
                Server = parameter.server;

                Client.NewDiscoResponseMessage -= Client_NewDiscoResponseMessage;
                Client.NewDiscoResponseMessage += Client_NewDiscoResponseMessage;
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if(Client != null)
            {
                Client.NewDiscoResponseMessage -= Client_NewDiscoResponseMessage;
            }
        }

        private void Client_NewDiscoResponseMessage(XMPPClient client, XMPP_API.Classes.Network.Events.NewDiscoResponseMessageEventArgs args)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
