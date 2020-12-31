﻿using System;

namespace Omemo.Classes.Messages
{
    /// <summary>
    /// Message based on: https://xmpp.org/extensions/xep-0384.html#protobuf-schema
    /// </summary>
    public class OmemoAuthenticatedMessage: IOmemoMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        /// <summary>
        /// The HMAC of the <see cref="OMEMO_MESSAGE"/>.
        /// </summary>
        public readonly byte[] HMAC;
        /// <summary>
        /// A byte-representation of an <see cref="OmemoMessage"/>
        /// </summary>
        public readonly byte[] OMEMO_MESSAGE;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public OmemoAuthenticatedMessage(byte[] hmac, byte[] omemoMessage)
        {
            HMAC = hmac;
            OMEMO_MESSAGE = omemoMessage;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public byte[] ToByteArray()
        {
            byte[] result = new byte[HMAC.Length + OMEMO_MESSAGE.Length];
            Buffer.BlockCopy(HMAC, 0, result, 0, HMAC.Length);
            Buffer.BlockCopy(OMEMO_MESSAGE, 0, result, HMAC.Length, OMEMO_MESSAGE.Length);
            return result;
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
