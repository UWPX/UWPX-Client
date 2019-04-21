using UWPX_UI_Context.Classes.DataContext.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPX_UI.Controls
{
    public sealed partial class QrCodeControl: UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public string QrCodeText
        {
            get => (string)GetValue(QrCodeTextProperty);
            set => SetValue(QrCodeTextProperty, value);
        }
        public static readonly DependencyProperty QrCodeTextProperty = DependencyProperty.Register(nameof(QrCodeText), typeof(string), typeof(QrCodeControl), new PropertyMetadata("", OnQrCodeTextChanged));

        public readonly QrCodeControlContext VIEW_MODEL = new QrCodeControlContext();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public QrCodeControl()
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
        private static void OnQrCodeTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is QrCodeControl qrCodeControl)
            {
                qrCodeControl.UpdateView(e);
            }
        }

        #endregion
    }
}
