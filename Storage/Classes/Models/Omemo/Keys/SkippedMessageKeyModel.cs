using System.ComponentModel.DataAnnotations;

namespace Storage.Classes.Models.Omemo.Keys
{
    public class SkippedMessageKeyModel: AbstractOmemoModel
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        [Key]
        public int id { get; set; }
        [Required]
        public uint nr { get; set; }
        public byte[] mk { get; set; }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public SkippedMessageKeyModel() { }

        public SkippedMessageKeyModel(uint nr, byte[] mk)
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
