using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using XMPP_API.Classes.Network.XML.Messages.XEP_0402;

namespace UWP_XMPP_Client.Controls
{
    public sealed partial class ManageBookmarksMasterControl : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public ConferenceItem Conference
        {
            get { return (ConferenceItem)GetValue(ConferenceProperty); }
            set { SetValue(ConferenceProperty, value); }
        }
        public static readonly DependencyProperty ConferenceProperty = DependencyProperty.Register(nameof(Conference), typeof(ConferenceItem), typeof(ManageBookmarksMasterControl), null);
    
        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 17/06/2018 Created [Fabian Sauter]
        /// </history>
        public ManageBookmarksMasterControl()
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


        #endregion
    }
}
