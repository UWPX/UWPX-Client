using System;
using System.Collections.Generic;
using Omemo.Classes.Keys;
using Omemo.Classes.Messages;

namespace Omemo.Classes
{
    public class DoubleRachet
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly IdentityKeyPair SENDER_IDENTITY_KEY;
        private readonly IOmemoStorage STORAGE;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public DoubleRachet(IdentityKeyPair senderIdentityKey, IOmemoStorage storage)
        {
            SENDER_IDENTITY_KEY = senderIdentityKey;
            STORAGE = storage;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        /// <summary>
        /// Generates the associated data (ad) by concatenating the senders and receivers public identity keys.
        /// </summary>
        /// <param name="receiverIdentityKey">The receivers public identity key.</param>
        public byte[] GetAssociatedData(ECPubKey receiverIdentityKey)
        {
            byte[] ad = new byte[SENDER_IDENTITY_KEY.pubKey.key.Length + receiverIdentityKey.key.Length];
            Buffer.BlockCopy(SENDER_IDENTITY_KEY.pubKey.key, 0, ad, 0, SENDER_IDENTITY_KEY.pubKey.key.Length);
            Buffer.BlockCopy(receiverIdentityKey.key, 0, ad, SENDER_IDENTITY_KEY.pubKey.key.Length, receiverIdentityKey.key.Length);
            return ad;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public List<IOmemoMessage> EncryptMessasge(byte[] msg, List<OmemoDeviceGroup> devices, IOmemoStorage storage)
        {
            byte[] key = KeyHelper.GenerateSymetricKey();
            // 32 byte (256 bit) of salt. Initialized with 0s.
            byte[] hkdfOutput = CryptoUtils.HkdfSha256(key, new byte[32], "OMEMO Payload");
            CryptoUtils.SplitKey(hkdfOutput, out byte[] encKey, out byte[] authKey, out byte[] iv);
            byte[] cipherText = CryptoUtils.Aes256CbcEncrypt(encKey, iv, msg);
            byte[] hmac = CryptoUtils.HmacSha256(authKey, cipherText);
            hmac = CryptoUtils.Truncate(hmac, 16);
            byte[] keyHmac = CryptoUtils.Concat(key, hmac);

            List<IOmemoMessage> msgs = new List<IOmemoMessage>();
            foreach (OmemoDeviceGroup group in devices)
            {
                foreach (Tuple<uint, Bundle> device in group.DEVICE_IDS)
                {
                    OmemoProtocolAddress address = new OmemoProtocolAddress(group.BARE_JID, device.Item1);
                    OmemoSession session = storage.LoadSession(address);
                    if (session is null)
                    {
                        // Use a new session:
                        Bundle bundle = device.Item2;
                        int preKeyIndex = bundle.GetRandomPreKeyIndex();
                        session = new OmemoSession(bundle, preKeyIndex, SENDER_IDENTITY_KEY);
                    }
                    OmemoAuthenticatedMessage authMsg = EncryptForDevice(keyHmac, session, GetAssociatedData(device.Item2.identityKey));

                    // To account for lost and out-of-order messages during the key exchange, OmemoKeyExchange structures are sent until a response by the recipient confirms that the key exchange was successfully completed.
                    if (session.nS == 0 || session.nR == 0)
                    {
                        msgs.Add(new OmemoKeyExchange(session.preKeyId, session.signedPreKeyId, SENDER_IDENTITY_KEY.pubKey, session.ek, authMsg));
                    }
                    else
                    {
                        msgs.Add(authMsg);
                    }

                    // Update the session and store it:
                    ++session.nS;
                    storage.StoreSession(address, session);
                }
            }

            return msgs;
        }

        /// <summary>
        /// Encrypts the given plain text with 
        /// </summary>
        /// <param name="plainText">The key, HMAC concatenation result.</param>
        /// <param name="session">The <see cref="OmemoSession"/> between the sender and receiver.</param>
        /// <param name="assData">Encode(IK_A) || Encode(IK_B) => Concatenation of Alices and Bobs public part of their identity key.</param>
        private OmemoAuthenticatedMessage EncryptForDevice(byte[] plainText, OmemoSession session, byte[] assData)
        {
            byte[] mk = session.NextMessageKey();
            // 32 byte (256 bit) of salt. Initialized with 0s.
            byte[] hkdfOutput = CryptoUtils.HkdfSha256(mk, new byte[32], "OMEMO Message Key Material");
            CryptoUtils.SplitKey(hkdfOutput, out byte[] encKey, out byte[] authKey, out byte[] iv);
            OmemoMessage omemoMessage = new OmemoMessage(session)
            {
                cipherText = CryptoUtils.Aes256CbcEncrypt(encKey, iv, plainText)
            };
            byte[] omemoMessageBytes = omemoMessage.ToByteArray();
            byte[] hmacInput = CryptoUtils.Concat(assData, omemoMessageBytes);
            byte[] hmacResult = CryptoUtils.HmacSha256(authKey, hmacInput);
            byte[] hmacTruncated = CryptoUtils.Truncate(hmacResult, 16);
            return new OmemoAuthenticatedMessage(hmacTruncated, omemoMessageBytes);
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
