using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Storage.Classes.Contexts;
using XMPP_API.Classes.Network.XML.Messages.XEP_0249;

namespace Storage.Classes.Models.Chat
{
    public class MucDirectInvitationModel: AbstractModel
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
        /// The <see cref="ChatMessageModel"/> associated with this invitation.
        /// </summary>
        [Required]
        public int messageId
        {
            get => _messageId;
            set => SetProperty(ref _messageId, value);
        }
        [NotMapped]
        private int _messageId;

        /// <summary>
        /// A small comment about why somebody invited you.
        /// </summary>
        public string reason
        {
            get => _reason;
            set => SetProperty(ref _reason, value);
        }
        [NotMapped]
        private string _reason;

        /// <summary>
        /// The room JID e.g. 'darkcave@macbeth.shakespeare.lit'.
        /// </summary>
        [Required]
        public string roomJid
        {
            get => _roomJid;
            set => SetProperty(ref _roomJid, value);
        }
        [NotMapped]
        private string _roomJid;

        /// <summary>
        /// The optional room password.
        /// </summary>
        public string roomPassword
        {
            get => _roomPassword;
            set => SetProperty(ref _roomPassword, value);
        }
        [NotMapped]
        private string _roomPassword;

        /// <summary>
        /// The state of the invitation e.g. <see cref="MucDirectInvitationState.ACCEPTED"/> or <see cref="MucDirectInvitationState.DECLINED"/>.
        /// </summary>
        [Required]
        public MucDirectInvitationState state
        {
            get => _state;
            set => SetProperty(ref _state, value);
        }
        [NotMapped]
        private MucDirectInvitationState _state;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public MucDirectInvitationModel() { }

        public MucDirectInvitationModel(DirectMUCInvitationMessage invitationMessage, ChatMessageModel msg)
        {
            messageId = msg.id;
            reason = invitationMessage.REASON;
            roomJid = invitationMessage.ROOM_JID;
            roomPassword = invitationMessage.ROOM_PASSWORD;
            state = MucDirectInvitationState.REQUESTED;
        }

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
