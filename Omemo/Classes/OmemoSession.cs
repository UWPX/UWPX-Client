using System.Collections.Generic;
using Omemo.Classes.Keys;

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
        public byte[] rootKey;
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
        public uint npS;
        /// <summary>
        /// Dictionary of skipped-over message keys, indexed by ratchet public key and message number.
        /// </summary>
        public Dictionary<uint, byte[]> mkSkipped = new Dictionary<uint, byte[]>();
        /// <summary>
        /// The id of the PreKey used to create establish this session.
        /// </summary>
        public uint preKeyId;
        /// <summary>
        /// The id of the signed PreKey used to create establish this session.
        /// </summary>
        public uint signedPreKeyId;

        /// <summary>
        /// 
        /// </summary>
        private const uint MAX_SKIP = 100;

        // State variables:


        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Creates a new <see cref="OmemoSession"/> for sending a new message.
        /// </summary>
        /// <param name="bundle">The <see cref="Bundle"/> of the receiving end.</param>
        /// /// <param name="preKeyIndex">The index of the <see cref="Bundle.preKeys"/> to use.</param>
        /// <param name="identityKeyPair">Our own <see cref="IdentityKeyPair"/>.</param>
        public OmemoSession(Bundle bundle, int preKeyIndex, IdentityKeyPair identityKeyPair)
        {
            EphemeralKeyPair ephemeralKeyPair = KeyHelper.GenerateEphemeralKeyPair();
            byte[] sk = bundle.GenerateSessionKey(identityKeyPair, ephemeralKeyPair, preKeyIndex);

            // We are only interested in the public key and discard the private key.
            ek = ephemeralKeyPair.pubKey;
            dhS = KeyHelper.GenerateKeyPair();
            dhR = new ECKeyPair(null, bundle.identityKey);
            rootKey = LibSignalUtils.KDF_RK(sk, CryptoUtils.SharedSecret(dhS.privKey, dhR.pubKey));
            ckS = rootKey;
            signedPreKeyId = bundle.signedPreKeyId;
            preKeyId = bundle.preKeys[preKeyIndex].id;
        }

        /// <summary>
        /// Creates a new <see cref="OmemoSession"/> for receiving a message,
        /// </summary>
        /// <param name="ownIdentityKeyPair"></param>
        /// <param name="sessionKey">The session key (SK) generated via <see cref="Bundle.GenerateSessionKey(IdentityKeyPair, EphemeralKeyPair, int)"/>.</param>
        public OmemoSession(IdentityKeyPair ownIdentityKeyPair, byte[] sessionKey)
        {
            //TODO: generate session key based on: https://www.signal.org/docs/specifications/x3dh/
            dhS = ownIdentityKeyPair;
            rootKey = sessionKey;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        /// <summary>
        /// Generates the next message key (mk) and returns it.
        /// <para/>
        /// Also updates <see cref="ckS"/> with its new value.
        /// </summary>
        public byte[] NextMessageKey()
        {
            byte[] mk = LibSignalUtils.KDF_CK(ckS, 0x01);
            ckS = LibSignalUtils.KDF_CK(ckS, 0x02);
            return mk;
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
