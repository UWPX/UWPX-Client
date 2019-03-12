using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using XMPP_API.Classes;

namespace UWPX_UI.Controls
{
    public sealed partial class AccountImagePresenceControl : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public Presence PresenceProp
        {
            get { return (Presence)GetValue(PresencePropProperty); }
            set { SetValue(PresencePropProperty, value); }
        }
        public static readonly DependencyProperty PresencePropProperty = DependencyProperty.Register(nameof(PresenceProp), typeof(Presence), typeof(AccountImagePresenceControl), new PropertyMetadata(Presence.Unavailable));

        public BitmapImage Image
        {
            get { return (BitmapImage)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }
        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register(nameof(Image), typeof(BitmapImage), typeof(AccountImagePresenceControl), new PropertyMetadata(null));

        public string Initials
        {
            get { return (string)GetValue(InitialsProperty); }
            set { SetValue(InitialsProperty, value); }
        }
        public static readonly DependencyProperty InitialsProperty = DependencyProperty.Register(nameof(Initials), typeof(string), typeof(AccountImagePresenceControl), new PropertyMetadata("\uE77B"));

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public AccountImagePresenceControl()
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
