using System;
using System.Linq;
using Omemo.Classes.Exceptions;
using Omemo.Classes.Keys;

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
        /// The sender public key part of the encryption key.
        /// </summary>
        public readonly ECPubKeyModel DH;
        public byte[] cipherText;

        /// <summary>
        /// The minimum size in bytes for a valid version of this message.
        /// </summary>
        public static int MIN_SIZE = sizeof(uint) + sizeof(uint) + KeyHelper.PUB_KEY_SIZE;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public OmemoMessage(byte[] data)
        {
            N = (uint)BitConverter.ToInt32(data, 0);
            PN = (uint)BitConverter.ToInt32(data, 4);
            DH = new ECPubKeyModel(new byte[KeyHelper.PUB_KEY_SIZE]);
            Buffer.BlockCopy(data, 8, DH.key, 0, DH.key.Length);
            int cipherTextLenth = data.Length - (8 + DH.key.Length);

            // Cipher text here is optional:
            if (cipherTextLenth > 0)
            {
                cipherText = new byte[data.Length - (8 + DH.key.Length)];
                Buffer.BlockCopy(data, 8 + DH.key.Length, cipherText, 0, cipherText.Length);
            }
            Validate();
        }

        public OmemoMessage(OmemoSessionModel session)
        {
            N = session.nS;
            PN = session.pn;
            DH = session.dhS.pubKey;
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
            int size = 4 + 4 + DH.key.Length;
            if (!(cipherText is null))
            {
                size += cipherText.Length;
            }
            byte[] result = new byte[size];
            Buffer.BlockCopy(BitConverter.GetBytes(N), 0, result, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(PN), 0, result, 4, 4);
            Buffer.BlockCopy(DH.key, 0, result, 8, DH.key.Length);
            if (!(cipherText is null))
            {
                Buffer.BlockCopy(cipherText, 0, result, 8 + DH.key.Length, cipherText.Length);
            }
            return result;
        }

        public void Validate()
        {
            if (DH?.key is null || DH.key.Length != KeyHelper.PUB_KEY_SIZE)
            {
                throw new OmemoException("Invalid " + nameof(OmemoMessage) + " DH.key.Length: " + DH?.key?.Length);
            }
        }

        public override bool Equals(object obj)
        {
            return obj is OmemoMessage msg && msg.N == N && msg.PN == PN && msg.DH.Equals(DH) && ((msg.cipherText is null && cipherText is null) || msg.cipherText.SequenceEqual(cipherText));
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
