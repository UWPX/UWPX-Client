using System;
using System.Collections.Generic;
using Omemo.Classes;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0384
{
    public interface IExtendedOmemoStorage: IOmemoStorage
    {
        void StoreDeviceListSubscription(string bareJid, Tuple<OmemoDeviceListSubscriptionState, DateTime> lastUpdate);

        /// <summary>
        /// Loads and returns the device list subscription state for the given bare JID and returns it.
        /// </summary>
        /// <param name="bareJid">The bare JID you want to retrieve device list subscription state for.</param>
        Tuple<OmemoDeviceListSubscriptionState, DateTime> LoadDeviceListSubscription(string bareJid);

        /// <summary>
        /// Loads all OMEMO devices for the given bare JID and returns them.
        /// </summary>
        /// <param name="bareJid">The bare JID you want to retrieve all devices for.</param>
        List<OmemoProtocolAddress> LoadDevices(string bareJid);

        /// <summary>
        /// Stores the given OMEMO device.
        /// </summary>
        /// <param name="devices">The devices to store.</param>
        void StoreDevices(List<OmemoProtocolAddress> devices);

        /// <summary>
        /// Loads and returns the OMEMO fingerprint to the given <see cref="OmemoProtocolAddress"/>.
        /// </summary>
        /// <param name="address">The <see cref="OmemoProtocolAddress"/> you want to retrieve the <see cref="OmemoFingerprint"/> for.</param>
        OmemoFingerprint LoadFingerprint(OmemoProtocolAddress address);

        /// <summary>
        /// Stores the given <see cref="OmemoFingerprint"/>.
        /// </summary>
        /// <param name="fingerprint">The <see cref="OmemoFingerprint"/> to store.</param>
        void StoreFingerprint(OmemoFingerprint fingerprint);
    }
}
