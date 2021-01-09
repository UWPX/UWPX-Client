using System;
using System.Linq;
using Chaos.NaCl;
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
        /// The sender public Ephemeral key.
        /// </summary>
        public readonly ECPubKey EK;
        public byte[] cipherText;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public OmemoMessage(byte[] data)
        {
            N = (uint)BitConverter.ToInt32(data, 0);
            PN = (uint)BitConverter.ToInt32(data, 4);
            EK = new ECPubKey(new byte[Ed25519.PublicKeySizeInBytes]);
            Buffer.BlockCopy(data, 8, EK.key, 0, EK.key.Length);
            int cipherTextLenth = data.Length - (8 + EK.key.Length);

            // Cipher text here is optional:
            if (cipherTextLenth > 0)
            {
                cipherText = new byte[data.Length - (8 + EK.key.Length)];
                Buffer.BlockCopy(data, 8 + EK.key.Length, cipherText, 0, cipherText.Length);
            }
        }

        public OmemoMessage(OmemoSession session)
        {
            N = session.nS + 1;
            PN = session.nS;
            EK = session.ek;
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
            int size = 4 + 4 + EK.key.Length;
            if (!(cipherText is null))
            {
                size += cipherText.Length;
            }
            byte[] result = new byte[size];
            Buffer.BlockCopy(BitConverter.GetBytes(N), 0, result, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(PN), 0, result, 4, 4);
            Buffer.BlockCopy(EK.key, 0, result, 8, EK.key.Length);
            if (!(cipherText is null))
            {
                Buffer.BlockCopy(cipherText, 0, result, 8 + EK.key.Length, cipherText.Length);
            }
            return result;
        }

        public override bool Equals(object obj)
        {
            return obj is OmemoMessage msg && msg.N == N && msg.PN == PN && msg.EK.Equals(EK) && ((msg.cipherText is null && cipherText is null) || msg.cipherText.SequenceEqual(cipherText));
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
