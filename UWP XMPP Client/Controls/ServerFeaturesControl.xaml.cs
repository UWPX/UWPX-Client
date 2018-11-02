using Data_Manager2.Classes.DBTables;
using System.Collections.ObjectModel;
using UWP_XMPP_Client.DataTemplates;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using XMPP_API.Classes;
using System;
using XMPP_API.Classes.Network.XML.Messages.XEP_0030;
using Windows.UI.Core;
using XMPP_API.Classes.Network.XML.Messages;

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
                SetValue(clientProperty, value);
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

        private readonly ObservableCollection<ServerFeaturesTemplate> SERVER_FEATURES;
        private MessageResponseHelper<IQMessage> discoInfoHelper;
        private MessageResponseHelper<IQMessage> discoItemsHelper;
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
            this.discoInfoHelper = null;
            this.discoItemsHelper = null;
            this.SERVER_FEATURES = new ObservableCollection<ServerFeaturesTemplate>();
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
            SERVER_FEATURES.Clear();

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
            if (Client != null && Chat != null)
            {
                if (discoInfoHelper != null)
                {
                    return;
                }
                discoInfoHelper = Client.GENERAL_COMMAND_HELPER.createDisco(Chat.chatJabberId, DiscoType.INFO, onDiscoMsg, onDiscoTimeout);
            }
        }

        private void sendDiscoItems()
        {
            if (Client != null && Chat != null)
            {
                if (discoItemsHelper != null)
                {
                    return;
                }
                discoItemsHelper = Client.GENERAL_COMMAND_HELPER.createDisco(Chat.chatJabberId, DiscoType.ITEMS, onDiscoMsg, onDiscoTimeout);
            }
        }

        private bool onDiscoMsg(MessageResponseHelper<IQMessage> helper, IQMessage msg)
        {
            if (msg is DiscoResponseMessage disco)
            {
                Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => showDiscoResult(disco)).AsTask();
                return true;
            }
            else if (msg is IQErrorMessage error)
            {
                Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => showDiscoError(error)).AsTask();
                return true;
            }
            return false;
        }

        private void onDiscoTimeout(MessageResponseHelper<IQMessage> helper)
        {
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => showDiscoError(null)).AsTask();
        }

        private void showDiscoError(IQErrorMessage error)
        {
            loading_spnl.Visibility = Visibility.Collapsed;

            items_icon.Visibility = Visibility.Collapsed;
            if (error is null)
            {
                noneFound_tblck.Text = "Server didn't answer in time!";
            }
            else
            {
                noneFound_tblck.Text = "Request failed with: " + error.ERROR_OBJ.ToString();
            }
            noneFound_tblck.Visibility = Visibility.Visible;
        }

        private void showDiscoResult(DiscoResponseMessage disco)
        {
            loading_spnl.Visibility = Visibility.Collapsed;
            if (disco.ERROR_RESULT != null)
            {
                items_icon.Visibility = Visibility.Collapsed;
                if (disco.DISCO_TYPE == DiscoType.UNKNOWN)
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
                        SERVER_FEATURES.Add(new ServerFeaturesTemplate()
                        {
                            name = f.VAR
                        });
                    }
                }
                foreach (DiscoItem i in disco.ITEMS)
                {
                    if (!string.IsNullOrWhiteSpace(i.JID))
                    {
                        SERVER_FEATURES.Add(new ServerFeaturesTemplate()
                        {
                            name = i.NAME ?? i.JID
                        });
                    }
                }
                items_icon.Visibility = Visibility.Visible;
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
