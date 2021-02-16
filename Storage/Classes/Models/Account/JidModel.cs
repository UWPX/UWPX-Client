using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shared.Classes;

namespace Storage.Classes.Models.Account
{
    public class JidModel: AbstractDataTemplate
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
        /// The local part of a bare or full Jabber ID e.g. 'coven' in 'coven@chat.shakespeare.lit'.
        /// </summary>
        [Required]
        public string localPart
        {
            get => _localPart;
            set => SetProperty(ref _localPart, value);
        }
        [NotMapped]
        private string _localPart;

        /// <summary>
        /// The domain part of a bare or full Jabber ID e.g. 'chat.shakespeare.lit' in 'coven@chat.shakespeare.lit'.
        /// </summary>
        [Required]
        public string domainPart
        {
            get => _domainPart;
            set => SetProperty(ref _domainPart, value);
        }
        [NotMapped]
        private string _domainPart;

        /// <summary>
        /// The resource part of a full Jabber ID e.g. 'phone' in 'coven@chat.shakespeare.lit/phone'.
        /// </summary>
        [Required]
        public string resourcePart
        {
            get => _resourcePart;
            set => SetProperty(ref _resourcePart, value);
        }
        [NotMapped]
        private string _resourcePart;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public string BareJid()
        {
            return localPart + '@' + domainPart;
        }

        public string FullJid()
        {
            return BareJid() + '/' + resourcePart;
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
