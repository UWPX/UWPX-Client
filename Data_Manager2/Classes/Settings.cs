using Microsoft.Toolkit.Uwp.Helpers;

namespace Data_Manager2.Classes
{
    public static class Settings
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public static readonly LocalObjectStorageHelper LOCAL_OBJECT_STORAGE_HELPER = new LocalObjectStorageHelper();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        /// <summary>
        /// Sets a setting with the given token.
        /// </summary>
        /// <param name="token">The token for saving.</param>
        /// <param name="value">The value that should get stored.</param>
        public static void setSetting(string token, object value)
        {
            Windows.Storage.ApplicationData.Current.LocalSettings.Values[token] = value;
        }

        /// <summary>
        /// Returns the setting behind the given token.
        /// </summary>
        /// <param name="token">The token for the setting.</param>
        /// <returns>Returns the setting behind the given token.</returns>
        public static object getSetting(string token)
        {
            try
            {
                return Windows.Storage.ApplicationData.Current.LocalSettings.Values[token];
            }
            catch
            {
                return null;
            }
        }

        public static bool getSettingBoolean(string token)
        {
            return getSettingBoolean(token, false);
        }

        public static bool getSettingBoolean(string token, bool fallBackValue)
        {
            object obj = getSetting(token);
            return obj is null ? fallBackValue : (bool)obj;
        }

        public static string getSettingString(string token)
        {
            return getSettingString(token, null);
        }

        public static string getSettingString(string token, string fallBackValue)
        {
            object obj = getSetting(token);
            return obj is null ? fallBackValue : (string)obj;
        }

        public static byte getSettingByte(string token)
        {
            return getSettingByte(token, 0);
        }

        public static byte getSettingByte(string token, byte fallBackValue)
        {
            object obj = getSetting(token);
            return obj is null ? fallBackValue : (byte)obj;
        }

        public static int getSettingInt(string token)
        {
            return getSettingInt(token, -1);
        }

        public static int getSettingInt(string token, int fallBackValue)
        {
            object obj = getSetting(token);
            return obj is null ? fallBackValue : (int)obj;
        }

        public static ushort getSettingUshort(string token)
        {
            return getSettingUshort(token, 0);
        }

        public static ushort getSettingUshort(string token, ushort fallBackValue)
        {
            object obj = getSetting(token);
            return obj is null ? fallBackValue : (ushort)obj;
        }

        public static double getSettingDouble(string token)
        {
            return getSettingDouble(token, -1);
        }

        public static double getSettingDouble(string token, double fallBackValue)
        {
            object obj = getSetting(token);
            return obj is null ? fallBackValue : (double)obj;
        }

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
