using Manager.Classes;
using UWPX_UI_Context.Classes.DataContext.Controls.OMEMO;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPX_UI.Controls.OMEMO
{
    public sealed partial class OmemoOwnFingerprintControl: UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public Client Client
        {
            get => (Client)GetValue(ClientProperty);
            set => SetValue(ClientProperty, value);
        }
        public static readonly DependencyProperty ClientProperty = DependencyProperty.Register(nameof(Client), typeof(Client), typeof(OmemoOwnFingerprintControl), new PropertyMetadata(null, OnClientChanged));

        public readonly OmemoOwnFingerprintControlContext VIEW_MODEL = new OmemoOwnFingerprintControlContext();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public OmemoOwnFingerprintControl()
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
        private void UpdateView(DependencyPropertyChangedEventArgs e)
        {
            VIEW_MODEL.UpdateView(e);
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private static void OnClientChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is OmemoOwnFingerprintControl omemoFingerprintControl)
            {
                omemoFingerprintControl.UpdateView(e);
            }
        }

        #endregion
    }
}
