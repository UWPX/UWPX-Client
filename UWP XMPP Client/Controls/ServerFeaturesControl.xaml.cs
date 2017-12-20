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
                sendDisco();
            }
        }
        public static readonly DependencyProperty chatProperty = DependencyProperty.Register("chat", typeof(ChatTable), typeof(ServerFeaturesControl), null);

        public XMPPClient Client
        {
            get { return (XMPPClient)GetValue(clientProperty); }
            set
            {
                SetValue(clientProperty, value);
                bindEvents();
                sendDisco();
            }
        }
        public static readonly DependencyProperty clientProperty = DependencyProperty.Register("client", typeof(XMPPClient), typeof(ServerFeaturesControl), null);

        private ObservableCollection<ServerFeaturesTemplate> featuresList;
        private string discoId;
        private Timer timer;
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
            this.discoId = null;
            this.timer = null;
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
        private void sendDisco()
        {
            if (discoId == null && Client != null && Chat != null)
            {
                items_icon.Visibility = Visibility.Collapsed;
                noneFound_tblck.Visibility = Visibility.Collapsed;
                loading_spnl.Visibility = Visibility.Visible;

                discoId = "";
                Task<string> t = Client.createDiscoAsync(Chat.chatJabberId);
                Task.Factory.StartNew(async () => discoId = await t);
                startTimer();
            }
        }

        private void startTimer()
        {
            timer = new Timer(async(obj) =>
            {
                discoId = null;
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => loadFeatures(null));
            }, null, 5000, Timeout.Infinite);
        }

        private void stopTimer()
        {
            timer?.Dispose();
        }

        private void bindEvents()
        {
            if (Client != null)
            {
                Client.NewDiscoResponseMessage -= Client_NewDiscoResponseMessage;
                Client.NewDiscoResponseMessage += Client_NewDiscoResponseMessage;
            }
        }

        private void loadFeatures(DiscoResponseMessage disco)
        {
            loading_spnl.Visibility = Visibility.Collapsed;
            featuresList.Clear();
            if(disco == null)
            {
                items_icon.Visibility = Visibility.Collapsed;
                noneFound_tblck.Text = "Server didn't answer in time!";
                noneFound_tblck.Visibility = Visibility.Visible;
            }
            else if(disco.ERROR_RESULT != null)
            {
                items_icon.Visibility = Visibility.Collapsed;
                noneFound_tblck.Text = "Server responded with an error of type: " + disco.ERROR_RESULT.TYPE + "\n and content:\n" + disco.ERROR_RESULT.CONTENT;
                noneFound_tblck.Visibility = Visibility.Visible;
            }
            else if(disco.FEATURES.Count <= 0)
            {
                items_icon.Visibility = Visibility.Collapsed;
                noneFound_tblck.Text = "None";
                noneFound_tblck.Visibility = Visibility.Visible;
            }
            else
            {
                foreach (DiscoFeature f in disco.FEATURES)
                {
                    featuresList.Add(new ServerFeaturesTemplate()
                    {
                        name = f.VAR ?? ""
                    });
                }
                items_icon.Visibility = Visibility.Visible;
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void Client_NewDiscoResponseMessage(XMPPClient client, XMPP_API.Classes.Network.Events.NewDiscoResponseMessageEventArgs args)
        {
            if (string.Equals(discoId, args.DISCO.getId()))
            {
                stopTimer();
                Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => loadFeatures(args.DISCO)).AsTask();
                discoId = null;
            }
        }

        #endregion
    }
}
