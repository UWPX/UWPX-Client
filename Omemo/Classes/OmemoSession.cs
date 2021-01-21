using System;
using Omemo.Classes.Keys;
using Omemo.Classes.Messages;

namespace Omemo.Classes
{
    public class OmemoSession
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        /// <summary>
        /// Key pair for the sending ratchet.
        /// </summary>
        public ECKeyPair dhS;
        /// <summary>
        /// Key pair for the receiving ratchet.
        /// </summary>
        public ECKeyPair dhR;
        /// <summary>
        /// Ephemeral key used for initiating this session. 
        /// </summary>
        public ECPubKey ek;
        /// <summary>
        /// 32 byte root key for encryption.
        /// </summary>
        public byte[] rk;
        /// <summary>
        /// 32 byte Chain Keys for sending.
        /// </summary>
        public byte[] ckS;
        /// <summary>
        /// 32 byte Chain Keys for receiving.
        /// </summary>
        public byte[] ckR = null;
        /// <summary>
        /// Message numbers for sending.
        /// </summary>
        public uint nS;
        /// <summary>
        /// Message numbers for receiving.
        /// </summary>
        public uint nR;
        /// <summary>
        /// Number of messages in previous sending chain.
        /// </summary>
        public uint pn;
        /// <summary>
        /// Skipped-over message keys, indexed by ratchet <see cref="ECPubKey"/> and message number. Raises an exception if too many elements are stored.
        /// </summary>
        public readonly SkippedMessageKeys MK_SKIPPED = new SkippedMessageKeys();
        /// <summary>
        /// The id of the PreKey used to create establish this session.
        /// </summary>
        public uint preKeyId;
        /// <summary>
        /// The id of the signed PreKey used to create establish this session.
        /// </summary>
        public uint signedPreKeyId;
        /// <summary>
        /// The associated data is created by concatenating the IdentityKeys of Alice and Bob.
        /// <para/>
        /// AD = Encode(IK_A) || Encode(IK_B).
        /// <para/>
        /// Alice is the party that actively initiated the key exchange, while Bob is the party that passively accepted the key exchange.
        /// </summary>
        public byte[] assData = null;

        /// <summary>
        /// Max number of skipped message keys to prevent DOS attacks.
        /// </summary>
        public const uint MAX_SKIP = 100;


        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Creates a new <see cref="OmemoSession"/> for sending a new message and initiating a new key exchange.
        /// </summary>
        /// <param name="receiverBundle">The <see cref="Bundle"/> of the receiving end.</param>
        /// /// <param name="receiverPreKeyIndex">The index of the <see cref="Bundle.preKeys"/> to use.</param>
        /// <param name="senderIdentityKeyPair">Our own <see cref="IdentityKeyPair"/>.</param>
        public OmemoSession(Bundle receiverBundle, int receiverPreKeyIndex, IdentityKeyPair senderIdentityKeyPair)
        {
            EphemeralKeyPair ephemeralKeyPair = KeyHelper.GenerateEphemeralKeyPair();
            byte[] sk = CryptoUtils.GenerateSenderSessionKey(senderIdentityKeyPair.privKey, ephemeralKeyPair.privKey, receiverBundle.identityKey, receiverBundle.signedPreKey, receiverBundle.preKeys[receiverPreKeyIndex].pubKey);

            // We are only interested in the public key and discard the private key.
            ek = ephemeralKeyPair.pubKey;
            dhS = KeyHelper.GenerateKeyPair();
            dhR = new ECKeyPair(null, receiverBundle.identityKey);
            Tuple<byte[], byte[]> tmp = LibSignalUtils.KDF_RK(sk, CryptoUtils.SharedSecret(dhS.privKey, dhR.pubKey));
            rk = tmp.Item1;
            ckS = tmp.Item2;
            signedPreKeyId = receiverBundle.signedPreKeyId;
            preKeyId = receiverBundle.preKeys[receiverPreKeyIndex].id;
            assData = CryptoUtils.Concat(senderIdentityKeyPair.pubKey.key, receiverBundle.identityKey.key);
        }

        /// <summary>
        /// Creates a new <see cref="OmemoSession"/> for receiving a new message and accepting a new key exchange.
        /// </summary>
        /// <param name="receiverIdentityKey">The receivers <see cref="IdentityKeyPair"/>.</param>
        /// <param name="receiverSignedPreKey">The receivers <see cref="SignedPreKey"/>.</param>
        /// <param name="receiverPreKey">The receivers <see cref="PreKey"/> selected by the sender.</param>
        /// <param name="keyExchangeMsg">The received <see cref="OmemoKeyExchangeMessage"/>.</param>
        public OmemoSession(IdentityKeyPair receiverIdentityKey, SignedPreKey receiverSignedPreKey, PreKey receiverPreKey, OmemoKeyExchangeMessage keyExchangeMsg)
        {
            dhS = receiverIdentityKey;
            rk = CryptoUtils.GenerateReceiverSessionKey(keyExchangeMsg.IK, keyExchangeMsg.EK, receiverIdentityKey.privKey, receiverSignedPreKey.preKey.privKey, receiverPreKey.privKey);
            assData = CryptoUtils.Concat(keyExchangeMsg.IK.key, receiverIdentityKey.pubKey.key);
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        /// <summary>
        /// Initiates the DH ratchet for decrypting the first message.
        /// </summary>
        /// <param name="msg">The received <see cref="OmemoMessage"/>.</param>
        public void InitDhRatchet(OmemoMessage msg)
        {
            pn = nS;
            nS = 0;
            nR = 0;
            dhR = new ECKeyPair(null, msg.DH);
            Tuple<byte[], byte[]> tmp = LibSignalUtils.KDF_RK(rk, CryptoUtils.SharedSecret(dhS.privKey, dhR.pubKey));
            rk = tmp.Item1;
            ckR = tmp.Item2;
            dhS = KeyHelper.GenerateKeyPair();
            tmp = LibSignalUtils.KDF_RK(rk, CryptoUtils.SharedSecret(dhS.privKey, dhR.pubKey));
            rk = tmp.Item1;
            ckS = tmp.Item2;
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
