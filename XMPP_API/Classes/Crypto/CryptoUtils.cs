using libsignal;
using libsignal.state;
using libsignal.util;
using org.whispersystems.libsignal.fingerprint;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace XMPP_API.Classes.Crypto
{
    public class CryptoUtils
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private static readonly NumericFingerprintGenerator FINGERPRINT_GENERATOR = new NumericFingerprintGenerator(5200);

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
        /// Based on: https://gist.github.com/charlesportwoodii/a571e1a3541b708df18881f086e31002
        /// </summary>
        public static string PBKDF2_SHA_1(string normalizedPassword, string salt, uint iterations)
        {
            KeyDerivationAlgorithmProvider provider = KeyDerivationAlgorithmProvider.OpenAlgorithm(KeyDerivationAlgorithmNames.Pbkdf2Sha1);

            // This is our secret password
            IBuffer buffSecret = CryptographicBuffer.ConvertStringToBinary(normalizedPassword, BinaryStringEncoding.Utf8);

            // Use the provided salt
            IBuffer buffSalt = CryptographicBuffer.ConvertStringToBinary(salt, BinaryStringEncoding.Utf8);

            // Create the derivation parameters.
            KeyDerivationParameters pbkdf2Params = KeyDerivationParameters.BuildForPbkdf2(buffSalt, iterations);

            // Create a key from the secret value.
            CryptographicKey keyOriginal = provider.CreateKey(buffSecret);

            // Derive a key based on the original key and the derivation parameters.
            IBuffer keyDerived = CryptographicEngine.DeriveKeyMaterial(keyOriginal, pbkdf2Params, 20);

            // Encode the key to a hexadecimal value (for display)
            return CryptographicBuffer.EncodeToHexString(keyDerived);
        }

        public static string HMAC_SHA_1(string data, string key)
        {
            return HMAC_SHA_1(data, Encoding.ASCII.GetBytes(key));
        }

        public static string HMAC_SHA_1(string data, byte[] key)
        {
            HMACSHA1 hMACSHA1 = new HMACSHA1(key);
            hMACSHA1.Initialize();
            byte[] result = hMACSHA1.ComputeHash(Encoding.ASCII.GetBytes(data));
            return Encoding.ASCII.GetString(result);
        }

        public static string SHA_1(string data)
        {
            return hash(data, HashAlgorithmNames.Sha1);
        }

        public static string xorStrings(string a, string b)
        {
            char[] key = b.ToCharArray();
            char[] output = new char[a.Length];

            for (int i = 0; i < a.Length; i++)
            {
                output[i] = (char)(a[i] ^ key[i % key.Length]);
            }

            return new string(output);
        }

        public static SignedPreKeyRecord generateOmemoSignedPreKey(IdentityKeyPair identityKeyPair)
        {
            return KeyHelper.generateSignedPreKey(identityKeyPair, 5);
        }

        public static IdentityKeyPair generateOmemoIdentityKeyPair()
        {
            return KeyHelper.generateIdentityKeyPair();
        }

        public static IList<PreKeyRecord> generateOmemoPreKeys()
        {
            return KeyHelper.generatePreKeys(0, 100);
        }

        public static uint generateOmemoDeviceId()
        {
            return KeyHelper.generateRegistrationId(false);
        }

        public static uint generateOmemoDeviceIds(IList<uint> usedDeviceIds)
        {
            // Try 10 times to get a random, unique device id:
            uint id;
            for (int i = 0; i < 10; i++)
            {
                id = generateOmemoDeviceId();
                if (!usedDeviceIds.Contains(id))
                {
                    return id;
                }
            }
            throw new InvalidOperationException("Failed to generate unique device id! " + nameof(usedDeviceIds) + ".Count = " + usedDeviceIds.Count);
        }

        public static Fingerprint generateOmemoFingerprint(string accountId, IdentityKey key)
        {
            return FINGERPRINT_GENERATOR.createFor(accountId, key, accountId, key);
        }

        #endregion

        #region --Misc Methods (Private)--
        // Source: https://docs.microsoft.com/en-us/uwp/api/windows.security.cryptography.core.hashalgorithmprovider
        private static string hash(string data, string algName)
        {
            // Convert the message string to binary data.
            IBuffer buffUtf8Msg = CryptographicBuffer.ConvertStringToBinary(data, BinaryStringEncoding.Utf8);

            // Create a HashAlgorithmProvider object.
            HashAlgorithmProvider objAlgProv = HashAlgorithmProvider.OpenAlgorithm(algName);

            // Demonstrate how to retrieve the name of the hashing algorithm.
            String strAlgNameUsed = objAlgProv.AlgorithmName;

            // Hash the message.
            IBuffer buffHash = objAlgProv.HashData(buffUtf8Msg);

            // Verify that the hash length equals the length specified for the algorithm.
            if (buffHash.Length != objAlgProv.HashLength)
            {
                throw new Exception("There was an error creating the hash");
            }

            // Convert the hash to a string (for display).
            String strHashBase64 = CryptographicBuffer.EncodeToBase64String(buffHash);

            // Return the encoded string
            return strHashBase64;
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
