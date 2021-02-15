using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Omemo.Classes.Keys
{
    public class SignedPreKeyModel
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [Required, ForeignKey(nameof(preKey) + "Id")]
        public PreKeyModel preKey { get; set; }
        [Required]
        public byte[] signature { get; set; }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public SignedPreKeyModel() { }

        public SignedPreKeyModel(PreKeyModel preKey, byte[] signature)
        {
            this.preKey = preKey;
            this.signature = signature;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public override bool Equals(object obj)
        {
            return obj is SignedPreKeyModel signedPreKey && signedPreKey.preKey.Equals(preKey) && signedPreKey.signature.SequenceEqual(signature);
        }

        public override int GetHashCode()
        {
            return preKey.GetHashCode() ^ signature.GetHashCode();
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
