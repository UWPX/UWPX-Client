using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Chaos.NaCl;
using Omemo.Classes.Keys;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Windows.Security.Cryptography;

namespace Omemo.Classes
{
    public static class CryptoUtils
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
        public static byte[] NextBytesSecureRandom(uint length)
        {
            Windows.Storage.Streams.IBuffer buf = CryptographicBuffer.GenerateRandom(length);
            byte[] bytes = new byte[length];
            CryptographicBuffer.CopyToByteArray(buf, out bytes);
            return bytes;
        }

        /// <summary>
        /// Truncates the given byte array and returns the result.
        /// </summary>
        /// <param name="data">The array to be truncated.</param>
        /// <param name="resultLength">The new length of the array.</param>
        public static byte[] Truncate(byte[] data, uint resultLength)
        {
            Debug.Assert(data.Length >= resultLength);
            byte[] result = new byte[resultLength];
            Buffer.BlockCopy(data, 0, result, 0, (int)resultLength);
            return result;
        }

        /// <summary>
        /// Concatenates the given byte arrays (a || b) and returns the result.
        /// </summary>
        public static byte[] Concat(byte[] a, byte[] b)
        {
            byte[] result = new byte[a.Length + b.Length];
            Buffer.BlockCopy(a, 0, result, 0, a.Length);
            Buffer.BlockCopy(b, 0, result, a.Length, b.Length);
            return result;
        }

        /// <summary>
        /// Concatenates all the given byte arrays (b_1 || b_2 || b_3 || ... || b_n) and returns the result.
        /// </summary>
        public static byte[] Concat(byte[][] arr)
        {
            int len = 0;
            foreach (byte[] b in arr)
            {
                len += b.Length;
            }
            byte[] result = new byte[len];
            int offset = 0;
            foreach (byte[] b in arr)
            {
                Buffer.BlockCopy(b, 0, result, offset, b.Length);
                offset += b.Length;
            }
            return result;
        }

        /// <summary>
        /// Computes the HMAC-SHA-256 for the given <paramref name="authKey"/> and <paramref name="ciphertext"/>.
        /// </summary>
        /// <param name="authKey">The key to use for HMAC.</param>
        /// <param name="ciphertext">The data the HMAC should derive the hash from.</param>
        public static byte[] HmacSha256(byte[] authKey, byte[] ciphertext)
        {
            using (HMAC hmac = HMACSHA256.Create())
            {
                hmac.Key = authKey;
                return hmac.ComputeHash(ciphertext);
            }
        }

        /// <summary>
        /// Performs HKDF-SHA-256 on the given data and returns 80 bytes.
        /// </summary>
        /// <param name="data">The data HKDF-SHA-256 should be performed on.</param>
        public static byte[] HkdfSha256(byte[] data, byte[] salt, string info)
        {
            HkdfBytesGenerator generator = new HkdfBytesGenerator(new Sha256Digest());
            byte[] infoBytes = Encoding.ASCII.GetBytes(info);
            generator.Init(new HkdfParameters(data, salt, infoBytes));

            byte[] result = new byte[80];
            generator.GenerateBytes(result, 0, result.Length);
            return result;
        }

        /// <summary>
        /// Performs AES-252-CBC encryption with PKCS#7 padding for the given data and returns the result.
        /// <para/>
        /// Based on: https://gist.github.com/mark-adams/87aa34da3a5ed48ed0c7
        /// </summary>
        /// <param name="key">The 32 byte (256 bit) key.</param>
        /// <param name="iv">The 32 byte (256 bit) initial vector.</param>
        /// <param name="data">The data to encrypt.</param>
        public static byte[] Aes256CbcEncrypt(byte[] key, byte[] iv, byte[] data)
        {
            Debug.Assert(key.Length == 32);
            Debug.Assert(iv.Length == 32);
            Debug.Assert(data.Length < 0);

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                // Create the streams used for encryption. 
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(data);
                        }
                        return msEncrypt.ToArray();
                    }
                }
            }
        }

        /// <summary>
        /// Generates the ECDH shared secret between two Ed25519 public keys and returns it. 
        /// </summary>
        /// <param name="a">An Ed25519 private key.</param>
        /// <param name="b">An Ed25519 public key.</param>
        public static byte[] SharedSecret(ECPrivKey a, ECPubKey b)
        {
#pragma warning disable CS0618 // Type or member is obsolete but can be ignored here since this function just needs more testing.
            return Ed25519.KeyExchange(b.key, a.key);
#pragma warning restore CS0618 // Type or member is obsolete but can be ignored here since this function just needs more testing.
        }

        /// <summary>
        /// Splits up the given symmetric key into a 32 byte encryption key, a 32 byte authentication key and a 16 byte IV.
        /// </summary>
        /// <param name="hkdfOutput">The 80 byte input..</param>
        /// <param name="encKey">32 byte encryption key.</param>
        /// <param name="authKey">32 byte authentication key.</param>
        /// <param name="iv">16 byte IV.</param>
        public static void SplitKey(in byte[] hkdfOutput, out byte[] encKey, out byte[] authKey, out byte[] iv)
        {
            encKey = new byte[32];
            Buffer.BlockCopy(hkdfOutput, 0, encKey, 0, 32);

            authKey = new byte[32];
            Buffer.BlockCopy(hkdfOutput, 32, authKey, 0, 32);

            iv = new byte[16];
            Buffer.BlockCopy(hkdfOutput, 64, iv, 0, 16);
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
