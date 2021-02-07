using Manager.Classes;
using UWPX_UI_Context.Classes.DataContext.Dialogs;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPX_UI.Dialogs
{
    public sealed partial class OmemoOwnFingerprintDialog: ContentDialog
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly OmemoOwnFingerprintDialogContext VIEW_MODEL = new OmemoOwnFingerprintDialogContext();

        public Client Client
        {
            get => (Client)GetValue(ClientProperty);
            set => SetValue(ClientProperty, value);
        }
        public static readonly DependencyProperty ClientProperty = DependencyProperty.Register(nameof(Client), typeof(Client), typeof(OmemoOwnFingerprintDialog), new PropertyMetadata(null));

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public OmemoOwnFingerprintDialog()
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
        private void close_btn_Click(Controls.IconButtonControl sender, RoutedEventArgs args)
        {
            Hide();
        }

        #endregion
    }
}
