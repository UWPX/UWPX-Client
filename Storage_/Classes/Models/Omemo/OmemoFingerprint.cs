using System;
using System.ComponentModel.DataAnnotations;

namespace Storage.Classes.Models.Omemo
{
    public class OmemoFingerprint
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        [Key]
        public string id { get; set; }
        /// <summary>
        /// The bare JID e.g. 'coven@chat.shakespeare.lit' we received the update from.
        /// </summary>
        [Required]
        public OmemoDevice device { get; set; }
        [Required]
        public Jid bareJid { get; set; }
        [Required]
        public DateTime lastSeen { get; set; }
        [Required]
        public bool trusted { get; set; }
        [Required]
        public byte[] identityPubKey { get; set; }

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
