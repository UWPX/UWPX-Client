using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Storage.Classes.Contexts;
using XMPP_API.Classes;
using XMPP_API.Classes.Network.XML.Messages.XEP_0045;
using XMPP_API.Classes.Network.XML.Messages.XEP_0048;

namespace Storage.Classes.Models.Chat
{
    public class MucInfoModel
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        [Key]
        public int id { get; set; }
        /// <summary>
        /// The <see cref="ChatModel"/> associated with this MUC.
        /// </summary>
        [Required]
        public ChatModel chat { get; set; }
        /// <summary>
        /// The users nickname for the chat.
        /// </summary>
        [Required]
        public string nickname { get; set; }
        /// <summary>
        /// The current state of the MUC e.g. 'ENTERING'.
        /// </summary>
        [Required]
        public MucState state { get; set; }
        /// <summary>
        /// True in case we should automatically enter the room as soon as the client is connected.
        /// </summary>
        [Required]
        public bool autoEnterRoom { get; set; }
        /// <summary>
        /// The users affiliation to the room.
        /// </summary>
        [Required]
        public MUCAffiliation affiliation { get; set; }
        /// <summary>
        /// The users role to the room.
        /// </summary>
        [Required]
        public MUCRole role { get; set; }
        /// <summary>
        /// An optional friendly name for the room.
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// The current room subject.
        /// </summary>
        public string subject { get; set; }
        /// <summary>
        /// The room password.
        /// </summary>
        public string password { get; set; }
        /// <summary>
        /// A list of all currently known MUC occupants.
        /// </summary>
        [Required]
        public List<MucOccupantModel> occupants { get; set; } = new List<MucOccupantModel>();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public Presence GetMucPresence()
        {
            switch (state)
            {
                case MucState.ENTERING:
                case MucState.DISCONNECTING:
                    return Presence.Chat;

                case MucState.ENTERD:
                    return Presence.Online;

                case MucState.ERROR:
                case MucState.KICKED:
                case MucState.BANED:
                    return Presence.Xa;

                case MucState.DISCONNECTED:
                default:
                    return Presence.Unavailable;
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public ConferenceItem ToConferenceItem()
        {
            return new ConferenceItem
            {
                autoJoin = autoEnterRoom,
                name = name,
                nick = nickname,
                password = password,
                jid = chat.bareJid
            };
        }

        public void Save()
        {
            using (MainDbContext ctx = new MainDbContext())
            {
                ctx.Update(this);
            }
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
