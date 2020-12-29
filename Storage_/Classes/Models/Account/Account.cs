using System.ComponentModel.DataAnnotations;
using Storage.Classes.Models.Omemo;
using XMPP_API.Classes;

namespace Storage.Classes.Models.Account
{
    public class Account
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        /// <summary>
        /// The unique Jabber ID of the account: user@domain e.g. 'coven@chat.shakespeare.lit'
        /// </summary>
        [Key]
        public string id { get; set; }
        /// <summary>
        /// The full Jabber ID of the account e.g. 'coven@chat.shakespeare.lit/phone'
        /// </summary>
        [Required]
        public Jid jid { get; set; }
        /// <summary>
        /// The complete server configuration for the account.
        /// </summary>
        [Required]
        public Server server { get; set; }
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
        public OmemoAccountInformation omemoInfo { get; set; }

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
