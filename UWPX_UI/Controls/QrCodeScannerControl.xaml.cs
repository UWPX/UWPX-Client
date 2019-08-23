using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Logging;
using Microsoft.Toolkit.Uwp.Helpers;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Shared.Classes;
using UWPX_UI_Context.Classes.DataContext.Controls;
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

        // Camera:
        private CameraHelper cameraHelper;

        // QR Code reader:
        private readonly BarcodeReader QR_CODE_READER;
        private readonly SemaphoreSlim QR_CODE_IMAGE_SEMA = new SemaphoreSlim(1);
        private SoftwareBitmap qrCodeBitmap = null;
        private bool shouldQrCodeScannerTaskRun = false;
        private Task qrCodeScannerTask = null;

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
        public async Task StartCameraAsync()
        {
            Logger.Info("Starting QR Code scanner camera...");
            VIEW_MODEL.MODEL.Loading = true;

            cameraHelper = new CameraHelper();
            CameraHelperResult result = await cameraHelper.InitializeAndStartCaptureAsync();
            if (result == CameraHelperResult.Success)
            {
                await cameraPreview_cp.StartAsync(cameraHelper);
                cameraHelper.FrameArrived += CameraHelper_FrameArrived;
                StartQrCodeTask();
                VIEW_MODEL.MODEL.HasAccess = true;
                Logger.Info("Started QR Code scanner camera.");
            }
            else
            {
                Logger.Error("Unable to start the QR Code scanner camera - " + result.ToString());
                VIEW_MODEL.MODEL.HasAccess = false;
            }
            VIEW_MODEL.MODEL.Loading = false;
        }

        public void StopCamera()
        {
            Logger.Info("Stopping QR Code scanner camera...");
            StopQrCodeTask();
            cameraPreview_cp.Stop();
            Logger.Info("Stopped QR Code scanner camera.");
        }
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
                        await Task.Delay(200).ConfAwaitFalse();
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

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await StartCameraAsync();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            StopCamera();
        }

        private void Current_Suspending(object sender, SuspendingEventArgs e)
        {
            StopCamera();
        }

        private async void Current_Resuming(object sender, object e)
        {
            await StartCameraAsync();
        }

        private void QR_CODE_READER_ResultFound(Result result)
        {
            Logger.Debug("Read QR Code: " + result.Text);
        }

        private void CameraHelper_FrameArrived(object sender, FrameEventArgs e)
        {
            SoftwareBitmap softwareBitmap = e.VideoFrame?.SoftwareBitmap;
            if (!(softwareBitmap is null) && QR_CODE_IMAGE_SEMA.CurrentCount >= 1)
            {
                // Swap the process frame to qrCodeBitmap and dispose the unused image:
                softwareBitmap = Interlocked.Exchange(ref qrCodeBitmap, softwareBitmap);
                softwareBitmap?.Dispose();
            }
        }

        private void CameraPreview_cp_PreviewFailed(object sender, PreviewFailedEventArgs e)
        {
            VIEW_MODEL.MODEL.HasAccess = false;
            VIEW_MODEL.MODEL.Loading = false;
            Logger.Error("Unable to start the QR Code scanner camera - " + e.Error);
        }
        #endregion
    }
}
