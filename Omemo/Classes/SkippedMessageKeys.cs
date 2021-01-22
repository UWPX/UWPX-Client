using System;
using System.Collections.Generic;
using System.Linq;
using Omemo.Classes.Keys;

namespace Omemo.Classes
{
    public class SkippedMessageKeys
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly List<Tuple<ECPubKey, Dictionary<uint, byte[]>>> MKS = new List<Tuple<ECPubKey, Dictionary<uint, byte[]>>>();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        /// <summary>
        /// Adds the given message key (<paramref name="mk"/>) to the stored message keys.
        /// </summary>
        public void SetMessageKey(ECPubKey dhr, uint nr, byte[] mk)
        {
            Tuple<ECPubKey, Dictionary<uint, byte[]>> mks = MKS.Where(x => x.Item1.Equals(dhr)).FirstOrDefault();
            if (mks is null)
            {
                MKS.Add(new Tuple<ECPubKey, Dictionary<uint, byte[]>>(dhr, new Dictionary<uint, byte[]> { { nr, mk } }));
            }
            else
            {
                mks.Item2[nr] = mk;
            }
        }

        /// <summary>
        /// Tries to find the requested message key. If found it will be returned and removed.
        /// </summary>
        public byte[] GetMessagekey(ECPubKey dhr, uint nr)
        {
            Tuple<ECPubKey, Dictionary<uint, byte[]>> mks = MKS.Where(x => x.Item1.Equals(dhr)).FirstOrDefault();
            if (mks is null)
            {
                return null;
            }
            else if (mks.Item2.ContainsKey(nr))
            {
                byte[] mk = mks.Item2[nr];
                mks.Item2.Remove(nr);
                if (mks.Item2.Count <= 0)
                {
                    MKS.Remove(mks);
                }
                return mk;
            }
            return null;
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
