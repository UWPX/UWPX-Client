using libsignal;
using System;
using System.Collections.Generic;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0384
{
    public interface IOmemoDeviceStore
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        void StoreDevices(IList<SignalProtocolAddress> devices);

        void StoreDevice(SignalProtocolAddress device);

        IList<SignalProtocolAddress> LoadDevices(string name);

        void StoreDeviceListSubscription(string name, Tuple<OmemoDeviceListSubscriptionState, DateTime> lastUpdate);

        Tuple<OmemoDeviceListSubscriptionState, DateTime> LoadDeviceListSubscription(string name);

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
