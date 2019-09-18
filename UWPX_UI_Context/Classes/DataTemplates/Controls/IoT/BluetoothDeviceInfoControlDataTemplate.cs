using Shared.Classes;
using XMPP_API_IoT.Classes.Bluetooth;

namespace UWPX_UI_Context.Classes.DataTemplates.Controls.IoT
{
    public class BluetoothDeviceInfoControlDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private BLEDevice _Device;
        public BLEDevice Device
        {
            get => _Device;
            set => SetProperty(ref _Device, value);
        }
        private string _ErrorMsg;
        public string ErrorMsg
        {
            get => _ErrorMsg;
            set => SetProperty(ref _ErrorMsg, value);
        }

        private string _DeviceName;
        public string DeviceName
        {
            get => _DeviceName;
            set => SetProperty(ref _DeviceName, value);
        }
        private string _DeviceManufacturer;
        public string DeviceManufacturer
        {
            get => _DeviceManufacturer;
            set => SetProperty(ref _DeviceManufacturer, value);
        }
        private string _DeviceLanguage;
        public string DeviceLanguage
        {
            get => _DeviceLanguage;
            set => SetProperty(ref _DeviceLanguage, value);
        }
        private string _DeviceSerialNumber;
        public string DeviceSerialNumber
        {
            get => _DeviceSerialNumber;
            set => SetProperty(ref _DeviceSerialNumber, value);
        }
        private string _DeviceHardwareRevision;
        public string DeviceHardwareRevision
        {
            get => _DeviceHardwareRevision;
            set => SetProperty(ref _DeviceHardwareRevision, value);
        }

        // JID:
        private string _Jid;
        public string Jid
        {
            get => _Jid;
            set => SetProperty(ref _Jid, value);
        }
        private string _JidPassword;
        public string JidPassword
        {
            get => _JidPassword;
            set => SetProperty(ref _JidPassword, value);
        }

        // WiFi:
        private string _WifiSsid;
        public string WifiSsid
        {
            get => _WifiSsid;
            set => SetProperty(ref _WifiSsid, value);
        }
        private string _WifiPassword;
        public string WifiPassword
        {
            get => _WifiPassword;
            set => SetProperty(ref _WifiPassword, value);
        }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


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
