using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Storage.Classes.Contexts;

namespace Storage.Classes.Models.Account
{
    public class MamRequestModel: AbstractModel
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
        /// The date and time of the last refresh.
        /// </summary>
        [Required]
        public DateTime lastUpdate
        {
            get => _lastUpdate;
            set => SetProperty(ref _lastUpdate, value);
        }
        [NotMapped]
        private DateTime _lastUpdate = DateTime.MinValue;

        /// <summary>
        /// The message ID of the last message or null in case no MAM message has been received before.
        /// </summary>
        public string lastMsgId
        {
            get => _lastMsgId;
            set => SetProperty(ref _lastMsgId, value);
        }
        [NotMapped]
        private string _lastMsgId;

        /// <summary>
        /// The message date of the last message or <see cref="DateTime.MaxValue"/> in case no MAM message has been received before.
        /// </summary>
        public DateTime lastMsgDate
        {
            get => _lastMsgDate;
            set => SetProperty(ref _lastMsgDate, value);
        }
        [NotMapped]
        private DateTime _lastMsgDate = DateTime.MaxValue;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public MamRequestModel()
        {
            lastUpdate = DateTime.MinValue;
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
