using System.Collections.Generic;
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
        public static Color GenForegroundColor(string s, bool redGreenCorrection, bool blueCorrection)
        {
            return GenForegroundColor(Encoding.UTF8.GetBytes(s), redGreenCorrection, blueCorrection);
        }

        public static Color GenForegroundColor(byte[] data, bool redGreenCorrection, bool blueCorrection)
        {
            // Angle generation:
            double hueAngle = GenHueAngle(data);

            // Corrections for Color Vision Deficiencies:
            hueAngle = DoColorBlindCorrection(hueAngle, redGreenCorrection, blueCorrection);

            IList<double> result = HsluvConverter.HsluvToRgb(new List<double> { hueAngle, 100.0, 50.0 });
            return Color.FromArgb(byte.MaxValue, (byte)(byte.MaxValue * result[0]), (byte)(byte.MaxValue * result[1]), (byte)(byte.MaxValue * result[2]));
        }

        #endregion

        #region --Misc Methods (Private)--
        /// <summary>
        /// Generates the hue angle based on the given data.
        /// </summary>
        /// <param name="data">The data for which the angle should be generated.</param>
        /// <returns>The color angle representing the given data.</returns>
        private static double GenHueAngle(byte[] data)
        {
            SHA1 sha1 = SHA1.Create();
            byte[] hash = sha1.ComputeHash(data);

            return ((hash[0] | hash[1] << 8) / 65536.0) * 360.0;
        }

        /// <summary>
        /// Performs the color correction for red/green an blue color blind people.
        /// Described in XEP-0392 (https://xmpp.org/extensions/xep-0392.html#algorithm-cvd)
        /// </summary>
        /// <param name="angle">The input hue angle.</param>
        /// <param name="redGreenCorrection">Should red/green correction be performed.</param>
        /// <param name="blueCorrection">Should blue correction be performed.</param>
        /// <returns>The resulting hue angle.</returns>
        private static double DoColorBlindCorrection(double angle, bool redGreenCorrection, bool blueCorrection)
        {
            // Correct Red/Green-blindness:
            if (redGreenCorrection)
            {
                angle = (angle + 90) % 180 - 90;
                angle %= 360;
            }

            // Correct Blue-blindness:
            if (blueCorrection)
            {
                angle %= 180;
            }

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
