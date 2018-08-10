using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.Security.Cryptography;

namespace XMPP_API.Classes.Crypto
{
    /// <summary>
    /// Based on: https://github.com/luke-park/SecureCompatibleEncryptionExamples/blob/master/C%23/SCEE.cs
    /// </summary>
    public class Aes128Gcm
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private const string ALGORITHM_NAME = "AES";
        private const int ALGORITHM_NONCE_SIZE = 12;
        private const int ALGORITHM_KEY_SIZE = 16;
        private const int PBKDF2_SALT_SIZE = 16;
        private const int PBKDF2_ITERATIONS = 32767;

        private readonly GcmBlockCipher CIPHER;
        private readonly SecureRandom SECURE_RANDOM;
        public ParametersWithIV cipherParameters { get; private set; }
        public byte[] key;
        public byte[] data;
        public byte[] encryptedData;

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
            this.CIPHER = new GcmBlockCipher(new AesEngine());
            this.SECURE_RANDOM = new SecureRandom();
            this.cipherParameters = null;
            this.key = null;
            this.data = null;
            this.encryptedData = null;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void encrypt()
        {
            // Generate a 96-bit nonce using a CSPRNG.
            byte[] nonce = new byte[ALGORITHM_NONCE_SIZE];
            SECURE_RANDOM.NextBytes(nonce);

            // Create the cipher instance and initialize.
            KeyParameter keyParam = ParameterUtilities.CreateKeyParameter(ALGORITHM_NAME, key);
            cipherParameters = new ParametersWithIV(keyParam, nonce);
            CIPHER.Init(true, cipherParameters);

            // Encrypt and prepend nonce.
            byte[] ciphertext = new byte[CIPHER.GetOutputSize(data.Length)];
            int length = CIPHER.ProcessBytes(data, 0, data.Length, ciphertext, 0);
            CIPHER.DoFinal(ciphertext, length);

            encryptedData = new byte[nonce.Length + ciphertext.Length];
            Array.Copy(nonce, 0, encryptedData, 0, nonce.Length);
            Array.Copy(ciphertext, 0, encryptedData, nonce.Length, ciphertext.Length);
        }

        public void decrypt()
        {
            // Retrieve the nonce and ciphertext.
            byte[] nonce = new byte[ALGORITHM_NONCE_SIZE];
            byte[] ciphertext = new byte[encryptedData.Length - ALGORITHM_NONCE_SIZE];
            Array.Copy(encryptedData, 0, nonce, 0, nonce.Length);
            Array.Copy(encryptedData, nonce.Length, ciphertext, 0, ciphertext.Length);

            // Create the cipher instance and initialize.
            KeyParameter keyParam = ParameterUtilities.CreateKeyParameter(ALGORITHM_NAME, key);
            cipherParameters = new ParametersWithIV(keyParam, nonce);
            CIPHER.Init(false, cipherParameters);

            // Decrypt and return result.
            data = new byte[CIPHER.GetOutputSize(ciphertext.Length)];
            int length = CIPHER.ProcessBytes(ciphertext, 0, ciphertext.Length, data, 0);
            CIPHER.DoFinal(data, length);
        }

        /// <summary>
        /// Generates a 16 byte symmetric AES key.
        /// </summary>
        public void generateKey()
        {
            key = new byte[16];
            SECURE_RANDOM.NextBytes(key);
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
