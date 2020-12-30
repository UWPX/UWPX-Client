using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Omemo.Classes.Keys;
using Omemo.Classes.Messages;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;

namespace Omemo.Classes
{
    public class DoubleRachet
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly IdentityKeyPair SENDER_IDENTITY_KEY;
        private readonly OmemoSession SESSION;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public DoubleRachet(OmemoSession session)
        {
            SESSION = session;
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
            byte[] ad = new byte[SENDER_IDENTITY_KEY.pubKey.key.Length + receiverIdentityKey.key.Length];
            Buffer.BlockCopy(SENDER_IDENTITY_KEY.pubKey.key, 0, ad, 0, SENDER_IDENTITY_KEY.pubKey.key.Length);
            Buffer.BlockCopy(receiverIdentityKey.key, 0, ad, SENDER_IDENTITY_KEY.pubKey.key.Length, receiverIdentityKey.key.Length);
            return ad;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public List<IOmemoMessage> EncryptMessasge(byte[] msg, List<> devices)
        {
            byte[] key = GenerateKey();
            byte[] hkdfOutput = HkdfSha256(key);
            SplitKey(hkdfOutput, out byte[] encKey, out byte[] authKey, out byte[] iv);
            byte[] cipherText = Aes256CbcEncrypt(encKey, iv, msg);
            byte[] hmac = HmacSha256(authKey, cipherText);
            hmac = Truncate(hmac, 16);
            byte[] keyHmac = Concat(key, hmac);

            List<IOmemoMessage> msgs = new List<IOmemoMessage>();

            return msgs;

            // TODO: Encrypt for each device
            // EncryptForDevice(key, keyHmac, Encode(IK_A) || Encode(IK_B));
        }

        /// <summary>
        /// Encrypts the given plain text with 
        /// </summary>
        /// <param name="msgKey">The key used for encrypting the actual message.</param>
        /// <param name="plaintext">The key, HMAC concatenation result.</param>
        /// <param name="assData">Encode(IK_A) || Encode(IK_B) => Concatenation of Alices and Bobs public part of their identity key.</param>
        public IOmemoMessage EncryptForDevice(byte[] msgKey, byte[] plainText, byte[] assData)
        {
            byte[] hkdfOutput = HkdfSha256(msgKey);
            SplitKey(hkdfOutput, out byte[] encKey, out byte[] authKey, out byte[] iv);
            byte[] ciphertext = Aes256CbcEncrypt(encKey, iv, plainText);
        }

        #endregion

        #region --Misc Methods (Private)--
        /// <summary>
        /// Performs AES-252-CBC encryption with PKCS#7 padding for the given data and returns the result.
        /// <para/>
        /// Based on: https://gist.github.com/mark-adams/87aa34da3a5ed48ed0c7
        /// </summary>
        /// <param name="key">The 32 byte (256 bit) key.</param>
        /// <param name="iv">The 32 byte (256 bit) initial vector.</param>
        /// <param name="data">The data to encrypt.</param>
        private byte[] Aes256CbcEncrypt(byte[] key, byte[] iv, byte[] data)
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
        /// Performs HKDF-SHA-256 on the given data and returns 80 bytes.
        /// </summary>
        /// <param name="data">The data HKDF-SHA-256 should be performed on.</param>
        private byte[] HkdfSha256(byte[] data)
        {
            HkdfBytesGenerator generator = new HkdfBytesGenerator(new Sha256Digest());
            // 32 byte (256 bit) of salt. Initialized with 0s.
            byte[] salt = new byte[32];
            byte[] info = Encoding.ASCII.GetBytes("OMEMO Payload");
            generator.Init(new HkdfParameters(data, salt, info));

            byte[] result = new byte[80];
            generator.GenerateBytes(result, 0, result.Length);
            return result;
        }

        /// <summary>
        /// Generates a 32 byte long cryptographically secure random data key and returns it.
        /// </summary>
        private byte[] GenerateKey()
        {
            return CryptoUtils.NextBytesSecureRandom(32);
        }

        /// <summary>
        /// Splits up the given key into a 32 byte encryption key, a 32 byte authentication key and a 16 byte IV.
        /// </summary>
        /// <param name="hkdfOutput">The 80 byte input..</param>
        /// <param name="encKey">32 byte encryption key.</param>
        /// <param name="authKey">32 byte authentication key.</param>
        /// <param name="iv">16 byte IV.</param>
        private void SplitKey(in byte[] hkdfOutput, out byte[] encKey, out byte[] authKey, out byte[] iv)
        {
            encKey = new byte[32];
            Buffer.BlockCopy(hkdfOutput, 0, encKey, 0, 32);

            authKey = new byte[32];
            Buffer.BlockCopy(hkdfOutput, 32, authKey, 0, 32);

            iv = new byte[16];
            Buffer.BlockCopy(hkdfOutput, 64, iv, 0, 16);
        }

        /// <summary>
        /// Computes the HMAC-SHA-256 for the given <paramref name="authKey"/> and <paramref name="ciphertext"/>.
        /// </summary>
        /// <param name="authKey">The key to use for HMAC.</param>
        /// <param name="ciphertext">The data the HMAC should derive the hash from.</param>
        private byte[] HmacSha256(byte[] authKey, byte[] ciphertext)
        {
            using (HMAC hmac = HMACSHA256.Create())
            {
                hmac.Key = authKey;
                return hmac.ComputeHash(ciphertext);
            }
        }

        private byte[] Truncate(byte[] data, uint resultLength)
        {
            Debug.Assert(data.Length >= resultLength);
            byte[] result = new byte[resultLength];
            Buffer.BlockCopy(data, 0, result, 0, (int)resultLength);
            return result;
        }

        private byte[] Concat(byte[] a, byte[] b)
        {
            byte[] result = new byte[a.Length + b.Length];
            Buffer.BlockCopy(a, 0, result, 0, a.Length);
            Buffer.BlockCopy(b, 0, result, a.Length, b.Length);
            return result;
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
