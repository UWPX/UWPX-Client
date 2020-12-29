using System.ComponentModel.DataAnnotations;
using Storage.Classes.Models.Account;

namespace Storage.Classes.Models.Omemo
{
    /// <summary>
    /// A pre key for XEP-0384 (OMEMO Encryption).
    /// </summary>
    public class OmemoPreKey: AbstractAccountModel
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        [Key]
        public string id { get; set; }
        /// <summary>
        /// The OMEMO pre key ID.
        /// </summary>
        [Required]
        public uint keyId { get; set; }
        /// <summary>
        /// The actual pre key.
        /// </summary>
        [Required]
        public byte[] key { get; set; }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


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
