using System.ComponentModel.DataAnnotations;

namespace Omemo.Classes.Keys
{
    public class PreKey: ECKeyPair
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        [Required]
        public uint keyId { get; set; }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public PreKey(ECPrivKey privKey, ECPubKey pubKey, uint keyId) : base(privKey, pubKey)
        {
            this.keyId = keyId;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public override bool Equals(object obj)
        {
            return obj is PreKey preKey && preKey.keyId == keyId && base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ (int)keyId;
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
