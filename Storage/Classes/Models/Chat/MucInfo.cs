using System.ComponentModel.DataAnnotations;
using XMPP_API.Classes.Network.XML.Messages.XEP_0045;

namespace Storage.Classes.Models.Chat
{
    public class MucInfo
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        [Key]
        public int id { get; set; }
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

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


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
