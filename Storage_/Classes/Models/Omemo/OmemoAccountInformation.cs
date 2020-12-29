using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Storage.Classes.Models.Omemo
{
    /// <summary>
    /// Information about the XEP-0384 (OMEMO Encryption) account status.
    /// </summary>
    public class OmemoAccountInformation
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        [Key]
        public string id { get; set; }
        /// <summary>
        /// Did we already successfully generate all XEP-0384 (OMEMO Encryption) keys?
        /// </summary>
        [Required]
        public bool keysGenerated { get; set; }
        /// <summary>
        /// The local unique device id.
        /// </summary>
        [Required]
        public uint deviceId { get; set; }
        /// <summary>
        /// Did we announce our bundle information?
        /// </summary>
        [Required]
        public bool bundleInfoAnnounced { get; set; }
        /// <summary>
        /// A list of pre keys for the current OMEMO bundle.
        /// Only valid in case <see cref="keysGenerated"/> is true.
        /// </summary>
        /// <summary>
        /// The private key pair.
        /// </summary>
        public byte[] omemoIdentityKeyPair { get; set; }
        /// <summary>
        /// The signed pre key.
        /// Only valid in case <see cref="keysGenerated"/> is true.
        /// </summary>
        public OmemoSignedPreKey signedPreKey { get; set; }
        /// <summary>
        /// A collection of pre keys to publish.
        /// Only valid in case <see cref="keysGenerated"/> is true.
        /// </summary>
        [Required]
        public List<OmemoPreKey> preKeys { get; set; }
        /// <summary>
        /// A collection of OMEMO capable devices.
        /// </summary>
        [Required]
        public List<OmemoDevice> devices { get; set; }

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
