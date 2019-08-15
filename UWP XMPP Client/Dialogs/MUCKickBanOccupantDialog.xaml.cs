using Data_Manager2.Classes.DBTables;
using System.Collections.ObjectModel;
using UWP_XMPP_Client.Controls.Muc;
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

        private readonly XMPPClient CLIENT;
        private readonly ChatTable CHAT;
        private readonly ObservableCollection<MUCOccupantTemplate> OCCUPANTS;

        private readonly bool CAN_BAN;
        private readonly bool CAN_KICK;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 08/03/2018 Created [Fabian Sauter]
        /// </history>
        public MUCKickBanOccupantDialog(ObservableCollection<MUCOccupantTemplate> occupants, XMPPClient client, ChatTable chat, bool canKick, bool canBan)
        {
            OCCUPANTS = occupants;
            CLIENT = client;
            CHAT = chat;
            CAN_KICK = canKick;
            CAN_BAN = canBan;
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
        private void showOccupants()
        {
            foreach (MUCOccupantTemplate t in OCCUPANTS)
            {
                occupants_itmsc.Items.Add(new MucKickBanOccupantControl(this, CLIENT, CHAT, CAN_KICK, CAN_BAN)
                {
                    Occupant = t.occupant
                });
            }
        }

        private void kickAll()
        {
            foreach (object o in occupants_itmsc.Items)
            {
                if (o is MucKickBanOccupantControl)
                {
                    MucKickBanOccupantControl c = o as MucKickBanOccupantControl;
                    c.kick();
                }
            }
        }

        private void banAll()
        {
            foreach (object o in occupants_itmsc.Items)
            {
                if (o is MucKickBanOccupantControl)
                {
                    MucKickBanOccupantControl c = o as MucKickBanOccupantControl;
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
            banAll_btn.IsEnabled = CAN_BAN;
            kickAll_btn.IsEnabled = CAN_KICK;

            showOccupants();
        }

        #endregion
    }
}
