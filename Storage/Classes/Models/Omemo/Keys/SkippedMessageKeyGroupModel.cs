using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Storage.Classes.Models.Omemo.Keys
{
    public class SkippedMessageKeyGroupModel: AbstractOmemoModel
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        [Key]
        public int id { get; set; }
        [Required]
        public byte[] dhr { get; set; }
        [Required]
        public List<SkippedMessageKeyModel> keys { get; set; } = new List<SkippedMessageKeyModel>();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public SkippedMessageKeyGroupModel() { }

        public SkippedMessageKeyGroupModel(byte[] dhr) { this.dhr = dhr; }

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
