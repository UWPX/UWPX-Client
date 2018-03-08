using System.Collections.ObjectModel;
using UWP_XMPP_Client.DataTemplates;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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
        public MUCKickBanOccupantDialog(ObservableCollection<MUCOccupantTemplate> occupants)
        {
            this.OCCUPANTS = occupants;
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
        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void banAll_btn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void kickAll_btn_Click(object sender, RoutedEventArgs e)
        {

        }

        #endregion
    }
}
