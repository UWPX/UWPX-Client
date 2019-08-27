using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Logging;
using Shared.Classes;
using UWPX_UI_Context.Classes.DataContext.Controls;
using UWPX_UI_Context.Classes.Events;
using Windows.ApplicationModel;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using ZXing;

namespace UWPX_UI.Controls
{
    public sealed partial class QrCodeScannerControl: UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly QrCodeScannerControlContext VIEW_MODEL = new QrCodeScannerControlContext();

        // QR Code reader:
        private readonly BarcodeReader QR_CODE_READER;
        private readonly SemaphoreSlim QR_CODE_IMAGE_SEMA = new SemaphoreSlim(1);
        private SoftwareBitmap qrCodeBitmap = null;
        private bool shouldQrCodeScannerTaskRun = false;
        private Task qrCodeScannerTask = null;

        public string QrCodeResultFilterRegex
        {
            get { return (string)GetValue(QrCodeResultFilterRegexProperty); }
            set { SetValue(QrCodeResultFilterRegexProperty, value); }
        }
        public static readonly DependencyProperty QrCodeResultFilterRegexProperty = DependencyProperty.Register(nameof(QrCodeResultFilterRegex), typeof(string), typeof(QrCodeScannerControl), new PropertyMetadata(".*", OnQrCodeResultFilterRegexChanged));

        public delegate void NewInvalidQrCodeEventHandler(QrCodeScannerControl sender, NewQrCodeEventArgs args);
        public event NewInvalidQrCodeEventHandler NewInvalidQrCode;

        public delegate void NewValidQrCodeEventHandler(QrCodeScannerControl sender, NewQrCodeEventArgs args);
        public event NewValidQrCodeEventHandler NewValidQrCode;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public QrCodeScannerControl()
        {
            // Prepare the QR Code reader:
            QR_CODE_READER = new BarcodeReader()
            {
                AutoRotate = true,
                TryInverted = true,
                Options = new ZXing.Common.DecodingOptions()
                {
                    PossibleFormats = new List<BarcodeFormat>() { BarcodeFormat.QR_CODE }
                }
            };
            QR_CODE_READER.Options.PossibleFormats.Clear();
            QR_CODE_READER.Options.PossibleFormats.Add(BarcodeFormat.QR_CODE);
            QR_CODE_READER.ResultFound += QR_CODE_READER_ResultFound;

            InitializeComponent();

            // Make sure we stop the preview when the app gets suspended.
            Application.Current.Suspending += Current_Suspending;

            // Resume the camera preview when the app gets resumed:
            Application.Current.Resuming += Current_Resuming;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void StartQrCodeTask()
        {
            shouldQrCodeScannerTaskRun = true;
            qrCodeScannerTask = Task.Run(async () =>
            {
                Logger.Info("Started QR Code scanner task.");
                while (shouldQrCodeScannerTaskRun)
                {
                    QR_CODE_IMAGE_SEMA.Wait();
                    if (qrCodeBitmap is null)
                    {
                        QR_CODE_IMAGE_SEMA.Release();
                        await Task.Delay(100).ConfAwaitFalse();
                    }
                    else
                    {
                        try
                        {
                            Result result = QR_CODE_READER.Decode(qrCodeBitmap);
                        }
                        catch (Exception e)
                        {
                            Logger.Error("Deconding QR Code bitmap failed with: ", e);
                        }
                        qrCodeBitmap.Dispose();
                        qrCodeBitmap = null;
                        QR_CODE_IMAGE_SEMA.Release();
                    }
                }
                Logger.Info("Ended QR Code scanner task.");
            });
        }

        private void StopQrCodeTask()
        {
            shouldQrCodeScannerTaskRun = false;
        }

        private void UpdateView(DependencyPropertyChangedEventArgs e)
        {
            VIEW_MODEL.UpdateView(e);
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            StartQrCodeTask();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            StopQrCodeTask();
        }

        private void Current_Suspending(object sender, SuspendingEventArgs e)
        {
            StopQrCodeTask();
        }

        private void Current_Resuming(object sender, object e)
        {
            StartQrCodeTask();
        }

        private void QR_CODE_READER_ResultFound(Result result)
        {
            Logger.Debug("Scanned QR Code: " + result.Text);
            if (!string.Equals(result.Text, VIEW_MODEL.MODEL.QrCode))
            {
                VIEW_MODEL.MODEL.QrCode = result.Text;
                if (VIEW_MODEL.IsValidQrCode(result.Text))
                {
                    NewValidQrCode?.Invoke(this, new NewQrCodeEventArgs(result.Text));
                }
                else
                {
                    NewInvalidQrCode?.Invoke(this, new NewQrCodeEventArgs(result.Text));
                }
            }
        }

        private static void OnQrCodeResultFilterRegexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is QrCodeScannerControl qrCodeScannerControl)
            {
                qrCodeScannerControl.UpdateView(e);
            }
        }

        private void CameraPreviewControl_FrameArrived(CameraPreviewControl sender, FrameArrivedEventArgs args)
        {
            if (QR_CODE_IMAGE_SEMA.CurrentCount >= 1)
            {
                args.GetSoftwareBitmap(ref qrCodeBitmap);
            }
        }
        #endregion
    }
}
