using QRCoder;
using System;
using System.Threading.Tasks;
using UWP_XMPP_Client.Classes;
using Windows.Storage.Streams;
using Windows.UI;
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
            bool darkTheme = UiUtils.isDarkThemeActive();
            Task.Run(async () =>
            {
                QRCodeData qRCodeData = QR_CODE_GENERATOR.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
                BitmapByteQRCode qRCode = new BitmapByteQRCode(qRCodeData);
                byte[] qRCodeGraphic;
                if (darkTheme)
                {
                    qRCodeGraphic = qRCode.GetGraphic(10, new byte[] { Colors.White.R, Colors.White.G, Colors.White.B }, new byte[] { Colors.Black.R, Colors.Black.G, Colors.Black.B });
                }
                else
                {
                    qRCodeGraphic = qRCode.GetGraphic(10);
                }

                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    QRCodeBitmap = new BitmapImage();
                    using (var stream = new InMemoryRandomAccessStream())
                    {
                        using (var writer = new DataWriter(stream))
                        {
                            writer.WriteBytes(qRCodeGraphic);
                            await writer.StoreAsync();
                            await writer.FlushAsync();
                            writer.DetachStream();
                        }
                        stream.Seek(0);
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


        #endregion
    }
}
