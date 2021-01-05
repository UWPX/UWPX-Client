using System;
using System.Collections.Generic;
using Chaos.NaCl;
using Windows.Security.Cryptography;

namespace Omemo.Classes.Keys
{
    public class Bundle
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        /// <summary>
        /// The public part of the signed PreKey.
        /// </summary>
        public ECPubKey signedPreKey;
        /// <summary>
        /// The id of the signed PreKey.
        /// </summary>
        public uint signedPreKeyId;
        /// <summary>
        /// The signature of the signed PreKey.
        /// </summary>
        public byte[] preKeySignature;
        /// <summary>
        /// The public part of the identity key.
        /// </summary>
        public ECPubKey identityKey;
        /// <summary>
        /// A collection of public parts of the <see cref="PreKey"/>s and their ID.
        /// </summary>
        public List<PreKey> preKeys = new List<PreKey>();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        /// <summary>
        /// Returns a secure random index for the <see cref="preKeys"/> list.
        /// </summary>
        public int GetRandomPreKeyIndex()
        {
            return (int)(CryptographicBuffer.GenerateRandomNumber() % preKeys.Count);
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public PreKey getRandomPreKey()
        {
            Random r = new Random();
            return preKeys[r.Next(0, preKeys.Count)];
        }

        /// <summary>
        /// Verifies the PreKey signature.
        /// </summary>
        /// <returns>True in case the signature is valid.</returns>
        public bool Verify()
        {
            return Ed25519.Verify(preKeySignature, signedPreKey.key, identityKey.key);
        }

        /// <summary>
        /// Generates the X3DH session key.
        /// <para/>
        /// Documentation: https://www.signal.org/docs/specifications/x3dh/
        /// </summary>
        /// <param name="identityKey">The <see cref="IdentityKeyPair"/> of the other part involved in this session.</param>
        /// <param name="ephemeralKeyPair">The <see cref="EphemeralKeyPair"/> of the other part involved in this session.</param>
        /// <param name="preKeysIndex">The index of one of the <see cref="preKeys"/> owned by this instance.</param>
        /// <returns>The session key (SK).</returns>
        public byte[] GenerateSessionKey(IdentityKeyPair identityKey, EphemeralKeyPair ephemeralKeyPair, int preKeysIndex)
        {
            byte[] dh1 = CryptoUtils.SharedSecret(identityKey.privKey, signedPreKey);
            byte[] dh2 = CryptoUtils.SharedSecret(ephemeralKeyPair.privKey, this.identityKey);
            byte[] dh3 = CryptoUtils.SharedSecret(ephemeralKeyPair.privKey, signedPreKey);
            byte[] dh4 = CryptoUtils.SharedSecret(ephemeralKeyPair.privKey, preKeys[preKeysIndex].pubKey);
            return CryptoUtils.Concat(new byte[][] { dh1, dh2, dh3, dh4 });
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
