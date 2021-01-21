using System;
using Chaos.NaCl;
using Omemo.Classes.Exceptions;
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
        /// The public key part of the senders <see cref="IdentityKeyPair"/>.
        /// </summary>
        public readonly ECPubKey IK;
        /// <summary>
        /// The public key part of the senders <see cref="EphemeralKeyPair"/>.
        /// </summary>
        public readonly ECPubKey EK;
        public readonly OmemoAuthenticatedMessage MESSAGE;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public OmemoKeyExchangeMessage(uint pkId, uint spkId, ECPubKey ik, ECPubKey ek, OmemoAuthenticatedMessage message)
        {
            PK_ID = pkId;
            SPK_ID = spkId;
            IK = ik;
            EK = ek;
            MESSAGE = message;
        }

        public OmemoKeyExchangeMessage(byte[] data)
        {
            PK_ID = BitConverter.ToUInt32(data, 0);
            SPK_ID = BitConverter.ToUInt32(data, 4);
            IK = new ECPubKey(new byte[Ed25519.PublicKeySizeInBytes]);
            Buffer.BlockCopy(data, 8, IK.key, 0, IK.key.Length);
            EK = new ECPubKey(new byte[Ed25519.PublicKeySizeInBytes]);
            Buffer.BlockCopy(data, 8 + IK.key.Length, EK.key, 0, EK.key.Length);
            byte[] msg = new byte[data.Length - 4 - 4 - IK.key.Length - EK.key.Length];
            Buffer.BlockCopy(data, 8 + IK.key.Length + EK.key.Length, msg, 0, msg.Length);
            MESSAGE = new OmemoAuthenticatedMessage(msg);
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
            byte[] msg = MESSAGE.ToByteArray();
            byte[] result = new byte[4 + 4 + IK.key.Length + EK.key.Length + msg.Length];
            Buffer.BlockCopy(BitConverter.GetBytes(PK_ID), 0, result, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(SPK_ID), 0, result, 4, 4);
            Buffer.BlockCopy(IK.key, 0, result, 8, IK.key.Length);
            Buffer.BlockCopy(EK.key, 0, result, 8 + IK.key.Length, EK.key.Length);
            Buffer.BlockCopy(msg, 0, result, 8 + IK.key.Length + EK.key.Length, msg.Length);
            return result;
        }

        public void Validate()
        {
            if (IK?.key is null || IK.key.Length != Ed25519.PublicKeySizeInBytes)
            {
                throw new OmemoException("Invalid " + nameof(OmemoKeyExchangeMessage) + " IK.key.Length: " + IK?.key?.Length);
            }
            if (EK?.key is null || EK.key.Length != Ed25519.PublicKeySizeInBytes)
            {
                throw new OmemoException("Invalid " + nameof(OmemoKeyExchangeMessage) + " IK.key.Length: " + EK?.key?.Length);
            }
            if (MESSAGE is null)
            {
                throw new OmemoException("Invalid " + nameof(OmemoKeyExchangeMessage) + " MESSAGE is null.");
            }
            MESSAGE.Validate();
        }

        public override bool Equals(object obj)
        {
            return obj is OmemoKeyExchangeMessage msg && msg.PK_ID == PK_ID && msg.SPK_ID == SPK_ID && msg.IK.Equals(IK) && msg.EK.Equals(EK) && msg.MESSAGE.Equals(MESSAGE);
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
