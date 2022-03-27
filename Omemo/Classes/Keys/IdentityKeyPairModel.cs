namespace Omemo.Classes.Keys
{
    /// <summary>
    /// Represents a Ed25519 key pair.
    /// </summary>
    public class IdentityKeyPairModel: ECKeyPairModel
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--


        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public IdentityKeyPairModel() { }

        public IdentityKeyPairModel(ECPrivKeyModel privKey, ECPubKeyModel pubKey) : base(privKey, pubKey) { }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        /// <summary>
        /// Creates a copy of the object, not including <see cref="id"/>.
        /// </summary>
        public IdentityKeyPairModel Clone()
        {
            return new IdentityKeyPairModel(privKey.Clone(), pubKey.Clone());
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
