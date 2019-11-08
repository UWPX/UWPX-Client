using Microsoft.Toolkit.Uwp.Connectivity;
using UWPX_UI_Context.Classes.DataTemplates.Controls.IoT;
using XMPP_API_IoT.Classes.Bluetooth;

namespace UWPX_UI_Context.Classes.DataContext.Controls.IoT
{
    public class BluetoothDeviceInfoControlContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly BluetoothDeviceInfoControlDataTemplate MODEL = new BluetoothDeviceInfoControlDataTemplate();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private string GetCurrentWiFiSsid()
        {
            if (NetworkHelper.Instance.ConnectionInformation.ConnectionType == ConnectionType.WiFi)
            {
                if (NetworkHelper.Instance.ConnectionInformation.NetworkNames.Count >= 1)
                {
                    return NetworkHelper.Instance.ConnectionInformation.NetworkNames[0];
                }
            }
            return "";
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void LoadInfos(BLEDevice device)
        {
            MODEL.Device = device;
            MODEL.DeviceName = device.CACHE.GetString(BTUtils.CHARACTERISTIC_DEVICE_NAME) ?? "";
            MODEL.DeviceHardwareRevision = device.CACHE.GetString(BTUtils.CHARACTERISTIC_HARDWARE_REVISION) ?? "";
            MODEL.DeviceLanguage = device.CACHE.GetString(BTUtils.CHARACTERISTIC_LANGUAGE) ?? "";
            MODEL.DeviceManufacturer = device.CACHE.GetString(BTUtils.CHARACTERISTIC_MANUFACTURER_NAME) ?? "";
            MODEL.DeviceSerialNumber = device.CACHE.GetString(BTUtils.CHARACTERISTIC_SERIAL_NUMBER) ?? "";
            MODEL.WifiSsid = GetCurrentWiFiSsid();
            MODEL.Jid = "testiot0@xmpp.uwpx.org";
            MODEL.JidPassword = "not a real password";
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
