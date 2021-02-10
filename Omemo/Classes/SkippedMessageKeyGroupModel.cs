using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Omemo.Classes.Keys;

namespace Omemo.Classes
{
    public class SkippedMessageKeyGroupModel
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [Required]
        public ECPubKeyModel dh { get; set; }
        [Required]
        public HashSet<SkippedMessageKeyModel> messageKeys { get; set; } = new HashSet<SkippedMessageKeyModel>();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public SkippedMessageKeyGroupModel() { }

        public SkippedMessageKeyGroupModel(ECPubKeyModel dh)
        {
            this.dh = dh;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public SkippedMessageKeyModel RemoveKey(uint nr)
        {
            SkippedMessageKeyModel skippedMessageKey = GetKey(nr);
            if (!(skippedMessageKey is null))
            {
                messageKeys.Remove(skippedMessageKey);
            }
            return skippedMessageKey;
        }

        public SkippedMessageKeyModel GetKey(uint nr)
        {
            return messageKeys.Where(k => k.nr == nr).FirstOrDefault();
        }

        public void SetKey(uint nr, byte[] mk)
        {
            SkippedMessageKeyModel skippedMessageKey = GetKey(nr);
            if (skippedMessageKey is null)
            {
                messageKeys.Add(new SkippedMessageKeyModel(nr, mk));
            }
            else
            {
                skippedMessageKey.mk = mk;
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
