using System;
using System.Linq;
using libsignal;
using libsignal.ecc;
using XMPP_API.Classes.Crypto;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0384
{
    public class OmemoFingerprint: IComparable
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly ECPublicKey IDENTITY_PUB_KEY;
        public readonly SignalProtocolAddress ADDRESS;
        public DateTime lastSeen;
        public bool trusted;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public OmemoFingerprint(ECPublicKey identityPubKey, SignalProtocolAddress address, DateTime lastSeen, bool trusted)
        {
            IDENTITY_PUB_KEY = identityPubKey;
            ADDRESS = address;
            this.lastSeen = lastSeen;
            this.trusted = trusted;
        }

        public OmemoFingerprint(ECPublicKey identityPubKey, SignalProtocolAddress address) : this(identityPubKey, address, DateTime.MinValue, false) { }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public byte[] getByteArrayFingerprint()
        {
            return CryptoUtils.getRawFromECPublicKey(IDENTITY_PUB_KEY);
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public bool checkIdentityKey(ECPublicKey other)
        {
            return other.serialize().SequenceEqual(IDENTITY_PUB_KEY.serialize());
        }

        public int CompareTo(object obj)
        {
            return ADDRESS.getName().GetHashCode() ^ ADDRESS.getDeviceId().GetHashCode();
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
