using System;
using System.Collections.Generic;
using Omemo.Classes;
using XMPP_API.Classes.Network.XML.Messages.XEP_0384;

namespace Manager.Classes
{
    public class OmemoStorage: IExtendedOmemoStorage
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
        public Tuple<OmemoDeviceListSubscriptionState, DateTime> LoadDeviceListSubscription(string bareJid)
        {
            throw new NotImplementedException();
        }

        public List<OmemoProtocolAddress> LoadDevices(string bareJid)
        {
            throw new NotImplementedException();
        }

        public OmemoFingerprint LoadFingerprint(OmemoProtocolAddress address)
        {
            throw new NotImplementedException();
        }

        public OmemoSession LoadSession(OmemoProtocolAddress address)
        {
            throw new NotImplementedException();
        }

        public void StoreDeviceListSubscription(string bareJid, Tuple<OmemoDeviceListSubscriptionState, DateTime> lastUpdate)
        {
            throw new NotImplementedException();
        }

        public void StoreDevices(List<OmemoProtocolAddress> devices)
        {
            throw new NotImplementedException();
        }

        public void StoreFingerprint(OmemoFingerprint fingerprint)
        {
            throw new NotImplementedException();
        }

        public void StoreSession(OmemoProtocolAddress address, OmemoSession session)
        {
            throw new NotImplementedException();
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
