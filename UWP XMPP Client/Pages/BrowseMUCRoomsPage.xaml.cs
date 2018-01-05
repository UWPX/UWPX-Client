using System;
using System.Threading;
using System.Threading.Tasks;
using UWP_XMPP_Client.Classes;
using UWP_XMPP_Client.DataTemplates;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using XMPP_API.Classes;
using XMPP_API.Classes.Network.XML.Messages.XEP_0030;

namespace UWP_XMPP_Client.Pages
{
    public sealed partial class BrowseMUCRoomsPage : Page
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private CustomObservableCollection<MUCRoomTemplate> rooms;
        private string discoId;
        private Timer timer;

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
            this.discoId = null;
            this.timer = null;
            this.rooms = new CustomObservableCollection<MUCRoomTemplate>();
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
        private void sendDisco()
        {
            if (discoId == null && Client != null)
            {
                main_grid.Visibility = Visibility.Collapsed;
                loading_grid.Visibility = Visibility.Visible;
                noneFound_notification.Dismiss();
                discoId = "";
                Task<string> t = Client.createDiscoAsync(Server, DiscoType.ITEMS);
                Task.Factory.StartNew(async () => discoId = await t);
                startTimer();
            }
        }

        private void stopTimer()
        {
            timer?.Dispose();
        }

        private void startTimer()
        {
            timer = new Timer(async (obj) =>
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => showResultDisco(null));
            }, null, 5000, Timeout.Infinite);
        }

        private void showResultDisco(DiscoResponseMessage disco)
        {
            stopTimer();
            loading_grid.Visibility = Visibility.Collapsed;
            main_grid.Visibility = Visibility.Visible;
            if (disco == null || disco.ITEMS == null || disco.ITEMS.Count <= 0)
            {
                // Show non found in app notification:
                noneFound_notification.Show("None found. Please retry!", 0);
                discoId = null;
                return;
            }
            rooms.Clear();
            foreach (DiscoItem i in disco.ITEMS)
            {
                rooms.Add(new MUCRoomTemplate()
                {
                    client = Client,
                    roomInfo = new MUCRoomInfo()
                    {
                        jid = i.JID ?? "",
                        name = i.NAME ?? ""
                    }
                });
            }
            discoId = null;
        }

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

                sendDisco();
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

        private async void Client_NewDiscoResponseMessage(XMPPClient client, XMPP_API.Classes.Network.Events.NewDiscoResponseMessageEventArgs args)
        {
            if(discoId != null && discoId.Equals(args.DISCO.getId()))
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => showResultDisco(args.DISCO));
            }
        }

        private void refresh_btn_Click(object sender, RoutedEventArgs e)
        {
            sendDisco();
        }
        #endregion
    }
}
