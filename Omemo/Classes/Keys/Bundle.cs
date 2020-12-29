using System;
using System.Collections.Generic;

namespace Omemo.Classes.Keys
{
    public class Bundle
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        /// <summary>
        /// The public part of the identity key.
        /// </summary>
        public readonly ECPubKey IDENTITY_KEY;
        /// <summary>
        /// The public part of the signed PreKey.
        /// </summary>
        public readonly ECPubKey SIGNED_PRE_KEY;
        /// <summary>
        /// The id of the signed PreKey.
        /// </summary>
        public readonly uint SIGNED_PRE_KEY_ID;
        /// <summary>
        /// The signature of the signed PreKey.
        /// </summary>
        public readonly byte[] PRE_KEY_SIGNATURE;
        /// <summary>
        /// A collection of public parts of the <see cref="PreKey"/>s and their ID.
        /// </summary>
        public readonly List<Tuple<ECPubKey, uint>> PRE_KEYS;

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
