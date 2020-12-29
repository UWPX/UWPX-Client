using System;

namespace Omemo.Classes.Messages
{
    /// <summary>
    /// Message based on: https://xmpp.org/extensions/xep-0384.html#protobuf-schema
    /// </summary>
    public class OmemoMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly uint n;
        public readonly uint pn;
        public readonly byte[] dhPub;
        public readonly byte[] ciphertext;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public byte[] ToByteArray()
        {
            int size = 4 + 4 + dhPub.Length;
            if (!(ciphertext is null))
            {
                size += ciphertext.Length;
            }
            byte[] result = new byte[size];
            Buffer.BlockCopy(BitConverter.GetBytes(n), 0, result, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(pn), 0, result, 4, 4);
            Buffer.BlockCopy(dhPub, 0, result, 8, dhPub.Length);
            if (!(ciphertext is null))
            {
                Buffer.BlockCopy(ciphertext, 0, result, 8 + dhPub.Length, ciphertext.Length);
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
