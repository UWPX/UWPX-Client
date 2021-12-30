using Storage.Classes.Models.Account;
using Storage.Classes.Models.Chat;
using UWPX_UI_Context.Classes.DataContext.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using XMPP_API.Classes;

namespace UWPX_UI.Controls
{
    public sealed partial class AccountImagePresenceControl: UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public Presence PresenceProp
        {
            get => (Presence)GetValue(PresencePropProperty);
            set => SetValue(PresencePropProperty, value);
        }
        public static readonly DependencyProperty PresencePropProperty = DependencyProperty.Register(nameof(PresenceProp), typeof(Presence), typeof(AccountImagePresenceControl), new PropertyMetadata(Presence.Unavailable));

        public ImageModel Image
        {
            get => (ImageModel)GetValue(ImageProperty);
            set => SetValue(ImageProperty, value);
        }
        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register(nameof(Image), typeof(ImageModel), typeof(AccountImagePresenceControl), new PropertyMetadata(null, OnImageChanged));

        public string BareJid
        {
            get => (string)GetValue(BareJidProperty);
            set => SetValue(BareJidProperty, value);
        }
        public static readonly DependencyProperty BareJidProperty = DependencyProperty.Register(nameof(BareJid), typeof(string), typeof(AccountImagePresenceControl), new PropertyMetadata("", OnBareJidChanged));

        public ChatType ChatType
        {
            get => (ChatType)GetValue(ChatTypeProperty);
            set => SetValue(ChatTypeProperty, value);
        }
        public static readonly DependencyProperty ChatTypeProperty = DependencyProperty.Register(nameof(ChatType), typeof(ChatType), typeof(AccountImagePresenceControl), new PropertyMetadata(ChatType.CHAT, OnChatTypeChanged));

        public Visibility PresenceVisibility
        {
            get => (Visibility)GetValue(PresenceVisibilityProperty);
            set => SetValue(PresenceVisibilityProperty, value);
        }
        public static readonly DependencyProperty PresenceVisibilityProperty = DependencyProperty.Register(nameof(PresenceVisibility), typeof(Visibility), typeof(AccountImagePresenceControl), new PropertyMetadata(Visibility.Visible));

        public readonly AccountImagePresenceControlContext VIEW_MODEL = new AccountImagePresenceControlContext();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public AccountImagePresenceControl()
        {
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


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private static void OnBareJidChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AccountImagePresenceControl control)
            {
                control.VIEW_MODEL.UpdateBareJid(control.BareJid);
                control.VIEW_MODEL.UpdateChatType(control.ChatType, control.BareJid);
            }
        }

        private static void OnChatTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AccountImagePresenceControl control)
            {
                control.VIEW_MODEL.UpdateChatType(control.ChatType, control.BareJid);
            }
        }

        private static async void OnImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AccountImagePresenceControl control)
            {
                await control.VIEW_MODEL.UpdateImageAsync(control.Image, control.BareJid);
                control.VIEW_MODEL.UpdateBareJid(control.BareJid);
            }
        }

        #endregion
    }
}
