using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Windows.UI;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0392
{
    public static class ConsistentColorGenerator
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--


        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public static Color GenColor(string s)
        {
            // Angle generation:
            double hueAngle = GenHueAngle(s);

            // Corrections for Color Vision Deficiencies:
            hueAngle = DoColorBlindCorrection(hueAngle);

            IList<double> result = HsluvConverter.HsluvToRgb(new List<double> { hueAngle, 100.0, 50.0 });
            return Color.FromArgb(byte.MaxValue, (byte)(byte.MaxValue * result[0]), (byte)(byte.MaxValue * result[1]), (byte)(byte.MaxValue * result[2]));
        }

        public static Color GenColor(byte[] data)
        {
            // Angle generation:
            double hueAngle = GenHueAngle(data);

            // Corrections for Color Vision Deficiencies:
            hueAngle = DoColorBlindCorrection(hueAngle);

            IList<double> result = HsluvConverter.HsluvToRgb(new List<double> { hueAngle, 100.0, 50.0 });
            return Color.FromArgb(byte.MaxValue, (byte)(byte.MaxValue * result[0]), (byte)(byte.MaxValue * result[1]), (byte)(byte.MaxValue * result[2]));
        }

        #endregion

        #region --Misc Methods (Private)--
        private static double GenHueAngle(string s)
        {
            return GenHueAngle(Encoding.UTF8.GetBytes(s));
        }

        private static double GenHueAngle(byte[] data)
        {
            SHA1 sha1 = SHA1.Create();
            byte[] hash = sha1.ComputeHash(data);

            if (!BitConverter.IsLittleEndian)
            {
                hash.Reverse();
            }

            return (BitConverter.ToInt16(hash, 0) / 65536.0) * 360.0;
        }

        private static double DoColorBlindCorrection(double angle)
        {
            // Correct Red/Green-blindness:
            angle = (angle + 90) % 180 - 90;
            angle %= 360;

            // Correct Blue-blindness:
            angle %= 180;

            return angle;
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
