using System;
using System.IO;
using System.Threading.Tasks;
using Logging;
using Shared.Classes;
using UWPX_UI_Context.Classes.DataContext.Controls;
using Windows.ApplicationModel;
using Windows.Media.Capture;
using Windows.System.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPX_UI.Controls
{
    public sealed partial class QrCodeScannerControl: UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly QrCodeScannerControlContext VIEW_MODEL = new QrCodeScannerControlContext();

        private MediaCapture camera;
        private DisplayRequest displayRequest = new DisplayRequest();
        private bool previewRunning;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public QrCodeScannerControl()
        {
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

                MediaCaptureInitializationSettings settings = new MediaCaptureInitializationSettings()
                {
                    StreamingCaptureMode = StreamingCaptureMode.Video
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
        }

        public async Task StopCameraAsync()
        {
            if (!(camera is null))
            {
                await SharedUtils.CallDispatcherAsync(async () =>
                {
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
        #endregion
    }
}
