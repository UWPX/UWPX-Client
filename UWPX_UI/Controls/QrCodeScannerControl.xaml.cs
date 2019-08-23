using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Logging;
using Shared.Classes;
using UWPX_UI_Context.Classes.DataContext.Controls;
using Windows.ApplicationModel;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Media.Capture.Frames;
using Windows.System.Display;
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
        private MediaCapture camera;
        private DisplayRequest displayRequest = new DisplayRequest();
        private bool previewRunning;

        // QR Code reader:
        private readonly BarcodeReader QR_CODE_READER;
        private MediaFrameReader frameReader;
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
            VIEW_MODEL.MODEL.Loading = true;
            try
            {
                camera = new MediaCapture();

                // Make sure we only access the video but not the audio stream for scanning QR Codes:
                MediaCaptureInitializationSettings settings = new MediaCaptureInitializationSettings()
                {
                    StreamingCaptureMode = StreamingCaptureMode.Video,
                    MemoryPreference = MediaCaptureMemoryPreference.Cpu
                };

                await camera.InitializeAsync(settings);

                displayRequest.RequestActive();
            }
            catch (UnauthorizedAccessException)
            {
                Logger.Error("Failed to access camera for scanning a QR Code!");
                VIEW_MODEL.MODEL.Loading = false;
                VIEW_MODEL.MODEL.HasAccess = false;
                return;
            }

            try
            {
                cameraPreview_ce.Source = camera;
                await camera.StartPreviewAsync();
                previewRunning = true;
                VIEW_MODEL.MODEL.HasAccess = true;
            }
            catch (FileLoadException)
            {
                Logger.Error("Can to access camera feed for the QR Code scanner. An other app has exclusive control!");
                camera.CaptureDeviceExclusiveControlStatusChanged += Camera_CaptureDeviceExclusiveControlStatusChanged;
                VIEW_MODEL.MODEL.HasAccess = false;
            }
            VIEW_MODEL.MODEL.Loading = false;

            await StartFrameListenerAsync();
        }

        public async Task StopCameraAsync()
        {
            if (!(camera is null))
            {
                await SharedUtils.CallDispatcherAsync(async () =>
                {
                    await StopFrameListenerAsync();

                    if (previewRunning)
                    {
                        previewRunning = false;

                        await camera.StopPreviewAsync();
                        displayRequest.RequestRelease();
                    }
                    cameraPreview_ce.Source = null;

                    camera.Dispose();
                    camera = null;
                });
            }
        }

        #endregion

        #region --Misc Methods (Private)--
        private async Task StartFrameListenerAsync()
        {
            try
            {
                if (camera.FrameSources.Count > 0)
                {
                    MediaFrameSource frameSource = camera.FrameSources.First().Value;
                    if (!(frameSource is null))
                    {
                        frameReader = await camera.CreateFrameReaderAsync(frameSource);
                        frameReader.FrameArrived += FrameReader_FrameArrived;
                        await frameReader.StartAsync();
                        StartQrCodeTask();
                    }
                    else
                    {
                        Logger.Error("QR Code scanner MediaFrameSource is null!");
                    }
                }
                else
                {
                    Logger.Error("QR Code scanner MediaFrameReader creation failed with: No camera available!");
                }
            }
            catch (Exception e)
            {
                Logger.Error("QR Code scanner MediaFrameReader creation failed with:", e);
            }
        }

        private async Task StopFrameListenerAsync()
        {
            StopQrCodeTask();
            if (!(frameReader is null))
            {
                frameReader.FrameArrived -= FrameReader_FrameArrived;
                await frameReader.StopAsync();
                frameReader.Dispose();
                frameReader = null;
            }
        }

        private void StartQrCodeTask()
        {
            shouldQrCodeScannerTaskRun = true;
            qrCodeScannerTask = Task.Run(async () =>
            {
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

        private async void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            await StopCameraAsync();
        }

        private async void Current_Suspending(object sender, SuspendingEventArgs e)
        {
            SuspendingDeferral deferral = e.SuspendingOperation.GetDeferral();
            await StopCameraAsync();
            deferral.Complete();
        }

        private async void Current_Resuming(object sender, object e)
        {
            await StartCameraAsync();
        }

        private async void Camera_CaptureDeviceExclusiveControlStatusChanged(MediaCapture sender, MediaCaptureDeviceExclusiveControlStatusChangedEventArgs args)
        {
            switch (args.Status)
            {
                case MediaCaptureDeviceExclusiveControlStatus.ExclusiveControlAvailable when !previewRunning:
                    await SharedUtils.CallDispatcherAsync(async () => await StartCameraAsync());
                    break;

                case MediaCaptureDeviceExclusiveControlStatus.SharedReadOnlyAvailable:
                    Logger.Error("Can to access camera feed for the QR Code scanner. An other app has exclusive control!");
                    VIEW_MODEL.MODEL.HasAccess = false;
                    break;
            }
        }

        private void QR_CODE_READER_ResultFound(Result result)
        {
            Logger.Debug("Read QR Code: " + result.Text);
        }

        private void FrameReader_FrameArrived(MediaFrameReader sender, MediaFrameArrivedEventArgs args)
        {
            MediaFrameReference frameRef = sender.TryAcquireLatestFrame();
            VideoMediaFrame frame = frameRef?.VideoMediaFrame;
            SoftwareBitmap softwareBitmap = frame?.SoftwareBitmap;
            if (!(softwareBitmap is null) && QR_CODE_IMAGE_SEMA.CurrentCount >= 1)
            {
                // Swap the process frame to qrCodeBitmap and dispose the unused image:
                softwareBitmap = Interlocked.Exchange(ref qrCodeBitmap, softwareBitmap);
                softwareBitmap?.Dispose();
            }
        }
        #endregion
    }
}
