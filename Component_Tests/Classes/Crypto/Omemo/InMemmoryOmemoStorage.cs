using System;
using System.Collections.Generic;
using Omemo.Classes;
using Omemo.Classes.Keys;
using XMPP_API.Classes.Network.XML.Messages.XEP_0384;

namespace Component_Tests.Classes.Crypto.Omemo
{
    public class InMemmoryOmemoStorage: IExtendedOmemoStorage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly Dictionary<OmemoProtocolAddress, OmemoSessionModel> SESSIONS = new Dictionary<OmemoProtocolAddress, OmemoSessionModel>();
        public readonly Dictionary<OmemoProtocolAddress, OmemoFingerprint> FINGERPRINTS = new Dictionary<OmemoProtocolAddress, OmemoFingerprint>();
        public readonly Dictionary<string, List<Tuple<OmemoProtocolAddress, string>>> DEVICES = new Dictionary<string, List<Tuple<OmemoProtocolAddress, string>>>();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public OmemoSessionModel LoadSession(OmemoProtocolAddress address)
        {
            return SESSIONS.ContainsKey(address) ? SESSIONS[address] : null;
        }

        public PreKeyModel ReplaceOmemoPreKey(PreKeyModel preKey)
        {
            // Not relevant for us
            return preKey;
        }

        public void StoreDeviceListSubscription(string bareJid, Tuple<OmemoDeviceListSubscriptionState, DateTime> lastUpdate)
        {
            throw new NotImplementedException();
        }

        public void StoreFingerprint(OmemoFingerprint fingerprint)
        {
            FINGERPRINTS[fingerprint.ADDRESS] = fingerprint;
        }

        public void StoreSession(OmemoProtocolAddress address, OmemoSessionModel session)
        {
            SESSIONS[address] = session;
        }

        public Tuple<OmemoDeviceListSubscriptionState, DateTime> LoadDeviceListSubscription(string bareJid)
        {
            throw new NotImplementedException();
        }

        public OmemoFingerprint LoadFingerprint(OmemoProtocolAddress address)
        {
            return FINGERPRINTS.ContainsKey(address) ? FINGERPRINTS[address] : null;
        }

        List<Tuple<OmemoProtocolAddress, string>> IExtendedOmemoStorage.LoadDevices(string bareJid)
        {
            return DEVICES.ContainsKey(bareJid) ? DEVICES[bareJid] : new List<Tuple<OmemoProtocolAddress, string>>();
        }

        public void StoreDevices(List<Tuple<OmemoProtocolAddress, string>> devices, string bareJid)
        {
            DEVICES[bareJid] = devices;
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
