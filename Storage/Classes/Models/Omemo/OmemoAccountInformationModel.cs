using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Omemo.Classes;
using Omemo.Classes.Keys;

namespace Storage.Classes.Models.Omemo
{
    /// <summary>
    /// Information about the XEP-0384 (OMEMO Encryption) account status.
    /// </summary>
    public class OmemoAccountInformationModel
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        [Key]
        public int id { get; set; }
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
        /// The local name for this device.
        /// </summary>
        public string deviceLabel { get; set; }
        /// <summary>
        /// Did we announce our bundle information?
        /// </summary>
        [Required]
        public bool bundleInfoAnnounced { get; set; }
        /// <summary>
        /// The identity key pair.
        /// Only valid in case <see cref="keysGenerated"/> is true.
        /// </summary>
        /// <summary>
        /// The private key pair.
        /// </summary>
        [Required]
        public IdentityKeyPairModel identityKey { get; set; }
        /// <summary>
        /// The signed PreKey.
        /// Only valid in case <see cref="keysGenerated"/> is true.
        /// </summary>
        [Required]
        public SignedPreKeyModel signedPreKey { get; set; }
        /// <summary>
        /// A collection of PreKeys to publish.
        /// Only valid in case <see cref="keysGenerated"/> is true.
        /// </summary>
        [Required]
        public List<PreKeyModel> preKeys { get; set; } = new List<PreKeyModel>();
        /// <summary>
        /// A collection of OMEMO capable devices.
        /// </summary>
        [Required]
        public List<OmemoDeviceModel> devices { get; set; } = new List<OmemoDeviceModel>();
        /// <summary>
        /// The device list subscription states for this chat.
        /// </summary>
        [Required]
        public OmemoDeviceListSubscriptionModel deviceListSubscription { get; set; }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        /// <summary>
        /// Generates all OMEMO keys and sets <see cref="keysGenerated"/> to true, as well as <see cref="bundleInfoAnnounced"/> to false.
        /// Should only be called once, when a new account has been added to prevent overlapping PreKeys.
        /// </summary>
        public void GenerateOmemoKeys()
        {
            Debug.Assert(!keysGenerated);
            deviceId = 0;
            bundleInfoAnnounced = false;
            identityKey = KeyHelper.GenerateIdentityKeyPair();
            signedPreKey = KeyHelper.GenerateSignedPreKey(0, identityKey.privKey);
            preKeys = KeyHelper.GeneratePreKeys(0, 100);
            keysGenerated = true;
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
