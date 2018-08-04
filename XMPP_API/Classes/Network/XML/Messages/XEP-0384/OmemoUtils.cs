using System;
using Windows.Security.Cryptography.Core;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0384
{
    public class OmemoUtils
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private static readonly Random R = new Random();

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
        public CryptographicKey genKeyPair()
        {
            AsymmetricKeyAlgorithmProvider provider = AsymmetricKeyAlgorithmProvider.OpenAlgorithm(AsymmetricAlgorithmNames.DsaSha256);
            return provider.CreateKeyPairWithCurveName(EccCurveNames.Curve25519);
        }

        public Int32 genClientId()
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
