using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Storage.Classes.Contexts;
using XMPP_API.Classes.Network.XML.Messages.XEP_0045;

namespace Storage.Classes.Models.Chat
{
    public class MucOccupantModel: AbstractModel
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
        /// The user nickname e.g. 'thirdwitch'.
        /// </summary>
        [Required]
        public string nickname
        {
            get => _nickname;
            set => SetProperty(ref _nickname, value);
        }
        [NotMapped]
        private string _nickname;

        /// <summary>
        /// The full jabber id of the user e.g. 'coven@chat.shakespeare.lit/thirdwitch'.
        /// </summary>
        public string fullJid
        {
            get => _fullJid;
            set => SetProperty(ref _fullJid, value);
        }
        [NotMapped]
        private string _fullJid;

        /// <summary>
        /// The affiliation of the user e.g. 'owner', 'admin', 'member' or 'none'.
        /// </summary>
        [Required]
        public MUCAffiliation affiliation
        {
            get => _affiliation;
            set => SetProperty(ref _affiliation, value);
        }
        [NotMapped]
        private MUCAffiliation _affiliation;

        /// <summary>
        /// The role of the user e.g. 'moderator', 'participant' or 'visitor'.
        /// </summary>
        [Required]
        public MUCRole role
        {
            get => _role;
            set => SetProperty(ref _role, value);
        }
        [NotMapped]
        private MUCRole _role;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public override void Remove(MainDbContext ctx, bool recursive)
        {
            ctx.Remove(this);
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
