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
        /// The sender public Ephemeral key.
        /// </summary>
        public readonly byte[] DH_PUB;
        public byte[] cipherText;

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
                cipherText = new byte[data.Length - (8 + DH_PUB.Length)];
                Buffer.BlockCopy(data, 8 + DH_PUB.Length, cipherText, 0, cipherText.Length);
            }
            else
            {
                cipherText = new byte[0];
            }
        }

        public OmemoMessage(uint n, uint pn, byte[] dhPub)
        {
            N = n;
            PN = pn;
            DH_PUB = dhPub;
            cipherText = null;
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
            if (!(cipherText is null))
            {
                size += cipherText.Length;
            }
            byte[] result = new byte[size];
            Buffer.BlockCopy(BitConverter.GetBytes(N), 0, result, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(PN), 0, result, 4, 4);
            Buffer.BlockCopy(DH_PUB, 0, result, 8, DH_PUB.Length);
            if (!(cipherText is null))
            {
                Buffer.BlockCopy(cipherText, 0, result, 8 + DH_PUB.Length, cipherText.Length);
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
