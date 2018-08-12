using QRCoder;
using System;
using System.Threading.Tasks;
using UWP_XMPP_Client.Classes;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace UWP_XMPP_Client.Controls
{
    public sealed partial class QRCodeControl : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public string QRCodeText
        {
            get { return (string)GetValue(QRCodeTextProperty); }
            set
            {
                if (QRCodeText != value)
                {
                    SetValue(QRCodeTextProperty, value);
                    updateQRCode();
                }
            }
        }
        public static readonly DependencyProperty QRCodeTextProperty = DependencyProperty.Register(nameof(QRCodeText), typeof(string), typeof(QRCodeControl), new PropertyMetadata(null));

        public BitmapImage QRCodeBitmap
        {
            get { return (BitmapImage)GetValue(QRCodeBitmapProperty); }
            set { SetValue(QRCodeBitmapProperty, value); }
        }
        public static readonly DependencyProperty QRCodeBitmapProperty = DependencyProperty.Register(nameof(QRCodeBitmap), typeof(BitmapImage), typeof(QRCodeControl), new PropertyMetadata(null));

        private readonly QRCodeGenerator QR_CODE_GENERATOR;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 17/03/2018 Created [Fabian Sauter]
        /// </history>
        public QRCodeControl()
        {
            this.QR_CODE_GENERATOR = new QRCodeGenerator();
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
        private void updateQRCode()
        {
            generating_pgr.Visibility = Visibility.Visible;
            string text = QRCodeText;

            bool darkTheme = !(bool)defaultQRCode_ckbx.IsChecked;
            Task.Run(async () =>
            {
                QRCodeData qRCodeData = QR_CODE_GENERATOR.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
                PngByteQRCode qRCode = new PngByteQRCode(qRCodeData);
                byte[] qRCodeGraphic;
                if (darkTheme)
                {
                    qRCodeGraphic = qRCode.GetGraphic(10, new byte[] { 0xFF, 0xFF, 0xFF }, new byte[] { 0x00, 0x00, 0x00 });
                }
                else
                {
                    qRCodeGraphic = qRCode.GetGraphic(10);
                }

                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {

                    using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
                    {
                        using (DataWriter writer = new DataWriter(stream.GetOutputStreamAt(0)))
                        {
                            writer.WriteBytes(qRCodeGraphic);
                            await writer.StoreAsync();
                        }
                        QRCodeBitmap = new BitmapImage();
                        await QRCodeBitmap.SetSourceAsync(stream);
                    }
                    generating_pgr.Visibility = Visibility.Collapsed;
                });
            });
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void defaultQRCode_ckbx_Checked(object sender, RoutedEventArgs e)
        {
            updateQRCode();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            bool darkTheme = UiUtils.isDarkThemeActive();
            defaultQRCode_ckbx.IsChecked = !darkTheme;
            defaultQRCode_ckbx.IsEnabled = darkTheme;
        }

        #endregion
    }
}
