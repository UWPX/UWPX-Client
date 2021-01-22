using System.ComponentModel.DataAnnotations;
using Omemo.Classes.Keys;

namespace Storage.Classes.Models.Omemo.Keys
{
    public class SignedPreKeyModel: AbstractOmemoModel
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        [Key]
        public int id { get; set; }
        [Required]
        public uint keyId { get; set; }
        [Required]
        public byte[] signature { get; set; }
        public byte[] privKey { get; set; }
        public byte[] pubKey { get; set; }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public SignedPreKeyModel() { }
        public SignedPreKeyModel(SignedPreKey key)
        {
            keyId = key.preKey.id;
            signature = key.signature;
            privKey = key.preKey.privKey.key;
            pubKey = key.preKey.pubKey.key;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public SignedPreKey ToSignedPreKey()
        {
            return new SignedPreKey(new PreKey(new ECPrivKey(privKey), new ECPubKey(pubKey), keyId), signature);
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
