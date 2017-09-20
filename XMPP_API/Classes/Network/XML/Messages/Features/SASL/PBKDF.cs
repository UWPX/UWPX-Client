using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace XMPP_API.Classes.Network.XML.Messages.Features.SASL
{
    // Source: https://gist.github.com/charlesportwoodii/a571e1a3541b708df18881f086e31002
    class PBKDF
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        /// <summary>
        /// The algorithm to use.
        /// </summary>
        private string algorithm = KeyDerivationAlgorithmNames.Pbkdf2Sha256;
        
        /// <summary>
        /// The PBDFK2 hash.
        /// </summary>
        public string hash;


        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Generate a PBDFK hash.
        /// </summary>
        /// <param name="password">Password</param>
        /// <param name="salt">Salt</param>
        /// <param name="algorithm">Algorithm</param>
        /// <param name="iterationCountIn">Iterations</param>
        /// <param name="targetSize">Target size</param>
        public PBKDF(string password, string salt, string algorithm = null, uint iterationCountIn = 100000, uint targetSize = 32)
        {
            // Use the provide KeyDerivationAlgorithm if provided, otherwise default to PBKDF2-SHA256
            if (algorithm == null)
                algorithm = this.algorithm;

            KeyDerivationAlgorithmProvider provider = KeyDerivationAlgorithmProvider.OpenAlgorithm(algorithm);

            // This is our secret password
            IBuffer buffSecret = CryptographicBuffer.ConvertStringToBinary(password, BinaryStringEncoding.Utf8);

            // Use the provided salt
            IBuffer buffSalt = CryptographicBuffer.ConvertStringToBinary(salt, BinaryStringEncoding.Utf8);

            // Create the derivation parameters.
            KeyDerivationParameters pbkdf2Params = KeyDerivationParameters.BuildForPbkdf2(buffSalt, iterationCountIn);

            // Create a key from the secret value.
            CryptographicKey keyOriginal = provider.CreateKey(buffSecret);

            // Derive a key based on the original key and the derivation parameters.
            IBuffer keyDerived = CryptographicEngine.DeriveKeyMaterial(
                keyOriginal,
                pbkdf2Params,
                targetSize
            );

            // Encode the key to a hexadecimal value (for display)
            hash = CryptographicBuffer.EncodeToHexString(keyDerived);
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


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
