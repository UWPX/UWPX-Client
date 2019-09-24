using System;
using System.Linq;

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
        public static readonly Guid CHARACTERISTIC_SETTINGS_DONE = Guid.Parse("00000005-0000-0000-0000-000000000002");
        public static readonly Guid CHARACTERISTIC_CHALLENGE_RESPONSE_READ = Guid.Parse("00000006-0000-0000-0000-000000000002");
        public static readonly Guid CHARACTERISTIC_CHALLENGE_RESPONSE_WRITE = Guid.Parse("00000007-0000-0000-0000-000000000002");
        public static readonly Guid CHARACTERISTIC_CHALLENGE_RESPONSE_UNLOCKED = Guid.Parse("00000008-0000-0000-0000-000000000002");

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public static byte[] HexStringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

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
