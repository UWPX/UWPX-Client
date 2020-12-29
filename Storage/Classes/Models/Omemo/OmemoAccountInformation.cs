using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Storage.Classes.Models.Account;

namespace Storage.Classes.Models.Omemo
{
    /// <summary>
    /// Information about the XEP-0384 (OMEMO Encryption) account status.
    /// </summary>
    public class OmemoAccountInformation: AbstractAccountModel
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
        public byte[] identityKeyPair { get; set; }
        /// <summary>
        /// A collection of signed pre keys.
        /// Only valid in case <see cref="keysGenerated"/> is true.
        /// </summary>
        [Required]
        public List<OmemoSignedPreKey> signedPreKeys { get; set; } = new List<OmemoSignedPreKey>();
        /// <summary>
        /// A collection of pre keys to publish.
        /// Only valid in case <see cref="keysGenerated"/> is true.
        /// </summary>
        [Required]
        public List<OmemoPreKey> preKeys { get; set; } = new List<OmemoPreKey>();
        /// <summary>
        /// A collection of OMEMO capable devices.
        /// </summary>
        [Required]
        public List<OmemoDevice> devices { get; set; } = new List<OmemoDevice>();
        /// <summary>
        /// The device list subscription states for this chat.
        /// </summary>
        public OmemoDeviceListSubscription deviceListSubscription { get; set; };

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
