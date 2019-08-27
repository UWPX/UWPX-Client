using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Logging;
using Shared.Classes;
using UWPX_UI_Context.Classes.DataContext.Controls;
using UWPX_UI_Context.Classes.Events;
using Windows.ApplicationModel;
using Windows.Devices.Enumeration;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Media.Capture.Frames;
using Windows.System.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPX_UI.Controls
{
    public sealed partial class CameraPreviewControl: UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly CameraPreviewControlContext VIEW_MODEL = new CameraPreviewControlContext();

        public delegate void PreviewFailedEventHandler(CameraPreviewControl sender, PreviewFailedEventArgs args);
        public event PreviewFailedEventHandler PreviewFailed;

        public delegate void FrameArrivedEventHandler(CameraPreviewControl sender, FrameArrivedEventArgs args);
        public event FrameArrivedEventHandler FrameArrived;

        // Preview:
        private bool previewRunning = false;
        private MediaCapture cameraCapture = null;
        private DisplayRequest displayRequest = new DisplayRequest();
        private int cameraIndex = -1;

        // Frame Reader:
        private bool frameListenerRunning = false;
        private MediaFrameReader frameReader;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public CameraPreviewControl()
        {
            InitializeComponent();
            UpdateViewState(Loading_State.Name);
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private void SetCameraTorchEnabled(bool enabled)
        {
            if (!(cameraCapture is null) && cameraCapture.VideoDeviceController.TorchControl.Supported)
            {
                cameraCapture.VideoDeviceController.TorchControl.Enabled = enabled;
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public async Task StartPreviewAsync(DeviceInformation device)
        {
            if (previewRunning)
            {
                Logger.Info("Camera preview already started.");
                return;
            }
            UpdateViewState(Loading_State.Name);

            try
            {
                cameraCapture = new MediaCapture();
                // Make sure we only access the video but not the audio stream for scanning QR Codes:
                MediaCaptureInitializationSettings settings = new MediaCaptureInitializationSettings()
                {
                    StreamingCaptureMode = StreamingCaptureMode.Video,
                    MemoryPreference = MediaCaptureMemoryPreference.Cpu,
                    VideoDeviceId = device.Id,
                    MediaCategory = MediaCategory.Other
                };

                await cameraCapture.InitializeAsync(settings);
                SetPropperPreviewRotation(cameraCapture, device);
                VIEW_MODEL.MODEL.LampAvailable = cameraCapture.VideoDeviceController.TorchControl.Supported;
                DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;
                displayRequest.RequestActive();
                camera_ce.Source = cameraCapture;
            }
            catch (UnauthorizedAccessException)
            {
                Logger.Error("Camera access denied.");
                await OnErrorAsync(PreviewError.ACCESS_DENIED);
                return;
            }

            try
            {
                await cameraCapture.StartPreviewAsync();
                previewRunning = true;
                UpdateViewState(Preview_State.Name);
            }
            catch (FileLoadException)
            {
                Logger.Error("Camera access denied. An other app has exclusive access. Try again later.");
                await StopPreviewAsync();
                await OnErrorAsync(PreviewError.ACCESS_DENIED_OTHER_APP);
            }

            await StartFrameListenerAsync();
        }

        public async Task StartPreviewAsync()
        {
            DeviceInformation device;
            try
            {
                // Find all available cameras:
                DeviceInformationCollection devices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
                VIEW_MODEL.MODEL.MultipleCamerasAvailable = devices.Count > 1;
                if (devices.Count <= 0)
                {
                    await OnErrorAsync(PreviewError.NO_CAMERA);
                    Logger.Info("No camera found.");
                    return;
                }
                if (cameraIndex < 0)
                {
                    // Try to get the rear camera by default:
                    for (int i = 0; i < devices.Count; i++)
                    {
                        if (devices[i].EnclosureLocation.Panel == Windows.Devices.Enumeration.Panel.Back)
                        {
                            cameraIndex = i;
                            break;
                        }
                    }
                }
                // Fall back to camera 0:
                if (devices.Count <= cameraIndex || cameraIndex < 0)
                {
                    cameraIndex = 0;
                }
                device = devices[cameraIndex];
            }
            catch (Exception e)
            {
                Logger.Error("Failed to load cameras.", e);
                return;
            }
            // Start the preview with the selected camera:
            await StartPreviewAsync(device);
        }

        public async Task StopPreviewAsync()
        {
            previewRunning = false;
            flashlight_btn.IsChecked = false;
            await StopFrameListenerAsync();

            if (!(cameraCapture is null))
            {
                try
                {
                    await cameraCapture.StopPreviewAsync();
                }
                catch (Exception)
                {
                }
                DisplayInformation.AutoRotationPreferences = DisplayOrientations.None;
                displayRequest.RequestRelease();
                camera_ce.Source = null;
                VIEW_MODEL.MODEL.LampAvailable = false;

                cameraCapture.Dispose();
                cameraCapture = null;
            }
        }

        #endregion

        #region --Misc Methods (Private)--
        private void UpdateViewState(string state)
        {
            VisualStateManager.GoToState(this, state, true);
        }

        private async Task StartFrameListenerAsync()
        {
            if (frameListenerRunning)
            {
                Logger.Info("Frame listener already running. Restarting it...");
                await StopFrameListenerAsync();
            }

            try
            {
                if (cameraCapture.FrameSources.Count > 0)
                {
                    MediaFrameSource frameSource = cameraCapture.FrameSources.First().Value;
                    int count = cameraCapture.FrameSources.Count;
                    if (!(frameSource is null))
                    {
                        frameReader = await cameraCapture.CreateFrameReaderAsync(frameSource);
                        frameReader.FrameArrived += FrameReader_FrameArrived;
                        await frameReader.StartAsync();
                        frameListenerRunning = true;
                    }
                    else
                    {
                        Logger.Info("MediaFrameSource is null.");
                        await OnErrorAsync(PreviewError.MEDIA_FRAME_IS_NULL);
                    }
                }
                else
                {
                    Logger.Info("MediaFrameReader creation failed with: No camera available.");
                    await OnErrorAsync(PreviewError.MEDIA_FRAME_NO_CAMERA);
                }
            }
            catch (Exception e)
            {
                Logger.Error("MediaFrameReader creation failed with:", e);
                await OnErrorAsync(PreviewError.MEDIA_FRAME_CREATION_FAILED);
            }
        }

        private async Task StopFrameListenerAsync()
        {
            if (!(frameReader is null))
            {
                frameReader.FrameArrived -= FrameReader_FrameArrived;
                try
                {
                    await frameReader.StopAsync();
                }
                catch (Exception)
                {
                }
                frameReader.Dispose();
                frameReader = null;
            }
        }

        private async Task OnErrorAsync(PreviewError error)
        {
            await StopPreviewAsync();
            UpdateViewState(Error_State.Name);
            PreviewFailed?.Invoke(this, new PreviewFailedEventArgs(error));
        }

        private async Task NextCameraAsync()
        {
            await StopPreviewAsync();
            cameraIndex++;
            await StartPreviewAsync();
        }

        private void SetPropperPreviewRotation(MediaCapture cameraCapture, DeviceInformation device)
        {
            if (!(device.EnclosureLocation is null))
            {
                if (device.EnclosureLocation.InDock)
                {
                    // 
                }
                else if (device.EnclosureLocation.InLid)
                {
                    camera_ce.FlowDirection = FlowDirection.RightToLeft;
                }
                else
                {
                    switch (device.EnclosureLocation.Panel)
                    {
                        case Windows.Devices.Enumeration.Panel.Front:
                            camera_ce.FlowDirection = FlowDirection.RightToLeft;
                            cameraCapture.SetPreviewRotation(VideoRotation.Clockwise270Degrees);
                            break;
                        case Windows.Devices.Enumeration.Panel.Back:
                            cameraCapture.SetPreviewRotation(VideoRotation.Clockwise90Degrees);
                            camera_ce.FlowDirection = FlowDirection.LeftToRight;
                            break;
                        case Windows.Devices.Enumeration.Panel.Top:
                        case Windows.Devices.Enumeration.Panel.Bottom:
                        case Windows.Devices.Enumeration.Panel.Left:
                        case Windows.Devices.Enumeration.Panel.Right:
                        default:
                            camera_ce.FlowDirection = FlowDirection.LeftToRight;
                            break;
                    }
                }
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private async void Current_Resuming(object sender, object e)
        {
            await StartPreviewAsync();
        }

        private async void Current_Suspending(object sender, SuspendingEventArgs e)
        {
            SuspendingDeferral deferral = e.SuspendingOperation.GetDeferral();
            await SharedUtils.CallDispatcherAsync(async () => await StopPreviewAsync());
            deferral.Complete();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Make sure we stop the preview when the app gets suspended:
            Application.Current.Suspending += Current_Suspending;
            // Resume the camera preview when the app gets resumed:
            Application.Current.Resuming += Current_Resuming;
            await StartPreviewAsync();
        }

        private async void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            // Make sure we stop the preview when the app gets suspended:
            Application.Current.Suspending -= Current_Suspending;
            // Resume the camera preview when the app gets resumed:
            Application.Current.Resuming -= Current_Resuming;
            await SharedUtils.CallDispatcherAsync(async () => await StopPreviewAsync());
        }

        private void FrameReader_FrameArrived(MediaFrameReader sender, MediaFrameArrivedEventArgs args)
        {
            MediaFrameReference frameRef = sender.TryAcquireLatestFrame();
            VideoMediaFrame frame = frameRef?.VideoMediaFrame;
            SoftwareBitmap softwareBitmap = frame?.SoftwareBitmap;
            if (!(softwareBitmap is null))
            {
                FrameArrivedEventArgs frameArgs = new FrameArrivedEventArgs();
                frameArgs.SetSoftwareBitmap(ref softwareBitmap);
                FrameArrived?.Invoke(this, frameArgs);
            }
        }

        private async void SwitchCamera_btn_Click(object sender, RoutedEventArgs e)
        {
            await NextCameraAsync();
        }

        private void Flashlight_btn_Click(object sender, RoutedEventArgs e)
        {
            SetCameraTorchEnabled((bool)flashlight_btn.IsChecked);
        }

        #endregion
    }
}
