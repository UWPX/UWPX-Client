using Data_Manager2.Classes.DBTables;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using UWP_XMPP_Client.DataTemplates;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using XMPP_API.Classes;
using System;
using XMPP_API.Classes.Network.XML.Messages.XEP_0030;
using Windows.UI.Core;
using System.Threading;

namespace UWP_XMPP_Client.Controls
{
    public sealed partial class ServerFeaturesControl : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public ChatTable Chat
        {
            get { return (ChatTable)GetValue(chatProperty); }
            set
            {
                SetValue(chatProperty, value);
                sendDiscos();
            }
        }
        public static readonly DependencyProperty chatProperty = DependencyProperty.Register("chat", typeof(ChatTable), typeof(ServerFeaturesControl), null);

        public XMPPClient Client
        {
            get { return (XMPPClient)GetValue(clientProperty); }
            set
            {
                if(Client != null)
                {
                    Client.NewDiscoResponseMessage -= Client_NewDiscoResponseMessage;
                }
                SetValue(clientProperty, value);
                bindEvents();
                sendDiscos();
            }
        }
        public static readonly DependencyProperty clientProperty = DependencyProperty.Register("client", typeof(XMPPClient), typeof(ServerFeaturesControl), null);

        public DiscoType discoType
        {
            get { return (DiscoType)GetValue(discoTypeProperty); }
            set
            {
                SetValue(discoTypeProperty, value);
                switch (value)
                {
                    case DiscoType.ITEMS:
                        title_tblck.Text = "Supported features (#items):";
                        break;
                    case DiscoType.INFO:
                        title_tblck.Text = "Supported features (#infos):";
                        break;
                    case DiscoType.UNKNOWN:
                    default:
                        title_tblck.Text = "Supported features (#infos):";
                        break;
                }
            }
        }
        public static readonly DependencyProperty discoTypeProperty = DependencyProperty.Register("discoType", typeof(DiscoType), typeof(ServerFeaturesControl), null);

        private ObservableCollection<ServerFeaturesTemplate> featuresList;
        private string discoItemsId;
        private string discoInfosId;
        private Timer timerItems;
        private Timer timerInfos;
        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 18/11/2017 Created [Fabian Sauter]
        /// </history>
        public ServerFeaturesControl()
        {
            this.discoItemsId = null;
            this.discoInfosId = null;
            this.timerItems = null;
            this.timerInfos = null;
            this.featuresList = new ObservableCollection<ServerFeaturesTemplate>();
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
        private void sendDiscos()
        {
            items_icon.Visibility = Visibility.Collapsed;
            noneFound_tblck.Visibility = Visibility.Collapsed;
            loading_spnl.Visibility = Visibility.Visible;
            featuresList.Clear();

            switch (discoType)
            {
                case DiscoType.ITEMS:
                    sendDiscoItems();
                    break;

                case DiscoType.INFO:
                    sendDiscoInfo();
                    break;

                default:
                    break;
            }
        }

        private void sendDiscoInfo()
        {
            if (discoInfosId == null && Client != null && Chat != null)
            {
                discoInfosId = "";
                Task<string> t = Client.createDiscoAsync(Chat.chatJabberId, DiscoType.INFO);
                Task.Run(async () => discoInfosId = await t);
                startTimerInfos();
            }
        }

        private void sendDiscoItems()
        {
            if (discoItemsId == null && Client != null && Chat != null)
            {
                discoItemsId = "";
                Task<string> t = Client.createDiscoAsync(Chat.chatJabberId, DiscoType.ITEMS);
                Task.Run(async () => discoItemsId = await t);
                startTimerItems();
            }
        }

        private void startTimerInfos()
        {
            timerInfos = new Timer(async (obj) =>
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => showResultDisco(null));
            }, null, 5000, Timeout.Infinite);
        }

        private void startTimerItems()
        {
            timerItems = new Timer(async (obj) =>
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => showResultDisco(null));
            }, null, 5000, Timeout.Infinite);
        }

        private void stopTimerInfos()
        {
            timerInfos?.Dispose();
        }

        private void stopTimerItems()
        {
            timerItems?.Dispose();
        }

        private void bindEvents()
        {
            if (Client != null)
            {
                Client.NewDiscoResponseMessage -= Client_NewDiscoResponseMessage;
                Client.NewDiscoResponseMessage += Client_NewDiscoResponseMessage;
            }
        }

        private void showResultDisco(DiscoResponseMessage disco)
        {
            loading_spnl.Visibility = Visibility.Collapsed;
            if (disco == null)
            {
                items_icon.Visibility = Visibility.Collapsed;
                noneFound_tblck.Text = "Server didn't answer in time!";
                noneFound_tblck.Visibility = Visibility.Visible;
            }
            else if (discoItemsId == null && discoInfosId == null)
            {
                if(featuresList.Count <= 0)
                {
                    items_icon.Visibility = Visibility.Collapsed;
                    noneFound_tblck.Text = "None";
                    noneFound_tblck.Visibility = Visibility.Visible;
                }
            }
            else if (disco.ERROR_RESULT != null)
            {
                items_icon.Visibility = Visibility.Collapsed;
                if(disco.DISCO_TYPE == DiscoType.UNKNOWN)
                {
                    noneFound_tblck.Text = "Server responded with an invalid answer! View the logs for more information.";
                }
                else
                {
                    noneFound_tblck.Text = "Server responded with an error of type: " + disco.ERROR_RESULT.TYPE + "\n and content:\n" + disco.ERROR_RESULT.CONTENT;
                }
                noneFound_tblck.Visibility = Visibility.Visible;
            }
            else
            {
                foreach (DiscoFeature f in disco.FEATURES)
                {
                    if (!string.IsNullOrWhiteSpace(f.VAR))
                    {
                        featuresList.Add(new ServerFeaturesTemplate()
                        {
                            name = f.VAR
                        });
                    }
                }
                foreach (DiscoItem i in disco.ITEMS)
                {
                    if (!string.IsNullOrWhiteSpace(i.JID))
                    {
                        featuresList.Add(new ServerFeaturesTemplate()
                        {
                            name = i.NAME ?? i.JID
                        });
                    }
                }
                items_icon.Visibility = Visibility.Visible;

                switch (disco.DISCO_TYPE)
                {
                    case DiscoType.ITEMS:
                        discoItemsId = null;
                        break;
                    case DiscoType.INFO:
                        discoInfosId = null;
                        break;
                }
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void Client_NewDiscoResponseMessage(XMPPClient client, XMPP_API.Classes.Network.Events.NewDiscoResponseMessageEventArgs args)
        {
            if (discoInfosId != null && string.Equals(discoInfosId, args.DISCO.ID))
            {
                stopTimerInfos();
                Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => showResultDisco(args.DISCO)).AsTask();
            }
            else if (discoItemsId != null && string.Equals(discoItemsId, args.DISCO.ID))
            {
                stopTimerItems();
                Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => showResultDisco(args.DISCO)).AsTask();
            }
        }

        #endregion
    }
}
