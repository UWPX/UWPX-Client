using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Storage.Classes.Models.Omemo;
using XMPP_API.Classes;
using XMPP_API.Classes.Network;

namespace Storage.Classes.Models.Account
{
    public class AccountModel
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        /// <summary>
        /// The unique bare Jabber ID of the account: user@domain e.g. 'coven@chat.shakespeare.lit'
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)] // Do not automatically let the DB generate this ID
        public string bareJid { get; set; }
        /// <summary>
        /// The full Jabber ID of the account e.g. 'coven@chat.shakespeare.lit/phone'
        /// </summary>
        [Required]
        public JidModel fullJid { get; set; }
        /// <summary>
        /// The complete server configuration for the account.
        /// </summary>
        [Required]
        public ServerModel server { get; set; }
        /// <summary>
        /// The presence priority within range -127 to 128 e.g. 0.
        /// </summary>
        [Required]
        public short presencePriorety { get; set; }
        /// <summary>
        /// Has the account been disabled.
        /// Required for auto connecting accounts.
        /// </summary>
        [Required]
        public bool disabled { get; set; }
        /// <summary>
        /// Hex representation of the account color e.g. '#E91E63'.
        /// </summary>
        [Required]
        public string color { get; set; }
        /// <summary>
        /// The current account presence e.g. 'online'.
        /// </summary>
        [Required]
        public Presence presence { get; set; }
        /// <summary>
        /// The optional account status message e.g. 'My status message!'.
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// Information about the XEP-0384 (OMEMO Encryption) account status.
        /// </summary>
        [Required]
        public OmemoAccountInformationModel omemoInfo { get; set; } = new OmemoAccountInformationModel();
        /// <summary>
        /// The status of the last time a MAM request happened.
        /// </summary>
        [Required]
        public MamRequestModel mamRequest { get; set; }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public XMPPAccount ToXMPPAccount()
        {
            XMPPAccount account = new XMPPAccount(new XMPPUser(fullJid.userPart, fullJid.domainPart, fullJid.resourcePart))
            {
                serverAddress = server.address,
                port = server.port,
                color = color,
                presencePriorety = presencePriorety,
                disabled = disabled,
                presence = presence,
                status = status,
                omemoDeviceId = omemoInfo.deviceId,
                omemoIdentityKey = omemoInfo.identityKey,
                omemoBundleInfoAnnounced = omemoInfo.bundleInfoAnnounced,
                omemoSignedPreKey = omemoInfo.signedPreKey,
                omemoDeviceLabel = omemoInfo.deviceLabel,
            };
            account.OMEMO_PRE_KEYS.AddRange(omemoInfo.preKeys);
            return account;
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
