using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Omemo.Classes.Keys;

namespace Omemo.Classes
{
    public class SkippedMessageKeyGroup
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        [Key]
        public int id { get; set; }
        [Required]
        public ECPubKey dh { get; set; }
        [Required]
        public HashSet<SkippedMessageKey> messageKeys { get; set; } = new HashSet<SkippedMessageKey>();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public SkippedMessageKeyGroup() { }

        public SkippedMessageKeyGroup(ECPubKey dh)
        {
            this.dh = dh;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public SkippedMessageKey RemoveKey(uint nr)
        {
            SkippedMessageKey skippedMessageKey = GetKey(nr);
            if (!(skippedMessageKey is null))
            {
                messageKeys.Remove(skippedMessageKey);
            }
            return skippedMessageKey;
        }

        public SkippedMessageKey GetKey(uint nr)
        {
            return messageKeys.Where(k => k.nr == nr).FirstOrDefault();
        }

        public void SetKey(uint nr, byte[] mk)
        {
            SkippedMessageKey skippedMessageKey = GetKey(nr);
            if (skippedMessageKey is null)
            {
                messageKeys.Add(new SkippedMessageKey(nr, mk));
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
