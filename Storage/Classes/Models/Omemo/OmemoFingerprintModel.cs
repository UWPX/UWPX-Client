using System;
using System.ComponentModel.DataAnnotations;
using Omemo.Classes;
using Omemo.Classes.Keys;
using XMPP_API.Classes.Network.XML.Messages.XEP_0384;

namespace Storage.Classes.Models.Omemo
{
    public class OmemoFingerprintModel
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        [Key]
        public int id { get; set; }
        [Required]
        public DateTime lastSeen { get; set; }
        [Required]
        public bool trusted { get; set; }
        [Required]
        public ECPubKeyModel identityKey { get; set; }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public OmemoFingerprintModel() { }

        public OmemoFingerprintModel(OmemoFingerprint fingerprint)
        {
            lastSeen = fingerprint.lastSeen;
            trusted = fingerprint.trusted;
            identityKey = fingerprint.IDENTITY_KEY;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public OmemoFingerprint ToOmemoFingerprint(OmemoProtocolAddress address)
        {
            return new OmemoFingerprint(identityKey, address, lastSeen, trusted);
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
