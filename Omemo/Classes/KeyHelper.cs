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
        /// Generates a new Ed25519 <see cref="IdentityKeyPair"/> and returns it.
        /// </summary>
        public static IdentityKeyPair GenerateIdentityKeyPair()
        {
            ECKeyPair pair = GenerateKeyPair();
            return new IdentityKeyPair(pair.privKey, pair.pubKey);
        }

        /// <summary>
        /// Generates a list of <see cref="PreKey"/>s and returns them.
        /// <para/>
        /// To keep the <see cref="PreKey"/>-IDs unique ensure to set start to (start + count) of the last run.
        /// </summary>
        /// <param name="start">The start it of the new <see cref="PreKey"/>.</param>
        /// <param name="count">How many <see cref="PreKey"/>s should be generated.</param>
        public static List<PreKey> GeneratePreKeys(uint start, uint count)
        {
            List<PreKey> preKeys = new List<PreKey>();
            for (uint i = start; i < (start + count); i++)
            {
                preKeys.Add(GeneratePreKey(i));
            }
            return preKeys;
        }

        /// <summary>
        /// Generates a new Ed25519 <see cref="PreKey"/> and returns it.
        /// </summary>
        /// <param name="id">The id of the <see cref="PreKey"/>.</param>
        public static PreKey GeneratePreKey(uint id)
        {
            ECKeyPair pair = GenerateKeyPair();
            return new PreKey(pair.privKey, pair.pubKey, id);
        }

        /// <summary>
        /// Generates a new Ed25519 <see cref="SignedPreKey"/> and returns it.
        /// </summary>
        /// <param name="id">The id of the <see cref="SignedPreKey"/>.</param>
        /// <param name="identiyKey">The private part of an <see cref="IdentityKeyPair"/> used for signing.</param>
        public static SignedPreKey GenerateSignedPreKey(uint id, ECPrivKey identiyKey)
        {
            PreKey preKey = GeneratePreKey(id);
            byte[] signature = Ed25519.Sign(preKey.pubKey.key, identiyKey.key);
            return new SignedPreKey(preKey, signature);
        }

        /// <summary>
        /// Generates a new Ed25519 <see cref="EphemeralKeyPair"/> and returns it.
        /// </summary>
        public static EphemeralKeyPair GenerateEphemeralKeyPair()
        {
            ECKeyPair pair = GenerateKeyPair();
            return new EphemeralKeyPair(pair.privKey, pair.pubKey);
        }

        /// <summary>
        /// Generates a new Ed25519 <see cref="ECKeyPair"/> and returns it.
        /// </summary>
        public static ECKeyPair GenerateKeyPair()
        {
            Ed25519.KeyPairFromSeed(out byte[] pubKey, out byte[] privKey, new byte[Ed25519.PrivateKeySeedSizeInBytes]);
            return new ECKeyPair(new ECPrivKey(privKey), new ECPubKey(pubKey));
        }

        /// <summary>
        /// Generates a 32 byte long cryptographically secure random data symetric key and returns it.
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
