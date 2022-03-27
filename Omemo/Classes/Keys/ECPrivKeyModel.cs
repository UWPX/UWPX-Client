using Omemo.Classes.Exceptions;

namespace Omemo.Classes.Keys
{
    public class ECPrivKeyModel: ECKeyModel
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--


        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// DO NOT USE! Only required for the DB. Use the other constructor instead.
        /// </summary>
        public ECPrivKeyModel() { }

        public ECPrivKeyModel(byte[] privKey) : base(privKey)
        {
            if (privKey.Length == KeyHelper.PUB_KEY_SIZE)
            {
                return;
            }

            throw new OmemoKeyException($"Invalid key length {privKey.Length} - expected {KeyHelper.PRIV_KEY_SIZE}.");
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        /// <summary>
        /// Creates a copy of the object, not including <see cref="id"/>.
        /// </summary>
        public new ECPrivKeyModel Clone()
        {
            return new ECPrivKeyModel(key);
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
