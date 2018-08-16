using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;

namespace XMPP_API.Classes.Crypto
{
    /// <summary>
    /// Based on: https://github.com/luke-park/SecureCompatibleEncryptionExamples/blob/master/C%23/SCEE.cs
    /// </summary>
    public class Aes128Gcm
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private const int IV_SIZE_BYTES = 12;
        private const int KEY_SIZE_BYTES = 16;
        private const int AUTH_TAG_SIZE_BITS = 128;

        private readonly SecureRandom SECURE_RANDOM;
        public byte[] key;
        public byte[] iv;
        public byte[] authTag;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 10/08/2018 Created [Fabian Sauter]
        /// </history>
        public Aes128Gcm()
        {
            this.SECURE_RANDOM = new SecureRandom();
            this.key = null;
            this.iv = null;
            this.authTag = null;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public byte[] encrypt(byte[] data)
        {
            // Create the cipher instance and initialize:
            GcmBlockCipher cipher = new GcmBlockCipher(new AesEngine());
            KeyParameter keyParam = new KeyParameter(key);
            AeadParameters aeadParams = new AeadParameters(keyParam, AUTH_TAG_SIZE_BITS, iv);
            cipher.Init(true, aeadParams);

            // Encrypt and prepend iv:
            byte[] ciphertext = new byte[cipher.GetOutputSize(data.Length)];
            int length = cipher.ProcessBytes(data, 0, data.Length, ciphertext, 0);
            cipher.DoFinal(ciphertext, length);
            authTag = cipher.GetMac();

            byte[] encryptedData = new byte[iv.Length + ciphertext.Length];
            Buffer.BlockCopy(iv, 0, encryptedData, 0, iv.Length);
            Buffer.BlockCopy(ciphertext, 0, encryptedData, iv.Length, ciphertext.Length);

            return encryptedData;
        }

        public byte[] decrypt(byte[] encryptedData)
        {
            // Retrieve the nonce and ciphertext:
            iv = new byte[IV_SIZE_BYTES];
            byte[] ciphertext = new byte[encryptedData.Length - IV_SIZE_BYTES];

            Buffer.BlockCopy(encryptedData, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(encryptedData, iv.Length, ciphertext, 0, ciphertext.Length);

            // Create the cipher instance and initialize:
            GcmBlockCipher cipher = new GcmBlockCipher(new AesEngine());
            KeyParameter keyParam = new KeyParameter(key);
            AeadParameters aeadParams = new AeadParameters(keyParam, AUTH_TAG_SIZE_BITS, iv);
            cipher.Init(false, aeadParams);

            // Decrypt and return result:
            byte[] data = new byte[cipher.GetOutputSize(ciphertext.Length)];
            int length = cipher.ProcessBytes(ciphertext, 0, ciphertext.Length, data, 0);
            cipher.DoFinal(data, length);

            return data;
        }

        /// <summary>
        /// Generates a 16 byte symmetric AES key.
        /// </summary>
        public void generateKey()
        {
            key = new byte[KEY_SIZE_BYTES];
            SECURE_RANDOM.NextBytes(key);
        }

        /// <summary>
        /// Generates a 12 byte iv.
        /// </summary>
        public void generateIv()
        {
            iv = new byte[IV_SIZE_BYTES];
            SECURE_RANDOM.NextBytes(iv);
        }
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
