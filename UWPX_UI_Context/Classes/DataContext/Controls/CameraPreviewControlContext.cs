using System;
using System.Linq;
using System.Threading.Tasks;
using Logging;
using UWPX_UI_Context.Classes.DataTemplates.Controls;
using Windows.Devices.Enumeration;
using Windows.Devices.Lights;

namespace UWPX_UI_Context.Classes.DataContext.Controls
{
    public class CameraPreviewControlContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly CameraPreviewControlDataTemplate MODEL = new CameraPreviewControlDataTemplate();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public async Task InitLampAsync()
        {
            Logger.Info("Loading camera lamp...");
            string selector = Lamp.GetDeviceSelector();
            DeviceInformationCollection devices = await DeviceInformation.FindAllAsync(selector);
            DeviceInformation deviceInfo = devices.FirstOrDefault(di => di.EnclosureLocation != null && di.EnclosureLocation.Panel == Panel.Back);
            if (deviceInfo is null)
            {
                Logger.Info("No camera lamp found.");
                MODEL.Lamp = null;
                return;
            }
            Logger.Info("Found camera lamp.");
            MODEL.Lamp = await Lamp.FromIdAsync(deviceInfo.Id);
        }

        public void DisposeLamp()
        {
            if (!(MODEL.Lamp is null))
            {
                MODEL.Lamp.IsEnabled = false;
                MODEL.Lamp.Dispose();
                MODEL.Lamp = null;
            }
        }

        public void SetLampEnabled(bool isEnabled)
        {
            if (!(MODEL.Lamp is null))
            {
                MODEL.Lamp.IsEnabled = isEnabled;
            }
        }

        public async Task SwitchCameraAsync()
        {

        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
