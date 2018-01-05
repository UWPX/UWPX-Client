using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using UWP_XMPP_Client.Classes;
using UWP_XMPP_Client.DataTemplates;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using XMPP_API.Classes;
using XMPP_API.Classes.Network.XML.Messages.XEP_0030;
using XMPP_API.Classes.Network.XML.Messages.XEP_0045;

namespace UWP_XMPP_Client.Controls
{
    public sealed partial class BrowseMUCRoomsDetailsControl : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public MUCRoomInfo RoomInfo
        {
            get { return (MUCRoomInfo)GetValue(RoomInfoProperty); }
            set
            {
                SetValue(RoomInfoProperty, value);
                sendDisco();
            }
        }
        public static readonly DependencyProperty RoomInfoProperty = DependencyProperty.Register("RoomInfo", typeof(MUCRoomInfo), typeof(BrowseMUCRoomsDetailsControl), null);

        public XMPPClient Client
        {
            get { return (XMPPClient)GetValue(ClientProperty); }
            set
            {
                if (Client != null)
                {
                    Client.NewDiscoResponseMessage -= Client_NewDiscoResponseMessage;
                }
                SetValue(ClientProperty, value);
                if (Client != null)
                {
                    Client.NewDiscoResponseMessage -= Client_NewDiscoResponseMessage;
                    Client.NewDiscoResponseMessage += Client_NewDiscoResponseMessage;
                }
                sendDisco();
            }
        }

        public static readonly DependencyProperty ClientProperty = DependencyProperty.Register("Client", typeof(XMPPClient), typeof(BrowseMUCRoomsDetailsControl), null);

        private string discoId;
        private Timer timer;
        private ObservableCollection<MUCRoomDetailsTemplate> details;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 04/01/2018 Created [Fabian Sauter]
        /// </history>
        public BrowseMUCRoomsDetailsControl()
        {
            this.InitializeComponent();
            this.timer = null;
            this.discoId = null;
            this.details = new ObservableCollection<MUCRoomDetailsTemplate>();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void requestRoomInfo()
        {

        }

        private void sendDisco()
        {
            if (discoId == null && RoomInfo != null && Client != null)
            {
                details_itmc.Visibility = Visibility.Collapsed;
                loading_grid.Visibility = Visibility.Visible;
                discoId = "";
                Task<string> t = Client.createDiscoAsync(RoomInfo.jid, DiscoType.INFO);
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

        private void showResultDisco(ExtendedDiscoResponseMessage disco)
        {
            stopTimer();
            loading_grid.Visibility = Visibility.Collapsed;
            details_itmc.Visibility = Visibility.Visible;
            if (disco == null || (disco.FIELDS == null && disco.FEATURES == null))
            {
                discoId = null;
                return;
            }
            details.Clear();
            foreach (DiscoField i in disco.FIELDS)
            {
                if (!i.isHidden())
                {
                    details.Add(new MUCRoomDetailsTemplate()
                    {
                        label = i.LABEL,
                        value = i.VALUE
                    });
                }
            }
            discoId = null;
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private async void Client_NewDiscoResponseMessage(XMPPClient client, XMPP_API.Classes.Network.Events.NewDiscoResponseMessageEventArgs args)
        {
            if (discoId != null && args.DISCO is ExtendedDiscoResponseMessage && args.DISCO.getId().Equals(discoId))
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => showResultDisco(args.DISCO as ExtendedDiscoResponseMessage));
            }
        }

        #endregion
    }
}
