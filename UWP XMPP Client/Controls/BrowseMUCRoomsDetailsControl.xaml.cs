using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using UWP_XMPP_Client.Classes;
using UWP_XMPP_Client.DataTemplates;
using UWP_XMPP_Client.Pages;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using XMPP_API.Classes;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.XEP_0030;
using XMPP_API.Classes.Network.XML.Messages.XEP_0045;
using XMPP_API.Classes.Network.XML.Messages.XEP_0045.Configuration;

namespace UWP_XMPP_Client.Controls
{
    public sealed partial class BrowseMUCRoomsDetailsControl : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private MessageResponseHelper messageResponseHelper;
        private CustomObservableCollection<MUCInfoOptionTemplate> options;

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
            this.messageResponseHelper = null;
            this.options = new CustomObservableCollection<MUCInfoOptionTemplate>();
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

        private void sendDisco(XMPPClient client, MUCRoomInfo info)
        {
            if (info != null && client != null)
            {
                details_itmc.Visibility = Visibility.Collapsed;
                loading_grid.Visibility = Visibility.Visible;

                if (messageResponseHelper != null)
                {
                    messageResponseHelper.Dispose();
                }

                messageResponseHelper = new MessageResponseHelper(client, onMessage, onTimeout);
                DiscoRequestMessage disco = new DiscoRequestMessage(client.getXMPPAccount().getIdDomainAndResource(), info.jid, DiscoType.INFO);
                messageResponseHelper.start(disco);
            }
        }

        private bool onMessage(AbstractMessage msg)
        {
            if (msg is ExtendedDiscoResponseMessage)
            {
                Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => showResultDisco(msg as ExtendedDiscoResponseMessage)).AsTask();
                return true;
            }
            return false;
        }

        private void onTimeout()
        {
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => showResultDisco(null)).AsTask();
        }

        private void showResultDisco(ExtendedDiscoResponseMessage disco)
        {
            options.Clear();
            messageResponseHelper?.Dispose();
            messageResponseHelper = null;

            if (disco != null && disco.roomConfig != null)
            {
                foreach (MUCInfoField o in disco.roomConfig.options)
                {
                    if (o.type != MUCInfoFieldType.HIDDEN)
                    {
                        options.Add(new MUCInfoOptionTemplate() { option = o });
                    }
                }
            }

            loading_grid.Visibility = Visibility.Collapsed;
            details_itmc.Visibility = Visibility.Visible;
        }

        private void showBackgroundForViewState(MasterDetailsViewState state)
        {
            backgroundImage_img.Visibility = state == MasterDetailsViewState.Both ? Visibility.Collapsed : Visibility.Visible;
            darkBackground_grid.Background = state == MasterDetailsViewState.Both ? new SolidColorBrush(Colors.Transparent) : main_grid.Background;
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            UiUtils.setBackgroundImage(backgroundImage_img);
            object o = (Window.Current.Content as Frame).Content;
            if (o is BrowseMUCRoomsPage)
            {
                BrowseMUCRoomsPage page = o as BrowseMUCRoomsPage;
                MasterDetailsView masterDetailsView = page.getMasterDetailsView();
                if (masterDetailsView != null)
                {
                    masterDetailsView.ViewStateChanged -= MasterDetailsView_ViewStateChanged;
                    masterDetailsView.ViewStateChanged += MasterDetailsView_ViewStateChanged;
                    showBackgroundForViewState(masterDetailsView.ViewState);
                }
            }
        }

        private void MasterDetailsView_ViewStateChanged(object sender, MasterDetailsViewState e)
        {
            showBackgroundForViewState(e);
        }

        private void UserControl_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if(args.NewValue is MUCRoomTemplate)
            {
                MUCRoomTemplate template = args.NewValue as MUCRoomTemplate;
                sendDisco(template.client, template.roomInfo);
            }
        }

        #endregion
    }
}
