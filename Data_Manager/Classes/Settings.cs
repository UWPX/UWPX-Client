using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Manager.Classes
{
    public class Settings
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--


        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Construktoren--


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
            object obj = getSetting(token);
            return obj != null && obj is bool && (bool)obj;
        }

        public static string getSettingString(string token)
        {
            object obj = getSetting(token);
            return obj == null ? null : (string)obj;
        }

        public static byte getSettingByte(string token)
        {
            object obj = getSetting(token);
            return obj == null ? (byte)0 : (byte)obj;
        }

        public static int getSettingInt(string token)
        {
            object obj = getSetting(token);
            return obj == null ? -1 : (int)obj;
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
