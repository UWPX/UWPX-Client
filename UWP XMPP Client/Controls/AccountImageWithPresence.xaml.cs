using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using XMPP_API.Classes;

namespace UWP_XMPP_Client.Controls
{
    public sealed partial class AccountImageWithPresence : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public Presence Presence
        {
            get { return (Presence)GetValue(PresenceProperty); }
            set
            {
                SetValue(PresenceProperty, value);
                onPresenceUpdated();
            }
        }
        public static readonly DependencyProperty PresenceProperty = DependencyProperty.Register("Presence", typeof(Presence), typeof(AccountImageWithPresence), null);

        public ImageSource Image
        {
            get { return (BitmapImage)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }
        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register("Image", typeof(BitmapImage), typeof(AccountImageWithPresence), null);

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 05/09/2017 Created [Fabian Sauter]
        /// </history>
        public AccountImageWithPresence()
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
        private void onPresenceUpdated()
        {
            switch (Presence)
            {
                case Presence.Online:
                    presence_brdr.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 16, 124, 16));
                    break;
                case Presence.Chat:
                    presence_brdr.BorderBrush = new SolidColorBrush(Colors.White);
                    break;
                case Presence.Away:
                    presence_brdr.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 235, 140, 16));
                    break;
                case Presence.Xa:
                    presence_brdr.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 235, 73, 16));
                    break;
                case Presence.Dnd:
                    presence_brdr.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 162, 16, 37));
                    break;
                default:
                    presence_brdr.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 76, 74, 75));
                    break;
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
