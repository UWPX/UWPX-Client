using Data_Manager2.Classes.DBTables;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWP_XMPP_Client.Controls
{
    public sealed partial class KickBanOccupantControl : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public MUCOccupantTable Occupant
        {
            get { return (MUCOccupantTable)GetValue(OccupantProperty); }
            set { SetValue(OccupantProperty, value); }
        }
        public static readonly DependencyProperty OccupantProperty = DependencyProperty.Register("Occupant", typeof(MUCOccupantTable), typeof(KickBanOccupantControl), null);

        public string Reason
        {
            get { return (string)GetValue(ReasonProperty); }
            set { SetValue(ReasonProperty, value); }
        }
        public static readonly DependencyProperty ReasonProperty = DependencyProperty.Register("Reason", typeof(string), typeof(KickBanOccupantControl), null);

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 02/03/2018 Created [Fabian Sauter]
        /// </history>
        public KickBanOccupantControl()
        {
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
        private void banSingle_btn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void kickSingle_btn_Click(object sender, RoutedEventArgs e)
        {

        }

        #endregion
    }
}
