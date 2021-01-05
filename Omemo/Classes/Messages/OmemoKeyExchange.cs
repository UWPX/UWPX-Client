using System;
using Omemo.Classes.Keys;

namespace Omemo.Classes.Messages
{
    /// <summary>
    /// Message based on: https://xmpp.org/extensions/xep-0384.html#protobuf-schema
    /// </summary>
    public class OmemoKeyExchange: IOmemoMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        /// <summary>
        /// PreKey id.
        /// </summary>
        public readonly uint pkId;
        /// <summary>
        /// Signed PreKey id.
        /// </summary>
        public readonly uint spkId;
        /// <summary>
        /// Public key part of the senders identity key.
        /// </summary>
        public readonly byte[] ik;
        /// <summary>
        /// Ephemeral key pair used by the X3DH key agreement.
        /// </summary>
        public readonly byte[] ek;
        public readonly OmemoAuthenticatedMessage message;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public OmemoKeyExchange(uint pkId, uint spkId, ECPubKey ik, ECPubKey ek, OmemoAuthenticatedMessage message)
        {
            this.pkId = pkId;
            this.spkId = spkId;
            this.ik = ik.key;
            this.ek = ek.key;
            this.message = message;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public byte[] ToByteArray()
        {
            byte[] msg = message.ToByteArray();
            byte[] result = new byte[4 + 4 + ik.Length + ek.Length + msg.Length];
            Buffer.BlockCopy(BitConverter.GetBytes(pkId), 0, result, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(spkId), 0, result, 4, 4);
            Buffer.BlockCopy(ik, 0, result, 8, ik.Length);
            Buffer.BlockCopy(ek, 0, result, 8 + ik.Length, ek.Length);
            Buffer.BlockCopy(msg, 0, result, 8 + ik.Length + ek.Length, msg.Length);
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
