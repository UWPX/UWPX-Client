using SQLite;
using XMPP_API.Classes.Network.XML.Messages.XEP_0249;

namespace Data_Manager2.Classes.DBTables
{
    [Table(DBTableConsts.MUC_DIRECT_INVITATION_TABLE)]
    public class MUCDirectInvitationTable
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        [PrimaryKey]
        // The id of the chat message
        public string chatMessageId { get; set; }
        // A small comment
        public string reason { get; set; }
        [NotNull]
        // The room JID e.g. 'darkcave@macbeth.shakespeare.lit'
        public string roomJid { get; set; }
        // The password for the room
        public string roomPassword { get; set; }
        // The state of the invitation e.g. ACCEPTED or DECLINED
        public MUCDirectInvitationState state { get; set; }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 02/03/2018 Created [Fabian Sauter]
        /// </history>
        public MUCDirectInvitationTable()
        {

        }

        public MUCDirectInvitationTable(DirectMUCInvitationMessage invitationMessage, string chatMessageId)
        {
            this.chatMessageId = chatMessageId;
            this.reason = invitationMessage.REASON;
            this.roomJid = invitationMessage.ROOM_JID;
            this.roomPassword = invitationMessage.ROOM_PASSWORD;
            this.state = MUCDirectInvitationState.REQUESTED;
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
