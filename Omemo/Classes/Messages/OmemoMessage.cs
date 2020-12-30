using System;

namespace Omemo.Classes.Messages
{
    /// <summary>
    /// Message based on: https://xmpp.org/extensions/xep-0384.html#protobuf-schema
    /// </summary>
    public class OmemoMessage: IOmemoMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        /// <summary>
        /// Message number.
        /// </summary>
        public readonly uint N;
        /// <summary>
        /// Number of messages in the previous sending chain.
        /// </summary>
        public readonly uint PN;
        /// <summary>
        /// The sender public key.
        /// </summary>
        public readonly byte[] DH_PUB;
        public readonly byte[] CIPHER_TEXT;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public OmemoMessage(byte[] data)
        {
            N = (uint)BitConverter.ToInt32(data, 0);
            PN = (uint)BitConverter.ToInt32(data, 4);
            DH_PUB = new byte[1];
            Buffer.BlockCopy(data, 8, DH_PUB, 0, DH_PUB.Length);
            int cipherTextLenth = data.Length - (8 + DH_PUB.Length);

            // Cipher text here is optional:
            if (cipherTextLenth > 0)
            {
                CIPHER_TEXT = new byte[data.Length - (8 + DH_PUB.Length)];
                Buffer.BlockCopy(data, 8 + DH_PUB.Length, CIPHER_TEXT, 0, CIPHER_TEXT.Length);
            }
            else
            {
                CIPHER_TEXT = new byte[0];
            }
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public byte[] ToByteArray()
        {
            int size = 4 + 4 + DH_PUB.Length;
            if (!(CIPHER_TEXT is null))
            {
                size += CIPHER_TEXT.Length;
            }
            byte[] result = new byte[size];
            Buffer.BlockCopy(BitConverter.GetBytes(N), 0, result, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(PN), 0, result, 4, 4);
            Buffer.BlockCopy(DH_PUB, 0, result, 8, DH_PUB.Length);
            if (!(CIPHER_TEXT is null))
            {
                Buffer.BlockCopy(CIPHER_TEXT, 0, result, 8 + DH_PUB.Length, CIPHER_TEXT.Length);
            }
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
