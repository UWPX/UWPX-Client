using System;
using System.Collections.Generic;
using System.Linq;
using Manager.Classes.Chat;
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
                subscription = DataCache.INSTANCE.GetChat(dbAccount.bareJid, bareJid, DataCache.INSTANCE.NewChatSemaLock())?.omemo?.deviceListSubscription;
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
                devices = DataCache.INSTANCE.GetChat(dbAccount.bareJid, bareJid, DataCache.INSTANCE.NewChatSemaLock())?.omemo?.devices;
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
                fingerprint = DataCache.INSTANCE.GetChat(dbAccount.bareJid, address.BARE_JID, DataCache.INSTANCE.NewChatSemaLock())?.omemo?.devices?.Where(d => d.deviceId == address.DEVICE_ID).FirstOrDefault()?.fingerprint;
            }
            return fingerprint?.ToOmemoFingerprint(address);
        }

        public OmemoSessionModel LoadSession(OmemoProtocolAddress address)
        {
            if (string.Equals(address.BARE_JID, dbAccount.bareJid))
            {
                return dbAccount.omemoInfo.devices.Where(d => d.deviceId == address.DEVICE_ID).FirstOrDefault()?.session;
            }
            else
            {
                return DataCache.INSTANCE.GetChat(dbAccount.bareJid, address.BARE_JID, DataCache.INSTANCE.NewChatSemaLock())?.omemo?.devices?.Where(d => d.deviceId == address.DEVICE_ID).FirstOrDefault()?.session;
            }
        }

        public PreKeyModel ReplaceOmemoPreKey(PreKeyModel preKey)
        {
            using (MainDbContext ctx = new MainDbContext())
            {
                // Remove the old key:
                dbAccount.omemoInfo.preKeys.Remove(preKey);
                ctx.Remove(preKey);

                // Generate a new one:
                PreKeyModel newPreKey = KeyHelper.GeneratePreKey(dbAccount.omemoInfo.maxPreKeyId++);
                dbAccount.omemoInfo.preKeys.Add(newPreKey);
                dbAccount.omemoInfo.bundleInfoAnnounced = false;

                // Store everything in the DB:
                ctx.Add(newPreKey);
                ctx.Update(dbAccount.omemoInfo);
                return newPreKey;
            }
        }

        public void StoreDeviceListSubscription(string bareJid, Tuple<OmemoDeviceListSubscriptionState, DateTime> lastUpdate)
        {
            if (string.Equals(bareJid, dbAccount.bareJid))
            {
                OmemoDeviceListSubscriptionModel subscription = dbAccount.omemoInfo.deviceListSubscription;
                subscription.lastUpdateReceived = lastUpdate.Item2;
                subscription.state = lastUpdate.Item1;
                using (MainDbContext ctx = new MainDbContext())
                {
                    ctx.Update(subscription);
                }
            }
            else
            {
                ChatModel chat = DataCache.INSTANCE.GetChat(dbAccount.bareJid, dbAccount.bareJid, DataCache.INSTANCE.NewChatSemaLock());
                if (chat is null)
                {
                    throw new InvalidOperationException("Failed to store device list subscription. Chat '" + bareJid + "' does not exist.");
                }
                OmemoDeviceListSubscriptionModel subscription = chat.omemo.deviceListSubscription;
                subscription.lastUpdateReceived = lastUpdate.Item2;
                subscription.state = lastUpdate.Item1;
                using (MainDbContext ctx = new MainDbContext())
                {
                    ctx.Update(subscription);
                }
            }
        }

        public void StoreDevices(List<OmemoProtocolAddress> devices, string bareJid)
        {
            IEnumerable<OmemoDeviceModel> newDevices = devices.Select(d => new OmemoDeviceModel(d));
            if (string.Equals(bareJid, dbAccount.bareJid))
            {
                dbAccount.omemoInfo.devices.Clear();
                using (MainDbContext ctx = new MainDbContext())
                {
                    ctx.RemoveRange(dbAccount.omemoInfo.devices);
                    ctx.AddRange(newDevices);
                    dbAccount.omemoInfo.devices.AddRange(newDevices);
                    ctx.Update(dbAccount.omemoInfo);
                }
            }
            else
            {
                OmemoChatInformationModel omemoChatInfo = DataCache.INSTANCE.GetChat(dbAccount.bareJid, dbAccount.bareJid, DataCache.INSTANCE.NewChatSemaLock())?.omemo;
                if (omemoChatInfo is null)
                {
                    throw new InvalidOperationException("Failed to store devices. Chat '" + bareJid + "' does not exist.");
                }
                using (MainDbContext ctx = new MainDbContext())
                {
                    ctx.RemoveRange(omemoChatInfo.devices);
                    ctx.AddRange(newDevices);
                    omemoChatInfo.devices.AddRange(newDevices);
                    ctx.Update(omemoChatInfo);
                }
            }
        }

        public void StoreFingerprint(OmemoFingerprint fingerprint)
        {
            OmemoDeviceModel device;
            if (string.Equals(fingerprint.ADDRESS.BARE_JID, dbAccount.bareJid))
            {
                device = dbAccount.omemoInfo.devices.Where(d => fingerprint.ADDRESS.DEVICE_ID == d.deviceId).FirstOrDefault();
            }
            else
            {
                ChatModel chat = DataCache.INSTANCE.GetChat(dbAccount.bareJid, dbAccount.bareJid, DataCache.INSTANCE.NewChatSemaLock());
                if (chat is null)
                {
                    throw new InvalidOperationException("Failed to store fingerprint. Chat '" + fingerprint.ADDRESS.BARE_JID + "' does not exist.");
                }
                device = chat.omemo.devices.Where(d => d.deviceId == fingerprint.ADDRESS.DEVICE_ID).FirstOrDefault();
            }

            if (device is null)
            {
                throw new InvalidOperationException("Failed to store fingerprint. Device '" + fingerprint.ADDRESS.ToString() + "' does not exist.");
            }
            if (device.fingerprint is null)
            {
                device.fingerprint = new OmemoFingerprintModel(fingerprint);
                using (MainDbContext ctx = new MainDbContext())
                {
                    ctx.Add(device.fingerprint);
                    ctx.Update(device);
                }
            }
            else
            {
                device.fingerprint.lastSeen = fingerprint.lastSeen;
                device.fingerprint.trusted = fingerprint.trusted;
                using (MainDbContext ctx = new MainDbContext())
                {
                    ctx.Update(device.fingerprint);
                }
            }
        }

        public void StoreSession(OmemoProtocolAddress address, OmemoSessionModel session)
        {
            OmemoDeviceModel device;
            if (string.Equals(address.BARE_JID, dbAccount.bareJid))
            {
                device = dbAccount.omemoInfo.devices.Where(d => address.DEVICE_ID == d.deviceId).FirstOrDefault();
            }
            else
            {
                ChatModel chat = DataCache.INSTANCE.GetChat(dbAccount.bareJid, dbAccount.bareJid, DataCache.INSTANCE.NewChatSemaLock());
                if (chat is null)
                {
                    throw new InvalidOperationException("Failed to store session. Chat '" + address.BARE_JID + "' does not exist.");
                }
                device = chat.omemo.devices.Where(d => d.deviceId == address.DEVICE_ID).FirstOrDefault();
            }

            if (device is null)
            {
                throw new InvalidOperationException("Failed to store session. Device '" + address.ToString() + "' does not exist.");
            }
            if (device.session is null)
            {
                device.session = session;
            }
            using (MainDbContext ctx = new MainDbContext())
            {
                if (device.session.id != session.id)
                {
                    ctx.Remove(device.session);
                    device.session = session;
                }
                ctx.Update(device.session);
                ctx.Update(device);
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
