using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XMPP_API.Classes.Network.XML.Messages.XEP_0249;

namespace Storage.Classes.Models.Chat
{
    public class MucDirectInvitationModel
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        /// <summary>
        /// The <see cref="ChatMessageModel"/> associated with this invitation.
        /// </summary>
        [Required]
        public int messageId { get; set; }
        /// <summary>
        /// A small comment about why somebody invited you.
        /// </summary>
        public string reason { get; set; }
        /// <summary>
        /// The room JID e.g. 'darkcave@macbeth.shakespeare.lit'.
        /// </summary>
        [Required]
        public string roomJid { get; set; }
        /// <summary>
        /// The optional room password.
        /// </summary>
        public string roomPassword { get; set; }
        /// <summary>
        /// The state of the invitation e.g. <see cref="MucDirectInvitationState.ACCEPTED"/> or <see cref="MucDirectInvitationState.DECLINED"/>.
        /// </summary>
        [Required]
        public MucDirectInvitationState state { get; set; }

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
