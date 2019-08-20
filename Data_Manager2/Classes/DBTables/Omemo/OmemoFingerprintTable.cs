using System;
using System.Text;
using libsignal;
using libsignal.ecc;
using SQLite;
using XMPP_API.Classes.Network.XML.Messages.XEP_0384;

namespace Data_Manager2.Classes.DBTables.Omemo
{
    [Table(DBTableConsts.OMEMO_FINGERPRINT_TABLE)]
    public class OmemoFingerprintTable
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        [PrimaryKey]
        public string id { get; set; }
        [NotNull]
        public string bareJid { get; set; }
        public uint deviceId { get; set; }
        [NotNull]
        public string chatId { get; set; }
        [NotNull]
        public byte[] identityPubKey { get; set; }
        public DateTime lastSeen { get; set; }
        public bool trusted { get; set; }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public OmemoFingerprintTable() { }
        public OmemoFingerprintTable(OmemoFingerprint fingerprint, string chatId)
        {
            this.chatId = chatId;
            bareJid = fingerprint.ADDRESS.getName();
            deviceId = fingerprint.ADDRESS.getDeviceId();
            id = generateId(chatId, bareJid, deviceId);
            identityPubKey = fingerprint.IDENTITY_PUB_KEY.serialize();
            lastSeen = fingerprint.lastSeen;
            trusted = fingerprint.trusted;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public static string generateId(string chatId, SignalProtocolAddress address)
        {
            return generateId(chatId, address.getName(), address.getDeviceId());
        }

        public static string generateId(string chatId, string bareJid, uint deviceId)
        {
            StringBuilder sb = new StringBuilder(chatId);
            sb.Append('_');
            sb.Append(bareJid);
            sb.Append(':');
            sb.Append(deviceId);
            return sb.ToString();
        }

        public OmemoFingerprint toOmemoFingerprint()
        {
            SignalProtocolAddress address = new SignalProtocolAddress(bareJid, deviceId);
            ECPublicKey pubKey = Curve.decodePoint(identityPubKey, 0);
            return new OmemoFingerprint(pubKey, address, lastSeen, trusted);
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
