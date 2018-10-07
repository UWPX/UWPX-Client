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
using System.Runtime.InteropServices.WindowsRuntime;
using System.Linq;

namespace XMPP_API.Classes.Crypto
{
    public static class CryptoUtils
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
        /// RFC 2898 with Sha1
        /// </summary>
        public static byte[] Pbkdf2Sha1(string normalizedPassword, byte[] salt, int iterations)
        {
            Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes(normalizedPassword, salt)
            {
                IterationCount = iterations
            };

            return deriveBytes.GetBytes(20);
        }

        public static byte[] HmacSha1(byte[] data, string key)
        {
            return HmacSha1(data, Encoding.ASCII.GetBytes(key));
        }

        public static byte[] HmacSha1(string data, string key)
        {
            return HmacSha1(Encoding.ASCII.GetBytes(data), Encoding.ASCII.GetBytes(key));
        }

        public static byte[] HmacSha1(string data, byte[] key)
        {
            return HmacSha1(Encoding.ASCII.GetBytes(data), key);
        }

        public static byte[] HmacSha1(byte[] data, byte[] key)
        {
            HMACSHA1 hmac = new HMACSHA1(key);
            hmac.Initialize();
            return hmac.ComputeHash(data);
        }

        public static byte[] SHA_1(byte[] data)
        {
            return hash(data, HashAlgorithmNames.Sha1);
        }

        public static byte[] xor(byte[] a, byte[] b)
        {
            byte[] output = new byte[b.Length];

            for (int i = 0; i < b.Length; i++)
            {
                output[i] = (byte)(b[i] ^ a[i % a.Length]);
            }

            return output;
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

        public static byte[] hexStringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        public static string byteArrayToHexString(byte[] data)
        {
            StringBuilder hex = new StringBuilder(data.Length * 2);
            foreach (byte b in data)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        public static void nextBytesSecureRandom(out byte[] b, in uint length)
        {
            IBuffer buf = CryptographicBuffer.GenerateRandom(length);
            CryptographicBuffer.CopyToByteArray(buf, out b);
        }

        #endregion

        #region --Misc Methods (Private)--
        // Source: https://docs.microsoft.com/en-us/uwp/api/windows.security.cryptography.core.hashalgorithmprovider
        private static byte[] hash(byte[] data, string algName)
        {
            // Convert the message string to binary data.
            IBuffer buffUtf8Msg = CryptographicBuffer.CreateFromByteArray(data);

            // Create a HashAlgorithmProvider object.
            HashAlgorithmProvider objAlgProv = HashAlgorithmProvider.OpenAlgorithm(algName);

            // Hash the message.
            IBuffer buffHash = objAlgProv.HashData(buffUtf8Msg);

            // Verify that the hash length equals the length specified for the algorithm.
            if (buffHash.Length != objAlgProv.HashLength)
            {
                throw new InvalidOperationException("There was an error creating the hash");
            }

            // Return the encoded string
            return buffHash.ToArray();
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
