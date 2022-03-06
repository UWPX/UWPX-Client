using System;

namespace XMPP_API_IoT.Classes.Bluetooth
{
    public static class BTUtils
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public static readonly Guid CHARACTERISTIC_DEVICE_NAME = Guid.Parse("00002A00-0000-1000-8000-00805F9B34FB");
        public static readonly Guid CHARACTERISTIC_LANGUAGE = Guid.Parse("00002AA2-0000-1000-8000-00805F9B34FB");
        public static readonly Guid CHARACTERISTIC_HARDWARE_REVISION = Guid.Parse("00002A27-0000-1000-8000-00805F9B34FB");
        public static readonly Guid CHARACTERISTIC_SERIAL_NUMBER = Guid.Parse("00002A25-0000-1000-8000-00805F9B34FB");
        public static readonly Guid CHARACTERISTIC_MANUFACTURER_NAME = Guid.Parse("00002A29-0000-1000-8000-00805F9B34FB");
        public static readonly Guid CHARACTERISTIC_WIFI_SSID = Guid.Parse("00000001-0000-0000-0000-000000000002");
        public static readonly Guid CHARACTERISTIC_WIFI_PASSWORD = Guid.Parse("00000002-0000-0000-0000-000000000002");
        public static readonly Guid CHARACTERISTIC_JID = Guid.Parse("00000003-0000-0000-0000-000000000002");
        public static readonly Guid CHARACTERISTIC_JID_PASSWORD = Guid.Parse("00000004-0000-0000-0000-000000000002");
        public static readonly Guid CHARACTERISTIC_JID_SENDER = Guid.Parse("00000005-0000-0000-0000-000000000002");
        public static readonly Guid CHARACTERISTIC_SETTINGS_DONE = Guid.Parse("00000006-0000-0000-0000-000000000002");

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public static void ReverseByteOrderIfNeeded(byte[] data)
        {
            if (!(data is null) && BitConverter.IsLittleEndian)
            {
                Array.Reverse(data);
            }
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
