using System.ComponentModel.DataAnnotations;

namespace Storage.Classes.Models.Account
{
    public class JidModel
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        [Key]
        public int id { get; set; }
        /// <summary>
        /// The user part of a bare or full Jabber ID e.g. 'coven' in 'coven@chat.shakespeare.lit'.
        /// </summary>
        [Required]
        public string userPart { get; set; }
        /// <summary>
        /// The domain part of a bare or full Jabber ID e.g. 'chat.shakespeare.lit' in 'coven@chat.shakespeare.lit'.
        /// </summary>
        [Required]
        public string domainPart { get; set; }
        /// <summary>
        /// The resource part of a full Jabber ID e.g. 'phone' in 'coven@chat.shakespeare.lit/phone'.
        /// </summary>
        [Required]
        public string resourcePart { get; set; }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public static JidModel FromString(string bareJid)
        {
            return new JidModel();
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
