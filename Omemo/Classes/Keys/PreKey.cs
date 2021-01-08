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
        public override bool Equals(object obj)
        {
            return obj is PreKey preKey && preKey.id == id && base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ (int)id;
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
