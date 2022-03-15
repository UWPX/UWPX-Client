using System;
using Logging;
using Omemo.Classes.Exceptions;

namespace Omemo.Classes.Keys
{
    public class ECPubKeyModel: ECKeyModel
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        /// <summary>
        /// Signal requires this prefix to be added to a base 64 representation of this key.
        /// </summary>
        public static byte BASE_64_KEY_PREFIX = 0x05;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// DO NOT USE! Only required for the DB. Use the other constructor instead.
        /// </summary>
        public ECPubKeyModel() { }

        public ECPubKeyModel(byte[] pubKey) : base(pubKey)
        {
            if (pubKey.Length == KeyHelper.PUB_KEY_SIZE)
            {
                return;
            }

            if (pubKey.Length == KeyHelper.PUB_KEY_SIZE + 1)
            {
                if (pubKey[0] == BASE_64_KEY_PREFIX)
                {
                    // Skip the prefix:
                    Buffer.BlockCopy(pubKey, 1, key, 0, KeyHelper.PUB_KEY_SIZE);
                    return;
                }
                throw new OmemoKeyException($"Invalid key prefix '{pubKey[0]}'. Expected: '{BASE_64_KEY_PREFIX}' at index 0 of the key for a {KeyHelper.PUB_KEY_SIZE + 1} byte long key.");
            }
            throw new OmemoKeyException($"Invalid key length {pubKey.Length} - expected {KeyHelper.PUB_KEY_SIZE} or {KeyHelper.PUB_KEY_SIZE + 1}.");
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        /// <summary>
        /// Creates a copy of the object, not including <see cref="id"/>.
        /// </summary>
        public new ECPubKeyModel Clone()
        {
            return new ECPubKeyModel(key);
        }

        /// <summary>
        /// Converts the given base 64 string to a <see cref="ECPubKeyModel"/> by removing the leading <see cref="BASE_64_KEY_PREFIX"/> byte at index 0.
        /// </summary>
        /// <param name="s">Base 64 string representing 33 bytes (32 + <see cref="BASE_64_KEY_PREFIX"/>).</param>
        /// <exception cref="InvalidOperationException">Throws this exception when the given string is not either <see cref="KeyHelper.PUB_KEY_SIZE"/> or (<see cref="KeyHelper.PUB_KEY_SIZE"/> + 1) byte long.</exception>
        public static ECPubKeyModel FromBase64String(string s)
        {
            byte[] data = Convert.FromBase64String(s);
            if (data.Length == KeyHelper.PUB_KEY_SIZE)
            {
                Logger.Warn($"Received a base 64 string representation of a {nameof(ECPubKeyModel)} without the leading {BASE_64_KEY_PREFIX} byte at index 0 and is therefore only {KeyHelper.PUB_KEY_SIZE} byte long, not {KeyHelper.PUB_KEY_SIZE + 1} like we expect it to be. This is a bug in the OMEMO implementation of your contacts client. Please let them know!");
                return new ECPubKeyModel(data);
            }
            else if (data.Length == KeyHelper.PUB_KEY_SIZE + 1)
            {
                if (data[0] == 0x05)
                {
                    byte[] dataNew = new byte[KeyHelper.PUB_KEY_SIZE];
                    Buffer.BlockCopy(data, 1, dataNew, 0, KeyHelper.PUB_KEY_SIZE);
                    return new ECPubKeyModel(dataNew);
                }
                else
                {
                    throw new InvalidOperationException($"Invalid {nameof(ECPubKeyModel)} with length {data.Length} expected to have {BASE_64_KEY_PREFIX} at index 0, but had {data[0]} there.");
                }
            }
            throw new InvalidOperationException($"Invalid {nameof(ECPubKeyModel)} size. Expected {KeyHelper.PUB_KEY_SIZE}, but received {data.Length}.");
        }

        /// <summary>
        /// Converts the <see cref="ECPubKeyModel"/> to a base 64 string representation. Prepends the <see cref="BASE_64_KEY_PREFIX"/> byte before converting it to a string.
        /// </summary>
        public string ToBase64String()
        {
            byte[] data = new byte[KeyHelper.PUB_KEY_SIZE + 1];
            data[0] = BASE_64_KEY_PREFIX;
            key.CopyTo(data, 1);
            return Convert.ToBase64String(data);
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
