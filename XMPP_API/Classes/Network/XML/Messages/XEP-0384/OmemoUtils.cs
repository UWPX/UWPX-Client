using System;
using curve25519;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0384
{
    public class OmemoUtils
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private static readonly Random R = new Random();
        private static readonly CSharpCurve25519Provider CURVE_25519 = new CSharpCurve25519Provider();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 04/08/2018 Created [Fabian Sauter]
        /// </history>
        private OmemoUtils()
        {
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public static byte[] genPubKey(byte[] privKey)
        {
            return CURVE_25519.generatePublicKey(privKey);
        }

        public static byte[] genPrivKey()
        {
            return CURVE_25519.generatePrivateKey();
        }

        public static Int32 genDeviceId()
        {
            byte[] buf = new byte[4];
            R.NextBytes(buf);
            return BitConverter.ToInt32(buf, 0);
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
