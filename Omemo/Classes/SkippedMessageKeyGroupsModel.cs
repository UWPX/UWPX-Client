using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Omemo.Classes.Keys;

namespace Omemo.Classes
{
    public class SkippedMessageKeyGroupsModel
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        [Key]
        public int id { get; set; }
        public readonly List<SkippedMessageKeyGroupModel> MKS = new List<SkippedMessageKeyGroupModel>();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        /// <summary>
        /// Adds the given message key (<paramref name="mk"/>) to the stored message keys.
        /// </summary>
        public void SetMessageKey(ECPubKeyModel dhr, uint nr, byte[] mk)
        {
            SkippedMessageKeyGroupModel group = MKS.Where(g => g.dh.Equals(dhr)).FirstOrDefault();
            if (group is null)
            {
                group = new SkippedMessageKeyGroupModel(dhr);
                MKS.Add(group);
            }
            group.SetKey(nr, mk);
        }

        /// <summary>
        /// Tries to find the requested message key. If found it will be returned and removed.
        /// </summary>
        public byte[] GetMessagekey(ECPubKeyModel dhr, uint nr)
        {
            SkippedMessageKeyGroupModel group = MKS.Where(g => g.dh.Equals(dhr)).FirstOrDefault();
            return group?.RemoveKey(nr)?.mk;
        }

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
