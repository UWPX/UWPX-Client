using System;
using System.Linq;
using Chaos.NaCl;
using Omemo.Classes.Keys;

namespace Omemo.Classes.Messages
{
    /// <summary>
    /// Message based on: https://xmpp.org/extensions/xep-0384.html#protobuf-schema
    /// </summary>
    public class OmemoKeyExchangeMessage: IOmemoMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        /// <summary>
        /// PreKey id.
        /// </summary>
        public readonly uint PK_ID;
        /// <summary>
        /// Signed PreKey id.
        /// </summary>
        public readonly uint SPK_ID;
        /// <summary>
        /// Public key part of the senders identity key.
        /// </summary>
        public readonly byte[] IK;
        /// <summary>
        /// Ephemeral key pair used by the X3DH key agreement.
        /// </summary>
        public readonly byte[] EK;
        public readonly OmemoAuthenticatedMessage MESSAGE;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public OmemoKeyExchangeMessage(uint pkId, uint spkId, ECPubKey ik, ECPubKey ek, OmemoAuthenticatedMessage message)
        {
            PK_ID = pkId;
            SPK_ID = spkId;
            IK = ik.key;
            EK = ek.key;
            MESSAGE = message;
        }

        public OmemoKeyExchangeMessage(byte[] data)
        {
            PK_ID = BitConverter.ToUInt32(data, 0);
            SPK_ID = BitConverter.ToUInt32(data, 4);
            IK = new byte[Ed25519.PublicKeySizeInBytes];
            Buffer.BlockCopy(data, 8, IK, 0, IK.Length);
            EK = new byte[Ed25519.PublicKeySizeInBytes];
            Buffer.BlockCopy(data, 8 + IK.Length, EK, 0, EK.Length);
            byte[] msg = new byte[data.Length - 4 - 4 - IK.Length - EK.Length];
            Buffer.BlockCopy(data, 8 + IK.Length + EK.Length, msg, 0, msg.Length);
            MESSAGE = new OmemoAuthenticatedMessage(msg);
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public byte[] ToByteArray()
        {
            byte[] msg = MESSAGE.ToByteArray();
            byte[] result = new byte[4 + 4 + IK.Length + EK.Length + msg.Length];
            Buffer.BlockCopy(BitConverter.GetBytes(PK_ID), 0, result, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(SPK_ID), 0, result, 4, 4);
            Buffer.BlockCopy(IK, 0, result, 8, IK.Length);
            Buffer.BlockCopy(EK, 0, result, 8 + IK.Length, EK.Length);
            Buffer.BlockCopy(msg, 0, result, 8 + IK.Length + EK.Length, msg.Length);
            return result;
        }

        public override bool Equals(object obj)
        {
            return obj is OmemoKeyExchangeMessage msg && msg.PK_ID == PK_ID && msg.SPK_ID == SPK_ID && msg.IK.SequenceEqual(IK) && msg.EK.SequenceEqual(EK) && msg.MESSAGE.Equals(MESSAGE);
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
