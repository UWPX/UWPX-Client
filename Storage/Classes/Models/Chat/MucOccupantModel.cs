using System.ComponentModel.DataAnnotations;
using XMPP_API.Classes.Network.XML.Messages.XEP_0045;

namespace Storage.Classes.Models.Chat
{
    public class MucOccupantModel
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        [Key]
        public int id { get; set; }
        /// <summary>
        /// The user nickname e.g. 'thirdwitch'.
        /// </summary>
        [Required]
        public string nickname { get; set; }
        /// <summary>
        /// The full jabber id of the user e.g. 'coven@chat.shakespeare.lit/thirdwitch'.
        /// </summary>
        public string fullJid { get; set; }
        /// <summary>
        /// The affiliation of the user e.g. 'owner', 'admin', 'member' or 'none'.
        /// </summary>
        [Required]
        public MUCAffiliation affiliation { get; set; }
        /// <summary>
        /// The role of the user e.g. 'moderator', 'participant' or 'visitor'.
        /// </summary>
        [Required]
        public MUCRole role { get; set; }

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
