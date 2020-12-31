using Chaos.NaCl;

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
        /// Signes the public key with the private key and the returns the signature.
        /// </summary>
        public byte[] GenerateSignature()
        {
            return Ed25519.Sign(pubKey.key, privKey.key);
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
