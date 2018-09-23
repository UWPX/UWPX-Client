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
        private const int AUTH_TAG_SIZE_BYTES = 16;

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
            byte[] ciphertext = new byte[AES_GCM_WRAPPER_CPP.calcEncryptSize((uint)data.Length)];
            authTag = new byte[AUTH_TAG_SIZE_BYTES];
            AES_GCM_WRAPPER_CPP.encrypt(ciphertext, authTag, data, key, iv);
            return ciphertext;
        }

        public byte[] decrypt(byte[] ciphertext)
        {
            byte[] data = new byte[AES_GCM_WRAPPER_CPP.calcDecryptSize((uint)ciphertext.Length)];
            AES_GCM_WRAPPER_CPP.decrypt(data, authTag, ciphertext, key, iv);
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
