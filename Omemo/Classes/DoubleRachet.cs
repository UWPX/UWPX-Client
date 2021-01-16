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
        private readonly IdentityKeyPair OWN_IDENTITY_KEY;
        private readonly IOmemoStorage STORAGE;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public DoubleRachet(IdentityKeyPair ownIdentityKey, IOmemoStorage storage)
        {
            OWN_IDENTITY_KEY = ownIdentityKey;
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
            byte[] ad = new byte[OWN_IDENTITY_KEY.pubKey.key.Length + receiverIdentityKey.key.Length];
            Buffer.BlockCopy(OWN_IDENTITY_KEY.pubKey.key, 0, ad, 0, OWN_IDENTITY_KEY.pubKey.key.Length);
            Buffer.BlockCopy(receiverIdentityKey.key, 0, ad, OWN_IDENTITY_KEY.pubKey.key.Length, receiverIdentityKey.key.Length);
            return ad;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        /// <summary>
        /// Encrypts the given plaintext message and returns the result.
        /// </summary>
        /// <param name="msg">The plain text message to encrypt.</param>
        /// <returns>A tuple consisting out of the cipher text and key-HMAC combination (Tuple[cipherText, keyHmac]).</returns>
        public Tuple<byte[], byte[]> EncryptMessasge(byte[] msg)
        {
            byte[] key = KeyHelper.GenerateSymetricKey();
            // 32 byte (256 bit) of salt. Initialized with 0s.
            byte[] hkdfOutput = CryptoUtils.HkdfSha256(key, new byte[32], "OMEMO Payload");
            CryptoUtils.SplitKey(hkdfOutput, out byte[] encKey, out byte[] authKey, out byte[] iv);
            byte[] cipherText = CryptoUtils.Aes256CbcEncrypt(encKey, iv, msg);
            byte[] hmac = CryptoUtils.HmacSha256(authKey, cipherText);
            hmac = CryptoUtils.Truncate(hmac, 16);
            byte[] keyHmac = CryptoUtils.Concat(key, hmac);
            return new Tuple<byte[], byte[]>(cipherText, keyHmac);
        }

        /// <summary>
        /// Encrypts the given <paramref name="keyHmac"/> combination for all given devices and returns the resulting messages.
        /// Uses the given <see cref="OmemoSession"/> for encrypting.
        /// Calls <see cref="IOmemoStorage.StoreSession(OmemoProtocolAddress, OmemoSession)"/> on completion for each session.
        /// </summary>
        /// <param name="keyHmac">The key HMAC combination, that should be encrypted.</param>
        /// <param name="devices">A collection of devices, we should encrypt the message for.</param>
        /// <returns>A collection of encrypted messages for each device grouped by their bare JID (List[Tuple[bareJid, List[Tuple[deviceId, IOmemoMessage]]]]).</returns>
        public List<Tuple<string, List<Tuple<uint, IOmemoMessage>>>> EncryptForDevices(byte[] keyHmac, List<OmemoDeviceGroup> devices)
        {
            List<Tuple<string, List<Tuple<uint, IOmemoMessage>>>> msgs = new List<Tuple<string, List<Tuple<uint, IOmemoMessage>>>>();
            foreach (OmemoDeviceGroup group in devices)
            {
                List<Tuple<uint, IOmemoMessage>> groupMsgs = new List<Tuple<uint, IOmemoMessage>>();
                foreach (KeyValuePair<uint, OmemoSession> device in group.SESSIONS)
                {
                    OmemoSession session = device.Value;
                    OmemoAuthenticatedMessage authMsg = EncryptForDevice(keyHmac, device.Value, GetAssociatedData(device.Value.dhR.pubKey));

                    // To account for lost and out-of-order messages during the key exchange, OmemoKeyExchange structures are sent until a response by the recipient confirms that the key exchange was successfully completed.
                    if (session.nS == 0 || session.nR == 0)
                    {
                        OmemoKeyExchangeMessage kexMsg = new OmemoKeyExchangeMessage(session.preKeyId, session.signedPreKeyId, OWN_IDENTITY_KEY.pubKey, session.ek, authMsg);
                        groupMsgs.Add(new Tuple<uint, IOmemoMessage>(device.Key, kexMsg));
                    }
                    else
                    {
                        groupMsgs.Add(new Tuple<uint, IOmemoMessage>(device.Key, authMsg));
                    }

                    // Update the session and store it:
                    ++session.nS;
                    STORAGE.StoreSession(new OmemoProtocolAddress(group.BARE_JID, device.Key), session);
                }
                msgs.Add(new Tuple<string, List<Tuple<uint, IOmemoMessage>>>(group.BARE_JID, groupMsgs));
            }
            return msgs;
        }

        public byte[] DecryptForDevice(OmemoAuthenticatedMessage authMsg, OmemoSession session)
        {
            OmemoMessage msg = new OmemoMessage(authMsg.OMEMO_MESSAGE);
            byte[] plainText = TrySkippedMessageKeys(msg, session);
            if (!(plainText is null))
            {
                return plainText;
            }

            if (!session.dhR.pubKey.Equals(msg.DH))
            {
                SkipMessageKeys(session, msg.PN);
                session.InitDhRatchet(msg);
            }
            SkipMessageKeys(session, msg.N);
            byte[] mk = LibSignalUtils.KDF_CK(session.ckR, 0x01);
            session.ckR = LibSignalUtils.KDF_CK(session.ckR, 0x02);
            ++session.nR;
            return DecryptForDevice(mk, msg);
        }

        #endregion

        #region --Misc Methods (Private)--
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

        private byte[] TrySkippedMessageKeys(OmemoMessage msg, OmemoSession session)
        {
            byte[] mk = session.MK_SKIPPED.GetMessagekey(msg.DH, msg.N);
            return !(mk is null) ? DecryptForDevice(mk, msg) : null;
        }

        private void SkipMessageKeys(OmemoSession session, uint until)
        {
            if (session.nR + OmemoSession.MAX_SKIP < until)
            {
                throw new InvalidOperationException("Failed to decrypt. Would skip to many message keys from " + session.nR + " to " + until + ", which is more than " + OmemoSession.MAX_SKIP + '.');
            }
            if (!(session.ckR is null))
            {
                while (session.nR < until)
                {
                    byte[] mk = LibSignalUtils.KDF_CK(session.ckR, 0x01);
                    session.ckR = LibSignalUtils.KDF_CK(session.ckR, 0x02);
                    session.MK_SKIPPED.SetMessageKey(session.dhR.pubKey, session.nR, mk);
                    ++session.nR;
                }
            }
        }

        private byte[] DecryptForDevice(byte[] mk, OmemoMessage msg)
        {
            // 32 byte (256 bit) of salt. Initialized with 0s.
            byte[] hkdfOutput = CryptoUtils.HkdfSha256(mk, new byte[32], "OMEMO Message Key Material");
            CryptoUtils.SplitKey(hkdfOutput, out byte[] encKey, out byte[] authKey, out byte[] iv);
            return CryptoUtils.Aes256CbcEncrypt(encKey, iv, msg.cipherText);
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
