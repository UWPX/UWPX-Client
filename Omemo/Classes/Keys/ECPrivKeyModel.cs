namespace Omemo.Classes.Keys
{
    public class ECPrivKeyModel: ECKeyModel
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--


        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ECPrivKeyModel() { }

        public ECPrivKeyModel(byte[] pubKey) : base(pubKey) { }

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
