using System;
using System.Diagnostics;
using System.Linq;
using Omemo.Classes.Exceptions;

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
        /// Truncated to 16 byte.
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
            Debug.Assert(hmac.Length == 16);
            HMAC = hmac;
            OMEMO_MESSAGE = omemoMessage;
        }

        public OmemoAuthenticatedMessage(byte[] data)
        {
            HMAC = new byte[16];
            OMEMO_MESSAGE = new byte[data.Length - HMAC.Length];
            Buffer.BlockCopy(data, 0, HMAC, 0, HMAC.Length);
            Buffer.BlockCopy(data, HMAC.Length, OMEMO_MESSAGE, 0, OMEMO_MESSAGE.Length);
            Validate();
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

        public void Validate()
        {
            if (OMEMO_MESSAGE is null || OMEMO_MESSAGE.Length < OmemoMessage.MIN_SIZE)
            {
                throw new OmemoException("Invalid " + nameof(OmemoAuthenticatedMessage) + " OMEMO_MESSAGE.Length < " + OmemoMessage.MIN_SIZE + ": " + OMEMO_MESSAGE?.Length);
            }
            if (HMAC is null || HMAC.Length != 16)
            {
                throw new OmemoException("Invalid " + nameof(OmemoAuthenticatedMessage) + " HMAC.Length != 16: " + HMAC?.Length);
            }
        }

        public override bool Equals(object obj)
        {
            return obj is OmemoAuthenticatedMessage msg && msg.HMAC.SequenceEqual(HMAC) && msg.OMEMO_MESSAGE.SequenceEqual(OMEMO_MESSAGE);
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
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
