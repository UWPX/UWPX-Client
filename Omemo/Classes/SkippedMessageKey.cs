using System.ComponentModel.DataAnnotations;

namespace Omemo.Classes
{
    public class SkippedMessageKey
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        [Key]
        public int id { get; set; }
        [Required]
        public uint nr { get; set; }
        [Required]
        public byte[] mk { get; set; }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public SkippedMessageKey() { }
        public SkippedMessageKey(uint nr, byte[] mk)
        {
            this.nr = nr;
            this.mk = mk;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public override int GetHashCode()
        {
            return (int)nr;
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
