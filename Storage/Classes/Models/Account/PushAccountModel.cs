using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Storage.Classes.Models.Account
{
    public class PushAccountModel: AbstractModel
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }
        [NotMapped]
        private int _id;

        /// <summary>
        /// The name of the node, where our server should publish to.
        /// </summary>
        public string node
        {
            get => _node;
            set => SetProperty(ref _node, value);
        }
        [NotMapped]
        private string _node;

        /// <summary>
        /// The secret for publishing.
        /// </summary>
        public string secret
        {
            get => _secret;
            set => SetProperty(ref _secret, value);
        }
        [NotMapped]
        private string _secret;

        /// <summary>
        /// The bare Jabber ID e.g. 'pushServer@chat.shakespeare.lit' of the push server, where our server should publish updates for us to.
        /// </summary>
        public string bareJid
        {
            get => _bareJid;
            set => SetProperty(ref _bareJid, value);
        }
        [NotMapped]
        private string _bareJid;

        /// <summary>
        /// True in case this configuration is valid and push should be enabled.
        /// </summary>
        [Required]
        public bool enabled
        {
            get => _enabled;
            set => SetProperty(ref _enabled, value);
        }
        [NotMapped]
        private bool _enabled;

        /// <summary>
        /// True in case enabling/disabling was successful.
        /// </summary>
        [Required]
        public bool published
        {
            get => _published;
            set => SetProperty(ref _published, value);
        }
        [NotMapped]
        private bool _published;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public PushAccountModel()
        {
            published = true;
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
