using System;
using System.Linq;
using Omemo.Classes;
using Omemo.Classes.Keys;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0384
{
    public class OmemoFingerprint
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly ECPubKey IDENTITY_KEY;
        public readonly OmemoProtocolAddress ADDRESS;
        public DateTime lastSeen;
        public bool trusted;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public OmemoFingerprint(ECPubKey identityKey, OmemoProtocolAddress address, DateTime lastSeen, bool trusted)
        {
            IDENTITY_KEY = identityKey;
            ADDRESS = address;
            this.lastSeen = lastSeen;
            this.trusted = trusted;
        }

        public OmemoFingerprint(ECPubKey identityKey, OmemoProtocolAddress address) : this(identityKey, address, DateTime.MinValue, false) { }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public bool checkIdentityKey(ECPubKey other)
        {
            return other.key.SequenceEqual(IDENTITY_KEY.key);
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
