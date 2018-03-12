using Data_Manager2.Classes.DBTables;
using System.Collections.ObjectModel;
using UWP_XMPP_Client.Controls;
using UWP_XMPP_Client.DataTemplates;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using XMPP_API.Classes;

namespace UWP_XMPP_Client.Dialogs
{
    public sealed partial class MUCKickBanOccupantDialog : ContentDialog
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public string Reason
        {
            get { return (string)GetValue(ReasonProperty); }
            set { SetValue(ReasonProperty, value); }
        }
        public static readonly DependencyProperty ReasonProperty = DependencyProperty.Register("Reason", typeof(string), typeof(MUCKickBanOccupantDialog), null);

        private XMPPClient client;
        private ChatTable chat;
        private readonly ObservableCollection<MUCOccupantTemplate> OCCUPANTS;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 08/03/2018 Created [Fabian Sauter]
        /// </history>
        public MUCKickBanOccupantDialog(ObservableCollection<MUCOccupantTemplate> occupants, XMPPClient client, ChatTable chat)
        {
            this.OCCUPANTS = occupants;
            this.client = client;
            this.chat = chat;
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
        private void showOccupants()
        {
            foreach (MUCOccupantTemplate t in OCCUPANTS)
            {
                occupants_itmsc.Items.Add(new KickBanOccupantControl(this, client, chat)
                {
                    Occupant = t.occupant
                });
            }
        }

        private void kickAll()
        {
            foreach (object o in occupants_itmsc.Items)
            {
                if (o is KickBanOccupantControl)
                {
                    KickBanOccupantControl c = o as KickBanOccupantControl;
                    c.kick();
                }
            }
        }

        private void banAll()
        {
            foreach (object o in occupants_itmsc.Items)
            {
                if (o is KickBanOccupantControl)
                {
                    KickBanOccupantControl c = o as KickBanOccupantControl;
                    c.ban();
                }
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Hide();
        }

        private void banAll_btn_Click(object sender, RoutedEventArgs e)
        {
            banAll();
        }

        private void kickAll_btn_Click(object sender, RoutedEventArgs e)
        {
            kickAll();
        }

        private void ContentDialog_Loaded(object sender, RoutedEventArgs e)
        {
            showOccupants();
        }

        #endregion
    }
}
