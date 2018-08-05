using QRCoder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

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
            Task t = updateQRCodeAsync();
        }

        private async Task updateQRCodeAsync()
        {
            QRCodeData qRCodeData = QR_CODE_GENERATOR.CreateQrCode(QRCodeText, QRCodeGenerator.ECCLevel.Q);
            BitmapByteQRCode qRCode = new BitmapByteQRCode(qRCodeData);
            var bitmapImage = new BitmapImage();

            using (var stream = new InMemoryRandomAccessStream())
            {
                using (var writer = new DataWriter(stream))
                {
                    writer.WriteBytes(qRCode.GetGraphic(20));
                    await writer.StoreAsync();
                    await writer.FlushAsync();
                    writer.DetachStream();
                }
                stream.Seek(0);
                await bitmapImage.SetSourceAsync(stream);
            }
            QRCodeBitmap = bitmapImage;
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
