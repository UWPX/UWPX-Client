using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Storage.Classes.Models.Chat
{
    public class SpamMessageModel: AbstractModel
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

        [Required]
        public DateTime lastReceived
        {
            get => _lastReceived;
            set => SetProperty(ref _lastReceived, value);
        }
        [NotMapped]
        private DateTime _lastReceived;

        [Required]
        public uint count
        {
            get => _count;
            set => SetProperty(ref _count, value);
        }
        [NotMapped]
        private uint _count;

        [Required]
        public string text
        {
            get => _text;
            set => SetProperty(ref _text, value);
        }
        [NotMapped]
        private string _text;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public SpamMessageModel()
        {
            lastReceived = DateTime.MinValue;
        }

        public SpamMessageModel(string text) : this()
        {
            this.text = text;
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
