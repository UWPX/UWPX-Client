using System;

namespace Omemo.Classes
{
    public static class LibSignalUtils
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
        /// HKDF-SHA-256 using the root key (<paramref name="rk"/>) as HKDF salt, the Diffie-Hellman shared secret (<paramref name="sharedSecret"/>) as HKDF input material and "OMEMO Root Chain" as HKDF info.
        /// </summary>
        /// <param name="rk">Root key.</param>
        /// <param name="sharedSecret">Diffie-Hellman shared secret.</param>
        /// <returns>A <see cref="Tuple"/> containing the new root key (rk) and the new chain key (ck).</returns>
        public static Tuple<byte[], byte[]> KDF_RK(byte[] rk, byte[] sharedSecret)
        {
            byte[] tmp = CryptoUtils.HkdfSha256(sharedSecret, rk, "OMEMO Root Chain", 64);
            byte[] rk_new = new byte[32];
            Buffer.BlockCopy(tmp, 0, rk_new, 0, rk_new.Length);
            byte[] ck_new = new byte[32];
            Buffer.BlockCopy(tmp, rk_new.Length, ck_new, 0, ck_new.Length);
            return new Tuple<byte[], byte[]>(rk_new, ck_new);
        }

        /// <summary>
        /// HMAC-SHA-256 using a chain key (<paramref name="ck"/>) as the HMAC key, a single byte constant 0x01 as HMAC input to produce the next message key (mk) and a single byte constant 0x02 as HMAC input to produce the next chain key.
        /// </summary>
        /// <param name="ck">The chain key.</param>
        /// <param name="operation">0x01 to produce the next message key (mk), or 0x02 as input to produce the next chain key.</param>
        public static byte[] KDF_CK(byte[] ck, byte operation)
        {
            return CryptoUtils.HmacSha256(ck, new byte[] { operation });
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
