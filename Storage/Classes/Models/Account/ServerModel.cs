using System.ComponentModel.DataAnnotations;
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
        public short port { get; set; }
        /// <summary>
        /// Defines how to handle the TLS connection.
        /// </summary>
        [Required]
        public TLSConnectionMode tlsMode { get; set; }
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
        public IgnoredCertificateErrorModel ignoredCertificateErrors { get; set; }


        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ServerModel()
        {
            port = 5222;
            tlsMode = TLSConnectionMode.FORCE;
            disableStreamManagement = false;
            disableMessageCarbons = false;
            ignoredCertificateErrors = new IgnoredCertificateErrorModel();
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
