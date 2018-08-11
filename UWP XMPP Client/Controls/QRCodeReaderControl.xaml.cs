using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Toolkit.Uwp.Helpers;
using System.Threading.Tasks;
using ZXing;
using System.Linq;
using Windows.Graphics.Imaging;
using Logging;
using Windows.UI.Xaml.Media.Imaging;
using System.Collections.Generic;
using Windows.System.Display;
using Windows.Media.Capture;
using Windows.Graphics.Display;
using Windows.UI.Core;
using Windows.Media.Capture.Frames;

namespace UWP_XMPP_Client.Controls
{
    public sealed partial class QRCodeReaderControl : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private MediaCapture mediaCapture;
        private MediaFrameReader frameReader;
        private readonly BarcodeReader QR_CODE_READER;
        private uint frameCounter;
        private readonly SoftwareBitmapSource CAMERA_IMAGE;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 11/08/2018 Created [Fabian Sauter]
        /// </history>
        public QRCodeReaderControl()
        {
            this.mediaCapture = null;
            this.frameReader = null;
            this.QR_CODE_READER = new BarcodeReader()
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
            this.frameCounter = 0;
            this.CAMERA_IMAGE = new SoftwareBitmapSource();
            this.InitializeComponent();
            Application.Current.Suspending += Current_Suspending;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private async Task startCameraAsync()
        {
            if (mediaCapture != null)
            {
                return;
            }

            var settings = new MediaCaptureInitializationSettings()
            {
                MemoryPreference = MediaCaptureMemoryPreference.Cpu,
                StreamingCaptureMode = StreamingCaptureMode.Video
            };

            mediaCapture = new MediaCapture();
            try
            {
                await mediaCapture.InitializeAsync(settings);
                DisplayInformation.AutoRotationPreferences = DisplayOrientations.Landscape;
            }
            catch (UnauthorizedAccessException e)
            {
                Logger.Error("[QRCodeReaderControl] Failed to start camera: Access denied!", e);
                return;
            }

            try
            {
                cameraPreview_ce.Source = mediaCapture;
                await mediaCapture.StartPreviewAsync();
                if (mediaCapture.FrameSources.Count <= 0)
                {
                    MediaFrameSource frameSource = mediaCapture.FrameSources.First().Value;
                    if (frameSource != null)
                    {
                        frameReader = await mediaCapture.CreateFrameReaderAsync(frameSource);
                        frameReader.FrameArrived -= FrameReader_FrameArrived;
                        frameReader.FrameArrived += FrameReader_FrameArrived;

                        QR_CODE_READER.ResultFound -= QR_CODE_READER_ResultFound;
                        QR_CODE_READER.ResultFound += QR_CODE_READER_ResultFound;
                    }
                    else
                    {
                        Logger.Error("[QRCodeReaderControl] Failed to start camera! No sources.");
                    }
                }
                else
                {
                    Logger.Error("[QRCodeReaderControl] Failed to start camera! No sources.");
                }
            }
            catch (Exception e)
            {
                Logger.Error("[QRCodeReaderControl] Failed to start camera!", e);
                return;
            }
        }

        private void FrameReader_FrameArrived(MediaFrameReader sender, MediaFrameArrivedEventArgs args)
        {
            throw new NotImplementedException();
        }

        private async Task stopCameraAsync()
        {
            if (mediaCapture != null)
            {
                await mediaCapture.StopPreviewAsync();
                mediaCapture.Dispose();
                mediaCapture = null;
            }

            if (frameReader != null)
            {
                frameReader.Dispose();
                frameReader = null;
            }
            frameReader.FrameArrived -= FrameReader_FrameArrived;
            QR_CODE_READER.ResultFound -= QR_CODE_READER_ResultFound;
            Application.Current.Suspending -= Current_Suspending;
        }

        private void readQRCode(SoftwareBitmap barcodeBitmap)
        {
            try
            {
                QR_CODE_READER.Decode(barcodeBitmap);
            }
            catch (Exception e)
            {
                Logger.Error("[QRCodeReaderControl] Failed to decode image!", e);
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await startCameraAsync();
        }

        private async void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            await stopCameraAsync();
        }

        private async void Current_Suspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            await stopCameraAsync();
            deferral.Complete();
        }

        private async void CAMERA_HELPER_FrameArrived(object sender, FrameEventArgs e)
        {
            // Update the image control:
            await CAMERA_IMAGE.SetBitmapAsync(e.VideoFrame.SoftwareBitmap);

            // Only try to read from every 10th frame:
            if (frameCounter++ >= 10)
            {
                frameCounter = 0;
                readQRCode(e.VideoFrame.SoftwareBitmap);
            }
        }

        private void QR_CODE_READER_ResultFound(Result result)
        {
            Logger.Debug("[QRCodeReaderControl] " + result.Text);
        }

        #endregion
    }
}
