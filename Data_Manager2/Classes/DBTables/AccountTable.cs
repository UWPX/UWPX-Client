using SQLite;
using XMPP_API.Classes.Network;
using XMPP_API.Classes;

namespace Data_Manager2.Classes.DBTables
{
    [Table(DBTableConsts.ACCOUNT_TABLE)]
    public class AccountTable
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        [PrimaryKey]
        // An unique id: jabberId @ domain e.g. 'coven@chat.shakespeare.lit'
        public string id { get; set; }
        [NotNull]
        // The account user id e.g. 'coven'
        public string userId { get; set; }
        [NotNull]
        // XMPP domain e.g. 'xmpp.shakespeare.lit'
        public string domain { get; set; }
        [NotNull]
        // The device/resource name e.g. 'phone' or 'W10PC'
        public string resource { get; set; }
        [NotNull]
        // The XMPP server address e.g. xmpp.jabber.org
        public string serverAddress { get; set; }
        // Server port e.g. 5222
        public int port { get; set; }
        // The presence priority with range -127 to 128 e.g. 0
        public short presencePriorety { get; set; }
        // Auto connect?
        public bool disabled { get; set; }
        // A color in hex format e.g. '#E91E63'
        public string color { get; set; }
        // The current presence for the account e.g. 'online'
        public Presence presence { get; set; }
        // The current status message for account e.g. 'My status'
        public string status { get; set; }
        // The private key for XEP-0384 (OMEMO Encryption)
        public byte[] omemoIdentityKeyPair { get; set; }
        // The id of the omemoSignedPreKey for XEP-0384 (OMEMO Encryption)
        public uint omemoSignedPreKeyId { get; set; }
        // The device id for XEP-0384 (OMEMO Encryption)
        public uint omemoDeviceId { get; set; }
        public bool omemoBundleInfoAnnounced { get; set; }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 17/11/2017 Created [Fabian Sauter]
        /// </history>
        public AccountTable()
        {

        }

        public AccountTable(XMPPAccount account)
        {
            this.id = account.getIdAndDomain();
            this.userId = account.user.userId;
            this.domain = account.user.domain;
            this.resource = account.user.resource;
            this.serverAddress = account.serverAddress;
            this.port = account.port;
            this.disabled = account.disabled;
            this.color = account.color;
            this.presence = account.presence;
            this.status = account.status;
            this.omemoDeviceId = account.omemoDeviceId;
            this.omemoIdentityKeyPair = account.omemoIdentityKeyPair?.serialize();
            this.omemoBundleInfoAnnounced = account.omemoBundleInfoAnnounced;
            this.omemoSignedPreKeyId = account.omemoSignedPreKeyId;
        }

        internal XMPPAccount toXMPPAccount()
        {
            return new XMPPAccount(new XMPPUser(userId, domain, resource), serverAddress, port)
            {
                color = color,
                presencePriorety = presencePriorety,
                disabled = disabled,
                presence = presence,
                status = status,
                omemoDeviceId = omemoDeviceId,
                omemoIdentityKeyPair = omemoIdentityKeyPair == null ? null : new libsignal.IdentityKeyPair(omemoIdentityKeyPair),
                omemoBundleInfoAnnounced = omemoBundleInfoAnnounced,
                omemoSignedPreKeyId = omemoSignedPreKeyId
            };
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
