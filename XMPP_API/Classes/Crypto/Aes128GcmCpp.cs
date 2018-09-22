using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMPP_API.Classes.Crypto
{
    public class Aes128GcmCpp
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private const int IV_SIZE_BYTES = 12;
        private const int KEY_SIZE_BYTES = 16;
        private const int AUTH_TAG_SIZE_BITS = 128;

        private readonly SecureRandom SECURE_RANDOM;
        private readonly AES_GCM.AesGcmWrapper AES_GCM_WRAPPER_CPP;
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
        /// 22/09/2018 Created [Fabian Sauter]
        /// </history>
        public Aes128GcmCpp()
        {
            this.SECURE_RANDOM = new SecureRandom();
            this.AES_GCM_WRAPPER_CPP = new AES_GCM.AesGcmWrapper();
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
            byte[] result = new byte[calcOutputSize(data.Length)];
            AES_GCM_WRAPPER_CPP.encrypt(result, data, key, iv);
            return result;
        }

        public byte[] decrypt(byte[] ciphertext)
        {
            byte[] result = new byte[calcOutputSize(ciphertext.Length)];
            AES_GCM_WRAPPER_CPP.decrypt(result, ciphertext, key, iv);
            return result;
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

        #endregion

        #region --Misc Methods (Private)--
        private int calcOutputSize(int inputSize)
        {
            return (inputSize / 16 + 1) * 16;
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
