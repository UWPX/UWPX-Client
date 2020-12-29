using Windows.Security.Cryptography;
using Windows.Storage.Streams;

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
            byte[] bytes = new byte[length];
            IBuffer buf = CryptographicBuffer.GenerateRandom(length);
            CryptographicBuffer.CopyToByteArray(buf, out bytes);
            return bytes;
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
