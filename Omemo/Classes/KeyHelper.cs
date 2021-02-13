using System.Collections.Generic;
using Chaos.NaCl;
using Omemo.Classes.Keys;

namespace Omemo.Classes
{
    public static class KeyHelper
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--


        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        /// <summary>
        /// Generates a new Ed25519 <see cref="IdentityKeyPairModel"/> and returns it.
        /// </summary>
        public static IdentityKeyPairModel GenerateIdentityKeyPair()
        {
            GenericECKeyPairModel pair = GenerateKeyPair();
            return new IdentityKeyPairModel(pair.privKey, pair.pubKey);
        }

        /// <summary>
        /// Generates a list of <see cref="PreKeyModel"/>s and returns them.
        /// <para/>
        /// To keep the <see cref="PreKeyModel"/>-IDs unique ensure to set start to (start + count) of the last run.
        /// </summary>
        /// <param name="start">The start it of the new <see cref="PreKeyModel"/>.</param>
        /// <param name="count">How many <see cref="PreKeyModel"/>s should be generated.</param>
        public static List<PreKeyModel> GeneratePreKeys(uint start, uint count)
        {
            List<PreKeyModel> preKeys = new List<PreKeyModel>();
            for (uint i = start; i < (start + count); i++)
            {
                preKeys.Add(GeneratePreKey(i));
            }
            return preKeys;
        }

        /// <summary>
        /// Generates a new Ed25519 <see cref="PreKeyModel"/> and returns it.
        /// </summary>
        /// <param name="id">The id of the <see cref="PreKeyModel"/>.</param>
        public static PreKeyModel GeneratePreKey(uint id)
        {
            GenericECKeyPairModel pair = GenerateKeyPair();
            return new PreKeyModel(pair.privKey, pair.pubKey, id);
        }

        /// <summary>
        /// Generates a new Ed25519 <see cref="SignedPreKeyModel"/> and returns it.
        /// </summary>
        /// <param name="id">The id of the <see cref="SignedPreKeyModel"/>.</param>
        /// <param name="identiyKey">The private part of an <see cref="IdentityKeyPairModel"/> used for signing.</param>
        public static SignedPreKeyModel GenerateSignedPreKey(uint id, ECPrivKeyModel identiyKey)
        {
            PreKeyModel preKey = GeneratePreKey(id);
            byte[] signature = Ed25519.Sign(preKey.pubKey.key, identiyKey.key);
            return new SignedPreKeyModel(preKey, signature);
        }

        /// <summary>
        /// Generates a new Ed25519 <see cref="EphemeralKeyPairModel"/> and returns it.
        /// </summary>
        public static EphemeralKeyPairModel GenerateEphemeralKeyPair()
        {
            GenericECKeyPairModel pair = GenerateKeyPair();
            return new EphemeralKeyPairModel(pair.privKey, pair.pubKey);
        }

        /// <summary>
        /// Generates a new Ed25519 <see cref="GenericECKeyPairModel"/> and returns it.
        /// </summary>
        public static GenericECKeyPairModel GenerateKeyPair()
        {
            Ed25519.KeyPairFromSeed(out byte[] pubKey, out byte[] privKey, new byte[Ed25519.PrivateKeySeedSizeInBytes]);
            return new GenericECKeyPairModel(new ECPrivKeyModel(privKey), new ECPubKeyModel(pubKey));
        }

        /// <summary>
        /// Generates a 32 byte long cryptographically secure random data symmetric key and returns it.
        /// </summary>
        public static byte[] GenerateSymetricKey()
        {
            return CryptoUtils.NextBytesSecureRandom(32);
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
