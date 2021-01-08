namespace Omemo.Classes.Keys
{
    public class ECKeyPair
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly ECPrivKey privKey;
        public readonly ECPubKey pubKey;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ECKeyPair(ECPrivKey privKey, ECPubKey pubKey)
        {
            this.privKey = privKey;
            this.pubKey = pubKey;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public override bool Equals(object obj)
        {
            return obj is ECKeyPair pair && ((pair.privKey is null && privKey is null) || pair.privKey.Equals(privKey)) && ((pair.pubKey is null && pubKey is null) || pair.pubKey.Equals(pubKey));
        }

        public override int GetHashCode()
        {
            int hash = 0;
            if (!(privKey is null))
            {
                hash = privKey.GetHashCode();
            }

            if (!(privKey is null))
            {
                hash ^= pubKey.GetHashCode();
            }

            return hash;
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
