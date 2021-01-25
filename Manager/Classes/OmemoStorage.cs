using System;
using System.Collections.Generic;
using System.Linq;
using Omemo.Classes;
using Omemo.Classes.Keys;
using Storage.Classes.Contexts;
using Storage.Classes.Models.Account;
using Storage.Classes.Models.Chat;
using Storage.Classes.Models.Omemo;
using XMPP_API.Classes.Network.XML.Messages.XEP_0384;

namespace Manager.Classes
{
    public class OmemoStorage: IExtendedOmemoStorage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly AccountModel dbAccount;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public OmemoStorage(AccountModel dbAccount)
        {
            this.dbAccount = dbAccount;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public Tuple<OmemoDeviceListSubscriptionState, DateTime> LoadDeviceListSubscription(string bareJid)
        {
            OmemoDeviceListSubscriptionModel subscription;
            if (string.Equals(bareJid, dbAccount.bareJid))
            {
                subscription = dbAccount.omemoInfo.deviceListSubscription;
            }
            else
            {
                using (ChatDbContext ctx = new ChatDbContext())
                {
                    subscription = ctx.Chats.Where(c => string.Equals(c.accountBareJid, dbAccount.bareJid) && string.Equals(c.bareJid, bareJid)).FirstOrDefault()?.omemo?.deviceListSubscription;
                }
            }
            if (subscription is null)
            {
                return new Tuple<OmemoDeviceListSubscriptionState, DateTime>(OmemoDeviceListSubscriptionState.NONE, DateTime.MinValue);
            }
            return new Tuple<OmemoDeviceListSubscriptionState, DateTime>(subscription.state, subscription.lastUpdateReceived);
        }

        public List<OmemoProtocolAddress> LoadDevices(string bareJid)
        {
            List<OmemoDeviceModel> devices;
            if (string.Equals(bareJid, dbAccount.bareJid))
            {
                devices = dbAccount.omemoInfo.devices;
            }
            else
            {
                using (ChatDbContext ctx = new ChatDbContext())
                {
                    devices = ctx.Chats.Where(c => string.Equals(c.accountBareJid, dbAccount.bareJid) && string.Equals(c.bareJid, bareJid)).FirstOrDefault()?.omemo?.devices;
                }
            }
            if (devices is null)
            {
                return new List<OmemoProtocolAddress>();
            }
            return devices.Select(d => new OmemoProtocolAddress(bareJid, d.deviceId)).ToList();
        }

        public OmemoFingerprint LoadFingerprint(OmemoProtocolAddress address)
        {
            OmemoFingerprintModel fingerprint;
            if (string.Equals(address.BARE_JID, dbAccount.bareJid))
            {
                fingerprint = dbAccount.omemoInfo.devices.Where(d => d.deviceId == address.DEVICE_ID).FirstOrDefault()?.fingerprint;
            }
            else
            {
                using (ChatDbContext ctx = new ChatDbContext())
                {
                    fingerprint = ctx.Chats.Where(c => string.Equals(c.accountBareJid, dbAccount.bareJid) && string.Equals(c.bareJid, address.BARE_JID)).FirstOrDefault()?.omemo?.devices?.Where(d => d.deviceId == address.DEVICE_ID).FirstOrDefault().fingerprint;
                }
            }
            if (fingerprint is null)
            {
                return null;
            }
            return new OmemoFingerprint(new ECPubKeyModel(fingerprint.identityPubKey), address, fingerprint.lastSeen, fingerprint.trusted);
        }

        public Omemo.Classes.OmemoSessionModel LoadSession(OmemoProtocolAddress address)
        {
            Storage.Classes.Models.Omemo.OmemoSessionModel session;
            if (string.Equals(address.BARE_JID, dbAccount.bareJid))
            {
                session = dbAccount.omemoInfo.devices.Where(d => d.deviceId == address.DEVICE_ID).FirstOrDefault()?.session;
            }
            else
            {
                using (ChatDbContext ctx = new ChatDbContext())
                {
                    session = ctx.Chats.Where(c => string.Equals(c.accountBareJid, dbAccount.bareJid) && string.Equals(c.bareJid, address.BARE_JID)).FirstOrDefault()?.omemo?.devices?.Where(d => d.deviceId == address.DEVICE_ID).FirstOrDefault().session;
                }
            }
            if (session is null)
            {
                return null;
            }
            return session.ToOmemoSession();
        }

        public void StoreDeviceListSubscription(string bareJid, Tuple<OmemoDeviceListSubscriptionState, DateTime> lastUpdate)
        {
            OmemoDeviceListSubscriptionModel subscription;
            if (string.Equals(bareJid, dbAccount.bareJid))
            {
                subscription = dbAccount.omemoInfo.deviceListSubscription;
            }
            else
            {
                using (ChatDbContext ctx = new ChatDbContext())
                {
                    ChatModel chat = ctx.Chats.Where(c => string.Equals(c.accountBareJid, dbAccount.bareJid) && string.Equals(c.bareJid, bareJid)).FirstOrDefault();
                    if (chat is null)
                    {
                        throw new InvalidOperationException("Failed to store device list subscription. Chat '" + bareJid + "' does not exist.");
                    }
                    subscription = chat.omemo.deviceListSubscription;
                }
            }
            subscription.lastUpdateReceived = lastUpdate.Item2;
            subscription.state = lastUpdate.Item1;
            subscription.Save();
        }

        public void StoreDevices(List<OmemoProtocolAddress> devices, string bareJid)
        {
            throw new NotImplementedException();
        }

        public void StoreFingerprint(OmemoFingerprint fingerprint)
        {
            throw new NotImplementedException();
        }

        public void StoreSession(OmemoProtocolAddress address, Omemo.Classes.OmemoSessionModel session)
        {
            OmemoDeviceModel device;
            if (string.Equals(address.BARE_JID, dbAccount.bareJid))
            {
                device = dbAccount.omemoInfo.devices.Where(d => address.DEVICE_ID == d.deviceId).FirstOrDefault();
            }
            else
            {
                using (ChatDbContext ctx = new ChatDbContext())
                {
                    ChatModel chat = ctx.Chats.Where(c => string.Equals(c.accountBareJid, dbAccount.bareJid) && string.Equals(c.bareJid, address.BARE_JID)).FirstOrDefault();
                    if (chat is null)
                    {
                        throw new InvalidOperationException("Failed to store session. Chat '" + address.BARE_JID + "' does not exist.");
                    }
                    device = chat.omemo.devices.Where(d => d.deviceId == address.DEVICE_ID).FirstOrDefault();
                }
            }

            if (device is null)
            {
                throw new InvalidOperationException("Failed to store session. Device '" + address.ToString() + "' does not exist.");
            }
            if (device.session is null)
            {
                device.session = new Storage.Classes.Models.Omemo.OmemoSessionModel(session);
                device.session.Save();
                device.Save();
            }
            else
            {
                device.session.Save();
            }
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
