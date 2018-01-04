using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWP_XMPP_Client.Controls
{
    public sealed partial class BrowseMUCRoomsMasterControl : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public string Jid
        {
            get { return (string)GetValue(JidProperty); }
            set
            {
                SetValue(JidProperty, value);
                showRoom();
            }
        }
        public static readonly DependencyProperty JidProperty = DependencyProperty.Register("Jid", typeof(string), typeof(BrowseMUCRoomsMasterControl), null);

        public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set
            {
                SetValue(NameProperty, value);
                showRoom();
            }
        }
        public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(BrowseMUCRoomsMasterControl), null);

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 04/01/2018 Created [Fabian Sauter]
        /// </history>
        public BrowseMUCRoomsMasterControl()
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
        private void showRoom()
        {
            if (string.IsNullOrEmpty(Name))
            {
                name_tblck.Visibility = Visibility.Collapsed;
            }
            else
            {
                name_tblck.Visibility = Visibility.Visible;
                name_tblck.Text = Name;
            }
            jid_tblck.Text = Jid ?? "";
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
