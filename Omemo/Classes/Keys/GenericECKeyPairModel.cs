namespace Omemo.Classes.Keys
{
    /// <summary>
    /// Represents a generic EC key pair holding a <see cref="ECPubKeyModel"/> and <see cref="ECPrivKeyModel"/>.
    /// </summary>
    public class GenericECKeyPairModel: ECKeyPairModel
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--


        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public GenericECKeyPairModel() { }

        public GenericECKeyPairModel(ECPrivKeyModel privKey, ECPubKeyModel pubKey) : base(privKey, pubKey) { }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


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
