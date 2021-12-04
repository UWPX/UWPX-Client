﻿using System;
using System.Collections.Generic;
using System.Linq;
using Manager.Classes.Chat;
using Omemo.Classes;
using Omemo.Classes.Keys;
using Shared.Classes.Threading;
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
                using (SemaLock semaLock = DataCache.INSTANCE.NewChatSemaLock())
                {
                    subscription = DataCache.INSTANCE.GetChat(dbAccount.bareJid, bareJid, semaLock)?.omemoInfo?.deviceListSubscription;
                }
            }
            if (subscription is null)
            {
                return new Tuple<OmemoDeviceListSubscriptionState, DateTime>(OmemoDeviceListSubscriptionState.NONE, DateTime.MinValue);
            }
            return new Tuple<OmemoDeviceListSubscriptionState, DateTime>(subscription.state, subscription.lastUpdateReceived);
        }

        public List<Tuple<OmemoProtocolAddress, string>> LoadDevices(string bareJid)
        {
            List<OmemoDeviceModel> devices;
            if (string.Equals(bareJid, dbAccount.bareJid))
            {
                devices = dbAccount.omemoInfo.devices;
            }
            else
            {
                using (SemaLock semaLock = DataCache.INSTANCE.NewChatSemaLock())
                {
                    devices = DataCache.INSTANCE.GetChat(dbAccount.bareJid, bareJid, semaLock)?.omemoInfo?.devices;
                }
            }
            if (devices is null)
            {
                return new List<Tuple<OmemoProtocolAddress, string>>();
            }
            return devices.Select(d => new Tuple<OmemoProtocolAddress, string>(new OmemoProtocolAddress(bareJid, d.deviceId), d.deviceLabel)).ToList();
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
                using (SemaLock semaLock = DataCache.INSTANCE.NewChatSemaLock())
                {
                    fingerprint = DataCache.INSTANCE.GetChat(dbAccount.bareJid, address.BARE_JID, semaLock)?.omemoInfo?.devices?.Where(d => d.deviceId == address.DEVICE_ID).FirstOrDefault()?.fingerprint;
                }
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
                using (SemaLock semaLock = DataCache.INSTANCE.NewChatSemaLock())
                {
                    return DataCache.INSTANCE.GetChat(dbAccount.bareJid, address.BARE_JID, semaLock)?.omemoInfo?.devices?.Where(d => d.deviceId == address.DEVICE_ID).FirstOrDefault()?.session;
                }
            }
        }

        public PreKeyModel ReplaceOmemoPreKey(PreKeyModel preKey)
        {
            using (MainDbContext ctx = new MainDbContext())
            {
                // Remove the old key:
                dbAccount.omemoInfo.preKeys.Remove(preKey);
                preKey.Remove(ctx, true);

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
                ChatModel chat;
                using (SemaLock semaLock = DataCache.INSTANCE.NewChatSemaLock())
                {
                    chat = DataCache.INSTANCE.GetChat(dbAccount.bareJid, bareJid, semaLock);
                }
                if (chat is null)
                {
                    throw new InvalidOperationException("Failed to store device list subscription. Chat '" + bareJid + "' does not exist.");
                }
                OmemoDeviceListSubscriptionModel subscription = chat.omemoInfo.deviceListSubscription;
                subscription.lastUpdateReceived = lastUpdate.Item2;
                subscription.state = lastUpdate.Item1;
                using (MainDbContext ctx = new MainDbContext())
                {
                    ctx.Update(subscription);
                }
            }
        }

        public void StoreDevices(List<Tuple<OmemoProtocolAddress, string>> devices, string bareJid)
        {
            IEnumerable<OmemoDeviceModel> newDevices = devices.Select(d => new OmemoDeviceModel(d.Item1) { deviceLabel = d.Item2 });
            if (string.Equals(bareJid, dbAccount.bareJid))
            {
                using (MainDbContext ctx = new MainDbContext())
                {
                    dbAccount.omemoInfo.devices.ForEach(d => d.Remove(ctx, true));
                    dbAccount.omemoInfo.devices.Clear();
                    ctx.Update(dbAccount.omemoInfo);
                    dbAccount.omemoInfo.devices.AddRange(newDevices.Where(d => d.deviceId != dbAccount.omemoInfo.deviceId));
                    ctx.Update(dbAccount.omemoInfo);
                }
            }
            else
            {
                OmemoChatInformationModel omemoChatInfo;
                using (SemaLock semaLock = DataCache.INSTANCE.NewChatSemaLock())
                {
                    omemoChatInfo = DataCache.INSTANCE.GetChat(dbAccount.bareJid, bareJid, semaLock)?.omemoInfo;
                }
                if (omemoChatInfo is null)
                {
                    throw new InvalidOperationException("Failed to store devices. Chat '" + bareJid + "' does not exist.");
                }
                using (MainDbContext ctx = new MainDbContext())
                {
                    omemoChatInfo.devices.ForEach(d => d.Remove(ctx, true));
                    omemoChatInfo.devices.Clear();
                    ctx.Update(omemoChatInfo);
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
                ChatModel chat;
                using (SemaLock semaLock = DataCache.INSTANCE.NewChatSemaLock())
                {
                    chat = DataCache.INSTANCE.GetChat(dbAccount.bareJid, fingerprint.ADDRESS.BARE_JID, semaLock);
                }
                if (chat is null)
                {
                    throw new InvalidOperationException("Failed to store fingerprint. Chat '" + fingerprint.ADDRESS.BARE_JID + "' does not exist.");
                }
                device = chat.omemoInfo.devices.Where(d => d.deviceId == fingerprint.ADDRESS.DEVICE_ID).FirstOrDefault();
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
                device.fingerprint.Update();
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
                ChatModel chat;
                using (SemaLock semaLock = DataCache.INSTANCE.NewChatSemaLock())
                {
                    chat = DataCache.INSTANCE.GetChat(dbAccount.bareJid, address.BARE_JID, semaLock);
                }
                if (chat is null)
                {
                    throw new InvalidOperationException("Failed to store session. Chat '" + address.BARE_JID + "' does not exist.");
                }
                device = chat.omemoInfo.devices.Where(d => d.deviceId == address.DEVICE_ID).FirstOrDefault();
            }

            if (device is null)
            {
                throw new InvalidOperationException("Failed to store session. Device '" + address.ToString() + "' does not exist.");
            }
            if (device.session is null)
            {
                device.session = session;
            }

            bool newSession = device.session.id != session.id;
            OmemoSessionModel oldSession = null;
            if (newSession)
            {
                oldSession = device.session;
                device.session = session;
            }

            device.Update();

            // Remove the old session:
            if (newSession)
            {
                using (MainDbContext ctx = new MainDbContext())
                {
                    oldSession.Remove(ctx, true);
                }
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
