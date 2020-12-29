using System.Collections.Generic;
using Omemo.Classes.Keys;
using Org.BouncyCastle.Math.EC.Rfc8032;
using Org.BouncyCastle.Security;

namespace Omemo.Classes
{
    public static class KeyHelper
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
        /// Generates a new Ed25519 identity key pair and returns it.
        /// </summary>
        public static IdentityKeyPair GenerateIdentityKeyPair()
        {
            ECKeyPair pair = GenerateKeyPair();
            return new IdentityKeyPair(pair.privKey, pair.pubKey);
        }

        /// <summary>
        /// Generates a list of <see cref="PreKey"/>s and returns them.
        /// <para/>
        /// To keep the <see cref="PreKey"/>-IDs unique ensure to set start to (start + count) of the last run.
        /// </summary>
        /// <param name="start">The start it of the new <see cref="PreKey"/>.</param>
        /// <param name="count">How many <see cref="PreKey"/>s should be generated.</param>
        public static List<PreKey> GeneratePreKeys(uint start, uint count)
        {
            List<PreKey> preKeys = new List<PreKey>();
            for (uint i = start; i < (start + count); i++)
            {
                preKeys.Add(GeneratePreKey(i));
            }
            return preKeys;
        }

        /// <summary>
        /// Generates a new Ed25519 <see cref="PreKey"/> and returns it.
        /// </summary>
        /// <param name="id">The id of the <see cref="PreKey"/>.</param>
        public static PreKey GeneratePreKey(uint id)
        {
            ECKeyPair pair = GenerateKeyPair();
            return new PreKey(pair.privKey, pair.pubKey, id);
        }

        /// <summary>
        /// Generates a new Ed25519 <see cref="SignedPreKey"/> and returns it.
        /// </summary>
        /// <param name="id">The id of the <see cref="SignedPreKey"/>.</param>
        public static SignedPreKey GenerateSignedPreKey(uint id)
        {
            PreKey preKey = GeneratePreKey(id);
            byte[] signature = preKey.GenerateSignature();
            return new SignedPreKey(preKey, signature);
        }

        #endregion

        #region --Misc Methods (Private)--
        /// <summary>
        /// Generates a new Ed25519 <see cref="ECKeyPair"/> and returns it.
        /// </summary>
        private static ECKeyPair GenerateKeyPair()
        {
            SecureRandom random = new SecureRandom();
            byte[] privKey = new byte[Consts.ED25519_KEY_SIZE_IN_BYTES];
            Ed25519.GeneratePrivateKey(random, privKey);
            byte[] pubKey = new byte[Ed25519.PublicKeySize];
            Ed25519.GeneratePublicKey(privKey, 0, pubKey, 0);
            return new ECKeyPair(new ECPrivKey(privKey), new ECPubKey(pubKey));
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
