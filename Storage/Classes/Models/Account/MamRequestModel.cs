using System;
using System.ComponentModel.DataAnnotations;

namespace Storage.Classes.Models.Account
{
    public class MamRequestModel
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        [Key]
        public int id { get; set; }
        /// <summary>
        /// The date and time of the last refresh.
        /// </summary>
        [Required]
        public DateTime lastUpdate { get; set; } = DateTime.MinValue;
        /// <summary>
        /// The message ID of the last message or null in case the request has never been run before.
        /// </summary>
        public string lastMsgId { get; set; }

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
