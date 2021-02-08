using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;

namespace Omemo.Classes.Keys
{
    public class ECKeyModel
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        [Key]
        public int id { get; set; }
        [Required]
        public byte[] key { get; set; }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ECKeyModel() { }

        public ECKeyModel(byte[] key)
        {
            Debug.Assert(!(key is null));
            this.key = key;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public override bool Equals(object obj)
        {
            return obj is ECKeyModel ecKey && ecKey.key.SequenceEqual(key);
        }

        public override int GetHashCode()
        {
            return key.GetHashCode();
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
