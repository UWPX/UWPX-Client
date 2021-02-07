using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Windows.Security.Cryptography.Certificates;
using XMPP_API.Classes.Network.TCP;

namespace Storage.Classes.Models.Account
{
    public class ServerModel
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        [Key]
        public int id { get; set; }
        /// <summary>
        /// The address of the server e.g. 'xmpp.jabber.org'.
        /// </summary>
        [Required]
        public string address { get; set; }
        /// <summary>
        /// The server port e.g. '5222'.
        /// </summary>
        [Required]
        public ushort port { get; set; } = 5222;
        /// <summary>
        /// Defines how to handle the TLS connection.
        /// </summary>
        [Required]
        public TLSConnectionMode tlsMode { get; set; } = TLSConnectionMode.FORCE;
        /// <summary>
        /// True in case XEP-0198 (Stream Management) should be disabled.
        /// </summary>
        [Required]
        public bool disableStreamManagement { get; set; }
        /// <summary>
        /// True in case XEP-0280 (Message Carbons) should be disabled.
        /// </summary>
        [Required]
        public bool disableMessageCarbons { get; set; }
        /// <summary>
        /// A collection of certificate errors that should be ignored during connecting to a server.
        /// </summary>
        [Required]
        public List<ChainValidationResult> ignoredCertificateErrors { get; set; } = new List<ChainValidationResult>();


        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ServerModel() { }

        public ServerModel(string address)
        {
            this.address = address;
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
