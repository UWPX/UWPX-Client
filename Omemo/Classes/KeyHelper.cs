using System.Collections.Generic;
using Omemo.Classes.Keys;
using Org.BouncyCastle.Math.EC.Rfc8032;
using X25519;

namespace Omemo.Classes
{
    public static class KeyHelper
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public static int PUB_KEY_SIZE = Ed25519.PublicKeySize;
        public static int PRIV_KEY_SIZE = Ed25519.SecretKeySize;

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
        /// Rolls over to one if ids grow larger than 0x7FFFFFFF (2^31-1).
        /// </summary>
        /// <param name="start">The start it of the new <see cref="PreKeyModel"/>.</param>
        /// <param name="count">How many <see cref="PreKeyModel"/>s should be generated.</param>
        public static List<PreKeyModel> GeneratePreKeys(uint start, uint count)
        {
            const uint MIN_VALID_INDEX = 1;
            const uint MAX_VALID_INDEX = 0x7FFFFFFF; // 2^31-1
            uint keyId = start;

            List<PreKeyModel> preKeys = new List<PreKeyModel>();
            for (uint i = 0; i < count; i++)
            {
                if (keyId > MAX_VALID_INDEX)
                {
                    keyId = MIN_VALID_INDEX;
                }
                else
                {
                    keyId++;
                }
                preKeys.Add(GeneratePreKey(keyId));
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
            byte[] signature = SignPreKey(preKey, identiyKey);
            return new SignedPreKeyModel(preKey, signature);
        }

        /// <summary>
        /// Generates the signature of the given <paramref name="preKey"/> and returns it.
        /// </summary>
        /// <param name="preKey">The <see cref="PreKeyModel"/> that should be signed.</param>
        /// <param name="identiyKey">The private Key used for signing the given <paramref name="preKey"/>.</param>
        /// <returns>The signature of the given <paramref name="preKey"/>.</returns>
        public static byte[] SignPreKey(PreKeyModel preKey, ECPrivKeyModel identiyKey)
        {
            byte[] pubKey = preKey.pubKey.ToByteArrayWithPrefix();
            byte[] signature = new byte[Ed25519.SignatureSize];
            Ed25519.Sign(identiyKey.key, 0, pubKey, 0, pubKey.Length, signature, 0);
            return signature;
        }

        /// <summary>
        /// Verifies the signature of the given data with the given <see cref="ECPubKeyModel"/>.
        /// </summary>
        /// <param name="identityKey">The <see cref="ECPubKeyModel"/> of the IdentityKey used for validating the signature.</param>
        /// <param name="preKey">The <see cref="ECPubKeyModel"/> of the PreKey, the signature should be verified for.</param>
        /// <param name="signature">The signature that should be verified.</param>
        /// <returns>True in case the signature is valid.</returns>
        public static bool VerifySignature(ECPubKeyModel identityKey, ECPubKeyModel preKey, byte[] signature)
        {
            return Ed25519.Verify(signature, 0, identityKey.key, 0, preKey.key, 0, preKey.key.Length);
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
            X25519KeyPair pair = X25519KeyAgreement.GenerateKeyPair();
            return new GenericECKeyPairModel(new ECPrivKeyModel(pair.PrivateKey), new ECPubKeyModel(pair.PublicKey));
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
