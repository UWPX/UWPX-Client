using Org.BouncyCastle.Math.EC.Rfc8032;

namespace Omemo.Classes.Keys
{
    public class PreKey: ECKeyPair
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly uint id;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public PreKey(ECPrivKey privKey, ECPubKey pubKey, uint id) : base(privKey, pubKey)
        {
            this.id = id;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        /// <summary>
        /// Signes the public key with the private key and the returns the signature of it.
        /// </summary>
        public byte[] GenerateSignature()
        {
            byte[] sig = new byte[Ed25519.SignatureSize];
            Ed25519.Sign(privKey.key, 0, pubKey.key, 0, (int)Consts.ED25519_KEY_SIZE_IN_BYTES, sig, 0);
            return sig;
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
