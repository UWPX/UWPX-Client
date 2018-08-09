using System.Collections.Generic;
using libsignal;
using libsignal.state;
using libsignal.util;
using org.whispersystems.libsignal.fingerprint;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0384.Signal
{
    public class OmemoUtils
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private static readonly NumericFingerprintGenerator FINGERPRINT_GENERATOR = new NumericFingerprintGenerator(5200);

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 04/08/2018 Created [Fabian Sauter]
        /// </history>
        private OmemoUtils()
        {
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public static SignedPreKeyRecord generateSignedPreKey(IdentityKeyPair identityKeyPair)
        {
            return KeyHelper.generateSignedPreKey(identityKeyPair, 5);
        }

        public static IdentityKeyPair generateIdentityKeyPair()
        {
            return KeyHelper.generateIdentityKeyPair();
        }

        public static IList<PreKeyRecord> generatePreKeys()
        {
            return KeyHelper.generatePreKeys(0, 100);
        }

        public static uint generateDeviceId()
        {
            return KeyHelper.generateRegistrationId(false);
        }

        public static uint generateDeviceId(List<uint> usedDeviceIds)
        {
            // Try 10 times to get a random, unique device id:
            uint id;
            for (int i = 0; i < 10; i++)
            {
                id = generateDeviceId();
                if (!usedDeviceIds.Contains(id))
                {
                    return id;
                }
            }
            throw new System.InvalidOperationException("Failed to generate unique device id! " + nameof(usedDeviceIds) + ".Count = " + usedDeviceIds.Count);
        }

        public static Fingerprint getFingerprint(string accountId, IdentityKey key)
        {
            return FINGERPRINT_GENERATOR.createFor(accountId, key, accountId, key);
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
