using System.Collections.Generic;
using Omemo.Classes;
using Omemo.Classes.Keys;
using Omemo.Classes.Messages;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0384
{
    public class OmemoDecryptionContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly OmemoProtocolAddress RECEIVER_ADDRESS;
        public readonly IdentityKeyPairModel RECEIVER_IDENTITY_KEY;
        public readonly SignedPreKeyModel RECEIVER_SIGNED_PRE_KEY;
        public readonly IEnumerable<PreKeyModel> RECEIVER_PRE_KEYS;
        public readonly IExtendedOmemoStorage STORAGE;
        public readonly bool TRUSTED_KEYS_ONLY;

        public OmemoKey key;
        public bool keyExchange;
        public PreKeyModel usedPreKey;
        public OmemoKeyExchangeMessage keyExchangeMsg;
        public OmemoAuthenticatedMessage authMsg;
        public OmemoProtocolAddress senderAddress;
        public OmemoSessionModel session;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public OmemoDecryptionContext(OmemoProtocolAddress receiverAddress, IdentityKeyPairModel receiverIdentityKey, SignedPreKeyModel receiverSignedPreKey, IEnumerable<PreKeyModel> receiverPreKeys, bool trustedKeysOnly, IExtendedOmemoStorage storage)
        {
            RECEIVER_ADDRESS = receiverAddress;
            RECEIVER_IDENTITY_KEY = receiverIdentityKey;
            RECEIVER_SIGNED_PRE_KEY = receiverSignedPreKey;
            RECEIVER_PRE_KEYS = receiverPreKeys;
            TRUSTED_KEYS_ONLY = trustedKeysOnly;
            STORAGE = storage;
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
