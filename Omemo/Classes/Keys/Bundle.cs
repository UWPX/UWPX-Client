using System;
using System.Collections.Generic;
using Windows.Security.Cryptography;

namespace Omemo.Classes.Keys
{
    public class Bundle
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        /// <summary>
        /// The public part of the signed PreKey.
        /// </summary>
        public ECPubKeyModel signedPreKey;
        /// <summary>
        /// The id of the signed PreKey.
        /// </summary>
        public uint signedPreKeyId;
        /// <summary>
        /// The signature of the signed PreKey.
        /// </summary>
        public byte[] preKeySignature;
        /// <summary>
        /// The public part of the identity key.
        /// </summary>
        public ECPubKeyModel identityKey;
        /// <summary>
        /// A collection of public parts of the <see cref="PreKeyModel"/>s and their ID.
        /// </summary>
        public List<PreKeyModel> preKeys = new List<PreKeyModel>();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        /// <summary>
        /// Returns a secure random index for the <see cref="preKeys"/> list.
        /// </summary>
        public int GetRandomPreKeyIndex()
        {
            return (int)(CryptographicBuffer.GenerateRandomNumber() % preKeys.Count);
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public PreKeyModel getRandomPreKey()
        {
            Random r = new Random();
            return preKeys[r.Next(0, preKeys.Count)];
        }

        /// <summary>
        /// Verifies the PreKey signature.
        /// </summary>
        /// <returns>True in case the signature is valid.</returns>
        public bool Verify()
        {
            return KeyHelper.VerifySignature(signedPreKey, identityKey.key, preKeySignature);
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
