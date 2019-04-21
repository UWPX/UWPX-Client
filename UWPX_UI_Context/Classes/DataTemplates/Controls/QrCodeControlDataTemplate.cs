using System;
using System.Threading.Tasks;
using QRCoder;
using Shared.Classes;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace UWPX_UI_Context.Classes.DataTemplates.Controls
{
    public sealed class QrCodeControlDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private bool _IsLoading;
        public bool IsLoading
        {
            get => _IsLoading;
            set => SetProperty(ref _IsLoading, value);
        }
        private bool _DefaultQrCode;
        public bool DefaultQrCode
        {
            get => _DefaultQrCode;
            set => SetDefaultQrCode(value);
        }
        private BitmapImage _QrCodeImage;
        public BitmapImage QrCodeImage
        {
            get => _QrCodeImage;
            set => SetProperty(ref _QrCodeImage, value);
        }

        private string curQrCodeText;
        private readonly QRCodeGenerator QR_CODE_GENERATOR = new QRCodeGenerator();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private void SetDefaultQrCode(bool value)
        {
            if (SetProperty(ref _DefaultQrCode, value, nameof(DefaultQrCode)))
            {
                GenQrCode();
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void UpdateView(string s)
        {
            curQrCodeText = s;
            GenQrCode();
        }

        #endregion

        #region --Misc Methods (Private)--
        private void GenQrCode()
        {
            if (curQrCodeText is null)
            {
                QrCodeImage = null;
            }
            else
            {
                IsLoading = true;
                Color c = GetQrCodeBackgroundColor();
                Task.Run(async () =>
                {
                    QRCodeData qRCodeData = QR_CODE_GENERATOR.CreateQrCode(curQrCodeText, QRCodeGenerator.ECCLevel.Q);
                    PngByteQRCode qRCode = new PngByteQRCode(qRCodeData);
                    byte[] qRCodeGraphic;
                    if (DefaultQrCode)
                    {
                        qRCodeGraphic = qRCode.GetGraphic(10);
                    }
                    else
                    {
                        qRCodeGraphic = qRCode.GetGraphic(10, new byte[] { (byte)~c.R, (byte)~c.G, (byte)~c.B }, new byte[] { c.R, c.G, c.B });
                    }

                    await SharedUtils.CallDispatcherAsync(async () =>
                    {
                        using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
                        {
                            using (DataWriter writer = new DataWriter(stream.GetOutputStreamAt(0)))
                            {
                                writer.WriteBytes(qRCodeGraphic);
                                await writer.StoreAsync();
                            }
                            QrCodeImage = new BitmapImage();
                            await QrCodeImage.SetSourceAsync(stream);
                        }
                        IsLoading = false;
                    });
                });
            }
        }

        private Color GetQrCodeBackgroundColor()
        {
            object brush = Application.Current.Resources["AppBackgroundAcrylicElementBrush"];
            if (brush is Microsoft.UI.Xaml.Media.AcrylicBrush acrylicWindowBrush)
            {
                return acrylicWindowBrush.TintColor;
            }
            else
            {
                return ((SolidColorBrush)brush).Color;
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
