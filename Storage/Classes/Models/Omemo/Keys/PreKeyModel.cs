using System.ComponentModel.DataAnnotations;
using Omemo.Classes.Keys;

namespace Storage.Classes.Models.Omemo.Keys
{
    public class PreKeyModel: AbstractOmemoModel
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        [Key]
        public int id { get; set; }
        [Required]
        public uint keyId { get; set; }
        public byte[] privKey { get; set; }
        public byte[] pubKey { get; set; }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public PreKeyModel() { }
        public PreKeyModel(PreKey key)
        {
            keyId = key.id;
            privKey = key.privKey.key;
            pubKey = key.pubKey.key;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public PreKey ToPreKey()
        {
            return new PreKey(new ECPrivKey(privKey), new ECPubKey(pubKey), keyId);
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
